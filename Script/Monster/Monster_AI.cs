using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Monster : MonoBehaviour
{
    private void Setting_AI()
    {
        _agent = GetComponent<NavMeshAgent>();
        _patterns = new Dictionary<string, (int index,MonsterPattern pattern)>(monsterInfo.patterns.Count);
        //패턴 설정
        for (int i = 0; i < monsterInfo.patterns.Count; i++)
        {
            MonsterPattern pattern = monsterInfo.patterns[i];
            _patterns.Add(pattern.patternName,(i,pattern));
        }
        GameManager.Instance.E_Debug1_Begin.AddListener(AI_Pattern);
    }
    //Private,Protected
    private Dictionary<string, (int index,MonsterPattern pattern)> _patterns;
    private MoveState _monsterMoveState = MoveState.Idle;
    private MonsterPattern _currentPattern;
    private NavMeshAgent _agent;
    private float _clipLength;
    protected HitState _hitState
    {
        get;
        private set;
    }
    //Public,Static
    public enum MoveState
    {
        Idle = 0,Pattern=1
    }
    public enum HitState {Ground=0,Air=1,Recovery=2}
    [HideInInspector] public float rotateCurrentVelocity;
    
    //Get
    public MonsterPattern Get_CurrentPattern()
    {
        return _currentPattern;
    }
    public bool Get_CanEvaded(float free_time)
    {
        if (_monsterMoveState != MoveState.Pattern || _currentTrailData == null) return false;

        var stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateinfo.normalizedTime;
        
        float currentTime = _clipLength*normalizedTime;
        float evadeMin = Mathf.Max(0,_clipLength*_currentTrailData.trailRange.x - free_time);
        float evadeMax = _clipLength * _currentTrailData.trailRange.y;

        return evadeMin <= currentTime && currentTime < evadeMax;
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
    public void AI_Pattern()
    {
        if (_animator.GetBool(GameManager.s_hit) || _animator.GetBool(GameManager.s_death)) return;
        var p = _patterns["DoubleSwing"];
        _animator.SetTrigger(GameManager.s_state_change);
        _animator.SetInteger(GameManager.s_state_type,1);
        _animator.SetInteger(GameManager.s_pattern,p.index);
        _animator.SetFloat(GameManager.s_speed,p.pattern.playSpeed);
        _currentPattern = p.pattern;
    }
    public virtual bool AI_Hit(Transform attacker,Transform prop,TrailData trailData)
    {
        return false;
    }
    
}
