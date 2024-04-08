using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Move : HeroAnim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool doPreInput = _hero != null && (_hero.HeroMoveState == Hero.MoveState.Roll);
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!_hero._spawned) _hero._spawned = true;
        animator.SetBool(GameManager.s_leftstate,false);
        if(doPreInput) _hero.Core_PreInput();
    }
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        //구르기
        if (_hero.Get_IsRollTiming())
        {
            Set_Roll(animator);
            return;
        }
        //변수 선언
        Transform mt = _hero.transform;
        float currentSpeed = _hero.Get_SpeedRatio();
        float currentAnimDeg = animator.GetFloat(GameManager.s_rot);
        float targetSpeed, targetDeg = mt.rotation.eulerAngles.y;
        //데이터 확정
        float acceleraton = _heroData.acceleration_normal;
        float deceleration = _heroData.deceleration_normal;
        float turnDuration = _heroData.turnDuration_normal;
        float moveMotionSpeed = _heroData.moveMotionSpeed_normal;
        //계산,동작
        if (GameManager.Bool_Move)
        {
            //이동
            targetSpeed = Mathf.Min(1.0f, currentSpeed + acceleraton * Time.deltaTime);
            _hero.Set_SpeedRatio(targetSpeed);
            animator.SetFloat(GameManager.s_speed,_heroData.moveCurve.Evaluate(_hero.Get_SpeedRatio()));
            //조이스틱 각도 계산
            float jsDeg = -Mathf.Atan2(GameManager.JS_Move.y, GameManager.JS_Move.x) * Mathf.Rad2Deg +
                          CamArm.instance.transform.rotation.eulerAngles.y+90;
            //플레이어 각도와의 차이 계산
            float degDiff = -mt.rotation.eulerAngles.y+jsDeg;
            while (degDiff < -180) degDiff += 360;
            while (degDiff > 180) degDiff -= 360;
            //Turn 애니메이션 전환 가능 여부 확인,전환
            bool canTurn = _hero.Get_SpeedRatio() > 0.95f
                           && 145 < Mathf.Abs(degDiff)
                           && Mathf.Abs(currentAnimDeg)<0.175f
                           && !animator.IsInTransition(0);
            if (canTurn)
            {
                animator.SetTrigger(GameManager.s_turn);
                isFinished = true;
                return;
            }
            //값들을 변환하여 회전 애니메이션에 반영, 실제 회전 각도 계산
            degDiff = Mathf.Clamp(degDiff, -60, 60) / 60.0f;
            targetDeg = Mathf.SmoothDampAngle(mt.eulerAngles.y, jsDeg,
                ref _hero.rotateCurrentVelocity, turnDuration * Mathf.Abs(degDiff));
            float animDeg = Mathf.SmoothDampAngle(currentAnimDeg, degDiff,
                ref _hero.rotAnimCurrentVelocity, 0.25f);
            animator.SetFloat(GameManager.s_rot,animDeg);
        }
        else
        {
            //이동
            targetSpeed = Mathf.Max(0.0f, currentSpeed - deceleration * Time.deltaTime);
            _hero.Set_SpeedRatio(targetSpeed);
            animator.SetFloat(GameManager.s_speed,_heroData.moveCurve.Evaluate(_hero.Get_SpeedRatio()));
            //회전 애니메이션
            float animDeg = Mathf.SmoothDampAngle(animator.GetFloat(GameManager.s_rot), 0,
                ref _hero.rotAnimCurrentVelocity, 0.25f);
            animator.SetFloat(GameManager.s_rot,animDeg);
        }
        
        //최종 움직임 처리
        _hero.Move_Nav(animator.deltaPosition*moveMotionSpeed
            ,Quaternion.Euler(0,targetDeg,0));
        //Footstep
        float currentFootstep = animator.GetFloat(GameManager.s_footstep);
        if(currentFootstep>0 && _hero.Get_AnimatorParameters_Footstep()<0) _hero.Effect_Footstep_R();
        else if(currentFootstep<0 && _hero.Get_AnimatorParameters_Footstep()>0) _hero.Effect_Footstep_L();
        _hero.Set_AnimatorParameters_Footstep(currentFootstep);;
    }
}
