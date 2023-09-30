using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;

public class HeroAnim_Hit_Strong : Hero_Anim_Base
{
    public AnimationCurve moveFlowCurve,motionSpeedCurve,endRatioCurve;
    private float lastRatio,endRatio,motionSpeed;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        endRatio = endRatioCurve.Evaluate(animator.GetFloat(GameManager.s_hit_rot) * 0.5f + 0.5f);
        motionSpeed = motionSpeedCurve.Evaluate(animator.GetFloat(GameManager.s_hit_rot) * 0.5f + 0.5f);
        lastRatio = 0;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        float normalizedTime = stateInfo.normalizedTime % 1.0f;
        float ratio = Mathf.Clamp(normalizedTime / endRatio,0,endRatio);

        float moveMagnitude = (moveFlowCurve.Evaluate(ratio) - moveFlowCurve.Evaluate(lastRatio)) 
                              * hero.hit_Strong_MoveDistance;
        Vector3 relativePos = animator.deltaPosition.normalized * moveMagnitude * motionSpeed;
        Quaternion nextRot = animator.rootRotation;
        movement.Move_Nav(relativePos, nextRot);
        lastRatio = ratio;

        if (normalizedTime > endRatio)
        {
            isFinished = true;
            animator.SetBool(GameManager.s_hit,false);
            return;
        }
    }
}
