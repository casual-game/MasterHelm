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
    private void Start()
    {
        Setting();
    }
    private void Setting()
    {
        _animator = GetComponent<Animator>();
        _hero = GetComponentInParent<Hero>();
        _agent = GetComponent<NavMeshAgent>();
        _outlinable = GetComponent<Outlinable>();
        _agent.updateRotation = false;
        _folder = transform.parent.Find("Folder");
        
        GameManager.instance.E_BTN_Action_Begin.AddListener(E_BTN_Action_Begin);
        GameManager.instance.E_BTN_Action_Fin.AddListener(E_BTN_Action_Fin);
        
        Setting_Core();
        Setting_Effect();
        Setting_LookAt();
        Setting_Equipment();
    }
    
    //Public
    public enum MoveState { Locomotion = 0,Roll = 1,Interact = 2,Hit=3,NormalAttack=4,StrongAttack=5}
    public MoveState HeroMoveState
    {
        get;
        private set;
    }
    public Ladder CurrentLadder
    {
        get;
        private set;
    }
    [HideInInspector] public float rotateCurrentVelocity,rotAnimCurrentVelocity; //회전 계산 시, ref로 사용됨.
    
    //Private
    private Transform _folder;
    private Animator _animator;
    private NavMeshAgent _agent;
    private Hero _hero;
    private Outlinable _outlinable;
    private HeroAnim_Base _animBase;
    private float _animatorParametersFootstep;//Animator의 Footstep 커브의 이전 버전 저장용으로 쓰임.
    private float _speedRatio; //애니메이터 파라미터 Easing에 사용됨
    
    //Setter
    public void Set_AnimBase(HeroAnim_Base animBase)
    {
        _animBase = animBase;
    }
    public void Set_Ladder(Ladder ladder)
    {
        CurrentLadder = ladder;
    }
    public void Set_AnimatorParameters_Footstep(float fp)
    {
        _animatorParametersFootstep = fp;
    }
    public void Set_HeroMoveState(MoveState heroMoveState)
    {
        HeroMoveState = heroMoveState;
    }
    public void Set_RotateCurrentVelocity(float f)
    {
        rotateCurrentVelocity = f;
    }
    public void Set_RotAnimCurrentVelocity(float f)
    {
        rotAnimCurrentVelocity = f;
    }
    public void Set_SpeedRatio(float f)
    {
        _speedRatio = f;
    }
    
    //Getter
    public NavMeshAgent Get_NavMeshAgent()
    {
        return _agent;
    }
    public float Get_AnimatorParameters_Footstep()
    {
        return _animatorParametersFootstep;
    }
    public float Get_SpeedRatio()
    {
        return _speedRatio;
    }
    
    //Move
    public void Move_Nav(Vector3 relativePos,Quaternion nextRot)
    {
        transform.rotation = nextRot;
        _agent.Move(relativePos);
    }
    public void Move_Normal(Vector3 nextPos,Quaternion nextRot)
    {
        transform.SetPositionAndRotation(nextPos, nextRot);
    }
    
}
