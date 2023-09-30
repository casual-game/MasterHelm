using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public partial class HeroMovement : MonoBehaviour
{
    //Public
    public enum MoveState { Locomotion = 0,Roll = 1,Interact = 2,Hit=3}
    [ReadOnly] public MoveState moveState = MoveState.Locomotion;
    //Private
    private Animator animator;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public PuppetMaster puppetMaster;
    private Hero hero;
    private Outlinable outlinable;
    //Animator State Machine에 사용되는 변수
    [HideInInspector] public Hero_Anim_Base anim_base;
    [HideInInspector] public Ladder currentLadder = null;
    [HideInInspector] public float animatorParameters_footstep;//Animator의 Footstep 커브의 이전 버전 저장용으로 쓰임.
    [HideInInspector] public float rotateCurrentVelocity,rotAnimCurrentVelocity;
    [HideInInspector] public float ratio_speed; //애니메이터 파라미터 Easing에 사용됨
    private void Start()
    {
        Setting();
    }
    void Setting()
    {
        puppetMaster = transform.parent.GetComponentInChildren<PuppetMaster>();
        animator = GetComponent<Animator>();
        hero = GetComponentInParent<Hero>();
        agent = GetComponent<NavMeshAgent>();
        outlinable = GetComponent<Outlinable>();
        agent.updateRotation = false;
        
        GameManager.instance.E_BTN_Action_Begin.AddListener(E_BTN_Action_Begin);
        GameManager.instance.E_BTN_Action_Fin.AddListener(E_BTN_Action_Fin);
        
        Setting_Effect();
        Setting_LookAt();
        
        GameManager.instance.E_Debug1_Begin.AddListener(Hit_Normal);
        GameManager.instance.E_Debug2_Begin.AddListener(Hit_Strong);
    }

    //Public
    public void Move_Nav(Vector3 relativePos,Quaternion nextRot)
    {
        transform.rotation = nextRot;
        agent.Move(relativePos);
    }
    public void Move_Normal(Vector3 nextPos,Quaternion nextRot)
    {
        transform.SetPositionAndRotation(nextPos, nextRot);
    }
    #region Action
    private float action_BeginTime = -100;
    public void E_BTN_Action_Begin()
    {
        action_BeginTime = Time.unscaledTime;
    }
    public void E_BTN_Action_Fin()
    {
       Vector3 currentPos= transform.position;
        bool isTimting = Time.unscaledTime - action_BeginTime < hero.dash_roll_delay;
        //인터렉션
        if (Interactable.currentInteractable != null)
        {
            Vector3 interactablePos;
            interactablePos = Interactable.currentInteractable.transform.position;
            interactablePos.y =currentPos.y;
            bool canInteract = moveState == MoveState.Locomotion 
                               && Vector3.Dot(transform.forward, (interactablePos - currentPos).normalized) > 0.3f;
            if (canInteract && isTimting)
            {
                currentLadder = (Ladder)Interactable.currentInteractable;
                float currentY = currentPos.y;
                float dist_bottom = Mathf.Abs(currentLadder.range.x - currentY);
                float dist_top = Mathf.Abs(currentLadder.range.y - currentY);
                animator.SetFloat(GameManager.s_ladder_speed, (dist_bottom < dist_top) ? 1 : -1);
                animator.SetBool(GameManager.s_ladder, true);
                Interactable.currentInteractable.Interact();
                action_BeginTime = -100;
                return;
            }
        }

        //구르기
        bool canRoll = moveState == MoveState.Locomotion;
        if (canRoll && isTimting)
        {
            animator.SetTrigger(GameManager.s_state_change);
            animator.SetInteger(GameManager.s_state_type,0);
            action_BeginTime = -100;
            return;
        }
        //Smashed이후 빠른 기상
        bool canSpeedRoll = Time.time < falledTime + hero.hit_Smash_RecoveryInputDelay && moveState == MoveState.Hit;
        if (canSpeedRoll)
        {
            Effect_FastRoll();
            animator.SetBool(GameManager.s_hit,false);
            animator.SetFloat(GameManager.s_hit_rot,-1);
            anim_base.isFinished = true;
            return;
        }
    }
    #endregion
    public void Hit_Normal()
    {
        //감속시킨다.
        ratio_speed *=0.35f;
        animator.SetFloat(GameManager.s_crouch,0.6f);
        //타겟 벡터
        Vector3 hitpoint = Vector3.zero;
        Vector3 targetHitVec = hitpoint-transform.position;
        targetHitVec.y = 0;

        Vector3 myLookVec = transform.forward;
        float targetHitDeg = Mathf.Atan2(targetHitVec.z, targetHitVec.x)*Mathf.Rad2Deg;
        float myLookDeg = Mathf.Atan2(myLookVec.z, myLookVec.x) * Mathf.Rad2Deg;
        float degDiff = targetHitDeg - myLookDeg;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        degDiff /= 180.0f;
        animator.SetFloat(GameManager.s_hit_rot,degDiff);
        animator.SetTrigger(GameManager.s_hit_additive);
        Effect_Hit_Normal();
    }

    private int hit_strong_type = 0;
    private float hit_strong_delay = 0.2f, hit_strong_time = -100;
    [HideInInspector] public float falledTime = -100;
    public AttackMotionType attackMotionType;
    public PlayerSmashedType playerSmashedType;
    public void Hit_Strong()
    {
        if (Time.time < hit_strong_time + hit_strong_delay) return;
        anim_base.isFinished = true;
        hit_strong_time = Time.time;
        
        animator.SetBool(GameManager.s_hit,true);
        animator.SetTrigger(GameManager.s_state_change);
        animator.SetFloat(GameManager.s_hit_rot,1);
        //히트 모션 타입 설정
        int smashedType = (int)playerSmashedType;
        if (smashedType < 0)
        {
            hit_strong_type = (hit_strong_type + 1) % 2;
            if (Time.time - blood_norm_lastTime > hero.blood_normal_delay)
            {
                blood_norm_lastTime = Time.time;
                Instantiate(bloodNOrm, transform.position + Vector3.up*0.8f, transform.rotation, null);
            }
            
            animator.SetInteger(GameManager.s_hit_type,hit_strong_type);
        }
        else
        {
            Effect_Roll();
            animator.SetInteger(GameManager.s_hit_type,smashedType);
        }
        //타겟 벡터
        Vector3 hitpoint = Vector3.zero;
        Vector3 targetHitVec = hitpoint-transform.position;
        targetHitVec.y = 0;

        Vector3 myLookVec = transform.forward;
        float targetHitDeg = Mathf.Atan2(targetHitVec.z, targetHitVec.x)*Mathf.Rad2Deg;
        float myLookDeg = Mathf.Atan2(myLookVec.z, myLookVec.x) * Mathf.Rad2Deg;
        float degDiff = targetHitDeg - myLookDeg + (int)attackMotionType;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        degDiff /= 180.0f;
        animator.SetFloat(GameManager.s_hit_rot,degDiff);
        animator.SetTrigger(GameManager.s_hit_additive);
        Effect_Hit_Strong();
    }
    public void FallDown()
    {
        falledTime = Time.time;
        Transform t = transform;
        p_smoke.transform.SetPositionAndRotation(t.position + t.forward*-0.2f ,t.rotation);
        p_smoke.Play();   
    }
}
