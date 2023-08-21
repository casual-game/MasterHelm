using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Death : StateMachineBehaviour
{
    private bool check = false;
    private string s_candeath = "CanDeath";
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!check && stateInfo.normalizedTime > 1)
        {
            check = true;
            animator.SetBool(s_candeath,true);
        }
    }
}
