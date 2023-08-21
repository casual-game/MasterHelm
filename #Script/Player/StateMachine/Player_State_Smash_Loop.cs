using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Smash_Loop : Player_State_Base
{
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if(finished) return;
        player.Move(animator.rootPosition,animator.rootRotation);
    }
}
