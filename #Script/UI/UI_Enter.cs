using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Enter : StateMachineBehaviour
{
    private bool check = false;
    private string s_canenter = "CanEnter";
    public float endRatio = 2.0f;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (!check && stateInfo.normalizedTime>endRatio)
        {
            check = true;
            animator.SetBool(s_canenter,true);
        }
    }
}
