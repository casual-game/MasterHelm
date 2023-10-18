using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Roll : HeroAnim_Base
{
    private int pattern = 0;
    private float startDeg,endDeg;
    private AnimationCurve rotateCurve = AnimationCurve.EaseInOut(0,0,1,1);
    private float rotateLimitNormalizedTime = 0.15f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        startDeg = movement.transform.rotation.eulerAngles.y;
        if (GameManager.Bool_Move)
        {
            endDeg = Mathf.Atan2(GameManager.JS_Move.y, GameManager.JS_Move.x) * Mathf.Rad2Deg +
                     CamArm.instance.transform.rotation.eulerAngles.y;
            endDeg = -endDeg + 180;
        }
        else endDeg = startDeg;
        pattern = 0;
        animator.SetBool(GameManager.s_leftstate,false);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        Transform mt = movement.transform;
        
        //조이스틱 각도 계산
        float jsDeg;
        if (GameManager.Bool_Move)
        {
            jsDeg = Mathf.Atan2(GameManager.JS_Move.y, GameManager.JS_Move.x) * Mathf.Rad2Deg +
                    CamArm.instance.transform.rotation.eulerAngles.y;
            jsDeg = -jsDeg + 180;
        }
        else jsDeg = mt.rotation.eulerAngles.y;
        //pattern에 알맞은 동작
        
        float targetDeg;
        if (pattern == 0)
        {
            float ratio = Mathf.Clamp01(stateInfo.normalizedTime / rotateLimitNormalizedTime);
            targetDeg = Mathf.LerpAngle(startDeg,endDeg, rotateCurve.Evaluate(ratio));
            if (stateInfo.normalizedTime > rotateLimitNormalizedTime) pattern++;
        }
        else
        {
            
            //플레이어 각도와의 차이 계산
            float degDiff = -mt.rotation.eulerAngles.y+jsDeg;
            while (degDiff < -180) degDiff += 360;
            while (degDiff > 180) degDiff -= 360;
            //값들을 변환하여 회전 애니메이션에 반영, 실제 회전 각도 계산
            degDiff = Mathf.Clamp(degDiff, -60, 60) / 60.0f;
            targetDeg = Mathf.SmoothDampAngle(mt.eulerAngles.y, jsDeg,
                ref movement.rotateCurrentVelocity, hero.turnDuration_roll * Mathf.Abs(degDiff));
        }
        //최종 이동 설정
        Vector3 targetPos = animator.deltaPosition * hero.moveMotionSpeed_roll;
        Quaternion targetRot = Quaternion.Euler(0,targetDeg,0);
        movement.Move_Nav(targetPos,targetRot);

        if (stateInfo.normalizedTime > 0.63f)
        {
            isFinished = true;
            movement.Set_AnimationState(Hero.AnimationState.Locomotion);
        }
    }
}
