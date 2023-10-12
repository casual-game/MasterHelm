using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Attack_Strong : HeroAnim_Base
{
    private bool _trailFinished,_weaponOff;
    private float _enteredTime;
    private Data_WeaponPack _weaponPack;
    public bool isLeft;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GameManager.AttackReleasedTime = -100;
        _weaponPack = isLeft? movement.weaponPack_StrongL: movement.weaponPack_StrongR;
        movement.attackIndex++;
        movement.currentAttackMotionData = _weaponPack.playerAttackMotionData_Strong;
        _enteredTime = Time.time;
        _trailFinished = false;
        _weaponOff = false;
        animator.speed = movement.currentAttackMotionData.playSpeed;
        animator.SetBool(GameManager.s_leftstate,movement.currentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState);
        movement.Effect_Smoke(0.25f);
        movement.trailEffect.active = true;
        movement.Effect_FastRoll();
        movement.Equip(_weaponPack);
        movement.Effect_Change();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
       
        float normalizedTime = stateInfo.normalizedTime;
        //무기 먼저 Dissolve Out
        if (!_weaponOff && normalizedTime > _weaponPack.weaponOffRatio)
        {
            _weaponOff = true;
            movement.UpdateTrail(movement.Get_CurrentWeaponPack(),false,false,false);
            movement.Equip(movement.weaponPack_Normal);
        }
        //핵심
        UpdateTrail(normalizedTime,_weaponPack);
        movement.Move_Nav(animator.deltaPosition*movement.currentAttackMotionData.moveSpeed,animator.rootRotation);
        
        
        if (IsNotAvailable(animator,stateInfo)) return; 
        //플레이어 잔상 Trail 끄기
        if (Time.time - _enteredTime > 0.3f && !_trailFinished)
        {
            _trailFinished = true;
            movement.trailEffect.active = false;
        }
        //종료 설정
        if (normalizedTime > movement.currentAttackMotionData.transitionRatio)
        {
            int targetIndex;
            bool checkLeft = movement.currentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState;
            if (checkLeft) targetIndex = movement.Get_LeftEnterIndex();
            else targetIndex = movement.Get_RightEnterIndex();
            animator.SetInteger(GameManager.s_chargeenterindex,targetIndex);
            movement.ChangeAnimationState(HeroMovement.AnimationState.Attack_Normal);
            movement.attackIndex = targetIndex;
            isFinished = true;
        }
        
    }
}