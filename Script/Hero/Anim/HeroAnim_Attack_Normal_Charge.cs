using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Attack_Normal_Charge : HeroAnim_Base
{
    private float _enteredTime;
    private bool _realEntered;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _enteredTime = Time.time;
        _realEntered = false;
        animator.speed = 0.75f;
        animator.SetBool(GameManager.s_charge_normal,false);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (cleanFinished) return;
        if (animator.IsInTransition(0) || isFinished || 
            Time.time - _enteredTime < _hero.CurrentAttackMotionData.chargeComboDelay) return;
        //구르기 필터링 
        if (_hero.Get_IsRollTiming())
        {
            Set_Roll(animator,false);
            cleanFinished = true;
            return;
        }
        //트렌지션 끝나고 한번 실행
        if (!_realEntered)
        {
            _realEntered = true;
            _hero.Equipment_UpdateTrail(_hero.Get_CurrentWeaponPack(),false,false,false);
            
            
            if (_hero.Get_LastWeaponPack() == _hero.weaponPack_StrongL ||
                _hero.Get_LastWeaponPack() == _hero.weaponPack_StrongR ||
                _hero.Get_CurrentWeaponPack() == _hero.weaponPack_StrongL ||
                _hero.Get_CurrentWeaponPack() == _hero.weaponPack_StrongR)
            {
                if (GameManager.DelayCheck_Attack() >= _heroData.preinput_attack && !GameManager.BTN_Attack)
                {
                    Set_Locomotion();
                    return;
                }
            }
            _hero.Equipment_Equip(_hero.weaponPack_Normal);
        }
        //공격 입력이 확인되면 다음 모션으로 이동
        if (GameManager.DelayCheck_Attack() < _heroData.preinput_attack)
        {
            //기본 공격
            if(!_hero.Get_Charged()) animator.SetTrigger(GameManager.s_transition);
            //강 공격
            else _hero.Set_AnimationState(Hero.AnimationState.Attack_Strong);
            isFinished = true;
        }
        //대기시간 지나고 입력 없으면 기본 자세로 복귀
        else if (_enteredTime + _hero.CurrentAttackMotionData.chargeWaitDuration < Time.time && !GameManager.BTN_Attack)
        {
            Set_Locomotion();
        }
        
    }
}
