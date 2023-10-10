using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Attack_Normal_Charge : HeroAnim_Base
{
    private float _enteredTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _enteredTime = Time.time;
        animator.speed = 0.75f;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (animator.IsInTransition(0) || isFinished) return;
        movement.UpdateTrail(movement.weaponPack_Main,false,false,false);
        //공격 입력이 확인되면 다음 모션으로 이동
        if (GameManager.DelayCheck_Attack() < hero.preinput_attack)
        {
            animator.SetTrigger(GameManager.s_transition);
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
