using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_StageClearMount : HeroAnim_Base
{
    private float ratio;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ratio = 0.0f;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        Transform sitPoint = Dragon.instance.sitPoint;
        Vector3 addVec = sitPoint.rotation*new Vector3(0, 0.6f, 0);
        ratio = Mathf.Clamp01(ratio + Time.deltaTime * 2);
        addVec = Vector3.Lerp(Vector3.zero, addVec, ratio);
        _hero.Move_Warp(sitPoint.position + addVec, sitPoint.rotation);
    }
}