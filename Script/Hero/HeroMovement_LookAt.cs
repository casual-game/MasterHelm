using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public partial class HeroMovement : MonoBehaviour
{
    private void Setting_LookAt()
    {
        GameManager.instance.E_BTN_Attack_Begin.AddListener(E_BTN_Attack_Pressed);
        GameManager.instance.E_BTN_Attack_Fin.AddListener(E_BTN_Attack_Released);
        _lookAtIK = GetComponent<LookAtIK>();
        Transform lookRootT = transform.Find("LookAt");
        _lookDisplayT = lookRootT.Find("LookDisplay");
        _lookTargetT = lookRootT.Find("LookTarget");
        _lookIcon = _lookDisplayT.GetChild(0);
        
        _lookScale = 0;
        _lookIcon.localScale = Vector3.zero;
        _lookAtIK.solver.SetLookAtWeight(0);
        _lookDisplayT.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
    }
    
    //Private
    private LookAtIK _lookAtIK;
    private Transform _lookDisplayT,_lookTargetT;
    private Transform _lookIcon;
    private float _lookDisplayRefDeg,_lookTargetRefDeg;
    private float _lookScale = 0;
    private float _chargeBeginTime = -100;
    private bool _charged = false;
    
    //Event
    private void E_BTN_Attack_Pressed()
    {
        bool canChargeMotion = HeroMoveState == MoveState.Locomotion || HeroMoveState == MoveState.Roll;
        if(canChargeMotion) _animator.SetBool(GameManager.s_charge_normal,true);
        
        p_charge_begin.Play();
        _charged = false;
        _chargeBeginTime = Time.time;
        E_BTN_Attack_RemoveListner();
        GameManager.instance.E_LateUpdate.AddListener(E_BTN_Attack_PressedUpdate);
    }
    private void E_BTN_Attack_Released()
    {
        //파티클, 애니메이션 원상복구
        _animator.SetBool(GameManager.s_charge_normal,false);
        p_charge_begin.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        p_charge_fin.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        //공격 설정. 상황 판단은 해당 함수 내부에서 체크한다.
        if (!_charged)
        {
            p_charge_Impact.transform.localScale = Vector3.one * 0.35f;
            p_charge_Impact.Play();
            Core_NormalAttack();
        }
        else Core_StrongAttack();
        //이벤트 삭제
        E_BTN_Attack_RemoveListner();
        GameManager.instance.E_LateUpdate.AddListener(E_BTN_Attack_ReleasedUpdate);
    }
    private void E_BTN_Attack_PressedUpdate()
    {
        bool canChargeMotion = HeroMoveState == MoveState.Locomotion || HeroMoveState == MoveState.Roll;
        //공격 조이스틱 각도 계산,Display 회전
        float jsDeg;
        if (!GameManager.Bool_Attack)
        {
            jsDeg = transform.rotation.eulerAngles.y;
        }
        else
        {
            jsDeg = Mathf.Atan2(GameManager.JS_Attack.y, GameManager.JS_Attack.x) * Mathf.Rad2Deg +
                    CamArm.instance.transform.rotation.eulerAngles.y;
            jsDeg = -jsDeg + 180;
        }
        
        float targetDisplayDeg = Mathf.SmoothDampAngle(_lookDisplayT.eulerAngles.y, 
            jsDeg, ref _lookDisplayRefDeg, _hero.lookDisplayDuration);
        _lookDisplayT.rotation = Quaternion.Euler(0,targetDisplayDeg,0);
        //플레이어 각도와의 차이 계산, Target 회전
        float degDiff;
        if (!GameManager.Bool_Attack)
        {
            degDiff = 0;
        }
        else
        {
            degDiff = -transform.rotation.eulerAngles.y+jsDeg;
            while (degDiff < -180) degDiff += 360;
            while (degDiff > 180) degDiff -= 360;
            if (_hero.lookRangeDeadZone.x > degDiff || degDiff > _hero.lookRangeDeadZone.y)
            {
                if (Mathf.DeltaAngle(transform.rotation.eulerAngles.y, _lookTargetT.rotation.eulerAngles.y) > 0)
                    degDiff = _hero.lookRange.y;
                else degDiff = _hero.lookRange.x;
            }
            degDiff = Mathf.Clamp(degDiff,_hero.lookRange.x, _hero.lookRange.y);
        } 
        float currentDiff = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, _lookTargetT.eulerAngles.y);
        float targetTargetDeg = Mathf.SmoothDamp(transform.rotation.eulerAngles.y + currentDiff, 
            transform.rotation.eulerAngles.y + degDiff, ref _lookTargetRefDeg, _hero.lookTargetDuration);
        _lookTargetT.rotation = Quaternion.Euler(0,targetTargetDeg,0);
        //크기 조정
        if (_lookScale < 1)
        {
            _lookScale += 5 * Time.deltaTime;
            _lookScale = Mathf.Clamp01(_lookScale);
            _lookIcon.localScale = Vector3.one * _lookScale;
            if(canChargeMotion) _lookAtIK.solver.SetLookAtWeight(_lookScale);
            else _lookAtIK.solver.SetLookAtWeight(0);
        }
        //차지
        if (!_charged && Time.time > _chargeBeginTime + _hero.chargeDuration)
        {
            _charged = true;
            p_charge_fin.Play();
            p_charge_Impact.transform.localScale = Vector3.one * 0.5f;
            p_charge_Impact.Play();
        }
    }
    private void E_BTN_Attack_ReleasedUpdate()
    {
        bool canChargeMotion = HeroMoveState == MoveState.Locomotion || HeroMoveState == MoveState.Roll;
        float eulery = transform.rotation.eulerAngles.y;
        float targetDeg = Mathf.SmoothDampAngle(_lookDisplayT.eulerAngles.y, 
            eulery, ref _lookDisplayRefDeg, _hero.lookDisplayDuration);
        float targetTargetDeg = Mathf.SmoothDamp(_lookTargetT.eulerAngles.y, 
            eulery, ref _lookTargetRefDeg, _hero.lookTargetDuration);
        //크기 조정
        if (_lookScale > 0)
        {
            _lookScale -= 3 * Time.deltaTime;
            _lookScale = Mathf.Clamp01(_lookScale);
        }
        //종료
        bool correctDeg = Mathf.DeltaAngle(targetTargetDeg, eulery) < 0.1f;
        if (_lookScale < 0.01f && correctDeg)
        {
            _lookScale = 0;
            _lookIcon.localScale = Vector3.zero;
            _lookAtIK.solver.SetLookAtWeight(0);
            _lookDisplayT.rotation = Quaternion.Euler(0,eulery,0);
            _lookTargetT.rotation = Quaternion.Euler(0,eulery,0);
            GameManager.instance.E_LateUpdate.RemoveListener(E_BTN_Attack_ReleasedUpdate);
        }
        else
        {
            _lookIcon.localScale = Vector3.one * _lookScale;
            _lookDisplayT.rotation = Quaternion.Euler(0,targetDeg,0);
            _lookTargetT.rotation = Quaternion.Euler(0,targetTargetDeg,0);
            if(canChargeMotion) _lookAtIK.solver.SetLookAtWeight(_lookScale);
            else _lookAtIK.solver.SetLookAtWeight(0);
        }
    }
    private void E_BTN_Attack_RemoveListner()
    {
        GameManager.instance.E_LateUpdate.RemoveListener(E_BTN_Attack_PressedUpdate);
        GameManager.instance.E_LateUpdate.RemoveListener(E_BTN_Attack_ReleasedUpdate);
    }
    
    //Getter
    public bool Get_Charged()
    {
        return _charged;
    }
}
