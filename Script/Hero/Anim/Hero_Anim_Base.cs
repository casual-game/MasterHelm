using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Anim_Base : StateMachineBehaviour
{
    public HeroMovement.MoveState moveState;
    private bool script_entered = false;
    protected Hero hero;
    protected HeroMovement movement;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!script_entered)
        {
            hero = animator.GetComponentInParent<Hero>();
            movement = animator.GetComponent<HeroMovement>();
            script_entered = true;
        }

        movement.moveState = moveState;
    }

    protected bool IsNotCurrentState(Animator animator, AnimatorStateInfo stateInfo)
    {
        return animator.IsInTransition(0) &&
               animator.GetNextAnimatorStateInfo(0).shortNameHash != stateInfo.shortNameHash;
    }
}
