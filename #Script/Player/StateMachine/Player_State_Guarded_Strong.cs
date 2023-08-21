using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Guarded_Strong : Player_State_Base
{
    public float moveSpeed = 1.0f;
    public float endRatio = 0.8f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        base.OnStateMove(animator, stateInfo, layerIndex);
        if(finished) return;
        player.Move(player.transform.position + animator.deltaPosition*moveSpeed,animator.rootRotation);
    }
}