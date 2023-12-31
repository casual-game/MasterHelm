using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Roll : HeroAnim_Base
{
    public float endRatio = 0.6f,moveSpeed = 1.0f,addEndDeg = 0;
    public bool turnImmediately = true;
    private int pattern = 0;
    private float startDeg,endDeg;
    private AnimationCurve rotateCurve = AnimationCurve.EaseInOut(0,0,1,1);
    private float rotateLimitNormalizedTime = 0.15f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        startDeg = _hero.transform.rotation.eulerAngles.y;
        if (GameManager.Bool_Move)
        {
            endDeg = Mathf.Atan2(GameManager.JS_Move.y, GameManager.JS_Move.x) * Mathf.Rad2Deg +
                     CamArm.instance.transform.rotation.eulerAngles.y;
            endDeg = -endDeg + 180 + addEndDeg;
        }
        else endDeg = startDeg;
        pattern = 0;
        animator.SetBool(GameManager.s_leftstate,false);
        _hero.Effect_Roll();
        _hero.Tween_Punch_Up_Compact(0.4f);
        if (turnImmediately)
        {
            Quaternion targetRot = Quaternion.Euler(0,endDeg,0);
            _hero.Move_Nav(Vector3.zero, targetRot);
            pattern = 1;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        Transform mt = _hero.transform;
        
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
            if (!turnImmediately)
            {
                //플레이어 각도와의 차이 계산
                float degDiff = -mt.rotation.eulerAngles.y + jsDeg;
                while (degDiff < -180) degDiff += 360;
                while (degDiff > 180) degDiff -= 360;
                //값들을 변환하여 회전 애니메이션에 반영, 실제 회전 각도 계산
                degDiff = Mathf.Clamp(degDiff, -60, 60) / 60.0f;
                targetDeg = Mathf.SmoothDampAngle(mt.eulerAngles.y, jsDeg,
                    ref _hero.rotateCurrentVelocity, _heroData.turnDuration_roll * Mathf.Abs(degDiff));
            }
            else targetDeg = endDeg;
            
        }
        //최종 이동 설정
        Vector3 targetPos = animator.deltaPosition * moveSpeed;
        Quaternion targetRot = Quaternion.Euler(0,targetDeg,0);
        _hero.Move_Nav(targetPos,targetRot);

        if (stateInfo.normalizedTime > endRatio)
        {
            isFinished = true;
            _hero.Set_RolledTime();
            animator.SetBool(GameManager.s_roll,false);
        }
    }
}
