using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Attack_Strong : HeroAnim_Base
{
    private bool _weaponOff;
    private Data_WeaponPack _weaponPack;
    public bool isLeft;
    
    private float _lookF;
    private Transform _lookT;
    private Quaternion _lookRot;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GameManager.AttackReleasedTime = -100;
        _weaponPack = isLeft? movement.weaponPack_StrongL: movement.weaponPack_StrongR;
        movement.Set_AttackIndex(movement.AttackIndex+1);
        movement.Set_CurrentAttackMotionData(_weaponPack.playerAttackMotionData_Strong);
        _weaponOff = false;
        animator.speed = movement.CurrentAttackMotionData.playSpeed;
        animator.ResetTrigger(GameManager.s_turn);
        animator.SetBool(GameManager.s_charge_normal,false);
        animator.SetBool(GameManager.s_leftstate,movement.CurrentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState);
        movement.Effect_Smoke(0.25f);
        movement.Equipment_Equip(_weaponPack);
        movement.Effect_Change();
        movement.Equipment_Collision_Reset(_weaponPack);

        Set_LookAt(ref movement.Get_LookT(), ref movement.Get_LookF(),movement.AttackIndex ==0);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (cleanFinished) return;
        //강공격으로 캔슬할 경우 처리
        if (!IsNotAvailable(animator,stateInfo) && GameManager.DelayCheck_Attack() < hero.preinput_attack && movement.Get_Charged())
        {
            movement.Equipment_UpdateTrail(_weaponPack,false,false,false);
            movement.Set_AnimationState(Hero.AnimationState.Attack_Strong);
            isFinished = true;
            return;
        }
        
        float normalizedTime = stateInfo.normalizedTime;
        bool isCurrentWeapon = movement.Get_CurrentWeaponPack() == _weaponPack;
        //무기 먼저 Dissolve Out. 설정에 따라 발동 안할수도 있음. 이 경우, Charge에서 해준다.
        if (!_weaponOff && isCurrentWeapon && normalizedTime > _weaponPack.weaponOffRatio)
        {
            _weaponOff = true;
            movement.Equipment_UpdateTrail(movement.Get_CurrentWeaponPack(),false,false,false);
            movement.Equipment_Equip(movement.weaponPack_Normal);
        }
        //핵심
        Update_Trail(normalizedTime,_weaponPack);
        if (_weaponOff || isCurrentWeapon)
        {
            Update_LookDeg(ref movement.Get_LookT(), ref movement.Get_LookF(),ref movement.Get_LookRot());
            movement.Move_Nav(animator.deltaPosition*movement.CurrentAttackMotionData.moveSpeed,movement.Get_LookRot());
        }
        
        
        
        if (IsNotAvailable(animator,stateInfo)) return; 
        //종료 설정
        if (normalizedTime > movement.CurrentAttackMotionData.transitionRatio)
        {
            int targetIndex;
            bool checkLeft = movement.CurrentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState;
            if (checkLeft) targetIndex = movement.Get_LeftEnterIndex();
            else targetIndex = movement.Get_RightEnterIndex();
            animator.SetInteger(GameManager.s_chargeenterindex,targetIndex);
            movement.Set_AnimationState(Hero.AnimationState.Attack_Normal);
            movement.Set_AttackIndex(targetIndex);
            isFinished = true;
        }
        
    }
}