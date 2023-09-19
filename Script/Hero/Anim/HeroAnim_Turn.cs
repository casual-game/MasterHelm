using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Turn : Hero_Anim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(GameManager.s_turn,false);
        movement.ratio_speed = 0.35f;
        animator.SetFloat(GameManager.s_speed,movement.ratio_speed);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotCurrentState(animator,stateInfo)) return;
        movement.Move(movement.transform.position + animator.deltaPosition * hero.moveMotionSpeed_normal * 0.5f,
            animator.rootRotation);
    }
}
