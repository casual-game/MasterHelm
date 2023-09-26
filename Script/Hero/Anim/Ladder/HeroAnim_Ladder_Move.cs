using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Ladder_Move : Hero_Anim_Base
{
    private float refSpeed;
    private float ladderSpeedSmoothTime = 0.1f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        refSpeed = 0;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (animator.IsInTransition(0)) return;
        if (IsNotAvailable(animator,stateInfo)) return;
        float targetSpeed;
        if (!GameManager.Bool_Move) targetSpeed = 0;
        else
        {
            float jsDeg = Mathf.Atan2(GameManager.JS_Move.y, GameManager.JS_Move.x) * Mathf.Rad2Deg +
                          CamArm.instance.transform.rotation.eulerAngles.y;
            jsDeg = -jsDeg + 180;
            float delta = Mathf.DeltaAngle(jsDeg, movement.currentLadder.transform.rotation.eulerAngles.y);
            delta = Mathf.Abs(delta);
            if (delta > 90) targetSpeed = 1;
            else targetSpeed = -1;
        }

        float currentSpeed = animator.GetFloat(GameManager.s_ladder_speed);
        float nextSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref refSpeed, ladderSpeedSmoothTime);
        animator.SetFloat(GameManager.s_ladder_speed,nextSpeed);

        Transform mt = movement.transform;
        Vector3 nextPos = mt.position + new Vector3(0,animator.deltaPosition.y*hero.ladderClimbMotionSpeed,0);
        float pos_y = Mathf.Clamp(nextPos.y, movement.currentLadder.range.x, movement.currentLadder.range.y);
        float ratio = (pos_y - movement.currentLadder.range.x) / (movement.currentLadder.range.y - movement.currentLadder.range.x);
        nextPos.y = pos_y;
        if (pos_y - movement.currentLadder.range.x < 0.5f)
        {
            animator.SetTrigger(GameManager.s_transition);
            animator.SetFloat(GameManager.s_ladder_speed,-1);
            isFinished = true;
            return;
        }
        else if (movement.currentLadder.range.y - pos_y < 1.0f)
        {
            animator.SetTrigger(GameManager.s_transition);
            animator.SetFloat(GameManager.s_ladder_speed,1);
            isFinished = true;
            return;
        }
        Quaternion nextRot = mt.rotation;
        movement.Move_Normal(nextPos,nextRot);
    }
}
