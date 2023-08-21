using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Idle : Player_State_Base
{
    public bool stopDuringTransition = false;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (finished) return;
        if(PreInput(animator)) return;
        if (stopDuringTransition && animator.IsInTransition(0)) return;
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (Canvas_Player.LS_Scale > 0.1f)
        {
            animator.SetFloat("StartMove", player.Deg_JSL_Relative());
            animator.SetBool("Move",true);
            finished = true;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (finished) return;
        base.OnStateMove(animator, stateInfo, layerIndex);
        
        player.Move(animator.rootPosition,animator.rootRotation);
    }
}
