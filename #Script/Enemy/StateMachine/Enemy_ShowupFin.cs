using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ShowupFin : Enemy_State_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Manager_Main.instance.mainData.audio_Effecting_Impact.Play();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        enemy.showingup = false;
        
    }
}
