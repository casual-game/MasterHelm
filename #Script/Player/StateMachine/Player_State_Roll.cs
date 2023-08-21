using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Roll : Player_State_Base
{

    public float turnSpeed = 120;
    private RollData data;
    private Quaternion beginRot,endRot;
    private float turnVel;
    private float beginTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        endRot = Quaternion.LookRotation(player.rollVec);
        data = player.data_Weapon_Main.roll_Normal;
        beginRot = player.transform.rotation;
        beginTime = Time.unscaledTime;
        player.roll_Special = false;
        player.particle_target.Enforce_StopEmmediately();
        player.audio_action_roll.Play();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (finished) return;
        animator.speed = data.animSpeed;
        
        Vector3 moveVec = player.transform.position+endRot *Vector3.forward *animator.deltaPosition.magnitude 
            * data.moveSpeed* data.animCurve.Evaluate(stateInfo.normalizedTime);
        float rotRatio = data.rotCurve.Evaluate(Mathf.Clamp01(stateInfo.normalizedTime * 6.5f));
        
        
        if (Canvas_Player.LS_Scale > 0.1f)
        {
            Vector3 controlVec = Quaternion.Euler(0, CamArm.Degree(), 0) *
                                 new Vector3(Canvas_Player.LS.x, 0, Canvas_Player.LS.y);
            float currentDeg = endRot.eulerAngles.y;
            float targetDeg = 90-Mathf.Atan2(controlVec.z, controlVec.x) * Mathf.Rad2Deg;
            float finalDeg = Mathf.MoveTowardsAngle(currentDeg, targetDeg, turnSpeed * Time.deltaTime);
            endRot = Quaternion.Euler(0,finalDeg, 0);
        }
        Quaternion moveRot = Quaternion.Lerp(beginRot,endRot,rotRatio);
        player.Move(moveVec, moveRot);
        if (stateInfo.normalizedTime > data.endRatio)
        {
            animator.SetBool("Strafe",false);
            Time.timeScale = 1.0f;
            animator.updateMode = AnimatorUpdateMode.Normal;
            if (player.CanSkill(animator)) player.Skill();
            else if(player.CanBow()) player.Bow();
            else if (player.CanAttack(animator)) player.Attack();
            else player.ChangeState(0);
            finished = true; 
            return;
        }
        
    }

}
