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
    public enum MoveState { Locomotion = 0,Roll = 1}
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
        bool canRoll = Time.unscaledTime - action_BeginTime < hero.dash_roll_delay
                       && moveState == MoveState.Locomotion
                       && !animator.GetBool(GameManager.s_roll);
        if (canRoll)
        {
            animator.SetBool(GameManager.s_roll,true);
        }
        action_BeginTime = -100;
    }
    #endregion
    
}
