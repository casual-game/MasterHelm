using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Attack_Normal_Main : HeroAnim_Base
{
    private bool _isLastAttack,_trailFinished;
    private float _enteredTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        movement.attackIndex++;
        movement.currentAttackMotionData = movement.weaponPack_Main.PlayerAttackMotionDatas[movement.attackIndex];
        _isLastAttack = movement.weaponPack_Main.PlayerAttackMotionDatas.Count - 1 == movement.attackIndex;
        _enteredTime = Time.time;
        _trailFinished = false;
        animator.speed = movement.currentAttackMotionData.playSpeed;

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
        //Trail
        bool trail_weaponL = false, trail_weaponR = false, trail_shield = false;
        foreach (var trailData in movement.currentAttackMotionData.TrailDatas)
        {
            bool canTrail = trailData.trailRange.x <= normalizedTime && normalizedTime < trailData.trailRange.y;
            if (canTrail)
            {
                trail_weaponL = trailData.weaponL;
                trail_weaponR = trailData.weaponR;
                trail_shield = trailData.shield;
                break;
            }
        }
        movement.UpdateTrail(movement.weaponPack_Main,trail_weaponL,trail_weaponR,trail_shield);
        movement.Move_Nav(animator.deltaPosition*movement.currentAttackMotionData.moveSpeed,animator.rootRotation);
        //isFinished 가 False이면 안함
        if (isFinished) return;
        if (!_isLastAttack && normalizedTime > movement.currentAttackMotionData.inputRatio)
        {
            //타이밍이 되면 무조건 Charge모션으로 넘어갑니다.
            animator.SetTrigger(GameManager.s_transition);
            isFinished = true;
        }
        else if (_isLastAttack && normalizedTime > movement.currentAttackMotionData.inputRatio)
        {
            movement.UpdateTrail(movement.weaponPack_Main,false,false,false);
            movement.ChangeAnimationState(HeroMovement.AnimationState.Locomotion);
            movement.Equip(null);
            movement.attackIndex = -1;
            isFinished = true;
        }
    }
}
