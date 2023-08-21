using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_Test : StateMachineBehaviour
{
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        animator.transform.rotation = animator.rootRotation;
    }
}
