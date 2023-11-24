using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster_Normal : Monster
{
    //Const, ReadOnly
    private const float HitStrongDelay = 0.3f;
    
    
    
    //Private
    private int _hitStrongType = 0; //강한 히트 모션은 2가지가 있다. 해당 종류를 설정한다.
    private float _hitStrongTime = -100; //히트는 HeroMovement 간격으로 호출 가능하다. 마지막 호출 시간 저장.
    
    //Setter
    public override bool AI_Hit(Transform attacker,Transform prop,TrailData trailData)
    {
        if (!Get_IsAlive() || !Get_IsReady() || Time.time < _hitStrongTime + HitStrongDelay) return false;
        _animBase.isFinished = true;
        Set_MonsterMoveState(MoveState.Hit);
        Equipment_UpdateTrail(false,false,false);
        //현 상태에 따른 히트 타입 설정
        AttackType attackType = trailData.attackType_ground;
        bool isAirSmash = trailData.isAirSmash;
        string attackString;
        int damage = Random.Range(trailData.damage.x, trailData.damage.y+1);
        HitType hitType;
        if (_hitState == HitState.Ground)
        {
            _animator.SetBool(GameManager.s_isair,false);
            
            switch (attackType)
            {
                case AttackType.Normal:
                    Set_HitState(HitState.Ground);
                    hitType = HitType.Normal;
                    Core_Damage_Normal(damage);
                    Effect(false);
                    
                    attackString =GameManager.s_normalattack;
                    break;
                case AttackType.Stun:
                    Set_HitState(HitState.Ground);
                    hitType = HitType.Stun;
                    Core_Damage_Strong(damage);
                    Effect(true);
                    attackString = GameManager.s_combobegin;
                    break;
                case AttackType.Smash:
                    Set_HitState(HitState.Ground);
                    hitType = HitType.Smash;
                    Core_Damage_Strong(damage);
                    Effect(true);
                    attackString = GameManager.s_smash;
                    break;
                case AttackType.Combo:
                    Set_HitState(HitState.Air);
                    hitType = HitType.Bound;
                    Core_Damage_Strong(damage);
                    Effect(true);
                    attackString = GameManager.s_combobegin;
                    break;
                default:
                    Set_HitState(HitState.Ground);
                    hitType = HitType.Normal;
                    Core_Damage_Normal(damage);
                    Effect(false);
                    attackString = GameManager.s_normalattack;
                    break;
            }
        }
        else if (_hitState == HitState.Air)
        {
            _animator.SetBool(GameManager.s_isair,true);
            if (isAirSmash)
            {
                Set_HitState(HitState.Recovery);
                hitType = HitType.Flip;
                Core_Damage_Strong(damage);
                Effect(true);
                attackString = GameManager.s_combofinish;
            }
            else
            {
                Set_HitState(HitState.Air);
                hitType = HitType.Screw;
                Core_Damage_Normal(damage);
                Effect(false);
                attackString = GameManager.s_truecombo;
            }
        }
        else
        {
            _hitStrongTime = Time.time;
            Core_Damage_Weak(damage);
            Punch_Down(1.5f);
            Effect_Hit_Normal();
            CamArm.instance.Tween_ShakeWeak();
            return true;
        }

        if (!Get_IsAlive()) attackString = GameManager.s_kill;
        GameManager.Instance.Combo(attackString);
        //회전
        Vector3 lookVec = attacker.position-transform.position;
        lookVec.y = 0;
        transform.rotation = Quaternion.LookRotation(lookVec);
        
        //애니메이터 설정
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
        if (isCombo) p_blood_combo.transform.rotation = prop.rotation * Quaternion.Euler(0, Random.Range(-10,10), 0);
        Effect_Hit_Strong(isBloodBottom,isCombo);
        return true;
        void Effect(bool isStrong)
        {
            if (isStrong || !Get_IsAlive())
            {
                CamArm.instance.Tween_ShakeStrong();
                GameManager.Instance.Shockwave(transform.position);
            }
            else
            {
                CamArm.instance.Tween_ShakeNormal();
            }
        }
    }

    
}
