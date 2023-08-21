using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_AttackReady : Player_State_Base
{
    private bool charging;
    private Quaternion startRot,endRot;
    private MotionData readyMotion;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        player.SetLeaning(false);
        animator.SetBool("Transition",false);
        animator.SetBool("Charge",true);
        animator.SetBool("Bow",false);
        charging = true;
        startRot = player.transform.rotation;
        Vector3 endRotVec;
        if (player.target == null)
        {
            if (Canvas_Player.LS_Scale < 0.1f) endRotVec = player.transform.rotation*Vector3.forward;
            else endRotVec = Quaternion.Euler(0,CamArm.Degree(),0)*
                             new Vector3(Canvas_Player.LS.x,0,Canvas_Player.LS.y);
        }
        else
        {
            endRotVec = player.target.transform.position - player.transform.position;
        }

        endRotVec.y = 0;
        endRot = Quaternion.LookRotation(endRotVec);
        player.isLeftSkill = null;
        
        player.motionData = player.data_Weapon_Main.normalAttacks[0].motion_Attack;
        readyMotion = player.data_Weapon_Main.normalAttacks[0].motion_AttackReady;
        Canvas_Player_World.instance.Update_Data();
        player.audio_mat_armor.Play();
        player.audio_mat_fabric_smooth.Play();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (finished) return;
        animator.speed = readyMotion.animSpeed * readyMotion.animSpeedCurve.Evaluate(stateInfo.normalizedTime);
        if (player.CanBow())
        {
            player.Bow();
            finished = true;
            return;
        }
        if (!Canvas_Player.RB_Pressed)
        {
            charging = false;
            animator.SetBool("Charge",false);
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (finished) return;
        
        player.Move(animator.rootPosition,Quaternion.Lerp(startRot,endRot,Mathf.Clamp01(stateInfo.normalizedTime)));
    }
}
