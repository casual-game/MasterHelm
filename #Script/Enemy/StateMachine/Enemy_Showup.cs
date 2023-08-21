using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy_Showup : Enemy_State_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        foreach (var renderer in enemy.skinnedMeshRenderers) renderer.enabled = true;
        foreach (var renderer in enemy.meshRenderers) renderer.enabled = true;
        enemy.audio_create.Play();
        enemy.State_Finish();
    }
    
}
