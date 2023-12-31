using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Anim_Base : StateMachineBehaviour
{
    public HeroMovement.MoveState moveState;
    public bool useNavPosition = true;
    public bool useTrail = false;
    private bool script_entered = false;
    protected Hero hero;
    protected HeroMovement movement;
    [HideInInspector] public bool isFinished = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!script_entered)
        {
            hero = animator.GetComponentInParent<Hero>();
            movement = animator.GetComponent<HeroMovement>();
            script_entered = true;
        }

        isFinished = false;
        movement.anim_base = this;
        movement.moveState = moveState;
        movement.agent.updatePosition = useNavPosition;
        movement.trailEffect.active = useTrail;
    }

    protected bool IsNotAvailable(Animator animator, AnimatorStateInfo stateInfo)
    {
        bool isNotCurrentState = animator.IsInTransition(0) &&
                              animator.GetNextAnimatorStateInfo(0).shortNameHash != stateInfo.shortNameHash;
        return isNotCurrentState || isFinished;
    }
}
