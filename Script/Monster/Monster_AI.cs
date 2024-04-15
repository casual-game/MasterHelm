using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public partial class Monster : MonoBehaviour
{
    private void Setting_AI()
    {
        _agent = GetComponent<NavMeshAgent>();
        _patterns = new Dictionary<string, (int index,MonsterPattern pattern)>(8);
        //패턴 설정
        AddPattern(monsterInfo.Pattern_0,0);
        AddPattern(monsterInfo.Pattern_1,1);
        AddPattern(monsterInfo.Pattern_2,2);
        AddPattern(monsterInfo.Pattern_3,3);
        AddPattern(monsterInfo.Pattern_4,4);
        AddPattern(monsterInfo.Pattern_5,5);
        AddPattern(monsterInfo.Pattern_6,6);
        AddPattern(monsterInfo.Pattern_7,7);
        GameManager.Instance.E_Debug1_Begin.AddListener(AI_Pattern_Attack);
        void AddPattern(MonsterPattern pattern,int index)
        {
            if (pattern.usePattern)
            {
                pattern.Setting();
                _patterns.Add(pattern.patternName,(index,pattern));
            }
            
        }
    }
    //Private,Protected
    private Dictionary<string, (int index,MonsterPattern pattern)> _patterns;
    private MoveState _monsterMoveState = MoveState.Idle;
    private NavMeshAgent _agent;
    private float _clipLength;
    protected HitState _hitState
    {
        get;
        private set;
    }

    private MonsterPattern _currentMonsterPattern;
    //Public,Static
    private static float _calculateTerm = 0.25f;
    public enum MoveState
    {
        Idle = 0,Pattern=1,Hit=2
    }
    public enum HitState {Ground=0,Stun = 0,Smash=1,Recovery=3}
    [HideInInspector] public float rotateCurrentVelocity;
    
    //Get
    public bool Get_CanJustRoll(float free_time)
    {
        if (_monsterMoveState != MoveState.Pattern || _currentTrailData == null) return false;

        var stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateinfo.normalizedTime;
        
        float currentTime = _clipLength*normalizedTime;
        float evadeMin = Mathf.Max(0,_clipLength*_currentTrailData.trailRange.x - free_time);
        float evadeMax = _clipLength * _currentTrailData.trailRange.y;

        return evadeMin <= currentTime && currentTime < evadeMax;
    }
    public MoveState Get_MonsterMoveState()
    {
        return _monsterMoveState;
    }
    public MonsterPattern Get_CurrentMonsterPattern()
    {
        return _currentMonsterPattern;
    }

    public NavMeshAgent Get_Agent()
    {
        return _agent;
    }
    //Set
    public void Set_HitState(HitState hitState)
    {
        if(hitState == HitState.Recovery) Effect_ChangeColor(monsterInfo.colorGroggy,0.25f);
        else if(_hitState == HitState.Recovery) Effect_ChangeColor(Color.clear,0.25f);
        _hitState = hitState;
    }
    public void Set_MonsterMoveState(MoveState moveState)
    {
        _monsterMoveState = moveState;
    }
    public void Set_ClipLength(float length)
    {
        _clipLength = length;
    }
    //AI_Base
    private void AI_Base_Cancel()
    {
        cancellationTokenSourcePattern.Cancel();
        _agent.isStopped = true;
    }
    //AI_Pattern
    public string patternName = "TripleSwing";
    private CancellationTokenSource cancellationTokenSourcePattern = new CancellationTokenSource();
    public void AI_Pattern_Attack()
    {
        if (_animator.GetBool(GameManager.s_hit) || _animator.GetBool(GameManager.s_force)) return;
        AI_Base_Cancel();
        var p = _patterns[patternName];
        p.pattern.Pointer_Reset();
        _animator.SetTrigger(GameManager.s_state_change);
        _animator.SetInteger(GameManager.s_state_type,1);
        _animator.SetInteger(GameManager.s_pattern,p.index);
        _animator.SetInteger(GameManager.s_125ms,p.pattern.Pointer_GetData_TransitionDuration());
        _currentMonsterPattern = p.pattern;
    }
    [Button]
    public void AI_Pattern_FollowPlayer()
    {
        AI_Base_Cancel();
        if (cancellationTokenSourcePattern != null) cancellationTokenSourcePattern.Dispose();
        cancellationTokenSourcePattern = new CancellationTokenSource();
        UT_AI_Pattern_FollowPlayer().Forget();
    }
    private async UniTaskVoid UT_AI_Pattern_FollowPlayer()
    {
        //초기 설정
        _agent.isStopped = false;
        float distance = 2.0f;
        _agent.stoppingDistance = distance;
        _agent.angularSpeed = 720;
        _agent.acceleration = 20.0f;
        _agent.speed = 2.0f;
        //이동
        while ((Hero.instance.transform.position-transform.position).sqrMagnitude > (distance*distance + 0.2f))
        {
            if (cancellationTokenSourcePattern.IsCancellationRequested) break;
            _agent.SetDestination(Hero.instance.transform.position);
            await UniTask.Delay(TimeSpan.FromSeconds(_calculateTerm),cancellationToken: cancellationTokenSourcePattern.Token);
        }
        //도착 후 설정
        _agent.isStopped = true;
        
        print("도착했어요!");
    }
    public virtual bool AI_Pattern_Hit(Transform prop,TrailData trailData)
    {
        AI_Base_Cancel();
        return false;
    }
    
    
    
}
