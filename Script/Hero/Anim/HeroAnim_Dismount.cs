using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Dismount : HeroAnim_Base
{
    private Vector3 startPos,endPos;
    private Quaternion startRot, endRot;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Transform dt = Dragon.instance.transform,dst = Dragon.instance.sitPoint;
        startPos = dst.position;
        endPos = dt.position + Vector3.back;
        startRot = dst.rotation;
        endRot = dt.rotation * Quaternion.Euler(0, -89, 0);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        
        float ratio = Mathf.Min(1, stateInfo.normalizedTime);
        Vector3 pos = Vector3.Lerp(startPos,endPos,ratio);
        Quaternion rot = Quaternion.Lerp(startRot,endRot,ratio);
        _hero.Move_Nav(pos-_hero.transform.position, rot);
        if (ratio > 0.99f)
        {
            isFinished = true;
            _hero._spawned = true;
            Debug.Log("Dismount");
            Set_Locomotion(animator);
        }
    }
}