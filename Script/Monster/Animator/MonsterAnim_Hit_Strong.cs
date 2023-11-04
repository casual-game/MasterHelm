using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Hit_Strong : MonsterAnim_Base
{
    public AnimationCurve moveFlowCurve;
    public float hit_movedistance = 1.0f;
    public float endRatio = 0.6f;
    private float lastRatio;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        lastRatio = 0;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        float normalizedTime = stateInfo.normalizedTime % 1.0f;
        float ratio = Mathf.Clamp(normalizedTime / endRatio,0,endRatio);

        float moveMagnitude = (moveFlowCurve.Evaluate(ratio) - moveFlowCurve.Evaluate(lastRatio)) 
                              * hit_movedistance;
        Vector3 relativePos = animator.deltaPosition.normalized * moveMagnitude;
        Quaternion nextRot = animator.rootRotation;
        _monster.Move_Nav(relativePos, nextRot);
        lastRatio = ratio;

        if (normalizedTime > endRatio)
        {
            isFinished = true;
            animator.SetBool(GameManager.s_hit,false);
            animator.SetInteger(GameManager.s_state_type,0);
            return;
        }
    }
}
