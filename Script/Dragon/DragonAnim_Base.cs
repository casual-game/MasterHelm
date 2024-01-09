using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnim_Base : StateMachineBehaviour
{
    protected Dragon _dragon;
    protected bool isFinished = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (_dragon == null) _dragon = animator.GetComponent<Dragon>();
        isFinished = false;
    }
    protected bool IsNotAvailable(Animator animator,AnimatorStateInfo stateInfo)
    {
        bool isNotCurrentState = animator.IsInTransition(0) &&
                                 animator.GetNextAnimatorStateInfo(0).shortNameHash != stateInfo.shortNameHash;
        return isNotCurrentState || isFinished;
    }
}
