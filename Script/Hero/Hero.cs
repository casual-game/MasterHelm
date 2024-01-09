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

public partial class Hero : MonoBehaviour
{
    public static Hero instance;
    public void Setting()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _outlinable = GetComponent<Outlinable>();
        _agent.updateRotation = false;
        
        GameManager.Instance.E_BTN_Action_Begin.AddListener(E_BTN_Action_Begin);
        GameManager.Instance.E_BTN_Action_Fin.AddListener(E_BTN_Action_Fin);
        
        Setting_Core();
        Setting_LookAt();
        Setting_Equipment();
        Setting_Effect();
        Setting_Spawn();
        Setting_Sound();
        frameMain = FindObjectOfType<Frame_Main>();
        frameMain.Setting(heroData.HP,heroData.MP_Slot_Capacity);
        instance = this;
    }
    
    //Public
    public enum MoveState { Locomotion = 0,Roll = 1,Interact = 2,Hit=3,NormalAttack=4,StrongAttack=5,RollJust=6}
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
    public HeroData heroData;
    [HideInInspector] public Frame_Main frameMain;
    [HideInInspector] public float rotateCurrentVelocity,rotAnimCurrentVelocity; //회전 계산 시, ref로 사용됨.
    
    //Private
    private Animator _animator;
    private NavMeshAgent _agent;
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
    public void Set_AnimatorUnscaledTime(bool useUnscaledTime)
    {
        if (_animator.updateMode == AnimatorUpdateMode.UnscaledTime && !useUnscaledTime)
            _animator.updateMode = AnimatorUpdateMode.Normal;
        else if (_animator.updateMode == AnimatorUpdateMode.Normal && useUnscaledTime)
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
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

    public MoveState Get_HeroMoveState()
    {
        return HeroMoveState;
    }
    //Move
    public void Move_Nav(Vector3 relativePos,Quaternion nextRot)
    {
        Transform t = transform;
        t.rotation = nextRot;

        bool cantMove = (HeroMoveState == MoveState.NormalAttack || HeroMoveState == MoveState.StrongAttack) 
                        &&_lookT != null && (t.position - _lookT.position).sqrMagnitude < 3.5f;
        if(!cantMove) _agent.Move(relativePos);
    }
    public void Move_Normal(Vector3 nextPos,Quaternion nextRot)
    {
        transform.SetPositionAndRotation(nextPos, nextRot);
    }

    public void Move_Warp(Vector3 nextPos,Quaternion nextRot)
    {
        _agent.Warp(nextPos);
        transform.rotation = nextRot;
    }
    
}
