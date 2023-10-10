using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Ladder_On_Top : HeroAnim_Base
{
    public AnimationCurve moveCurve;
    private Vector3 startPos, endPos;
    private Quaternion startRot, endRot;
    private float startRatio;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetFloat(GameManager.s_ladder_speed,0);
        Transform mt = movement.transform;
        Transform lt = movement.currentLadder.transform;
        startRot = mt.rotation;
        endRot = Quaternion.Euler(0,180 + lt.rotation.eulerAngles.y,0);
        startPos = mt.position;
        endPos = lt.position + lt.forward * 1.4f;
        endPos.y = movement.currentLadder.range.y - 1.09f;
        startRatio = stateInfo.normalizedTime;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        if (stateInfo.normalizedTime< 0.4f)
        {
            float ratio = moveCurve.Evaluate((stateInfo.normalizedTime-startRatio)/(0.4f-startRatio));
            Vector3 nextPos = Vector3.Lerp(startPos, endPos, ratio);
            Quaternion nextRot = Quaternion.Lerp(startRot, endRot, ratio);
            movement.Move_Normal(nextPos, nextRot);
        }
        else
        {
            movement.Move_Normal(endPos,endRot);
            isFinished = true;
            return;
        }
    }
}
