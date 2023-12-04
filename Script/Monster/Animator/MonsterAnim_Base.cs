using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Base : StateMachineBehaviour
{
    public Monster.MoveState moveState;
    
    [HideInInspector] public bool isFinished = false;
    protected Monster _monster;
    private bool script_entered = false;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!script_entered)
        {
            script_entered = true;
            _monster = animator.GetComponent<Monster>();
        }
        _monster.Set_AnimBase(this);
        isFinished = false;
        _monster.Set_MonsterMoveState(moveState);
        _monster.Set_ClipLength(stateInfo.length);
    }
    protected bool IsNotAvailable(Animator animator, AnimatorStateInfo stateInfo)
    {
        bool isNotCurrentState = animator.IsInTransition(0) &&
                                 animator.GetNextAnimatorStateInfo(0).shortNameHash != stateInfo.shortNameHash;
        return isNotCurrentState || isFinished;
    }

}
