using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Start : Player_State_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        player.audio_Start.Play();
    }
}
