using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Attack_Normal_Main : HeroAnim_Base
{
    private bool _isLastAttack,_trailFinished,_strongFinished;
    private float _enteredTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GameManager.AttackReleasedTime = -100;
        movement.Set_AttackIndex(movement.AttackIndex+1);
        movement.Set_CurrentAttackMotionData(movement.weaponPack_Normal.PlayerAttackMotionDatas_Normal[movement.AttackIndex]);
        _isLastAttack = movement.weaponPack_Normal.PlayerAttackMotionDatas_Normal.Count - 1 == movement.AttackIndex;
        _enteredTime = Time.time;
        _trailFinished = false;
        _strongFinished = false;
        animator.speed = movement.CurrentAttackMotionData.playSpeed;
        animator.SetBool(GameManager.s_leftstate,movement.CurrentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState);
        movement.Equipment_Equip(movement.weaponPack_Normal);
        movement.Equipment_Collision_Reset(movement.weaponPack_Normal);
        if (movement.AttackIndex == 0) movement.Effect_Smoke(0.25f);
        if (_isLastAttack)
        {
            movement.Effect_Smoke();
            movement.p_charge.Play();
            movement.trailEffect.active = true;
            movement.Tween_Blink_Evade(1.0f);
        }
        
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (cleanFinished) return;
        if (_strongFinished) return;
        //마지막 공격일때.. Trail 끄기, 필터링
        if (_isLastAttack)
        {
            if (Time.time - _enteredTime > 0.3f && !_trailFinished)
            {
                _trailFinished = true;
                movement.trailEffect.active = false;
            }
            if (IsNotAvailable(animator,stateInfo)) return; 
        }
        //isFinished 가 False여도 동작한다.
        float normalizedTime = stateInfo.normalizedTime;
        UpdateTrail(normalizedTime,movement.weaponPack_Normal);
        movement.Move_Nav(animator.deltaPosition*movement.CurrentAttackMotionData.moveSpeed,animator.rootRotation);
        //차지 공격으로 캔슬한 경우
        if (GameManager.DelayCheck_Attack() < hero.preinput_attack && movement.Get_Charged() && !GameManager.BTN_Attack)
        {
            movement.Equipment_UpdateTrail(movement.weaponPack_Normal,false,false,false);
            movement.Set_AnimationState(Hero.AnimationState.Attack_Strong);
            isFinished = true;
            _strongFinished = true;
        }
        //구르기 입력 확인, 구르기
        if (_isLastAttack && Time.time - _enteredTime > movement.CurrentAttackMotionData.chargeWaitDuration 
                          && movement.Get_CurrentRollDelay() < hero.preinput_roll)
        {
            movement.Core_Roll();
            isFinished = true;
            _strongFinished = true;
            return;
        }
        //isFinished 가 False이면 안함
        if (isFinished) return;
        //타이밍이 되면 무조건 Charge모션으로 넘어갑니다.
        if (!_isLastAttack && normalizedTime > movement.CurrentAttackMotionData.transitionRatio)
        {
            
            animator.SetTrigger(GameManager.s_transition);
            isFinished = true;
        }
        else if (_isLastAttack && normalizedTime > movement.CurrentAttackMotionData.transitionRatio)
        {
            Set_Locomotion();
        }
    }
}
