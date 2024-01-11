using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Mount : HeroAnim_Base
{
    public AnimationCurve curve;

    private Vector3 startPos;
    private Quaternion startRot;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        var t = _hero.transform;
        startPos = t.position;
        startRot = t.rotation;
        Set_Cancel(animator);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        
        float ratio = curve.Evaluate(Mathf.Min(1, stateInfo.normalizedTime));
        Vector3 pos = Vector3.Lerp(startPos,Dragon.instance.sitPoint.position,ratio);
        Quaternion rot = Quaternion.Lerp(startRot,Dragon.instance.sitPoint.rotation,ratio);
        _hero.Move_Warp(pos, rot);
    }
}
