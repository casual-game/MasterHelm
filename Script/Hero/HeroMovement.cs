using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public partial class HeroMovement : MonoBehaviour
{
    //Public
    public enum MoveState { Locomotion = 0,Roll = 1,Interact = 2,Hit=3}
    [ReadOnly] public MoveState moveState = MoveState.Locomotion;
    //Private
    private Animator animator;
    [HideInInspector] public NavMeshAgent agent;
    private Hero hero;
    //Animator State Machine에 사용되는 변수
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
        animator = GetComponent<Animator>();
        hero = GetComponentInParent<Hero>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        
        GameManager.instance.E_BTN_Action_Begin.AddListener(E_BTN_Action_Begin);
        GameManager.instance.E_BTN_Action_Fin.AddListener(E_BTN_Action_Fin);
        
        Setting_Effect();
        Setting_LookAt();
        
        GameManager.instance.E_Debug1_Begin.AddListener(Hit_Normal);
        GameManager.instance.E_Debug2_Begin.AddListener(Hit_Strong);
        GameManager.instance.E_Debug3_Begin.AddListener(Hit_Smash);
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
            animator.SetInteger(GameManager.s_state_target,0);
            action_BeginTime = -100;
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
    }

    private int hit_strong_type = 0;
    public void Hit_Strong()
    {
        hit_strong_type = (hit_strong_type + 1) % 2;
        animator.SetBool(GameManager.s_hit,true);
        animator.SetTrigger(GameManager.s_state_change);
        
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
    }
    public void Hit_Smash()
    {
        
    }
}
