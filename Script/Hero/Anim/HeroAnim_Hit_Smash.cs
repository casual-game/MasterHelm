using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class HeroAnim_Hit_Smash : Hero_Anim_Base
{
    public float motionSpeed = 1.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        //이동
        Vector3 relativePos = animator.deltaPosition * motionSpeed * hero.hit_Smash_MotionSpeed;
        Quaternion nextRot = animator.rootRotation;

        movement.Move_Nav(relativePos, nextRot);
    }
}