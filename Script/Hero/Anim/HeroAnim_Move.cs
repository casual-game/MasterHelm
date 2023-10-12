using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Move : HeroAnim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(GameManager.s_leftstate,false);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        //변수 선언
        Transform mt = movement.transform;
        float currentSpeed = movement.ratio_speed;
        float currentAnimDeg = animator.GetFloat(GameManager.s_rot);
        float targetSpeed, targetDeg = mt.rotation.eulerAngles.y;
        //데이터 확정
        bool isCrouching = GameManager.BTN_Action;
        float acceleraton = isCrouching?hero.acceleration_crouch:hero.acceleration_normal;
        float deceleration = isCrouching?hero.deceleration_crouch:hero.deceleration_normal;
        float turnDuration = isCrouching?hero.turnDuration_crouch:hero.turnDuration_normal;
        float moveMotionSpeed = isCrouching?hero.moveMotionSpeed_crouch:hero.moveMotionSpeed_normal;
        //계산,동작
        if (GameManager.Bool_Move)
        {
            //이동
            targetSpeed = Mathf.Min(1.0f, currentSpeed + acceleraton * Time.deltaTime);
            movement.ratio_speed = targetSpeed;
            animator.SetFloat(GameManager.s_speed,hero.moveCurve.Evaluate(movement.ratio_speed));
            //조이스틱 각도 계산
            float jsDeg = Mathf.Atan2(GameManager.JS_Move.y, GameManager.JS_Move.x) * Mathf.Rad2Deg +
                          CamArm.instance.transform.rotation.eulerAngles.y;
            jsDeg = -jsDeg + 180;
            //플레이어 각도와의 차이 계산
            float degDiff = -mt.rotation.eulerAngles.y+jsDeg;
            while (degDiff < -180) degDiff += 360;
            while (degDiff > 180) degDiff -= 360;
            //Turn 애니메이션 전환 가능 여부 확인,전환
            bool canTurn = !isCrouching
                           &&movement.ratio_speed > 0.95f
                           && 145 < Mathf.Abs(degDiff)
                           && Mathf.Abs(currentAnimDeg)<0.175f
                           && !animator.IsInTransition(0);
            if (canTurn)
            {
                animator.SetTrigger(GameManager.s_transition);
                isFinished = true;
                return;
            }
            //값들을 변환하여 회전 애니메이션에 반영, 실제 회전 각도 계산
            degDiff = Mathf.Clamp(degDiff, -60, 60) / 60.0f;
            targetDeg = Mathf.SmoothDampAngle(mt.eulerAngles.y, jsDeg,
                ref movement.rotateCurrentVelocity, turnDuration * Mathf.Abs(degDiff));
            float animDeg = Mathf.SmoothDampAngle(currentAnimDeg, degDiff,
                ref movement.rotAnimCurrentVelocity, 0.25f);
            animator.SetFloat(GameManager.s_rot,animDeg);
        }
        else
        {
            //이동
            targetSpeed = Mathf.Max(0.0f, currentSpeed - deceleration * Time.deltaTime);
            movement.ratio_speed = targetSpeed;
            animator.SetFloat(GameManager.s_speed,hero.moveCurve.Evaluate(movement.ratio_speed));
            //회전 애니메이션
            float animDeg = Mathf.SmoothDampAngle(animator.GetFloat(GameManager.s_rot), 0,
                ref movement.rotAnimCurrentVelocity, 0.25f);
            animator.SetFloat(GameManager.s_rot,animDeg);
        }
        //웅크리기 처리
        float currentCrouching = animator.GetFloat(GameManager.s_crouch);
        float nextCrouching = Mathf.Lerp(currentCrouching, isCrouching ? 1 : 0, hero.crouchingSpeed * Time.deltaTime);
        animator.SetFloat(GameManager.s_crouch,nextCrouching);
        //최종 움직임 처리
        movement.Move_Nav(animator.deltaPosition*moveMotionSpeed
            ,Quaternion.Euler(0,targetDeg,0));
        //Footstep
        float currentFootstep = animator.GetFloat(GameManager.s_footstep);
        if(currentFootstep>0 && movement.animatorParameters_footstep<0) movement.Effect_Footstep_R();
        else if(currentFootstep<0 && movement.animatorParameters_footstep>0) movement.Effect_Footstep_L();
        movement.animatorParameters_footstep = currentFootstep;
    }
}
