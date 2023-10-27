using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Normal : HeroAnim_Base
{
    public float motionSpeed = 1.0f;
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        
        Vector3 relativePos = animator.deltaPosition*motionSpeed;
        Quaternion nextRot = animator.rootRotation;

        _hero.Move_Nav(relativePos, nextRot);
    }
}
