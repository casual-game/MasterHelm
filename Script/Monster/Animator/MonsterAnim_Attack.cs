using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Attack : MonsterAnim_Base
{
    public int index;
    
    private MonsterPattern _pattern;
    private int minIndex, maxIndex;
    private bool toIdle = true;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _monster.Set_HitState(Monster.HitState.Ground);
        _pattern = _monster.Get_CurrentPattern();
        int count=0;
        bool min = false, max = false;
        for (int i = 0; i < _pattern.trailDatas.Count; i++)
        {
            var td = _pattern.trailDatas[i];
            if (td.isTransition) count++;
            if (!min && count == index)
            {
                min = true;
                minIndex = count;
            }

            if (!max && (i == _pattern.trailDatas.Count - 1 || _pattern.trailDatas[i + 1].isTransition))
            {
                max = true;
                maxIndex = i;
                if (i < _pattern.trailDatas.Count - 1 && _pattern.trailDatas[i + 1].isTransition) toIdle = false;
            }
            
        }
        Debug.Log("Min: "+minIndex+", Max: "+maxIndex + ", ToIdle: "+toIdle + " Index: "+moveState);
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
        Update_Trail(normalizedTime,minIndex,maxIndex, _pattern.trailDatas);
        //Idle로 이동 혹은 다음 콤보로 transition.
        if (normalizedTime > _pattern.endRatio)
        {
            if (toIdle)
            {
                isFinished = true;
                animator.SetInteger(GameManager.s_state_type,0);
                animator.SetTrigger(GameManager.s_transition);
                animator.SetInteger(GameManager.s_125ms,1);
                Debug.Log("Idle");
                return;
            }
            else
            {
                animator.SetInteger(GameManager.s_125ms,-1);
                animator.SetInteger(GameManager.s_125ms,_staticTrailData.transition125ms);
                animator.SetTrigger(GameManager.s_transition);
                Debug.Log("Combo");
            }
        }
    }
    
}
