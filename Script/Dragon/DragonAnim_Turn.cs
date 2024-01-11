using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnim_Turn : DragonAnim_Base
{
    private float ratio;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ratio = Mathf.Abs(_dragon.degDiff / 180.0f);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;

        Transform t = animator.transform;
        Vector3 pos = t.position;
        float deltaRot = animator.deltaRotation.eulerAngles.y;
        while (deltaRot < -180) deltaRot += 360;
        while (deltaRot > 180) deltaRot -= 360;
        Quaternion rot = Quaternion.Euler(0,t.rotation.eulerAngles.y+deltaRot*ratio*1.175f,0);
        _dragon.transform.SetPositionAndRotation(pos,rot);
    }
}