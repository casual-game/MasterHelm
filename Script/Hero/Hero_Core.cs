using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Hero : MonoBehaviour
{
    private void Setting_Core()
    {
        AttackIndex = -1;
    }
    //Const, ReadOnly
    private const float HitStrongDelay = 0.2f;
    
    //Private 
    private float _actionBeginTime = -100;//액션 버튼 입력 시간
    private float _inputTimeAction = -100;//액션 버튼 입력~해제까지 걸린 시간
    private int _hitStrongType = 0; //강한 히트 모션은 2가지가 있다. 해당 종류를 설정한다.
    private float _hitStrongTime = -100; //히트는 HeroMovement 간격으로 호출 가능하다. 마지막 호출 시간 저장.
    private int _fastRoll = 0; //빠른 구르기는 한번에 눌러야 한다. 입력 횟수 카운트용.
    private float _falledTime = -100;//빠른 구르기를 위한 낙하 타이밍 저장.
    private float _rolledTime;
    private Transform _rollLookT = null;
    //Public
    public enum AnimationState
    {
        Locomotion = 0,
        Attack_Normal = 1,
        Attack_Strong = 2
    }
    public PlayerAttackMotionData CurrentAttackMotionData
    {
        get;
        private set;
    }
    public int AttackIndex
    {
        get;
        private set;
    }
    
    //Getter
    public float Get_CurrentRollDelay()
    {
        return Time.time - _inputTimeAction;
    }
    public bool Get_IsRollTiming()
    {
        return Time.unscaledTime - _actionBeginTime < heroData.dash_roll_delay
                         && Time.unscaledTime - _rolledTime > heroData.roll_delay;
    }

    public Transform Get_RollLookT()
    {
        return _rollLookT;
    }
    //Setter
    public void Set_CurrentAttackMotionData(PlayerAttackMotionData data)
    {
        CurrentAttackMotionData = data;
    }
    public void Set_AttackIndex(int attackIndex)
    {
        AttackIndex = attackIndex;
    }
    public void Set_RolledTime()
    {
        _rolledTime = Time.unscaledTime;
    }
    public void Set_RoolLookT(Transform t)
    {
        _rollLookT = t;
    }
    //Core
    public void Core_PreInput()
    {
        
        if (GameManager.DelayCheck_Attack() < heroData.preinput_attack)
        {
            Core_NormalAttack();
        }
        else
        {
            if(GameManager.BTN_Attack && p_charge_begin.isPlaying) _animator.SetBool(GameManager.s_charge_normal,true);
        }
        
    }
    public void Core_ResetRollInput()
    {
        _inputTimeAction = -100;
    }
    public void Core_NormalAttack()
    {
        if (HeroMoveState == MoveState.Locomotion)
        {
            CamArm.instance.Tween_CamAttackVec(true);
            GameManager.Reset_AttackRealeasedTime();
            _animator.SetInteger(GameManager.s_chargeenterindex,-1);
            Deactivate_CustomMaterial();
            _animator.SetInteger(GameManager.s_state_type, (int)AnimationState.Attack_Normal);
            _animator.SetTrigger(GameManager.s_state_change);
            if(Get_LookDeg().HasValue) Set_UseManualTargeting(Get_LookDeg().Value);
            else Set_UseAutoTargeting();
        }
    }
    public void Core_StrongAttack()
    {
        if (HeroMoveState is MoveState.Locomotion or MoveState.Roll or MoveState.RollJust && Get_Charged())
        {
            CamArm.instance.Tween_CamAttackVec(true);
            GameManager.Reset_AttackRealeasedTime();
            Activate_SuperArmor();
            _animator.SetInteger(GameManager.s_state_type, (int)AnimationState.Attack_Strong);
            _animator.SetTrigger(GameManager.s_state_change);
            if(Get_LookDeg().HasValue) Set_UseManualTargeting(Get_LookDeg().Value);
            else Set_UseAutoTargeting();
        }
        else Core_NormalAttack();
    }
    public bool Core_Hit_Normal()
    {
        CamArm.instance.Tween_CamAttackVec(false);
        //감속시킨다.
        _speedRatio *=0.35f;
        //타겟 벡터
        Vector3 hitpoint = GameManager.V3_Zero;
        Vector3 targetHitVec = hitpoint-transform.position;
        targetHitVec.y = 0;

        Vector3 myLookVec = transform.forward;
        float targetHitDeg = Mathf.Atan2(targetHitVec.z, targetHitVec.x)*Mathf.Rad2Deg;
        float myLookDeg = Mathf.Atan2(myLookVec.z, myLookVec.x) * Mathf.Rad2Deg;
        float degDiff = targetHitDeg - myLookDeg;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        degDiff /= 180.0f;
        _animator.SetFloat(GameManager.s_hit_rot,degDiff);
        _animator.SetTrigger(GameManager.s_hit_additive);
        Tween_Punch_Down_Compact(1.5f);
        Effect_Hit_Normal();
        return true;
    }
    public bool Core_Hit_Strong(TrailData_Monster trailData, Vector3 hitPoint)
    {
        //저스트 회피,중복히트 필터링
        if (Time.time < _hitStrongTime + HitStrongDelay || HeroMoveState == MoveState.RollJust || !_spawned) return false;
        //구르기, 슈퍼아머 처리.
        if (_superarmor || HeroMoveState == MoveState.Roll)
        {
            float dmg = Random.Range(trailData.damage.x, trailData.damage.y + 1);
            frameMain.HP_Damage(Mathf.RoundToInt(dmg*heroData.RollDamageRatio));
            Tween_Punch_Down(1.5f);
            Effect_Hit_SuperArmor();
            CamArm.instance.Tween_ShakeSuperArmor();
            GameManager.Instance.Combo(GameManager.s_superarmor);
            return true;
        }
        //각종 변수,데이터 설정
        bool alive = frameMain.HP_Damage(Random.Range(trailData.damage.x,trailData.damage.y+1));
        if (!alive)
        {
            Despawn().Forget();
            _spawned = false;
            return true;
        }
        CamArm.instance.Tween_CamAttackVec(false);
        Set_HeroMoveState(MoveState.Hit);
        _fastRoll = 2;
        _animBase.cleanFinished = true;
        _animBase.isFinished = true;
        _hitStrongTime = Time.time;
        _hitStrongType = (_hitStrongType + 1) % 2;
        AttackIndex = -1;
        //장비 초기화
        Equipment_Equip(null,false);
        if (weaponPack_Normal != null) Equipment_UpdateTrail(weaponPack_Normal,false,false,false);
        if (weaponPack_StrongL != null) Equipment_UpdateTrail(weaponPack_StrongL,false,false,false);
        if (weaponPack_StrongR != null) Equipment_UpdateTrail(weaponPack_StrongR,false,false,false);
        //Animator 초기화
        _animator.SetBool(GameManager.s_leftstate,false);
        _animator.SetBool(GameManager.s_hit,true);
        _animator.SetTrigger(GameManager.s_state_change);
        //히트 처리
        if (trailData.hitType == HitType.Normal)
        {
            //상대적 벡터 계산
            Vector3 targetHitVec = hitPoint-transform.position;
            targetHitVec.y = 0;
            Vector3 myLookVec = transform.forward;
            float targetHitDeg = Mathf.Atan2(targetHitVec.z, targetHitVec.x)*Mathf.Rad2Deg;
            float myLookDeg = Mathf.Atan2(myLookVec.z, myLookVec.x) * Mathf.Rad2Deg;
            float degDiff = targetHitDeg - myLookDeg + (int)trailData.attackMotionType;
            while (degDiff < -180) degDiff += 360;
            while (degDiff > 180) degDiff -= 360;
            degDiff /= 180.0f;
            _animator.SetFloat(GameManager.s_hit_rot,degDiff);
            //각종 처리
            _animator.SetInteger(GameManager.s_hit_type,_hitStrongType);
            Tween_Punch_Down(1.4f);
            CamArm.instance.Tween_ShakeNormal_Hero();
            SoundManager.Play(sound_voice_hit_normal);
            SoundManager.Play(SoundManager.instance.sound_hit_normal);
        }
        else
        {
            //회전
            Vector3 lookVec = hitPoint - transform.position;
            lookVec.y = 0;
            transform.rotation = Quaternion.LookRotation(lookVec);
            //각종 처리
            _animator.SetInteger(GameManager.s_hit_type,(int)trailData.hitType);
            Tween_Punch_Down(1.1f);
            CamArm.instance.Tween_ShakeStrong_Hero();
            SoundManager.Play(sound_voice_hit_strong);
            SoundManager.Play(SoundManager.instance.sound_hit_smash);
        }
        bool isBloodBottom = trailData.hitType is HitType.Normal or HitType.Bound or HitType.Stun;
        Effect_Hit_Strong(isBloodBottom);
        return true;
    }
    
    //이벤트 시스템
    private void E_BTN_Action_Begin()
    {
        _actionBeginTime = Time.unscaledTime;
    }
    private void E_BTN_Action_Fin()
    {
        _fastRoll = Mathf.Clamp(_fastRoll-1, 0, 3);
       Vector3 currentPos= transform.position;
        bool isTimting = Time.unscaledTime - _actionBeginTime < heroData.dash_roll_delay
            && Time.unscaledTime - _rolledTime > heroData.roll_delay;
        //인터렉션
        if (Interactable.currentInteractable != null)
        {
            Vector3 interactablePos;
            interactablePos = Interactable.currentInteractable.transform.position;
            interactablePos.y =currentPos.y;
            bool canInteract = HeroMoveState == MoveState.Locomotion 
                               && Vector3.Dot(transform.forward, (interactablePos - currentPos).normalized) > 0.3f;
            if (canInteract && isTimting)
            {
                CurrentLadder = (Ladder)Interactable.currentInteractable;
                float currentY = currentPos.y;
                float dist_bottom = Mathf.Abs(CurrentLadder.range.x - currentY);
                float dist_top = Mathf.Abs(CurrentLadder.range.y - currentY);
                _animator.SetFloat(GameManager.s_ladder_speed, (dist_bottom < dist_top) ? 1 : -1);
                _animator.SetBool(GameManager.s_ladder, true);
                Interactable.currentInteractable.Interact();
                _actionBeginTime = -100;
                return;
            }
        }

        //구르기
        bool canRoll = HeroMoveState != MoveState.Hit && HeroMoveState != MoveState.Roll
                                                      && HeroMoveState != MoveState.RollJust
                                                      && !_animator.GetBool(GameManager.s_hit)
                                                      && !_animator.GetBool(GameManager.s_death);
        /*
        if (isTimting)
        {
            if (canRoll) Core_Roll();
            else _inputTimeAction = Time.time;
        }
        */

        //Smashed이후 빠른 기상
        bool canSpeedRoll = Time.time < _falledTime + heroData.hit_Smash_RecoveryInputDelay 
                            && HeroMoveState == MoveState.Hit && _fastRoll>0;
        if (canSpeedRoll)
        {
            Tween_Blink_Evade(1.0f);
            _animator.SetBool(GameManager.s_hit,false);
            _animator.SetFloat(GameManager.s_hit_rot,-1);
            _animBase.isFinished = true;
            return;
        }
    }
    
    

    
}
