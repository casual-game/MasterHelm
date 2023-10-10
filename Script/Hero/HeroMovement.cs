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
    public enum MoveState { Locomotion = 0,Roll = 1,Interact = 2,Hit=3,NormalAttack=4,StrongAttack=5}
    [ReadOnly] public MoveState moveState = MoveState.Locomotion;
    //Private
    private Transform folder;
    private Animator animator;
    [HideInInspector] public NavMeshAgent agent;
    private Hero hero;
    private Outlinable outlinable;
    //Animator State Machine에 사용되는 변수
    [HideInInspector] public HeroAnim_Base anim_base;
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
        outlinable = GetComponent<Outlinable>();
        agent.updateRotation = false;
        folder = transform.parent.Find("Folder");
        
        GameManager.instance.E_BTN_Action_Begin.AddListener(E_BTN_Action_Begin);
        GameManager.instance.E_BTN_Action_Fin.AddListener(E_BTN_Action_Fin);
        
        Setting_Effect();
        Setting_LookAt();
        Setting_Equipment();
        
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
    
}
