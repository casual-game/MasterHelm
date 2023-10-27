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
        _weaponPack = isLeft? _hero.weaponPack_StrongL: _hero.weaponPack_StrongR;
        _hero.Set_AttackIndex(_hero.AttackIndex+1);
        _hero.Set_CurrentAttackMotionData(_weaponPack.playerAttackMotionData_Strong);
        _weaponOff = false;
        animator.speed = _hero.CurrentAttackMotionData.playSpeed;
        animator.ResetTrigger(GameManager.s_turn);
        animator.SetBool(GameManager.s_charge_normal,false);
        animator.SetBool(GameManager.s_leftstate,_hero.CurrentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState);
        _hero.Effect_Smoke(0.25f);
        _hero.Equipment_Equip(_weaponPack);
        _hero.Effect_Change();
        _hero.Equipment_Collision_Reset(_weaponPack);
        

        Set_LookAt(ref _hero.Get_LookT(), ref _hero.Get_LookF(),_hero.AttackIndex ==0);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (cleanFinished) return;
        //강공격으로 캔슬할 경우 처리
        if (!IsNotAvailable(animator,stateInfo) && GameManager.DelayCheck_Attack() < _heroData.preinput_attack && _hero.Get_Charged())
        {
            _hero.Equipment_UpdateTrail(_weaponPack,false,false,false);
            _hero.Set_AnimationState(Hero.AnimationState.Attack_Strong);
            isFinished = true;
            return;
        }
        
        float normalizedTime = stateInfo.normalizedTime;
        bool isCurrentWeapon = _hero.Get_CurrentWeaponPack() == _weaponPack;
        //무기 먼저 Dissolve Out. 설정에 따라 발동 안할수도 있음. 이 경우, Charge에서 해준다.
        if (!_weaponOff && isCurrentWeapon && normalizedTime > _weaponPack.weaponOffRatio)
        {
            _weaponOff = true;
            _hero.Equipment_UpdateTrail(_hero.Get_CurrentWeaponPack(),false,false,false);
            _hero.Equipment_Equip(_hero.weaponPack_Normal);
        }
        //핵심
        Update_Trail(normalizedTime,_weaponPack);
        if (_weaponOff || isCurrentWeapon)
        {
            Update_LookDeg(ref _hero.Get_LookT(), ref _hero.Get_LookF(),ref _hero.Get_LookRot());
            _hero.Move_Nav(animator.deltaPosition*_hero.CurrentAttackMotionData.moveSpeed,_hero.Get_LookRot());
        }
        
        
        
        if (IsNotAvailable(animator,stateInfo)) return; 
        //종료 설정
        if (normalizedTime > _hero.CurrentAttackMotionData.transitionRatio)
        {
            int targetIndex;
            bool checkLeft = _hero.CurrentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState;
            if (checkLeft) targetIndex = _hero.Get_LeftEnterIndex();
            else targetIndex = _hero.Get_RightEnterIndex();
            animator.SetInteger(GameManager.s_chargeenterindex,targetIndex);
            _hero.Set_AnimationState(Hero.AnimationState.Attack_Normal);
            _hero.Set_AttackIndex(targetIndex);
            isFinished = true;
        }
        
    }
}