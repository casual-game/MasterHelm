using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_NormalAttack : Player_State_Attack
{
    
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        bool wasSkill = player.isSkill;
        attackData = player.data_Weapon_Main.normalAttacks[attackIndex].motion_Attack;
        if(attackIndex!=0 || wasSkill) Canvas_Player_World.instance.Update_Data();
        player.isCharge = false;
        animator.SetBool("Charge",false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (finished) return;
        
        bool canInput = attackData.canInput.x <= stateInfo.normalizedTime
                        && stateInfo.normalizedTime < attackData.canInput.y;
        if (canInput)
        {
            if (CheckSkill())
            {
                NextSkill(animator);
                finished = true;
                return;
            }
            else if (player.CanBow())
            {
                player.Bow();
                finished = true;
                return;
            }
            else if (player.CanRoll())
            {
                player.Roll();
                finished = true;
                return;
            }
            else if (player.CanStrafe())
            {
                player.Strafe();
                finished = true;
                return;
            }
            else if (CheckAttack())
            {
                NextAttack(animator);
                finished = true;
                return;
            }
        }
    }
    
}

