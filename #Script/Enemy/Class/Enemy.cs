using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dest.Math;
using DG.Tweening;
using HighlightPlus;
using Pathfinding;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class Enemy : MonoBehaviour
{
    public static List<Enemy> enemies = new List<Enemy>();
    [HideInInspector] public bool showingup = true;
    [HideInInspector] public Enemy_State_Base stateMachineBehavior = null;
    #region 인게임 변수
    [TitleGroup("InGame")][ReadOnly] public float currentHP=0;
    #endregion
    #region Data변수
    [TitleGroup("Data")][FoldoutGroup("Data/Base Setting")][LabelText("Pointer 높이 설정")] public float targetedHeight = 0.8f;
    [TitleGroup("Data")][FoldoutGroup("Data/Base Setting")][LabelText("Hit시 이동 속도")] public float hitMoveSpeed=0.6f;
    [TitleGroup("Data")][FoldoutGroup("Data/Base Setting")][LabelText("HP")] public float hp = 100;
    [TitleGroup("Data")][FoldoutGroup("Data/Base Setting")][LabelText("GUARD")] public float guard = 50;
    

    [TitleGroup("Data")] [FoldoutGroup("Data/Base Setting")] [LabelText("드랍테이블")] public DropTable dropTable;
    [TitleGroup("Data")] [FoldoutGroup("Data/Props & Transform")] public Transform slot_L, slot_R, slot_Shield;
    [TitleGroup("Data")] [FoldoutGroup("Data/Props & Transform")] public Transform deco_Back, deco_Head, deco_Hip;

    
    #endregion
    #region private 컴포넌트
    private EnemyRoot root;
    [HideInInspector] public Prefab_Prop prefab_Weapon_L, prefab_Weapon_R, prefab_Shield;
    [HideInInspector] public float camDistanceRatio = 1.0f;
    [HideInInspector] public bool death = false,isGuardBreak = false;
    [HideInInspector] public CharacterController cc;
    [HideInInspector] public Animator animator;
    [HideInInspector] public HighlightEffect highlight;
    [HideInInspector] public PuppetMaster puppetMaster;
    protected List<Animator> animators;
    private bool isFirstSetting = true;
    private bool disabled = false;
    private GameObject shadow;
    [HideInInspector] public SkinnedMeshRenderer[] skinnedMeshRenderers;
    [HideInInspector] public MeshRenderer[] meshRenderers;
    #endregion
    //노멀한 함수들//=====================================================================================================
    #region 근본 함수
    public void Setting(Prefab_Prop weapon_L,Prefab_Prop weapon_R,Prefab_Prop shield)
    {
        disabled = false;
        showingup = true;
        //Deco
        int count;
        if (deco_Back.childCount > 0)
        {
            count = Random.Range(0, deco_Back.childCount);
            deco_Back.GetChild(count).gameObject.SetActive(true);
        }
        if (deco_Head.childCount > 0)
        {
            count = Random.Range(0, deco_Head.childCount);
            deco_Head.GetChild(count).gameObject.SetActive(true);
        }
        if (deco_Hip.childCount > 0)
        {
            count = Random.Range(0, deco_Hip.childCount);
            deco_Hip.GetChild(count).gameObject.SetActive(true);
        }
        
        
        
        if (isFirstSetting)
        {
            isFirstSetting = false;
            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            seeker = GetComponent<Seeker>();
            highlight = GetComponent<HighlightEffect>();
            puppetMaster = transform.parent.GetComponentInChildren<PuppetMaster>();
            
            transform.parent.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
            transform.parent.GetComponentInChildren<Canvas>(true).worldCamera = CamArm.instance.uiCam;
            root = GetComponentInParent<EnemyRoot>();
            shadow = transform.Find("Shadow").gameObject;
            Setting_Sound();
            FirstSetting_UI();
        }

        gameObject.layer = LayerMask.NameToLayer("Enemy");
        animators = GetComponentsInChildren<Animator>().ToList();
        animators.Remove(animator);
        enemies.Add(this);
        shadow.SetActive(true);
        //Prop 생성
        if (weapon_L != null)
        {
            prefab_Weapon_L = weapon_L;
            prefab_Weapon_L.Setting_Enemy(slot_L,this);
            prefab_Weapon_L.On(true,slot_L);
        }
        if (weapon_R != null)
        {
            prefab_Weapon_R = weapon_R;
            prefab_Weapon_R.Setting_Enemy(slot_R,this);
            prefab_Weapon_R.On(true,slot_R);
        }
        if (shield != null)
        {
            prefab_Shield = shield;
            prefab_Shield.Setting_Enemy(slot_Shield,this);
            prefab_Shield.On(true,slot_Shield);
        }
        
        //죽었다면 다시 살리기..일부는 OnDisable에서 수행됩니다.
        
        puppetMaster.mode = PuppetMaster.Mode.Active;
        puppetMaster.mappingWeight = 0;
        StopCoroutine("C_Death");
        foreach (var anim in animators)
        {
            anim.speed = 0.5f;
            anim.Play("Dissolve",0,0);
            anim.enabled = false;
        }
        death = false;
        canvas.gameObject.SetActive(true);
        currentHP = hp;
        health_Bar.fillAmount = 1;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        
        //Partical 
        PF_Setting();
        State_Setting();
        Setting_UI();
        Pattern_Setting();
        Setting_Effect();
        //렌더러 설정
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        
        List<Renderer> rs = new List<Renderer>();
        foreach (var mr in meshRenderers) if(mr.transform.parent != transform) rs.Add(mr);
        rs.AddRange(skinnedMeshRenderers);
        highlight.SetTargets(transform,rs.ToArray());
        highlight.highlighted = true;
    }
    public void Disable()
    {
        if (disabled) return;
        disabled = true;
        puppetMaster.mode = PuppetMaster.Mode.Disabled;
        puppetMaster.mappingWeight = 0;
        Pattern_Disable();
        State_Disable();
        PF_Disable();
        for (int i = 0; i < deco_Back.childCount; i++)
        {
            deco_Back.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < deco_Head.childCount; i++)
        {
            deco_Head.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < deco_Hip.childCount; i++)
        {
            deco_Hip.GetChild(i).gameObject.SetActive(false);
        }

        if (Player.instance != null && Player.instance.target == this)
        {
            Player.instance.SetClosestTarget();
        }
        if (enemies.Contains(this)) enemies.Remove(this);
        Canvas_Player.instance.OnLateUpdate.RemoveListener(UI_Update_UnitFrame);
        Canvas_Player.instance.OnLateUpdate.RemoveListener(UI_Update_Guard);
        root.Disable();
    }
    public void Move(Vector3 pos,Quaternion rot)
    {
        cc.Move(pos-transform.position + Vector3.down * 9.8f * Time.deltaTime);
        transform.rotation = rot;
    }
    #endregion
    #region 공격
    [HideInInspector]public Data_EnemyMotion.SingleAttackData currentSingleAttackData=null;
    [HideInInspector] public bool attack_attacked = false;
    [HideInInspector] public float attack_attackedTime;
    #endregion
    #region 피격 관련 함수들

    protected Vector3 hitWeaponRot;
    public virtual void Hit(bool isCounter,Vector3 hitWeaponRot, int? hitType = null, bool isArrow = false) 
    {
        if (death || Player.instance == null) return;
        this.hitWeaponRot = Quaternion.LookRotation(hitWeaponRot).eulerAngles;
        
        //회전
        Vector3 rotateVec = Player.instance.transform.position - transform.position; rotateVec.y = 0; 
        
        Move(transform.position,Quaternion.LookRotation(rotateVec));
        //가드
        float damage = Player.instance.isStrong ? 20 : 10;
        if (guard_use)
        {
            bool isRevenge = Player.instance.IsRevengeSkill();
            if (guard_full)
            {
                UI_SetGuardGauge(currentGuard + damage);
                UI_BreakGuard();
                Effect_Hit_GuardBreak();
                Player.instance.Particle_FireRing(transform.position + Vector3.up);
                animator.SetInteger("Num", 6);
                State_Begin_Danger(DangerState.Hit, TransitionType.Immediately);
            }
            else if (animator.GetInteger("State") == 4)
            {
                UI_SetGuardGauge(currentGuard + damage);
                if (guard_full || isRevenge)
                {
                    UI_SetGuardGauge(Mathf.Infinity);
                    animator.SetInteger("Num", 5);
                    State_Begin_Danger(DangerState.Hit, TransitionType.Immediately);
                    if(isRevenge) Effect_Hit_Revenge();
                    else
                    {
                        Player.instance.audio_Hit_Notice.Play();
                        Effect_Hit_Strong();
                    }
                }
                else if (Player.instance.isStrong)
                {
                    animator.SetInteger("Num", 7);
                    State_Begin_Danger(DangerState.Hit, TransitionType.Immediately);
                    Effect_Guard();
                }
                else
                {
                    animator.SetTrigger("Hit");
                    Effect_Guard();
                }
            }
            else
            {
                
                UI_SetGuardGauge(currentGuard + damage);
                if (guard_full || isRevenge)
                {
                    UI_SetGuardGauge(Mathf.Infinity);
                    animator.SetInteger("Num", 5);
                    if(isRevenge) Effect_Hit_Revenge();
                    else
                    {
                        Player.instance.audio_Hit_Notice.Play();
                        Effect_Hit_Strong();
                    }
                }
                else
                {
                    animator.SetInteger("Num", 7);
                    Effect_Guard();
                }
                State_Begin_Danger(DangerState.Hit, TransitionType.Immediately);
            }
            
            highlight.HitFX(Manager_Main.instance.mainData.enemy_HitColor,0.5f);
            return;
        }
        //히트
        else
        {
            UI_SetHPGauge(currentHP - damage);
            if (death) Effect_Death();
            else if (Player.instance.IsRevengeSkill()) Effect_Hit_Revenge();
            else if (isCounter) Effect_Hit_Counter();
            else if (Player.instance.isStrong) Effect_Hit_Strong();
            else Effect_Hit_Normal();
        }
        //애니메이션
        if (death)
        {
            //impact = Manager_Main.instance.mainData.impact_Smash;
            gameObject.layer = LayerMask.NameToLayer("Ragdoll");
            highlight.HitFX(Manager_Main.instance.mainData.enemy_ExecutedColor,1.5f);
            highlight.outline = 0; 
            canvas.gameObject.SetActive(false);
            Particle_Blood_Smash();
            Particle_Blood_Normal();
            Executed();
        }
        else
        {
            highlight.HitFX(Manager_Main.instance.mainData.enemy_HitColor,0.25f);
            bool cantRevenge = isGuardBreak && !Player.instance.IsRevengeSkill();
            if (!Player.instance.isStrong || cantRevenge)
            {
                animator.SetTrigger("Hit");
                return;
            }
            Cancel();
            if (Player.instance.isSkill)
            {
                State_Begin_Danger(DangerState.Hit, TransitionType.Immediately);
                animator.SetInteger("Num",(int)Player.instance.skillData.hitType);
            }
            else
            {
                hitType = hitType== null ? (int) Player.instance.motionData.hitType : hitType;
                if (hitType == 5 && Vector3.Dot(transform.forward, 
                        Player.instance.transform.position - transform.position) > 0)
                {
                    hitType = 6;
                }
                State_Begin_Danger(DangerState.Hit, TransitionType.Immediately);
                animator.SetInteger("Num",hitType.Value);
            }
        }
        //Impact();
        //카메라
    }
    public bool IsDead()
    {
        return death;
    }
    public bool CalculateForward()
    {
        Vector3 playerEnemyDist = transform.position - Player.instance.transform.position;
        playerEnemyDist.y = 0;
        return Vector3.Dot(Player.instance.transform.forward, playerEnemyDist.normalized) > 0.1f;
    }
    protected void Cancel()
    {
        if(prefab_Shield!=null) prefab_Shield.Trail_Off();
        if(prefab_Weapon_L!=null) prefab_Weapon_L.Trail_Off();
        if(prefab_Weapon_R!=null) prefab_Weapon_R.Trail_Off();
    }
    public void Executed()
    {
        if (enemies.Count == 1 && enemies[0] == this)
        {
            Player.instance.executedTarget = this;
        }
        
        gameObject.layer = LayerMask.NameToLayer("Ragdoll");
        shadow.SetActive(false);
        highlight.highlighted = false; 
        Cancel();
        if (enemies.Contains(this)) enemies.Remove(this);
        if (Player.instance != null && Player.instance.target == this) Player.instance.SetClosestTarget();
        if (prefab_Shield != null) prefab_Shield.Drop_Enemy();
        if (prefab_Weapon_L != null) prefab_Weapon_L.Drop_Enemy();
        if (prefab_Weapon_R != null) prefab_Weapon_R.Drop_Enemy();
        
        
        StartCoroutine("C_Executed_Physics");
        StartCoroutine("C_Death");
        
        State_Begin_Danger(DangerState.Smashed, TransitionType.Immediately);
        animator.SetInteger("Num",2);
    }
    private IEnumerator C_Death()
    {
        yield return new WaitForSeconds(.35f);
        puppetMaster.mode = PuppetMaster.Mode.Active;
        puppetMaster.mappingWeight = 1;
        yield return new WaitForSeconds(0.75f);
        if(Player.instance.executedTarget == this) Player.instance.executedTarget = null;
        yield return new WaitForSeconds(1.0f);
        CreateOrb(Vector3.zero);
        foreach (var anim in animators) anim.enabled = true;
        yield return new WaitForSeconds(6.0f);
        puppetMaster.mode = PuppetMaster.Mode.Disabled;
        
        Disable();
    }
    private IEnumerator C_Executed_Physics()
    {
        float physicsTime = Time.time;
        while (Time.time-physicsTime<3.0f)
        {
            int jlength = DestructibleObject.destructibleObjects.Count;
            for (int j = jlength-1; j >= 0; j--)
            {
                DestructibleObject destructible = DestructibleObject.destructibleObjects[j];
                Vector3 distVec = transform.position-destructible.transform.GetChild(0).position;
                distVec.y = 0;
                if (Vector3.Magnitude(distVec) < cc.radius + 0.1f+destructible.radius)
                {
                    
                    destructible.Explode(transform.position,false);
                }
            }

            yield return null;
        }
    }
    protected void CreateOrb(Vector3 addVec)
    {
        GameObject g;
        //스페셜 오브 생성
        if (Random.Range(0, 7) == 0)
        {
            g = Manager_Pooler.instance.Get("Orb_Special");
            Canvas_Player.instance.AddTable_SpecialOrb(dropTable);
        }
        //일반 오브 생성
        else
        {
            g = Manager_Pooler.instance.Get("Orb_Normal");
            Canvas_Player.instance.AddTable_SpecialOrb(dropTable);
        }
        g.transform.position = transform.position+Vector3.up +transform.forward*-0.5f + addVec;
        g.SetActive(true);
    }
    #endregion
    
    //기타//====================================================================================================

    public void Effect_FirstRoar()
    {
        
    }
    public void Effect_Showup()
    {
        //CamArm.instance.Shake(Manager_Main.instance.mainData.impact_Smooth);
        particle_smoke.Play();
        
    }
    //기즈모 그리기//====================================================================================================
    public Capsule3 Get_Capsule3()
    {
        Vector3 p0 = transform.position+cc.center + Vector3.down * (cc.height * 0.5f-cc.radius), 
            p1 = transform.position+cc.center + Vector3.up * (cc.height * 0.5f-cc.radius);
        return new Capsule3(new Segment3(p0,p1), cc.radius);
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Path 그리기
        if (paths.Count>0)
        {
            for (int i = 0; i < paths[0].vectorPath.Count; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(paths[0].vectorPath[i],0.1f);   
            }
        }
        //충돌범위 그리기
        if (cc == null) cc = GetComponent<CharacterController>();
        Vector3 p0 = transform.position+cc.center + Vector3.down * (cc.height * 0.5f-cc.radius), 
            p1 = transform.position+cc.center + Vector3.up * (cc.height * 0.5f-cc.radius);
        Capsule3 capsule3 = new Capsule3(new Segment3(p0,p1), cc.radius);
        DrawCapsule(capsule3);
    }
    #region Draw관련 함수들
    
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
    #endregion
    
    #endif
}
