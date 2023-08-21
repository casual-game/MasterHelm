using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State_Hit : Enemy_State_Base
{
    public Quaternion lookRot;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Vector3 lookVec = Player.instance.transform.position - animator.transform.position;
        lookVec.y = 0;
        lookRot = Quaternion.LookRotation(lookVec);
        enemy.Move(enemy.transform.position,lookRot);
        
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if(finished) return;
        
        
        if (animator.IsInTransition(0) && 
            animator.GetNextAnimatorStateInfo(0).shortNameHash!= stateInfo.shortNameHash)
        {
            finished = true;
            return;
        }
        enemy.Move(enemy.transform.position+animator.deltaPosition*enemy.hitMoveSpeed,lookRot);
    }
}
