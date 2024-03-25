using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster_Normal : Monster
{
    //Const, ReadOnly
    private const float HitStrongDelay = 0.3f;
    
    
    
    //Private
    private int normalHitType = 0; //강한 히트 모션은 2가지가 있다. 해당 종류를 설정한다.
    private float _hitStrongTime = -100; //히트는 HeroMovement 간격으로 호출 가능하다. 마지막 호출 시간 저장.
    
    //Setter
    public override bool AI_Hit(Transform prop,TrailData trailData)
    {
        if (!Get_IsAlive() || !Get_IsReady() || Time.time < _hitStrongTime + HitStrongDelay 
            || _hitState == HitState.Recovery || _hitState == HitState.Smash) return false;
        int damage = trailData.GetDamage();
        Set_MonsterMoveState(MoveState.Hit);
        Equipment_UpdateTrail(false,false,false);
        bool isBigHit = trailData.attackType != AttackType.Normal && _hitState != HitState.Recovery;
        
        _animBase.isFinished = true;
        _hitStrongTime = Time.time;
        
        //노멀 히트
        switch (trailData.attackType)
        {
            case AttackType.Normal:
            default:
                Set_HitState(HitState.Ground);
                SoundManager.Play(SoundContainer_Ingame.instance.sound_hit_normal);
                break;
            case AttackType.Stun:
                Set_HitState(HitState.Stun);
                SoundManager.Play(SoundContainer_Ingame.instance.sound_hit_smash);
                break;
            case AttackType.Smash:
                Set_HitState(HitState.Smash);
                SoundManager.Play(SoundContainer_Ingame.instance.sound_hit_smash);
                break;
        }
        //회전
        Vector3 currentPos = transform.position;
        Vector3 lookVec = Hero.instance.transform.position-currentPos;
        lookVec.y = 0;
        transform.rotation = Quaternion.LookRotation(lookVec);
        //데미지,애니메이터,파티클,연출
        Core_Damage(damage);
        Quaternion effectRot = prop.rotation * Quaternion.Euler(0, Random.Range(-10,10), 0);
        Effect_Hit_Strong(isBigHit,effectRot);
        if (!Get_IsAlive())
        {
            GameManager.Instance.ComboText_Kill(currentPos);
            Punch_Down(1.1f);
            _animator.SetBool(GameManager.s_death,true);
            _animator.SetInteger(GameManager.s_hit_type,trailData.attackType == AttackType.Smash?0:1);
            CamArm.instance.Tween_ShakeStrong();
            GameManager.Instance.Shockwave(transform.position,1.0f);
            Despawn().Forget();
            Voice_Death();
        }
        else if (isBigHit)
        {
            if(trailData.attackType == AttackType.Smash) GameManager.Instance.ComboText_Smash(currentPos);
            else GameManager.Instance.ComboText_Norm(currentPos);
            Punch_Down(1.1f);
            _animator.SetBool(GameManager.s_hit,true);
            _animator.SetInteger(GameManager.s_hit_type,(int)trailData.attackType);
            CamArm.instance.Tween_ShakeStrong();
            GameManager.Instance.Shockwave(transform.position,1.0f);
            Voice_Hit(true);
        }
        else
        {
            GameManager.Instance.ComboText_Norm(currentPos);
            Punch_Down(1.0f);
            _animator.SetBool(GameManager.s_hit,true);
            normalHitType = (normalHitType + 1) % 2;
            _animator.SetInteger(GameManager.s_hit_type,normalHitType);
            CamArm.instance.Tween_ShakeNormal();
            Voice_Hit();
        }
        _highlightEffect.HitFX();
        _animator.SetTrigger(GameManager.s_state_change);
        return true;
    }

    
}
