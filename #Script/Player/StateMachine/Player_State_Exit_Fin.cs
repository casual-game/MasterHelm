using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Exit_Fin : Player_State_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Manager_Main.instance.Clear_Begin2();
    }
}
