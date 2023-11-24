using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster_Strong : Monster
{
    //Const, ReadOnly
    private const float HitStrongDelay = 0.3f;
    
    
    
    //Private
    private int _hitStrongType = 0; //강한 히트 모션은 2가지가 있다. 해당 종류를 설정한다.
    private float _hitStrongTime = -100; //히트는 HeroMovement 간격으로 호출 가능하다. 마지막 호출 시간 저장.
    
    //Setter
    
    public override bool AI_Hit(Transform attacker,Transform prop,TrailData trailData)
    {
        if (!Get_IsAlive() || !Get_IsReady() || Time.time <  HitStrongDelay + _hitStrongTime) return false;
        
        //현 상태에 따른 히트 타입 설정
        AttackType attackType = trailData.attackType_ground;
        string attackString;
        int damage = Random.Range(trailData.damage.x, trailData.damage.y+1);
        switch (attackType)
        {
            case AttackType.Normal:
                Core_Damage_Normal(damage);
                Effect(false);
                attackString =GameManager.s_normalattack;
                break;
            default:
                Set_HitState(HitState.Ground);
                Core_Damage_Normal(damage);
                Effect(true);
                attackString =GameManager.s_smash;
                break;
        }
        

        if (!Get_IsAlive()) attackString = GameManager.s_kill;
        GameManager.Instance.Combo(attackString);
        return true;
        void Effect(bool isStrong)
        {
            _hitStrongTime = Time.time;
            if (isStrong || !Get_IsAlive())
            {
                Vector3 pos = transform.position;
                CamArm.instance.Tween_ShakeStrong();
                GameManager.Instance.Shockwave(pos);
                Effect_Hit_Strong(false,true);
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
            else
            {
                CamArm.instance.Tween_ShakeNormal();
                Effect_Hit_Strong(false,false);
                Punch_Down(1.5f);
            }
        }
    }
}
