using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Smash_Begin : Player_State_Base
{
    public float moveSpeed = 1.0f;
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if(finished) return;
        player.Move(player.transform.position + animator.deltaPosition*moveSpeed,animator.rootRotation);
        
    }
}
