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
    public override void Core_Hit_Normal()
    {
        //타겟 벡터
        Vector3 hitpoint = GameManager.V3_Zero;
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
    public override void Core_Hit_Strong(Transform attacker,TrailData trailData)
    {
        if (!Get_IsAlive() || Time.time < _hitStrongTime + HitStrongDelay) return;
        
        
        //현 상태에 따른 히트 타입 설정
        AttackType attackType = trailData.attackType_ground;
        bool isAirSmash = trailData.isAirSmash;
        string attackString;
        HitType hitType;
        if (_hitState == HitState.Ground)
        {
            _animator.SetBool(GameManager.s_isair,false);
            
            switch (attackType)
            {
                case AttackType.Normal:
                    _hitState = HitState.Ground;
                    hitType = HitType.Normal;
                    Core_Damage(20,false);
                    if(Get_IsAlive()) CamArm.instance.Tween_ShakeNormal();
                    attackString =GameManager.s_normalattack;
                    break;
                case AttackType.Stun:
                    _hitState = HitState.Ground;
                    hitType = HitType.Stun;
                    Core_Damage(20,true);
                    if (Get_IsAlive())
                    {
                        GameManager.Instance.Shockwave_Normal(transform.position);
                        CamArm.instance.Tween_ShakeStrong();
                    }
                    attackString = GameManager.s_combobegin;
                    break;
                case AttackType.Smash:
                    _hitState = HitState.Ground;
                    hitType = HitType.Smash;
                    Core_Damage(20,true);
                    if (Get_IsAlive())
                    {
                        GameManager.Instance.Shockwave_Normal(transform.position);
                        CamArm.instance.Tween_ShakeStrong();
                    }
                    attackString = GameManager.s_finalattack;
                    break;
                case AttackType.Combo:
                    _hitState = HitState.Air;
                    hitType = HitType.Bound;
                    Core_Damage(20,true);
                    if (Get_IsAlive())
                    {
                        GameManager.Instance.Shockwave_Normal(transform.position);
                        CamArm.instance.Tween_ShakeStrong();
                    }
                    attackString = GameManager.s_combobegin;
                    break;
                default:
                    _hitState = HitState.Ground;
                    hitType = HitType.Normal;
                    Core_Damage(20,false);
                    if(Get_IsAlive()) CamArm.instance.Tween_ShakeNormal();
                    attackString = GameManager.s_normalattack;
                    break;
            }
        }
        else if (_hitState == HitState.Air)
        {
            _animator.SetBool(GameManager.s_isair,true);
            
            
            if (isAirSmash)
            {
                _hitState = HitState.Recovery;
                hitType = HitType.Flip;
                Core_Damage(20,true);
                if (Get_IsAlive())
                {
                    GameManager.Instance.Shockwave_Normal(transform.position);
                    CamArm.instance.Tween_ShakeStrong();
                }
                attackString = GameManager.s_finalattack;
            }
            else
            {
                _hitState = HitState.Air;
                hitType = HitType.Screw;
                Core_Damage(20,false);
                if(Get_IsAlive()) CamArm.instance.Tween_ShakeNormal();
                attackString = GameManager.s_truecombo;
            }
        }
        else return;

        if (!Get_IsAlive()) attackString = GameManager.s_finalattack;
        GameManager.Instance.Combo(attackString);
        //회전
        Vector3 lookVec = attacker.position-transform.position;
        lookVec.y = 0;
        transform.rotation = Quaternion.LookRotation(lookVec);
        
        //애니메이터 설정
        _animBase.isFinished = true;
        _hitStrongTime = Time.time;
        _hitStrongType = (_hitStrongType + 1) % 2;
        _animator.SetBool(GameManager.s_hit,true);
        _animator.SetTrigger(GameManager.s_state_change);
        if (hitType == HitType.Normal)
        {
            Punch_Down(1.0f);
            _animator.SetInteger(GameManager.s_hit_type,_hitStrongType);
        }
        else
        {
            Punch_Down(1.1f);
            _animator.SetInteger(GameManager.s_hit_type,(int)hitType);
        }
        
        //이펙트 생성
        bool isBloodBottom = hitType is HitType.Normal or HitType.Bound or HitType.Stun;
        bool isCombo = (attackType != AttackType.Normal && _hitState == HitState.Ground) 
                       || _hitState == HitState.Air || isAirSmash;
        if (isCombo) p_blood_combo.transform.rotation = attacker.rotation * Quaternion.Euler(0, Random.Range(-10,10), 0);
        Effect_Hit_Strong(isBloodBottom,isCombo);
    }

    protected override void Core_Damage(float damage,bool isStrong)
    {
        base.Core_Damage(damage,isStrong);
        
    }
}
