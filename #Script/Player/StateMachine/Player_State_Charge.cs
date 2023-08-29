using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Charge : Player_State_Base
{
    private float startNormalizedTime;
    public float fullchargeRatio = 1.1f;
    public float overChargeRatio = 2.0f;
    private bool createdEffect = false, firstImpact = false;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        startNormalizedTime = stateInfo.normalizedTime;
        createdEffect = false;
        firstImpact = false;
        animator.SetInteger("ComboIndex",-1);
        player.audio_mat_armor.Play();
        player.audio_mat_fabric_smooth.Play();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (finished) return;
        if (player.CanBow())
        {
            player.Bow();
            finished = true;
            return;
        }
        bool fullCharged = stateInfo.normalizedTime - startNormalizedTime > fullchargeRatio;
        bool overCharged = stateInfo.normalizedTime - startNormalizedTime > overChargeRatio;
        if (fullCharged && !createdEffect)
        {
            player.audio_mat_armor.Play();
            player.audio_mat_fabric_compact.Play();
            player.audio_action_firering.Play();
            Manager_Main.instance.mainData.audio_Effecting_Impact.Play();
            createdEffect = true;
            player.Particle_Smoke();
            if (player.prefab_shield != null) player.prefab_shield.charge_Effect.Play();
            if (player.prefab_weaponL != null) player.prefab_weaponL.charge_Effect.Play();
            if (player.prefab_weaponR != null) player.prefab_weaponR.charge_Effect.Play();
        }
        if (!Canvas_Player.RB_Pressed || overCharged)
        {
            animator.SetBool("Charge",fullCharged);
            animator.SetTrigger("Transition");
            finished = true;
        }

        if (!firstImpact && stateInfo.normalizedTime - startNormalizedTime > 0.1f&& Canvas_Player.RB_Pressed)
        {
            firstImpact = true;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (finished) return;
        
        player.Move(animator.rootPosition,animator.rootRotation);
    }
}
