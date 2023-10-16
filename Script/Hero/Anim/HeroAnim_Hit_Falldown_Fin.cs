using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class HeroAnim_Hit_Falldown_Fin : HeroAnim_Base
{
    public float motionSpeed = 1.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(GameManager.s_hit,false);
        movement.Core_ResetRollInput();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        
        Vector3 relativePos = animator.deltaPosition*motionSpeed;
        Quaternion nextRot = animator.rootRotation;

        movement.Move_Nav(relativePos, nextRot);
    }
}
