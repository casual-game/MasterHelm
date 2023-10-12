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
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (animator.IsInTransition(0) || isFinished || 
            Time.time - _enteredTime < movement.currentAttackMotionData.chargeComboDelay) return;
        if (!_realEntered)
        {
            _realEntered = true;
            movement.UpdateTrail(movement.Get_CurrentWeaponPack(),false,false,false);
            movement.Equip(movement.weaponPack_Normal);
        }
        
        //공격 입력이 확인되면 다음 모션으로 이동
        if (GameManager.DelayCheck_Attack() < hero.preinput_attack)
        {
            //기본 공격
            if(!movement.IsCharged()) animator.SetTrigger(GameManager.s_transition);
            //강 공격
            else movement.ChangeAnimationState(HeroMovement.AnimationState.Attack_Strong);
            isFinished = true;
        }
        //대기시간 지나고 입력 없으면 기본 자세로 복귀
        else if (_enteredTime + movement.currentAttackMotionData.chargeWaitDuration < Time.time && !GameManager.BTN_Attack)
        {
            movement.ChangeAnimationState(HeroMovement.AnimationState.Locomotion);
            movement.Equip(null);
            movement.attackIndex = -1;
            isFinished = true;
        }
        
    }
}
