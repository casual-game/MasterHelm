using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public partial class HeroMovement : MonoBehaviour
{
    private LookAtIK lookAtIK;
    private Transform lookDisplayT,lookTargetT;
    private Transform lookIcon;
    private float lookDisplayRefDeg,lookTargetRefDeg;
    private float lookScale = 0;
    private float chargeBeginTime = -100;
    private bool charged = false;
    private void Setting_LookAt()
    {
        GameManager.instance.E_BTN_Attack_Begin.AddListener(Attack_Pressed);
        GameManager.instance.E_BTN_Attack_Fin.AddListener(Attack_Released);
        lookAtIK = GetComponent<LookAtIK>();
        Transform lookRootT = transform.Find("LookAt");
        lookDisplayT = lookRootT.Find("LookDisplay");
        lookTargetT = lookRootT.Find("LookTarget");
        lookIcon = lookDisplayT.GetChild(0);
        
        lookScale = 0;
        lookIcon.localScale = Vector3.zero;
        lookAtIK.solver.SetLookAtWeight(0);
        lookDisplayT.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
    }
    private void Attack_Pressed()
    {
        bool canChargeMotion = moveState == MoveState.Locomotion || moveState == MoveState.Roll;
        if(false) animator.SetBool(GameManager.s_charge_normal,true);
        p_charge_begin.Play();
        charged = false;
        chargeBeginTime = Time.unscaledTime;
        
        RemoveListner();
        GameManager.instance.E_LateUpdate.AddListener(PressedUpdate);
    }

    private void Attack_Released()
    {
        animator.SetBool(GameManager.s_charge_normal,false);
        p_charge_begin.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        p_charge_fin.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (!charged)
        {
            p_charge_Impact.transform.localScale = Vector3.one * 0.35f;
            p_charge_Impact.Play();
            NormalAttack();
        }
        else StrongAttack();
        RemoveListner();
        GameManager.instance.E_LateUpdate.AddListener(ReleasedUpdate);
    }

    private void PressedUpdate()
    {
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
        
        float targetDisplayDeg = Mathf.SmoothDampAngle(lookDisplayT.eulerAngles.y, 
            jsDeg, ref lookDisplayRefDeg, hero.lookDisplayDuration);
        lookDisplayT.rotation = Quaternion.Euler(0,targetDisplayDeg,0);
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
            if (hero.lookRangeDeadZone.x > degDiff || degDiff > hero.lookRangeDeadZone.y)
            {
                if (Mathf.DeltaAngle(transform.rotation.eulerAngles.y, lookTargetT.rotation.eulerAngles.y) > 0)
                    degDiff = hero.lookRange.y;
                else degDiff = hero.lookRange.x;
            }
            degDiff = Mathf.Clamp(degDiff,hero.lookRange.x, hero.lookRange.y);
        } 
        float currentDiff = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, lookTargetT.eulerAngles.y);
        float targetTargetDeg = Mathf.SmoothDamp(transform.rotation.eulerAngles.y + currentDiff, 
            transform.rotation.eulerAngles.y + degDiff, ref lookTargetRefDeg, hero.lookTargetDuration);
        lookTargetT.rotation = Quaternion.Euler(0,targetTargetDeg,0);
        //크기 조정
        if (lookScale < 1)
        {
            lookScale += 5 * Time.deltaTime;
            lookScale = Mathf.Clamp01(lookScale);
            lookIcon.localScale = Vector3.one * lookScale;
            lookAtIK.solver.SetLookAtWeight(lookScale);
        }
        //차지
        if (!charged && Time.unscaledTime > chargeBeginTime + hero.chargeDuration)
        {
            charged = true;
            p_charge_fin.Play();

            p_charge_Impact.transform.localScale = Vector3.one * 0.5f;
            p_charge_Impact.Play();
        }
    }

    private void ReleasedUpdate()
    {
        float eulery = transform.rotation.eulerAngles.y;
        float targetDeg = Mathf.SmoothDampAngle(lookDisplayT.eulerAngles.y, 
            eulery, ref lookDisplayRefDeg, hero.lookDisplayDuration);
        float targetTargetDeg = Mathf.SmoothDamp(lookTargetT.eulerAngles.y, 
            eulery, ref lookTargetRefDeg, hero.lookTargetDuration);
        //크기 조정
        if (lookScale > 0)
        {
            lookScale -= 3 * Time.deltaTime;
            lookScale = Mathf.Clamp01(lookScale);
        }
        //종료
        bool correctDeg = Mathf.DeltaAngle(targetTargetDeg, eulery) < 0.1f;
        if (lookScale < 0.01f && correctDeg)
        {
            lookScale = 0;
            lookIcon.localScale = Vector3.zero;
            lookAtIK.solver.SetLookAtWeight(0);
            lookDisplayT.rotation = Quaternion.Euler(0,eulery,0);
            lookTargetT.rotation = Quaternion.Euler(0,eulery,0);
            GameManager.instance.E_LateUpdate.RemoveListener(ReleasedUpdate);
        }
        else
        {
            lookIcon.localScale = Vector3.one * lookScale;
            lookDisplayT.rotation = Quaternion.Euler(0,targetDeg,0);
            lookTargetT.rotation = Quaternion.Euler(0,targetTargetDeg,0);
            lookAtIK.solver.SetLookAtWeight(lookScale);
        }
    }

    private void RemoveListner()
    {
        GameManager.instance.E_LateUpdate.RemoveListener(PressedUpdate);
        GameManager.instance.E_LateUpdate.RemoveListener(ReleasedUpdate);
    }
}
