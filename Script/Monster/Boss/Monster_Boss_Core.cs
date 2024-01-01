using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Monster_Boss : Monster
{

    //Const, ReadOnly
    private const float HitStrongDelay = 0.3f;
    
    //Public
    public enum BossInteractionState
    {
        Normal = 0, AttackReady = 1, CanCounter = 2,Groggy = 3
    }
    [ReadOnly] public BossInteractionState interactionState;
    [FoldoutGroup("Effect")] public ParticleSystem p_groundimpact;
    //Private
    private int _hitStrongType = 0; //강한 히트 모션은 2가지가 있다. 해당 종류를 설정한다.
    private float _hitStrongTime = -100; //히트는 HeroMovement 간격으로 호출 가능하다. 마지막 호출 시간 저장.
    private Material[] ms_groggy;
    //Core
    public override bool AI_Hit(Transform attacker,Transform prop,TrailData trailData)
    {
        if (!Get_IsAlive() || !Get_IsReady() || Time.time <  HitStrongDelay + _hitStrongTime) return false;
        Equipment_UpdateTrail(false,false,false);
        //현 상태에 따른 히트 타입 설정
        AttackType attackType = trailData.attackType_ground;
        string attackString;
        int damage = Random.Range(trailData.damage.x, trailData.damage.y+1);
        switch (interactionState)
        {
            case BossInteractionState.Normal:
            default:
                if (attackType == AttackType.Normal)
                {
                    Core_Damage_Normal(damage);
                    Effect_Normal();
                    attackString =GameManager.s_normalattack;
                }
                else
                {
                    Set_HitState(HitState.Ground);
                    Core_Damage_Strong(damage);
                    Effect_Strong();
                    attackString =GameManager.s_smash;
                }
                break;
            case BossInteractionState.AttackReady:
                Core_Damage_Weak(damage);
                Effect_Weak();
                attackString =String.Empty;
                break;
            case BossInteractionState.CanCounter:
                if (attackType == AttackType.Normal)
                {
                    Core_Damage_Normal(damage);
                    Effect_Normal();
                    attackString =GameManager.s_normalattack;
                }
                else
                {
                    Set_HitState(HitState.Ground);
                    Core_Damage_Strong(damage,false);
                    Effect_Strong_Counter(damage);
                    attackString =GameManager.s_counter;
                }
                break;
            case BossInteractionState.Groggy:
                if (attackType == AttackType.Normal)
                {
                    Core_Damage_Normal(damage);
                    Effect_Normal();
                    attackString =GameManager.s_normalattack;
                }
                else
                {
                    Set_HitState(HitState.Ground);
                    Core_Damage_Strong(damage);
                    Effect_Strong_Groggy();
                    attackString =GameManager.s_smash;
                }
                break;
        }
        
        if (!Get_IsAlive()) attackString = GameManager.s_kill;
        if(attackString != String.Empty) GameManager.Instance.Combo(attackString);
        return true;
        void Effect_Normal()
        {
            if(!Get_IsAlive()) _animator.SetTrigger(GameManager.s_state_change);
            _hitStrongTime = Time.time;
            if (!Get_IsAlive())
            {
                Effect_Strong();
                return;
            }
            CamArm.instance.Tween_ShakeNormal();
            Effect_Hit_Strong(true,false,GameManager.Q_Identity);
            Punch_Down(1.5f);
        }
        void Effect_Strong_Groggy()
        {
            if(!Get_IsAlive()) _animator.SetTrigger(GameManager.s_state_change);
            _hitStrongTime = Time.time;
            
            Vector3 pos = transform.position;
            CamArm.instance.Tween_ShakeStrong();
            GameManager.Instance.Shockwave(pos);
            Effect_Hit_Strong(false,true,GameManager.Q_Identity);
            Punch_Down(1.0f);
        }
        void Effect_Strong_Counter(int damage)
        {
            _hitStrongTime = Time.time;
            
            Vector3 pos = transform.position;
            CamArm.instance.Tween_Impact(transform,particleScale,damage);
            //GameManager.Instance.Shockwave(pos);
            Effect_Hit_Counter();
            Hero.Blink(0.5f);
            Punch_Down(1.0f);
            Core_InteractionState(BossInteractionState.Groggy);
            _animBase.isFinished = true;
                
            _animator.SetBool(GameManager.s_hit,true);
            _animator.SetTrigger(GameManager.s_state_change);
            _animator.SetInteger(GameManager.s_hit_type,2);
            p_spawn.Play();

            Vector3 lookVec = attacker.position - pos;
            lookVec.y = 0;
            transform.rotation = Quaternion.LookRotation(lookVec);
        }
        void Effect_Strong()
        {
            _hitStrongTime = Time.time;
            
            Vector3 pos = transform.position;
            CamArm.instance.Tween_ShakeStrong();
            GameManager.Instance.Shockwave(pos);
            Effect_Hit_Strong(false,true,GameManager.Q_Identity);
            Punch_Down(1.0f);
                
            _animBase.isFinished = true;
                
            _hitStrongType = (_hitStrongType + 1) % 2;
            _animator.SetBool(GameManager.s_hit,true);
            _animator.SetTrigger(GameManager.s_state_change);
            _animator.SetInteger(GameManager.s_hit_type,_hitStrongType);

            Vector3 lookVec = attacker.position - pos;
            lookVec.y = 0;
            transform.rotation = Quaternion.LookRotation(lookVec);
        }
        void Effect_Weak()
        {
            if(!Get_IsAlive()) _animator.SetTrigger(GameManager.s_state_change);
            _hitStrongTime = Time.time;
            if (!Get_IsAlive())
            {
                Effect_Strong();
                return;
            }
            Punch_Down(1.5f);
            Effect_Hit_Normal();
            CamArm.instance.Tween_ShakeWeak();
        }
    }
    [Button]
    public void Core_InteractionState(BossInteractionState state)
    {
        if (state == interactionState) return;
        switch (state)
        {
            case BossInteractionState.Groggy:
            case BossInteractionState.Normal:
            default:
                Deactivate_CustomMaterial();
                break;
            case BossInteractionState.CanCounter:
                t_blink = Tween.Custom(Color.white, Color.clear, duration: 0.75f,
                    onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
                    ,ease: Ease.InQuad);
                Activate_CanCounter();
                break;
            case BossInteractionState.AttackReady:
                Activate_AttackReady();
                break;
        }
        interactionState = state;
    }
    //CustomMaterialController
    public void Activate_AttackReady()
    {
        if (_customMaterialController == null) return;
        _customMaterialController.Activate(GameManager.s_attackready);
    }
    public void Activate_CanCounter()
    {
        if (_customMaterialController == null) return;
        _customMaterialController.Activate(GameManager.s_cancounter);
    }
    //AnimationEvent
    public void GroggyDown()
    {
        CamArm.instance.Tween_ShakeDown();
        p_groundimpact.Play();
        p_spawn.Play();
        var t = transform;
        ParticleManager.Play(ParticleManager.instance.pd_smoke,t.position + Vector3.up*0.1f,t.rotation,particleScale);
        Punch_Down(1.0f);
    }
}
