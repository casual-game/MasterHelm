using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Enter_Fin : StateMachineBehaviour
{
    public float transitionDuration = 2.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        CamArm.instance.StartFOV();
        CamArm.instance.Cutscene(2.5f,0.2f,1.75f,
            Manager_Main.instance.startBarricade.pointT,Manager_Main.instance.startBarricade);
        Canvas_Player.instance.audio_EnterFin.Play();
        SoundManager.instance.Ingame_Main();
        Player.instance.audio_Ready.Play();
    }
}
