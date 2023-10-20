using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Hit_Falldown_Fin : MonsterAnim_Base
{
    public float motionSpeed = 1.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(GameManager.s_hit,false);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        //이동
        Vector3 relativePos = animator.deltaPosition * motionSpeed;
        Quaternion nextRot = animator.rootRotation;

        _monster.Move_Nav(relativePos, nextRot);
    }
}
