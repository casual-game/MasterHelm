using System.Collections;
using System.Collections.Generic;
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
        GameManager.Instance.E_Debug1_Begin.AddListener(AI_Pattern);
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
    public enum MoveState
    {
        Idle = 0,Pattern=1,Hit=2
    }
    public enum HitState {Ground=0,Air=1,Recovery=2}
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
    public bool Get_CanSideRoll()
    {
        return _monsterMoveState == MoveState.Pattern && _currentTrailData != null;
    }
    public MoveState Get_MonsterMoveState()
    {
        return _monsterMoveState;
    }
    public MonsterPattern Get_CurrentMonsterPattern()
    {
        return _currentMonsterPattern;
    }
    //Set
    public void Set_HitState(HitState hitState)
    {
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
    //AI
    public string patternName = "TripleSwing";
    public void AI_Pattern()
    {
        if (_animator.GetBool(GameManager.s_hit) || _animator.GetBool(GameManager.s_force)) return;
        
        var p = _patterns[patternName];
        p.pattern.Pointer_Reset();
        _animator.SetTrigger(GameManager.s_state_change);
        _animator.SetInteger(GameManager.s_state_type,1);
        _animator.SetInteger(GameManager.s_pattern,p.index);
        _animator.SetInteger(GameManager.s_125ms,p.pattern.Pointer_GetData_TransitionDuration());
        _currentMonsterPattern = p.pattern;
    }
    public virtual bool AI_Hit(Transform attacker,Transform prop,TrailData trailData)
    {
        return false;
    }
    
}
