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
    
    //Public
    public enum AnimationState
    {
        Locomotion = 0,
        Roll = 1,
        Attack_Normal = 2,
        Attack_Strong = 3
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
    
    //Setter
    public void Set_AnimationState(AnimationState animationState)
    {
        _animator.SetInteger(GameManager.s_state_type, (int)animationState);
        _animator.SetTrigger(GameManager.s_state_change);
    }
    public void Set_CurrentAttackMotionData(PlayerAttackMotionData data)
    {
        CurrentAttackMotionData = data;
    }
    public void Set_AttackIndex(int attackIndex)
    {
        AttackIndex = attackIndex;
    }
    
    //Core
    public void Core_PreInput()
    {
        
        if (GameManager.DelayCheck_Attack() < heroData.preinput_attack)
        {
            Core_NormalAttack();
        }
        else if (Get_CurrentRollDelay() < heroData.preinput_roll)
        {
            Core_Roll();
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
            _animator.SetInteger(GameManager.s_chargeenterindex,-1);
            Set_AnimationState(AnimationState.Attack_Normal);
        }
    }
    public void Core_StrongAttack()
    {
        if (HeroMoveState is MoveState.Locomotion or MoveState.Roll && Get_Charged())
            Set_AnimationState(AnimationState.Attack_Strong);
        else Core_NormalAttack();
    }
    public void Core_Roll()
    {
        AttackIndex = -1;
        if(_currentWeaponPack!=null) Equipment_UpdateTrail(_currentWeaponPack,false,false,false);
        Equipment_Equip(null);
        Set_AnimationState(AnimationState.Roll);
        _actionBeginTime = -100;
        return;
    }
    [Button]
    public void Core_Hit_Normal()
    {
        //감속시킨다.
        _speedRatio *=0.35f;
        _animator.SetFloat(GameManager.s_crouch,0.6f);
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
    }
    [Button]
    public void Core_Hit_Strong(AttackMotionType attackMotionType,PlayerSmashedType playerSmashedType)
    {
        if (HeroMoveState == MoveState.Roll)
        {
            Core_Hit_Normal();
            return;
        }
        if (Time.time < _hitStrongTime + HitStrongDelay) return;
        _fastRoll = 2;
        _animBase.cleanFinished = true;
        _animBase.isFinished = true;
        _hitStrongTime = Time.time;
        _hitStrongType = (_hitStrongType + 1) % 2;
        AttackIndex = -1;
        _animator.SetBool(GameManager.s_leftstate,false);
        if (_currentWeaponPack != null) Equipment_UpdateTrail(_currentWeaponPack,false,false,false);
        Equipment_Equip(null);
        
        _animator.SetBool(GameManager.s_hit,true);
        _animator.SetTrigger(GameManager.s_state_change);
        //_animator.SetFloat(GameManager.s_hit_rot,1);
        if (playerSmashedType == PlayerSmashedType.None)
        {
            _animator.SetInteger(GameManager.s_hit_type,_hitStrongType);
            Tween_Punch_Down(1.05f);
        }
        else
        {
            _animator.SetInteger(GameManager.s_hit_type,(int)playerSmashedType);
            Tween_Punch_Down(1.1f);
        }
        //타겟 벡터
        Vector3 hitpoint = GameManager.V3_Zero;
        Vector3 targetHitVec = hitpoint-transform.position;
        targetHitVec.y = 0;

        Vector3 myLookVec = transform.forward;
        float targetHitDeg = Mathf.Atan2(targetHitVec.z, targetHitVec.x)*Mathf.Rad2Deg;
        float myLookDeg = Mathf.Atan2(myLookVec.z, myLookVec.x) * Mathf.Rad2Deg;
        float degDiff = targetHitDeg - myLookDeg + (int)attackMotionType;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        degDiff /= 180.0f;
        _animator.SetFloat(GameManager.s_hit_rot,degDiff);
        _animator.SetTrigger(GameManager.s_hit_additive);
        
        bool isBloodBottom = playerSmashedType is PlayerSmashedType.None or PlayerSmashedType.Bound or PlayerSmashedType.Stun;
        Effect_Hit_Strong(isBloodBottom);
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
        bool isTimting = Time.unscaledTime - _actionBeginTime < heroData.dash_roll_delay;
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
        bool canRoll = HeroMoveState == MoveState.Locomotion;
        if (isTimting)
        {
            if (canRoll) Core_Roll();
            else _inputTimeAction = Time.time;
        }
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
    
    //애니메이션 이벤트
    public void FallDown()
    {
        _falledTime = Time.time;
        Transform t = transform;
        p_smoke.transform.SetPositionAndRotation(t.position + t.forward*-0.2f ,t.rotation);
        p_smoke.Play();   
    }
    

    
}
