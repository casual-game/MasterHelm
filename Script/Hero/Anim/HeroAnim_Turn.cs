using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Turn : HeroAnim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        movement.ratio_speed = 0.35f;
        animator.SetFloat(GameManager.s_speed,movement.ratio_speed);
        animator.SetBool(GameManager.s_leftstate,true);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        movement.Move_Nav(animator.deltaPosition * hero.moveMotionSpeed_normal * 0.5f,
            animator.rootRotation);
    }
}
