using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Base : StateMachineBehaviour
{
    [HideInInspector] public bool isFinished = false;
    
    private bool script_entered = false;
    protected Monster _monster;

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
    }
    protected bool IsNotAvailable(Animator animator, AnimatorStateInfo stateInfo)
    {
        bool isNotCurrentState = animator.IsInTransition(0) &&
                                 animator.GetNextAnimatorStateInfo(0).shortNameHash != stateInfo.shortNameHash;
        return isNotCurrentState || isFinished;
    }
}
