using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AddCoin : StateMachineBehaviour
{
    private string str = "AddCoin";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        int current = animator.GetInteger(str);
        if (current > 0)
        {
            animator.SetInteger(str, current - 1);
            int result = int.Parse(Canvas_Player.instance.tmp_Coin.text) + 1;
            Canvas_Player.instance.tmp_Coin.text = result.ToString();
        }
    }
}
