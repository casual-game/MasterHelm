using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnim_FlyAway : DragonAnim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
    }
}
