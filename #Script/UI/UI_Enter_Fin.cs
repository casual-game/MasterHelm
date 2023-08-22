using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Enter_Fin : StateMachineBehaviour
{
    public float transitionDuration = 2.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Manager_Main.instance.Spawner_GameStart();
    }
}
