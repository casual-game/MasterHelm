using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Attack : MonsterAnim_Base
{
    private MonsterPattern _pattern;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _monster.Set_HitState(Monster.HitState.Ground);
        _pattern = _monster.Get_CurrentPattern();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        
        if (IsNotAvailable(animator, stateInfo)) return;
        float normalizedTime = stateInfo.normalizedTime;
        Transform t = _monster.transform;
        //이동,회전
        Quaternion targetRot;
        if (_pattern.rotateRange.x <= normalizedTime && normalizedTime < _pattern.rotateRange.y)
        {
            float targetDeg;

            Vector3 distVec = t.position - Hero.instance.transform.position;
            float playerDeg = -90 - Mathf.Atan2(distVec.z, distVec.x) * Mathf.Rad2Deg;
            float degDiff = -t.rotation.eulerAngles.y + playerDeg;
            while (degDiff < -180) degDiff += 360;
            while (degDiff > 180) degDiff -= 360;
            degDiff = Mathf.Clamp(degDiff, -60, 60) / 60.0f;
            targetDeg = Mathf.SmoothDampAngle(t.eulerAngles.y, playerDeg,
                ref _monster.rotateCurrentVelocity, _pattern.rotateDuration * Mathf.Abs(degDiff));
            targetRot = Quaternion.Euler(0, targetDeg, 0);
        }
        else targetRot = animator.rootRotation;
        _monster.Move_Nav(animator.deltaPosition*_pattern.motionSpeed,targetRot);
        //트레일
        Update_Trail(normalizedTime, _pattern.trailDatas);
        //복귀
        if (normalizedTime > _pattern.endRatio)
        {
            isFinished = true;
            animator.SetInteger(GameManager.s_state_type,0);
            animator.SetTrigger(GameManager.s_transition);
            return;
        }
    }
}
