using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnim_Normal : DragonAnim_Base
{
    public bool mount = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if(mount) _dragon.Mount();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
    }
}
