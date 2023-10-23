using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster_Normal : Monster
{
    //Const, ReadOnly
    private const float HitStrongDelay = 0.3f;
    
    //Public
    public enum AnimationState
    {
        Locomotion = 0,
        Roll = 1,
        Attack_Normal = 2,
        Attack_Strong = 3
    }
    
    //Private
    private int _hitStrongType = 0; //강한 히트 모션은 2가지가 있다. 해당 종류를 설정한다.
    private float _hitStrongTime = -100; //히트는 HeroMovement 간격으로 호출 가능하다. 마지막 호출 시간 저장.
    
    //Setter
    public void Set_AnimationState(AnimationState animationState)
    {
        _animator.SetInteger(GameManager.s_state_type, (int)animationState);
        _animator.SetTrigger(GameManager.s_state_change);
    }
    [Button]
    public override void Core_Hit_Normal()
    {
        //타겟 벡터
        Vector3 hitpoint = Vector3.zero;
        Vector3 targetHitVec = hitpoint-transform.position;
        targetHitVec.y = 0;

        Vector3 myLookVec = transform.forward;
        float targetHitDeg = Mathf.Atan2(targetHitVec.z, targetHitVec.x)*Mathf.Rad2Deg;
        float myLookDeg = Mathf.Atan2(myLookVec.z, myLookVec.x) * Mathf.Rad2Deg;
        float degDiff = targetHitDeg - myLookDeg;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        degDiff /= 180.0f;
        _animator.SetFloat(GameManager.s_hit_rot,degDiff);
        _animator.SetTrigger(GameManager.s_hit_additive);
        Punch_Down_Compact(1.5f);
        Effect_Hit_Normal();
    }
    [Button]
    public override void Core_Hit_Strong(PlayerSmashedType playerSmashedType = PlayerSmashedType.None)
    {
        if (Time.time < _hitStrongTime + HitStrongDelay) return;
        _animBase.isFinished = true;
        _hitStrongTime = Time.time;
        _hitStrongType = (_hitStrongType + 1) % 2;
        
        
        _animator.SetBool(GameManager.s_hit,true);
        _animator.SetTrigger(GameManager.s_state_change);
        if (playerSmashedType == PlayerSmashedType.None)
        {
            Punch_Down(1.0f);
            _animator.SetInteger(GameManager.s_hit_type,_hitStrongType);
        }
        else
        {
            Punch_Down(1.1f);
            _animator.SetInteger(GameManager.s_hit_type,(int)playerSmashedType);
        }
        
        bool isBloodBottom = playerSmashedType is PlayerSmashedType.None or PlayerSmashedType.Bound or PlayerSmashedType.Stun;
        Effect_Hit_Strong(isBloodBottom);
    }
}
