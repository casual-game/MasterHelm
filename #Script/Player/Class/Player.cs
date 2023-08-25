 using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
 using AmazingAssets.AdvancedDissolve;
 using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
 using Dest.Math;
 using DG.Tweening;
 using FIMSpace;
using HighlightPlus;
using Pathfinding;
 using RootMotion.Dynamics;
 using Random = UnityEngine.Random;

public partial class Player : MonoBehaviour
{
    #region 변수 모음
    public static Player instance;
    [HideInInspector] public Player_State_Base stateMachineBahavior = null;
    [HideInInspector] public bool? isLeftSkill = false;
    [HideInInspector] public LeaningAnimator leaningAnimator;
    [HideInInspector] public Animator animator;
    private ParticleSystem particle_Footstep,particle_WaterRipples,particle_FireRing,particle_FireRingColor;
    
    private ParticleSystem particle_Dash;
    [HideInInspector]public HighlightEffect highlight;
    private Transform particleParent;
    [HideInInspector]public int lastState=0;
    private AIPath ai;
    [HideInInspector] public CharacterController cc;
    [HideInInspector] public PuppetMaster puppetMaster;
    [HideInInspector] public Enemy executedTarget = null;
    #endregion

    #region Movement/AttackData
    [FoldoutGroup("Movement")] public AnimationCurve moveCurve = AnimationCurve.Linear(0,0,1,1);
    [FoldoutGroup("Movement")] public float accelerateDuration = 0.3f, decelerateDuration = 0.15f
        ,moveSpeed = 1.0f,strafeSpeed = 0.9f,turnDuration = 0.25f,hitDamage = 40;
    [FoldoutGroup("AttackData")] public float preInput_Attack = 0.5f, preInput_Roll = 0.5f, rollInputDelay = 0.2f,revengeDelay = 0.25f;
    [FoldoutGroup("AttackData")] public float attackStopDist = 0.5f;
    #endregion


    public void Setting()
    {
        instance = this;
        cc = GetComponent<CharacterController>();
        ai = GetComponent<AIPath>();
        highlight = GetComponent<HighlightEffect>();
        puppetMaster = transform.parent.GetComponentInChildren<PuppetMaster>();
        leaningAnimator = GetComponent<LeaningAnimator>();
        leaningAnimator.User_DeliverIsGrounded(true);
        animator = GetComponent<Animator>();
        
        Setting_Equipment();
        Set_Pointer();
        Setting_Particle();
        Setting_Sound();
        ChangeWeaponData(CurrentWeaponData.Main);
        
        highlight.outlineWidth = 1.0f;
    }
    public void ChangeState(int _state,bool immediately = false,bool slow = false)
    {
        stateMachineBahavior.finished = true;
        
        lastState = state;
        state = _state;
        animator.SetInteger("State",_state);
        if(slow) animator.SetTrigger("ChangeState_Slow");
        else if(immediately) animator.SetTrigger("ChangeState_Immediately");
        else animator.SetTrigger("ChangeState");
        switch (_state)
        {
            case 0:
                animator.speed = 1;
                //이 친구의 PointerMode_Guard세팅은 해당 State에서 직접 해줍니다.
                //이 친구의 stamina recover speed 세팅은 해당 State에서 직접 해줍니다.
                break;
            case 3:
                animator.speed = 1;
                guarded = false;
                PointerMode_Guard(false);
                break;
            case 7:
                animator.speed = 1;
                guarded = false;
                PointerMode_Bow();
                break;
            default:
                animator.speed = 1;
                guarded = false;
                PointerMode_Guard(false);
                break;
        }

    }
    public void Move(Vector3 pos,Quaternion rot)
    {
        ai.FinalizeMovement(pos,rot);
    }
    
    #region 무기,장비,스킬

    [HideInInspector] public bool isSkill = false, isStrong = false,isCharge = false;
    [HideInInspector] public MotionData_Attack motionData;
    [HideInInspector] public SkillData skillData;
    [FoldoutGroup("Transform")] public Transform T_Hand_L,T_Hand_R,T_Shield,T_Back,T_Foot_L,T_Foot_R,T_CamRoot,T_Head,T_Canvas;
    [HideInInspector] public Data_Shield data_Shield = null;
    [HideInInspector] public Data_Bow data_Bow = null;
    [HideInInspector] public Data_Weapon data_Weapon_Main = null;
    [HideInInspector] public Data_Weapon data_Weapon_SkillL = null;
    [HideInInspector] public Data_Weapon data_Weapon_SkillR = null;

    [HideInInspector] public Prefab_Prop prefab_weaponL = null,
        prefab_weaponR = null,
        prefab_shield = null,
        prefab_weaponL_SkillL = null,
        prefab_weaponR_SkillL = null,
        prefab_shield_SkillL = null,
        prefab_weaponL_SkillR = null,
        prefab_weaponR_SkillR = null,
        prefab_shield_SkillR = null;
    [HideInInspector] public Prefab_Bow prefab_bow = null;
    [HideInInspector] public int? leftComboIndex = null, rightComboIndex = null;
    /// <summary>
    /// 각종 장비를 장착하고 초기화한다.
    /// </summary>
    void Setting_Equipment()
    {
        //무기 장착
        if (data_Weapon_Main == null) Debug.LogError("무기는 꼭 장착해야 한다!");
        if (data_Weapon_Main.prefab_Left != null)
        {
            prefab_weaponL = Instantiate(data_Weapon_Main.prefab_Left);
            prefab_weaponL.Setting_Player(T_Hand_L,data_Weapon_Main.elementalAttributes,true);
            prefab_weaponL.transform.parent = prefab_weaponL.parent;
            prefab_weaponL.transform.localPosition = prefab_weaponL.T_hold.localPosition;
            prefab_weaponL.transform.localRotation = prefab_weaponL.T_hold.localRotation;
            prefab_weaponL.transform.localScale = prefab_weaponL.T_hold.localScale;
           
        }
        if (data_Weapon_Main.prefab_Right != null)
        {
            prefab_weaponR = Instantiate(data_Weapon_Main.prefab_Right);
            prefab_weaponR.Setting_Player(T_Hand_R,data_Weapon_Main.elementalAttributes,true);
            prefab_weaponR.transform.parent = prefab_weaponR.parent;
            prefab_weaponR.transform.localPosition = prefab_weaponR.T_hold.localPosition;
            prefab_weaponR.transform.localRotation = prefab_weaponR.T_hold.localRotation;
            prefab_weaponR.transform.localScale = prefab_weaponR.T_hold.localScale;
            
        }
        if (data_Shield != null)
        {
            prefab_shield = Instantiate(data_Shield.prefab);
            
            if (data_Weapon_Main.useShield)
            {
                prefab_shield.Setting_Player(T_Shield,data_Shield.elementalAttributes,true);
                prefab_shield.transform.parent = prefab_shield.parent;
                prefab_shield.transform.localPosition = prefab_shield.T_hold.localPosition;
                prefab_shield.transform.localRotation = prefab_shield.T_hold.localRotation;
                prefab_shield.transform.localScale = prefab_shield.T_hold.localScale;
                
            }
            else
            {
                prefab_shield.Setting_Player(T_Back,data_Shield.elementalAttributes,true);
                prefab_shield.transform.parent = prefab_shield.parent;
                prefab_shield.transform.localPosition = data_Shield.localPos;
                prefab_shield.transform.localRotation = Quaternion.Euler(data_Shield.localRot);
                prefab_shield.transform.localScale = data_Shield.localScale;
                
            }
            
        }
        
        if (data_Weapon_SkillL == null) Debug.LogError("skill_L 없음!");
        if (data_Weapon_SkillL.prefab_Left != null)
        {
            prefab_weaponL_SkillL = Instantiate(data_Weapon_SkillL.prefab_Left);
            prefab_weaponL_SkillL.Setting_Player(T_Hand_L,data_Weapon_SkillL.elementalAttributes);
            prefab_weaponL_SkillL.transform.parent = prefab_weaponL_SkillL.parent;
            prefab_weaponL_SkillL.transform.localPosition = prefab_weaponL_SkillL.T_hold.localPosition;
            prefab_weaponL_SkillL.transform.localRotation = prefab_weaponL_SkillL.T_hold.localRotation;
            prefab_weaponL_SkillL.transform.localScale = prefab_weaponL_SkillL.T_hold.localScale;
           
        }
        if (data_Weapon_SkillL.prefab_Right != null)
        {
            prefab_weaponR_SkillL = Instantiate(data_Weapon_SkillL.prefab_Right);
            prefab_weaponR_SkillL.Setting_Player(T_Hand_R,data_Weapon_SkillL.elementalAttributes);
            prefab_weaponR_SkillL.transform.parent = prefab_weaponR_SkillL.parent;
            prefab_weaponR_SkillL.transform.localPosition = prefab_weaponR_SkillL.T_hold.localPosition;
            prefab_weaponR_SkillL.transform.localRotation = prefab_weaponR_SkillL.T_hold.localRotation;
            prefab_weaponR_SkillL.transform.localScale = prefab_weaponR_SkillL.T_hold.localScale;
            
        }
        if (data_Weapon_SkillR == null) Debug.LogError("skill_R 없음!");
        if (data_Weapon_SkillR.prefab_Left != null)
        {
            prefab_weaponL_SkillR = Instantiate(data_Weapon_SkillR.prefab_Left);
            prefab_weaponL_SkillR.Setting_Player(T_Hand_L,data_Weapon_SkillR.elementalAttributes);
            prefab_weaponL_SkillR.transform.parent = prefab_weaponL_SkillR.parent;
            prefab_weaponL_SkillR.transform.localPosition = prefab_weaponL_SkillR.T_hold.localPosition;
            prefab_weaponL_SkillR.transform.localRotation = prefab_weaponL_SkillR.T_hold.localRotation;
            prefab_weaponL_SkillR.transform.localScale = prefab_weaponL_SkillR.T_hold.localScale;
           
        }
        if (data_Weapon_SkillR.prefab_Right != null)
        {
            prefab_weaponR_SkillR = Instantiate(data_Weapon_SkillR.prefab_Right);
            prefab_weaponR_SkillR.Setting_Player(T_Hand_R,data_Weapon_SkillR.elementalAttributes);
            prefab_weaponR_SkillR.transform.parent = prefab_weaponR_SkillR.parent;
            prefab_weaponR_SkillR.transform.localPosition = prefab_weaponR_SkillR.T_hold.localPosition;
            prefab_weaponR_SkillR.transform.localRotation = prefab_weaponR_SkillR.T_hold.localRotation;
            prefab_weaponR_SkillR.transform.localScale = prefab_weaponR_SkillR.T_hold.localScale;
            
        }
        if (data_Bow == null) Debug.LogError("bow 없음!");
        else
        {
            prefab_bow = Instantiate(data_Bow.prefab);
            prefab_bow.Setting(T_Hand_L,T_Hand_R);
        }
        //공격 데이터 설정
        rightComboIndex = 0;
        for (int i = 0; i < data_Weapon_Main.normalAttacks.Count; i++)
        {
            var attackData = data_Weapon_Main.normalAttacks[i];
            if (leftComboIndex == null && attackData.motion_Attack.endMotionType== MotionData_Attack.MotionType.LEFT
                && i + 1 != data_Weapon_Main.normalAttacks.Count)
            {
                leftComboIndex = i + 1;
                break;
            }
        }
        
    }
    public enum CurrentWeaponData
    {
        Main=0,Skill_L=1,Skill_R=2,Bow=3
    }

    [HideInInspector] public CurrentWeaponData currentWeaponData= CurrentWeaponData.Main;

    [HideInInspector] public void ChangeWeaponData(CurrentWeaponData data)
    {
        
        
        currentWeaponData = data;
        switch (currentWeaponData)
        {
            case CurrentWeaponData.Main:
                if(prefab_weaponL!=null) prefab_weaponL.On(true,T_Hand_L);
                if(prefab_weaponR!=null) prefab_weaponR.On(true,T_Hand_R);
                if (prefab_shield != null)
                {
                    if(data_Weapon_Main.useShield)prefab_shield.On(true,T_Shield);
                    else
                    {
                        prefab_shield.transform.parent = T_Back;
                        prefab_shield.transform.localPosition = data_Shield.localPos;
                        prefab_shield.transform.localRotation = Quaternion.Euler(data_Shield.localRot);
                        prefab_shield.transform.localScale = data_Shield.localScale;
                        prefab_shield.animator.SetBool("On",true);
                    }
                }
                
                if(prefab_weaponL_SkillL!=null) prefab_weaponL_SkillL.On(false,null);
                if(prefab_weaponR_SkillL!=null) prefab_weaponR_SkillL.On(false,null);
                if(prefab_shield_SkillL!=null) prefab_shield_SkillL.On(false,null);
                
                if(prefab_weaponL_SkillR!=null) prefab_weaponL_SkillR.On(false,null);
                if(prefab_weaponR_SkillR!=null) prefab_weaponR_SkillR.On(false,null);
                if(prefab_shield_SkillR!=null) prefab_shield_SkillR.On(false,null);
                
                if(prefab_bow!=null) prefab_bow.On(false);
                break;
            case CurrentWeaponData.Skill_L:
                if(prefab_weaponL!=null) prefab_weaponL.On(false,null);
                if(prefab_weaponR!=null) prefab_weaponR.On(false,null);
                if(prefab_shield!=null) prefab_shield.On(false,null);
                
                if(prefab_weaponL_SkillL!=null) prefab_weaponL_SkillL.On(true,T_Hand_L);
                if(prefab_weaponR_SkillL!=null) prefab_weaponR_SkillL.On(true,T_Hand_R);
                if(prefab_shield_SkillL!=null) prefab_shield_SkillL.On(true,T_Shield);
                
                if(prefab_weaponL_SkillR!=null) prefab_weaponL_SkillR.On(false,null);
                if(prefab_weaponR_SkillR!=null) prefab_weaponR_SkillR.On(false,null);
                if(prefab_shield_SkillR!=null) prefab_shield_SkillR.On(false,null);
                
                if(prefab_bow!=null) prefab_bow.On(false);
                
                break;
            case CurrentWeaponData.Skill_R:
                if(prefab_weaponL!=null) prefab_weaponL.On(false,null);
                if(prefab_weaponR!=null) prefab_weaponR.On(false,null);
                if(prefab_shield!=null) prefab_shield.On(false,null);
                
                if(prefab_weaponL_SkillL!=null) prefab_weaponL_SkillL.On(false,null);
                if(prefab_weaponR_SkillL!=null) prefab_weaponR_SkillL.On(false,null);
                if(prefab_shield_SkillL!=null) prefab_shield_SkillL.On(false,null);
                
                if(prefab_weaponL_SkillR!=null) prefab_weaponL_SkillR.On(true,T_Hand_L);
                if(prefab_weaponR_SkillR!=null) prefab_weaponR_SkillR.On(true,T_Hand_R);
                if(prefab_shield_SkillR!=null) prefab_shield_SkillR.On(true,T_Shield);
                
                if(prefab_bow!=null) prefab_bow.On(false);
                
                break;
            case CurrentWeaponData.Bow:
                if(prefab_weaponL!=null) prefab_weaponL.On(false,null,true);
                if(prefab_weaponR!=null) prefab_weaponR.On(false,null,true);
                if(prefab_shield!=null) prefab_shield.On(false,null,true);
                
                if(prefab_weaponL_SkillL!=null) prefab_weaponL_SkillL.On(false,null,true);
                if(prefab_weaponR_SkillL!=null) prefab_weaponR_SkillL.On(false,null,true);
                if(prefab_shield_SkillL!=null) prefab_shield_SkillL.On(false,null,true);
                
                if(prefab_weaponL_SkillR!=null) prefab_weaponL_SkillR.On(false,null,true);
                if(prefab_weaponR_SkillR!=null) prefab_weaponR_SkillR.On(false,null,true);
                if(prefab_shield_SkillR!=null) prefab_shield_SkillR.On(false,null,true);
                
                if(prefab_bow!=null) prefab_bow.On(true);
                break;
        }
        Highlight_ChangeRenderer(data);
    }
    
    #endregion
    #region 각종 함수들
    //각도 관련
    public float Deg_JSL_Absolute()
    {
        float deg_js = DEG(Canvas_Player.Degree_Left());
        float deg_cam = DEG(CamArm.Degree());
        return deg_js + deg_cam;
    }
    public float Deg_JSL_Relative()
    {
        float deg_js = DEG(Canvas_Player.Degree_Left());
        float deg_player = DEG(transform.rotation.eulerAngles.y);
        float deg_cam = DEG(CamArm.Degree());
        return DEG(deg_js + deg_cam - deg_player);
    }
    public float DEG(float deg)
    {
        while (deg > 180) deg -= 360;
        while (deg < -180) deg += 360;
        return deg;
    }

    public Enemy ColsestEnemy(float min)
    {
        float dist = min;
        Enemy _e = null;
        foreach (var e in Enemy.enemies)
        {
            Vector3 distVec = transform.position - e.transform.position;
            distVec.y = 0;
            float magnitude = distVec.magnitude;
            if (magnitude < dist)
            {
                _e = e;
                dist = magnitude;
            }
        }

        return _e;
    }
    public Capsule3 Get_Capsule3()
    {
        Vector3 p0 = transform.position+cc.center + Vector3.down * (cc.height * 0.5f-cc.radius), 
            p1 = transform.position+cc.center + Vector3.up * (cc.height * 0.5f-cc.radius);
        return new Capsule3(new Segment3(p0,p1), cc.radius);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (cc == null) cc = GetComponent<CharacterController>();
        Vector3 p0 = transform.position+cc.center + Vector3.down * (cc.height * 0.5f-cc.radius), 
            p1 = transform.position+cc.center + Vector3.up * (cc.height * 0.5f-cc.radius);
        Capsule3 capsule3 = new Capsule3(new Segment3(p0,p1), cc.radius);
        DrawCapsule(capsule3);
        
    }
    protected void DrawCapsule(Capsule3 capsule)
    {
        Vector3 axis = capsule.Segment.Direction;
        ProjectionPlanes projPlane = axis.GetProjectionPlane();
        Vector3 side = projPlane == ProjectionPlanes.YZ ? Vector3ex.UnitZ : Vector3ex.UnitX;
        side = axis.Cross(ref side);
        Vector3 side1 = side.Cross(ref axis);

        Vector3 p0 = capsule.Segment.P0;
        Vector3 p1 = capsule.Segment.P1;
        Vector3 offset = side * capsule.Radius;

        DrawSegment(p0 + offset, p1 + offset);
        DrawSegment(p0 - offset, p1 - offset);
        offset = side1 * capsule.Radius;
        DrawSegment(p0 + offset, p1 + offset);
        DrawSegment(p0 - offset, p1 - offset);

        Gizmos.DrawWireSphere(p0, capsule.Radius);
        Gizmos.DrawWireSphere(p1, capsule.Radius);
    }
    protected void DrawSegment(Vector2 p0, Vector2 p1)
    {
        Gizmos.DrawLine(p0, p1);
    }
    #endif
    #endregion
}
