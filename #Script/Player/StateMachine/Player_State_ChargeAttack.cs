using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Player_State_ChargeAttack : Player_State_Attack
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        attackData = player.data_Weapon_Main.normalAttacks[attackIndex].motion_ChargeAttack;
        bool isDifferent = attackData != player.data_Weapon_Main.normalAttacks[attackIndex].motion_Attack;
        if(isDifferent) Canvas_Player_World.instance.Update_Data();
        player.isStrong = true;
        player.isCharge = true;
        player.SuperArmor(true);
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
            else if (player.CanBow())
            {
                player.Bow();
                finished = true;
                return;
            }
        }
    }
}
