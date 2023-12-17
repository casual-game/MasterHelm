using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Attack_Normal_Main : HeroAnim_Base
{
    private bool _isLastAttack,_strongFinished;
    private float _enteredTime;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GameManager.AttackReleasedTime = -100;
        _hero.Set_AttackIndex(_hero.AttackIndex+1);
        _hero.Set_CurrentAttackMotionData(_hero.weaponPack_Normal.PlayerAttackMotionDatas_Normal[_hero.AttackIndex]);
        
        _isLastAttack = _hero.weaponPack_Normal.PlayerAttackMotionDatas_Normal.Count - 1 == _hero.AttackIndex;
        _enteredTime = Time.time;
        _strongFinished = false;
        animator.speed = _hero.CurrentAttackMotionData.playSpeed;
        animator.ResetTrigger(GameManager.s_turn);
        animator.SetBool(GameManager.s_charge_normal,false);
        animator.SetBool(GameManager.s_leftstate,_hero.CurrentAttackMotionData.playerAttackType_End == PlayerAttackType.LeftState);
        _hero.Equipment_Equip(_hero.weaponPack_Normal);
        _hero.Equipment_Collision_Reset(_hero.weaponPack_Normal);
        SoundManager.Play(_hero.sound_friction_cloth);
        if (_hero.AttackIndex == 0)
        {
            SoundManager.Play(_hero.sound_chain);
            _hero.Effect_Smoke(0.25f);
        }
        if (_isLastAttack)
        {
            _hero.Effect_Smoke();
            _hero.Particle_Charge_Main();
            _hero.Activate_SuperArmor();
            CamArm.instance.Tween_Skill();
            SoundManager.Play(_hero.sound_combat_skill);
        }
        else _hero.Deactivate_CustomMaterial();
        Set_LookAt(ref _hero.Get_LookT(), ref _hero.Get_LookF(),_hero.AttackIndex ==0);
        
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (cleanFinished) return;
        if (_strongFinished) return;
        //구르기 필터링 
        if (_hero.Get_IsRollTiming())
        {
            Set_Roll(animator);
            cleanFinished = true;
            _strongFinished = true;
            return;
        }
        //마지막 공격일때..필터링
        if (_isLastAttack && IsNotAvailable(animator, stateInfo)) return;
        //isFinished 가 False여도 동작한다.
        float normalizedTime = stateInfo.normalizedTime;
        Update_Trail(animator,stateInfo,normalizedTime,_hero.weaponPack_Normal);
        Update_LookDeg(ref _hero.Get_LookT(), ref _hero.Get_LookF(),ref _hero.Get_LookRot());
        _hero.Move_Nav(animator.deltaPosition*_hero.CurrentAttackMotionData.moveSpeed,_hero.Get_LookRot());
        
        //차지 공격으로 캔슬한 경우
        if (GameManager.DelayCheck_Attack() < _heroData.preinput_attack && _hero.Get_Charged() && !GameManager.BTN_Attack)
        {
            _hero.Equipment_UpdateTrail(_hero.weaponPack_Normal,false,false,false);
            Set_Attack_Strong(animator);
            isFinished = true;
            _strongFinished = true;
        }
        //isFinished 가 False이면 안함
        if (isFinished) return;
        //타이밍이 되면 무조건 Charge모션으로 넘어갑니다.
        if (!_isLastAttack && normalizedTime > _hero.CurrentAttackMotionData.transitionRatio)
        {
            animator.SetTrigger(GameManager.s_transition);
            isFinished = true;
        }
        else if (_isLastAttack && normalizedTime > _hero.CurrentAttackMotionData.transitionRatio)
        {
            Set_Locomotion(animator);
        }
    }
}
