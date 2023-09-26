using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Ladder_Off_Top : Hero_Anim_Base
{
    public AnimationCurve moveCurve;
    private Vector3 startPos, endPos;
    private Quaternion startRot, endRot;
    private float startRatio;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Transform mt = movement.transform;
        startPos = mt.position;
        startRot = mt.rotation;
        endPos = movement.currentLadder.upPoint;

        float endPos_x = Mathf.Lerp(startPos.x, endPos.x, 0.1f);
        float endPos_y = Mathf.Lerp(startPos.y, endPos.y, 0.75f);
        float endPos_z = Mathf.Lerp(startPos.z, endPos.z, 0.1f);
        endPos = new Vector3(endPos_x, endPos_y, endPos_z);
        endRot = Quaternion.Euler(0,movement.currentLadder.transform.rotation.eulerAngles.y + 180,0);
        startRatio = stateInfo.normalizedTime;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        if (stateInfo.normalizedTime < 0.45f)
        {
            float ratio = moveCurve.Evaluate((stateInfo.normalizedTime-startRatio) / (0.45f-startRatio));
            Vector3 nextPos = Vector3.Lerp(startPos,endPos,ratio);
            Quaternion nextRot = Quaternion.Lerp(startRot,endRot,ratio);
            movement.Move_Normal(nextPos, nextRot);
        }
    }
}
