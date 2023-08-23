using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public partial class Enemy : MonoBehaviour
{
    [FoldoutGroup("UnitFrame")] public Transform T_UI_XZ, T_UI_Y;
    [FoldoutGroup("UnitFrame")] public Vector3 unitGrameVec;
    [FoldoutGroup("UnitFrame")] public AnimationCurve uiCurve;
    [FoldoutGroup("UnitFrame")] public Canvas canvas;
    [FoldoutGroup("UnitFrame")] public Image health_Lerp, health_Bar, guard_Bar;
    [FoldoutGroup("UnitFrame")] public List<Image> guard_Family = new List<Image>();
    [FoldoutGroup("UnitFrame")] public ParticleSystem guard_particle;

    protected Animator canvasAnimator;
    protected float currentGuard = 0,guardSpeed = 8.0f;
    protected bool guard_full = false;
    protected bool guard_use = false;
    protected float guardTime = -100,guardDelay = 1.75f;
    protected string s_guard = "Guard";

    protected Color guard_color_normal = new Color(1, 80.0f / 255.0f, 0, 1),
        guard_color_full = new Color(113.0f / 255.0f, 219.0f / 255.0f, 0, 1);
    protected virtual void FirstSetting_UI()
    {
        canvas.transform.parent = Manager_Main.instance._folder_;
        canvas.transform.rotation = Quaternion.Euler(60,45,0);
        canvasAnimator = canvas.GetComponent<Animator>();
    }
    protected void Setting_UI()
    {
        Canvas_Player.instance.OnLateUpdate.AddListener(UI_Update_UnitFrame);
        guard_use = guard > 0.1f && prefab_Shield!=null;
        if (guard_use)
        {
            currentGuard = 0;
            guard_Bar.fillAmount = 0;
            guard_Bar.color = guard_color_normal;
            Canvas_Player.instance.OnLateUpdate.AddListener(UI_Update_Guard);
            animator.SetBool(s_guard,true);
        }
        else
        {
            foreach (var gImage in guard_Family)
            {
                gImage.fillAmount = 0;
            }

            animator.SetBool(s_guard, false);
        }
    }
    protected virtual void UI_Update_UnitFrame()
    {
        Vector3 framePos = T_UI_XZ.position;
        framePos.y = T_UI_Y.position.y;
        framePos += unitGrameVec;
        canvas.transform.position =framePos;
    }
    protected virtual void UI_SetHPGauge(float value)
    {
        if (value <= 1) death = true;
        value = Mathf.Clamp(value, 0, hp);
        currentHP = value;

        float fillamount = currentHP / hp;

        health_Bar.DOKill();
        health_Lerp.DOKill();


        health_Bar.DOFillAmount(fillamount, 0.5f).SetUpdate(true).SetEase(Ease.OutCirc);
        health_Lerp.DOFillAmount(fillamount, 1.25f).SetUpdate(true).SetEase(Ease.InOutCubic).SetDelay(2.0f);
    }

    protected virtual void UI_SetGuardGauge(float value)
    {
        if (value > guard-0.1f)
        {
            guard_Bar.color = guard_color_full;
            guard_particle.Play();
            value = guard;
            guard_full = true;
        }
        else
        {
            guard_Bar.color = guard_color_normal;
            guard_full = false;
        }

        currentGuard = value;
        guard_Bar.DOKill();
        guard_Bar.DOFillAmount(currentGuard/guard, 0.2f).SetUpdate(true);
        guardTime = Time.unscaledTime;
    }
    protected virtual void UI_Update_Guard()
    {
        if (Time.unscaledTime - guardTime > guardDelay && currentGuard > 0 && !DOTween.IsTweening(guard_Bar))
        {
            guard_full = false;
            currentGuard = Mathf.Clamp(currentGuard - Time.unscaledDeltaTime * guardSpeed, 0, guard);
            guard_Bar.fillAmount = currentGuard / guard;
            guard_Bar.color = guard_color_normal;
        }
    }
    protected virtual void UI_BreakGuard()
    {
        guard_Bar.DOKill();
        guard_particle.Play();
        guard_use = false;
        animator.SetBool(s_guard, false);
        
        //
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos.y = 0;
        numPos = transform.position + numPos.normalized * 0.5f;
        //
        Canvas_Player.instance.OnLateUpdate.RemoveListener(UI_Update_Guard);
        foreach (var gImage in guard_Family)
        {
            gImage.DOKill();
            gImage.DOFillAmount(0, 0.25f).SetUpdate(true);
        }
        //
        if(prefab_Shield!=null) prefab_Shield.Drop_Enemy();
    }
    public bool IsGuard()
    {
        return guard_use;
    }
}
