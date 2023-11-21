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
    private MonsterPattern _currentPattern;
    private NavMeshAgent _agent;
    protected HitState _hitState
    {
        get;
        private set;
    }
    //Public
    public enum HitState {Ground=0,Air=1,Recovery=2}
    [HideInInspector] public float rotateCurrentVelocity;
    
    //Get,Set
    public MonsterPattern Get_CurrentPattern()
    {
        return _currentPattern;
    }
    public void Set_HitState(HitState hitState)
    {
        _hitState = hitState;
    }
    //AI
    public void AI_Pattern()
    {
        if (_animator.GetBool(GameManager.s_hit) || _animator.GetBool(GameManager.s_death)) return;
        var p = _patterns["DoubleSwing"];
        _animator.SetTrigger(GameManager.s_state_change);
        _animator.SetInteger(GameManager.s_state_type,1);
        _animator.SetInteger(GameManager.s_pattern,p.index);
        _currentPattern = p.pattern;
    }
    public virtual bool AI_Hit(Transform attacker,Transform prop,TrailData trailData)
    {
        return false;
    }
    
}
