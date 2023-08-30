using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy_Showup : Enemy_State_Base
{
    public bool playDeco = true;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        foreach (var renderer in enemy.skinnedMeshRenderers) renderer.enabled = true;
        foreach (var renderer in enemy.meshRenderers) renderer.enabled = true;
        
        enemy.State_Finish();
        foreach (var anim in enemy.animators)
        {
            anim.speed = 1.0f;
            anim.Play("Dissolve_Create",0,0);
        }
        
        if (playDeco)
        {
            enemy.audio_create.Play();
            CamArm.instance.Impact(Manager_Main.instance.mainData.impact_SpecialSmooth);
            enemy.particle_charge.Play();
            enemy.highlight.HitFX(Color.white,2.0f,2);
        }
    }
    
}
