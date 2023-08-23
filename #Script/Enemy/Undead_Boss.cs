using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class Undead_Boss : Enemy
{
    public Data_Impact impact_footstep, impact_groggy;
    public float stunDuration=3.0f;
    public Data_Audio audio_footstep, audio_groggy;
    public Color color_Guard_Default, color_Guard_DefaultHit, color_Guard_Full, 
        color_Guard_FullHit, color_Guard_Stun,color_Health_Default,color_Health_Hit;
    private ParticleSystem particle_Footstep,particle_Fire,particle_Shockwave,particle_Groggy;
    
    private string s_startboss = "StartBoss",s_activate = "Activate",s_deactivate = "Deactivate";
    private bool isStun = false;
    
    

    private IEnumerator CPattern_UndeadBoss_Main()
    {
        
        while (showingup) yield return null;
        
        while (true)
        {
            yield return State_Set(StartCoroutine(CState_Chase_Fast(2.0f)));
            
            //플레이어 보는 방향에 따라 앞,뒤 기본 공격, i번 반복
            for (int i = 0; i < Random.Range(1,3); i++)
            {
                if (IsLookingPlayer())  yield return State_Set(StartCoroutine(CState_Attack(1)));
                else yield return State_Set(StartCoroutine(CState_Attack(0)));
                if(PERCENT(50)) yield return State_Set(StartCoroutine(CState_Wait(2.0f)));
                
                yield return State_Set(StartCoroutine(CState_Wait(2.0f)));
            }
            //콤보
            if (Distance()<2.5f)  yield return State_Set(StartCoroutine(CState_Attack(2)));
            else yield return State_Set(StartCoroutine(CState_Attack(3)));
            //밟기
            yield return State_Set(StartCoroutine(CState_Attack(4)));
            
            //잠깐 정지
            yield return State_Set(StartCoroutine(CState_Wait(2.0f)));
            
        }
    }
    private IEnumerator CPattern_UndeadBoss_Main_Strong()
    {
        particle_Fire.Play();
        while (true)
        {
            yield return State_Set(StartCoroutine(CState_Chase_Fast(2.0f)));
            
            //회전 2연격 + 기본공격
            if (IsLookingPlayer())  yield return State_Set(StartCoroutine(CState_Attack(5)));
            else yield return State_Set(StartCoroutine(CState_Attack(6)));
            
            yield return State_Set(StartCoroutine(CState_Attack(5)));
            yield return State_Set(StartCoroutine(CState_Wait(0.5f)));
            
            //거리에 따라 연속콤보 + Smash
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                if (Distance() < 2.5f) yield return State_Set(StartCoroutine(CState_Attack(8)));
                else yield return State_Set(StartCoroutine(CState_Attack(9)));
                yield return State_Set(StartCoroutine(CState_Wait(2.0f)));
            }
            yield return State_Set(StartCoroutine(CState_Chase_Fast(2.0f)));
            //박수 콤보, 정지
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                yield return State_Set(StartCoroutine(CState_Attack(7)));
                yield return State_Set(StartCoroutine(CState_Wait(2.0f)));
            }
            
        }
    }
    private IEnumerator CPattern_UndeadBoss_Stunned()
    {
        particle_Fire.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        while (showingup) yield return null;

        float endtime = Time.unscaledTime + stunDuration;
        while (true)
        {
            if (Time.unscaledTime > endtime && !death)
            {
                isStun = false;
                guard_full = false;
                guard_use = true;
                UI_SetGuardGauge(0);
                guard_particle.Play();
                Canvas_Player.instance.OnLateUpdate.AddListener(UI_Update_Guard);
                animator.SetBool(s_guard,true);
                Pattern_Set(StartCoroutine(CPattern_UndeadBoss_Main_Strong()));
                yield break;
            }

            float ratio = (endtime - Time.unscaledTime) / stunDuration;
            guard_Bar.fillAmount = ratio;
            yield return null;
        }
    }

    private bool IsLookingPlayer()
    {
        Vector3 vec = Player.instance.transform.position - transform.position;
        vec.y = 0;
        return Vector3.Dot(transform.forward, vec.normalized)>-0.5f;
    }

    private float Distance()
    {
        Vector3 distVec = Player.instance.transform.position - transform.position;
        distVec.y = 0;
        return distVec.magnitude;
    }
    
    protected override void Pattern_Setting()
    {
        camDistanceRatio = 1.55f;
        
        base.Pattern_Setting();
        StartBoss();
        
        if(guard_use) Pattern_Set(StartCoroutine(CPattern_UndeadBoss_Main()));
        else Pattern_Set(StartCoroutine(CPattern_UndeadBoss_Stunned()));
    }

    protected override void Setting_Sound()
    {
        base.Setting_Sound();
        SoundManager.instance.Add(audio_footstep);
        SoundManager.instance.Add(audio_groggy);
    }

    protected override void FirstSetting_UI()
    {
        canvas.worldCamera = CamArm.instance.uiCam;
        canvas.transform.parent = Manager_Main.instance._folder_;
        canvasAnimator = canvas.GetComponent<Animator>();
    }
    protected override void FirstSetting_Effect()
    {
        base.FirstSetting_Effect();
        Transform t = transform.Find("Particle");
        particle_Footstep = t.Find("Footstep").GetComponent<ParticleSystem>();
        particle_Shockwave = t.Find("Shockwave").GetComponent<ParticleSystem>();
        particle_Fire = t.Find("Fire").GetComponent<ParticleSystem>();
        particle_Groggy = t.Find("Groggy").GetComponent<ParticleSystem>();
    }


    public override void Hit(bool isCounter,Vector3 hitWeaponRot, int? hitType = null, bool isArrow = false) 
    {
        if (death || Player.instance == null) return;
        this.hitWeaponRot = hitWeaponRot;
        Data_Impact impact;
        //가드
        float damage = Player.instance.isStrong ? 20 : 10;
        if (guard_use)
        {
            UI_SetGuardGauge(currentGuard + damage);
            //가드 브레이크
            if (guard_full)
            {
                Player.instance.Particle_JustParrying();
                Effect_Hit_GuardBreak(true);
                HitFX();
                CamArm.instance.speedline_once.Play();
                Particle_Blood_Smash();
                Particle_Hit_Revenge();
                
                UI_BreakGuard();
                Player.instance.Particle_FireRing(transform.position + Vector3.up);
                animator.SetInteger("Num", 5);
                //impact = Manager_Main.instance.mainData.impact_Smash;
                State_Begin_Danger(DangerState.Hit, TransitionType.Immediately);
                particle_Shockwave.Play();
                particle_parrying.Play();
                particle_smoke.Play();
            }
            //가드 게이지 남았을 경우
            else
            {
                
                if (Player.instance.IsRevengeSkill())
                {
                    Effect_Hit_Strong();
                    HitFX();
                    Player.instance.audio_Hit_Notice.Play();
                    Particle_Blood_Smash();
                    particle_smoke.Play();
                    
                    animator.SetInteger("Num",0);
                    State_Begin_Danger(DangerState.Hit, TransitionType.Immediately);
                }
                else
                {
                    Effect_Hit_Normal();
                    HitFX();
                    if (Player.instance.isStrong)
                    {
                        Particle_Blood_Smash();
                    }
                    else
                    {
                        Particle_Blood_Normal();
                    }
                    animator.SetTrigger("Hit");
                }
                
            }

            //가드-카메라
            /*
            CamArm.instance.Shake(impact);
            CamArm.instance.Stop(impact.stopDuration, 0.05f);
            CamArm.instance.Chromatic(impact.chromaticStrength, 
                0.1f, impact.stopDuration + 0.35f);
            highlight.HitFX(Manager_Main.instance.enemy_HitColor,0.5f);
            */
            return;
        }
        //히트
        else
        {
            UI_SetHPGauge(currentHP - damage);
            if (death) Effect_Death(true);
            else if (isCounter)
            {
                Effect_Hit_Counter();
                HitFX();
            }
            else if (Player.instance.isStrong)
            {
                particle_smoke.Play();
                Effect_Hit_Strong();
                HitFX();
            }
            else
            {
                Effect_Hit_Normal();
                HitFX();
            }
        }
        //애니메이션
        if (death)
        {
            //impact = Manager_Main.instance.mainData.impact_Smash;
            gameObject.layer = LayerMask.NameToLayer("Ragdoll");
            highlight.HitFX(Manager_Main.instance.mainData.enemy_ExecutedColor,1.5f);
            highlight.outline = 0;
            
            Particle_Blood_Smash();
            Particle_Blood_Normal();
            Boss_Death();
            
            //CamArm.instance.Shake(impact);
            //CamArm.instance.Hit(0.05f,3.0f,Color.white,0.35f);
            //CamArm.instance.Chromatic(0.05f, 0.1f, 3.0f);
            //CamArm.instance.Stop(2.5f,.45f);
            return;
        }
        else
        {
            highlight.HitFX(Manager_Main.instance.mainData.enemy_HitColor,0.25f);
            /*
            if (Player.instance.isRevengeSkill && Player.instance.isRevengeSkillActivated) impact = Manager_Main.instance.mainData.impact_Revenge;
            else if (Player.instance.isStrong) impact = Manager_Main.instance.mainData.impact_Smash;
            else impact = Manager_Main.instance.mainData.impact_Smash;
            */
            animator.SetTrigger("Hit");
            //Impact();
            return;
        }
        
    }
    protected override void UI_BreakGuard()
    {
        guard_Bar.DOKill();
        guard_particle.Play();
        guard_use = false;
        guard_full = false;
        
        animator.SetBool(s_guard, false);
        //
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos.y = 0;
        numPos = transform.position + numPos.normalized * 0.5f;
        Manager_Main.instance.Text_Big(numPos, "UNGARD");
        //
        Canvas_Player.instance.OnLateUpdate.RemoveListener(UI_Update_Guard);
        
        
        
        Pattern_Set(StartCoroutine(CPattern_UndeadBoss_Stunned()));
        isStun = true;
        guard_Bar.DOKill();
        guard_Bar.DOColor(color_Guard_Stun, 0.5f);
    }
    protected override void UI_Update_Guard()
    {
        if (Time.unscaledTime - guardTime > guardDelay && currentGuard > 0 && !DOTween.IsTweening(guard_Bar))
        {
            guard_full = false;
            currentGuard = Mathf.Clamp(currentGuard - Time.unscaledDeltaTime * guardSpeed, 0, guard);
            guard_Bar.fillAmount = currentGuard / guard;
            guard_Bar.color = guard_color_normal;
        }
    }
    protected override void UI_Update_UnitFrame()
    {
        return;
    }
    protected override void UI_SetGuardGauge(float value)
    {
        if (!IsGuard()|| isStun) return;
        guard_Bar.DOKill();
        
        if (value > guard-0.1f)
        {
            guard_particle.Play();
            value = guard;
            guard_full = true;
            
            guard_Bar.color = color_Guard_FullHit;
            guard_Bar.DOColor(color_Guard_Full, 1.5f);
        }
        else
        {
            guard_full = false;

            guard_Bar.color = color_Guard_DefaultHit;
            guard_Bar.DOColor(color_Guard_Default, 1.5f);
        }

        currentGuard = value;
        
        guard_Bar.DOFillAmount(currentGuard/guard, 0.2f).SetEase(Ease.OutCirc).SetUpdate(true);
        
        guardTime = Time.unscaledTime;
    }
    protected override void UI_SetHPGauge(float value)
    {
        base.UI_SetHPGauge(value);
        health_Bar.color = color_Health_Hit;
        health_Bar.DOColor(color_Health_Default, 0.5f).SetUpdate(true);
    }
    public void StartBoss()
    {
        animator.SetTrigger(s_startboss);
        canvasAnimator.Play(s_activate,0,0);
    }

    private void HitFX()
    {
        highlight.HitFX(Color.red,0.5f,2.0f);
    }
    #region Death
    public void Boss_Death()
    {
        Cancel();
        if (prefab_Shield != null) prefab_Shield.Drop_Enemy();
        if (prefab_Weapon_L != null) prefab_Weapon_L.Drop_Enemy();
        if (prefab_Weapon_R != null) prefab_Weapon_R.Drop_Enemy();
        if (enemies.Contains(this)) enemies.Remove(this);
        particle_Fire.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        particle_parrying.Play();
        particle_Shockwave.Play();
        Player.instance.Particle_JustParrying();
        StartCoroutine("C_Boss_Death");
        State_Begin_Danger(DangerState.Dead, TransitionType.Immediately);
    }
    private IEnumerator C_Boss_Death()
    {
        
        yield return new WaitForSeconds(1.25f);
        puppetMaster.mode = PuppetMaster.Mode.Active;
        for (int i = 0; i < 5; i++)
        {
            CreateOrb(new Vector3(Random.Range(-1.25f,1.25f),0,Random.Range(-1.25f,1.25f)));
            yield return new WaitForSecondsRealtime(Random.Range(0.15f,0.45f));
        }
        canvasAnimator.CrossFade(s_deactivate,0,0);
        yield return new WaitForSeconds(1.0f);
        
        foreach (var anim in animators) anim.enabled = true;
        yield return new WaitForSeconds(6.0f);
        puppetMaster.mode = PuppetMaster.Mode.Disabled;
        Disable();
    }

    #endregion
    public void Effect_Footstep()
    {
        CamArm.instance.Impact(impact_footstep);
        particle_Footstep.Play();
        audio_footstep.Play();
    }
    public void Effect_Groggy()
    {
        CamArm.instance.Impact(impact_groggy);
        particle_Groggy.Play();
        audio_groggy.Play();
    }
    
}