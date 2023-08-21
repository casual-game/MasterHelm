using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State_Rise : Enemy_State_Base
{
    
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if(finished) return;
        enemy.Move(animator.rootPosition,animator.rootRotation);
        if (animator.IsInTransition(0) && 
            animator.GetNextAnimatorStateInfo(0).shortNameHash!= stateInfo.shortNameHash)
        {
            finished = true;
            return;
        }
    }
    
}