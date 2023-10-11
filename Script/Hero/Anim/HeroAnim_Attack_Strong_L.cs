using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Attack_Strong_L : HeroAnim_Base
{
    private bool _trailFinished,_weaponOff;
    private float _enteredTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        movement.attackIndex++;
        movement.currentAttackMotionData = movement.weaponPack_SkillL.playerAttackMotionData_Strong;
        _enteredTime = Time.time;
        _trailFinished = false;
        _weaponOff = false;
        animator.speed = movement.currentAttackMotionData.playSpeed;
        animator.SetBool(GameManager.s_leftstate,movement.currentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState);
        movement.Effect_Smoke(0.25f);
        movement.trailEffect.active = true;
        movement.Effect_FastRoll();
        movement.Equip(movement.weaponPack_SkillL);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return; 
        //Trail 끄기
        if (Time.time - _enteredTime > 0.3f && !_trailFinished)
        {
            _trailFinished = true;
            movement.trailEffect.active = false;
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
        movement.UpdateTrail(movement.weaponPack_SkillL,trail_weaponL,trail_weaponR,trail_shield);
        movement.Move_Nav(animator.deltaPosition*movement.currentAttackMotionData.moveSpeed,animator.rootRotation);
        //무기 Trail 종료
        if (!_weaponOff && normalizedTime > movement.weaponPack_SkillL.weaponOffRatio)
        {
            _weaponOff = true;
            movement.Equip(movement.weaponPack_Main);
        }
        //종료 설정
        if (normalizedTime > movement.currentAttackMotionData.inputRatio)
        {
            int targetIndex;
            bool checkLeft = movement.currentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState;
            if (checkLeft) targetIndex = movement.Get_LeftEnterIndex();
            else targetIndex = movement.Get_RightEnterIndex();
            animator.SetInteger(GameManager.s_chargeenterindex,targetIndex);
            movement.UpdateTrail(movement.weaponPack_Main,false,false,false);
            movement.ChangeAnimationState(HeroMovement.AnimationState.Attack_Normal);
            movement.attackIndex = targetIndex;
            isFinished = true;
        }
        
    }
}