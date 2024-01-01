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
    private bool _usedMP = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GameManager.AttackReleasedTime = -100;
        _weaponPack = isLeft? _hero.weaponPack_StrongL: _hero.weaponPack_StrongR;
        _hero.Set_AttackIndex(_hero.AttackIndex+1);
        _hero.Set_CurrentAttackMotionData(_weaponPack.playerAttackMotionData_Strong);
        
        _weaponOff = false;
        _usedMP = false;
        animator.speed = _hero.CurrentAttackMotionData.playSpeed;
        animator.ResetTrigger(GameManager.s_turn);
        animator.SetBool(GameManager.s_charge_normal,false);
        animator.SetBool(GameManager.s_leftstate,_hero.CurrentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState);
        _hero.Equipment_Equip(_weaponPack,false,2.0f,5.0f);
        
        _hero.Equipment_Collision_Reset(_weaponPack);
        _hero.frameMain.MP_Use();
        if (isLeft)
        {
            _hero.Particle_Charge_L();
            _hero.Set_CurrentTrail(_hero.weaponPack_StrongL.playerAttackMotionData_Strong.TrailDatas[0]);
        }
        else
        {
            _hero.Particle_Charge_R();
            _hero.Set_CurrentTrail(_hero.weaponPack_StrongR.playerAttackMotionData_Strong.TrailDatas[0]);
        }
        CamArm.instance.Tween_Skill();
        Set_LookAt(ref _hero.Get_LookT(), ref _hero.Get_LookF(),_hero.AttackIndex ==0);
        SoundManager.Play(_hero.sound_combat_skill);
        SoundManager.Play(_hero.sound_friction_cloth);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (cleanFinished) return;
        //구르기 필터링 
        if (_hero.Get_IsRollTiming())
        {
            Set_Roll(animator);
            cleanFinished = true;
            return;
        }
        //강공격으로 캔슬할 경우 처리
        if (!IsNotAvailable(animator,stateInfo) && GameManager.DelayCheck_Attack() < _heroData.preinput_attack 
                                                && _hero.Get_Charged() && !GameManager.BTN_Attack)
        {
            _hero.Equipment_UpdateTrail(_weaponPack,false,false,false);
            Set_Attack_Strong(animator);
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
        Update_Trail(animator,stateInfo,normalizedTime,_weaponPack);
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
            Set_Attack_Normal(animator);
            _hero.Set_AttackIndex(targetIndex);
            isFinished = true;
        }
        
    }
}