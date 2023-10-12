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
        movement.attackIndex++;
        movement.currentAttackMotionData = movement.weaponPack_Normal.PlayerAttackMotionDatas_Normal[movement.attackIndex];
        _isLastAttack = movement.weaponPack_Normal.PlayerAttackMotionDatas_Normal.Count - 1 == movement.attackIndex;
        _enteredTime = Time.time;
        _trailFinished = false;
        _strongFinished = false;
        animator.speed = movement.currentAttackMotionData.playSpeed;
        animator.SetBool(GameManager.s_leftstate,movement.currentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState);
        if (_isLastAttack)
        {
            movement.Effect_Smoke();
            movement.p_charge.Play();
            movement.trailEffect.active = true;
            movement.Effect_FastRoll();
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
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
        movement.Move_Nav(animator.deltaPosition*movement.currentAttackMotionData.moveSpeed,animator.rootRotation);
        
        if (GameManager.DelayCheck_Attack() < hero.preinput_attack && movement.IsCharged() && !GameManager.BTN_Attack)
        {
            movement.UpdateTrail(movement.weaponPack_Normal,false,false,false);
            movement.ChangeAnimationState(HeroMovement.AnimationState.Attack_Strong);
            isFinished = true;
            _strongFinished = true;
        }
        //isFinished 가 False이면 안함
        if (isFinished) return;
        //타이밍이 되면 무조건 Charge모션으로 넘어갑니다.
        if (!_isLastAttack && normalizedTime > movement.currentAttackMotionData.transitionRatio)
        {
            
            animator.SetTrigger(GameManager.s_transition);
            isFinished = true;
        }
        else if (_isLastAttack && normalizedTime > movement.currentAttackMotionData.transitionRatio)
        {
            movement.UpdateTrail(movement.weaponPack_Normal,false,false,false);
            movement.ChangeAnimationState(HeroMovement.AnimationState.Locomotion);
            movement.Equip(null);
            movement.attackIndex = -1;
            isFinished = true;
        }
    }
}
