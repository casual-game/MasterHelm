using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public partial class Hero : MonoBehaviour
{
    private void Setting_LookAt()
    {
        GameManager.Instance.E_BTN_Attack_Begin.AddListener(E_BTN_Attack_Pressed);
        GameManager.Instance.E_BTN_Attack_Fin.AddListener(E_BTN_Attack_Released);
        _lookAtIK = GetComponent<LookAtIK>();
        Transform lookRootT = transform.Find("LookAt");
        _lookDisplayT = lookRootT.Find("LookDisplay");
        _lookTargetT = lookRootT.Find("LookTarget");
        _lookIcon = _lookDisplayT.GetChild(0);
        
        _lookScale = 0;
        _lookIcon.localScale = GameManager.V3_Zero;
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
    private float? _lookDeg;
    private float _lookF;
    private Transform _lookT;
    private Quaternion _lookRot;
    
    //Event
    private void E_BTN_Attack_Pressed()
    {
        bool canChargeMotion = HeroMoveState is MoveState.Locomotion or MoveState.Roll or MoveState.RollJust;
        if(canChargeMotion) _animator.SetBool(GameManager.s_charge_normal,true);
        
        p_charge_begin.Play();
        _charged = false;
        _chargeBeginTime = Time.unscaledTime;
        _lookDeg = null;
        E_BTN_Attack_RemoveListner();
        GameManager.Instance.E_LateUpdate.AddListener(E_BTN_Attack_PressedUpdate);
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
            p_charge_Impact.transform.localScale = GameManager.V3_One * 0.35f;
            p_charge_Impact.Play();
            Core_NormalAttack();
        }
        else Core_StrongAttack();
        //이벤트 삭제
        E_BTN_Attack_RemoveListner();
        GameManager.Instance.E_LateUpdate.AddListener(E_BTN_Attack_ReleasedUpdate);
    }
    private void E_BTN_Attack_PressedUpdate()
    {
        bool canChargeMotion = HeroMoveState is MoveState.Locomotion or MoveState.Roll or MoveState.RollJust;
        Quaternion myRot = transform.rotation;
        //공격 조이스틱 각도 계산,Display 회전
        float jsDeg;
        if (!GameManager.Bool_Attack)
        {
            jsDeg = myRot.eulerAngles.y;
        }
        else
        {
            jsDeg = Mathf.Atan2(GameManager.JS_Attack.y, GameManager.JS_Attack.x) * Mathf.Rad2Deg +
                    CamArm.instance.transform.rotation.eulerAngles.y;
            jsDeg = -jsDeg + 180;
        }
        
        float targetDisplayDeg = Mathf.SmoothDampAngle(_lookDisplayT.eulerAngles.y, 
            jsDeg, ref _lookDisplayRefDeg, heroData.lookDisplayDuration);
        _lookDisplayT.rotation = Quaternion.Euler(0,targetDisplayDeg,0);
        //플레이어 각도와의 차이 계산, Target 회전
        float degDiff;
        if (!GameManager.Bool_Attack)
        {
            degDiff = 0;
            _lookDeg = null;
        }
        else
        {
            degDiff = -myRot.eulerAngles.y+jsDeg;
            while (degDiff < -180) degDiff += 360;
            while (degDiff > 180) degDiff -= 360;
            _lookDeg = myRot.eulerAngles.y + degDiff;
            if (heroData.lookRangeDeadZone.x > degDiff || degDiff > heroData.lookRangeDeadZone.y)
            {
                if (Mathf.DeltaAngle(myRot.eulerAngles.y, _lookTargetT.rotation.eulerAngles.y) > 0)
                    degDiff = heroData.lookRange.y;
                else degDiff = heroData.lookRange.x;
            }
            
            degDiff = Mathf.Clamp(degDiff,heroData.lookRange.x, heroData.lookRange.y);
            
        }
        float currentDiff = Mathf.DeltaAngle(myRot.eulerAngles.y, _lookTargetT.eulerAngles.y);
        float targetTargetDeg = Mathf.SmoothDamp(myRot.eulerAngles.y + currentDiff, 
            myRot.eulerAngles.y + degDiff, ref _lookTargetRefDeg, heroData.lookTargetDuration);
        
        _lookTargetT.rotation = Quaternion.Euler(0,targetTargetDeg,0);
        //크기 조정
        if (_lookScale < 1)
        {
            _lookScale += 5 * Time.unscaledDeltaTime;
            _lookScale = Mathf.Clamp01(_lookScale);
            _lookIcon.localScale = GameManager.V3_One * _lookScale;
            if(canChargeMotion) _lookAtIK.solver.SetLookAtWeight(_lookScale);
            else _lookAtIK.solver.SetLookAtWeight(0);
        }
        else if(!canChargeMotion) _lookAtIK.solver.SetLookAtWeight(0);
        //차지
        if (!_charged && Time.unscaledTime > _chargeBeginTime + heroData.chargeDuration && frameMain.MP_CanUse())
        {
            _charged = true;
            p_charge_fin.Play();
            p_charge_Impact.transform.localScale = GameManager.V3_One * 0.5f;
            p_charge_Impact.Play();
            Tween_Punch_Down_Compact(1.2f);
            Tween_Blink_Evade(1.0f);
            Effect_SuperArmor(true);
        }
    }
    private void E_BTN_Attack_ReleasedUpdate()
    {
        bool canChargeMotion = HeroMoveState is MoveState.Locomotion or MoveState.Roll or MoveState.RollJust;
        float eulery = transform.rotation.eulerAngles.y;
        float targetDeg = Mathf.SmoothDampAngle(_lookDisplayT.eulerAngles.y, 
            eulery, ref _lookDisplayRefDeg, heroData.lookDisplayDuration);
        float targetTargetDeg = Mathf.SmoothDamp(_lookTargetT.eulerAngles.y, 
            eulery, ref _lookTargetRefDeg, heroData.lookTargetDuration);
        //크기 조정
        if (_lookScale > 0)
        {
            _lookScale -= 3 * Time.unscaledDeltaTime;
            _lookScale = Mathf.Clamp01(_lookScale);
        }
        //종료
        bool correctDeg = Mathf.DeltaAngle(targetTargetDeg, eulery) < 0.1f;
        if (_lookScale < 0.01f && correctDeg)
        {
            _lookScale = 0;
            _lookIcon.localScale = GameManager.V3_Zero;
            _lookAtIK.solver.SetLookAtWeight(0);
            _lookDisplayT.rotation = Quaternion.Euler(0,eulery,0);
            _lookTargetT.rotation = Quaternion.Euler(0,eulery,0);
            GameManager.Instance.E_LateUpdate.RemoveListener(E_BTN_Attack_ReleasedUpdate);
        }
        else
        {
            _lookIcon.localScale = GameManager.V3_One * _lookScale;
            _lookDisplayT.rotation = Quaternion.Euler(0,targetDeg,0);
            _lookTargetT.rotation = Quaternion.Euler(0,targetTargetDeg,0);
            if(canChargeMotion) _lookAtIK.solver.SetLookAtWeight(_lookScale);
            else _lookAtIK.solver.SetLookAtWeight(0);
        }
    }
    private void E_BTN_Attack_RemoveListner()
    {
        GameManager.Instance.E_LateUpdate.RemoveListener(E_BTN_Attack_PressedUpdate);
        GameManager.Instance.E_LateUpdate.RemoveListener(E_BTN_Attack_ReleasedUpdate);
    }
    
    //Getter
    public bool Get_Charged()
    {
        return _charged;
    }
    public float? Get_LookDeg()
    {
        return _lookDeg;
    }
    public ref float Get_LookF()
    {
        return ref _lookF;
    }
    public ref Transform Get_LookT()
    {
        return ref _lookT;
    }
    public ref Quaternion Get_LookRot()
    {
        return ref _lookRot;
    }
}
