using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_MountIdle : HeroAnim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        _hero.Move_Warp(Dragon.instance.sitPoint.position, Dragon.instance.sitPoint.rotation);
    }
}
