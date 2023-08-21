using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Smash_End : Player_State_Base
{
    public float endRatio = 0.85f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        player.puppetMaster.SwitchToDisabledMode();
        if (player.prefab_weaponL != null) player.prefab_weaponL.Pickup_Player();
        if (player.prefab_weaponR != null) player.prefab_weaponR.Pickup_Player();
        if (player.prefab_shield != null) player.prefab_shield.Pickup_Player();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (finished) return;
        player.Move(animator.rootPosition, animator.rootRotation);
        if (stateInfo.normalizedTime > endRatio)
        {
            if (player.CanSkill(animator))
            {
                player.Skill();
            }
            else if (player.CanAttack(animator))
            {
                player.Attack();
            }
            else
            {
                player.ChangeState(0);
            }
            finished = true; 
            return;
        }
    }
}
