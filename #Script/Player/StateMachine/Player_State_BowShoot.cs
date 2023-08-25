using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_BowShoot : Player_State_Base
{
    public float moveSpeed = 0.7f;
    private bool weaponChanged = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        weaponChanged = false;

        player.Shoot();
        player.prefab_bow.SetHold(false);
        player.prefab_bow.SetPulling(false);
        player.audio_Bow_Shoot.Play();
        Canvas_Player.RB_PressedTime = -100;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (finished) return;

        if (!weaponChanged && stateInfo.normalizedTime > 0.4f)
        {
            player.ChangeWeaponData(Player.CurrentWeaponData.Main);
            weaponChanged = true;
        }
        else if (weaponChanged)
        {
            bool normalInputCheck = Canvas_Player.RB_Pressed ||
                                    Time.time - Canvas_Player.RB_PressedTime < player.preInput_Attack;
            bool CanCombo = false;

            if (player.rightComboIndex.HasValue)
            {
                player.ChangeWeaponData(Player.CurrentWeaponData.Main);
                animator.SetInteger("ComboIndex", player.rightComboIndex.Value);
                CanCombo = true;
            }

            if (normalInputCheck && CanCombo)
            {
                player.ChangeState(1);
                finished = true;
                return;
            }
            else if (stateInfo.normalizedTime > 0.6f)
            {
                finished = true;
                if (player.CanRoll())
                {
                    player.Roll();
                }
                else
                {
                    player.ChangeState(0);
                }
                return;
            }
        }
    }


    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (finished) return;
        
        Vector3 pos = player.transform.position + animator.deltaPosition * moveSpeed;
        player.Move(pos,animator.rootRotation);
    }
}
