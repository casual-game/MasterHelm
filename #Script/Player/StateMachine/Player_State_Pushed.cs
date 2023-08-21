using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Pushed : Player_State_Base
{
    public float moveSpeed = 1.0f;
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if(finished) return;

        if (stateInfo.normalizedTime > 0.85f)
        {
            player.ChangeState(0);
            finished = true;
            return;
        }
        player.Move(player.transform.position + animator.deltaPosition*moveSpeed,animator.rootRotation);
    }
}