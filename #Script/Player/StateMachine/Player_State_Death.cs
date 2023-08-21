using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Death : Player_State_Base
{
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.Move(animator.rootPosition,animator.rootRotation);
        base.OnStateMove(animator, stateInfo, layerIndex);
        
    }
}
