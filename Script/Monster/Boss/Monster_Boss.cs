using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class Monster_Boss : Monster
{
    //Public
    [FoldoutGroup("UI")] public Image img_health_root,img_nameplate;
    [FoldoutGroup("UI")] public RectTransform rect_shake;
    private Vector2 _shakeAnchoredPos;
    private Tween t_shake;
     
    private readonly float _uiDuration = 0.6f;
    protected override void Setting_UI()
    {
        base.Setting_UI();
        deathDealy = 1.0f;
        dissolveSpeed = 1.0f;
        _shakeAnchoredPos = rect_shake.anchoredPosition;
    }
    protected override void ActivateUI()
    {
        base.ActivateUI();
        seq_ui.Complete();
        img_health_root.rectTransform.localScale = GameManager.V3_Zero;
        img_nameplate.rectTransform.localScale = GameManager.V3_Zero;
        img_health_root.rectTransform.sizeDelta = new Vector2(282, 106.3343f);

        img_health_main.rectTransform.offsetMin = new Vector2(0, 0);
        img_health_main.rectTransform.offsetMax = new Vector2(0, 0);
        img_health_main.rectTransform.anchorMin = new Vector2(0f, 0f);
        img_health_main.rectTransform.anchorMax = new Vector2(1f, 1f);
        img_health_main.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
        img_health_lerp.rectTransform.offsetMin = new Vector2(0, 0);
        img_health_lerp.rectTransform.offsetMax = new Vector2(0, 0);
        img_health_lerp.rectTransform.anchorMin = new Vector2(0f, 0f);
        img_health_lerp.rectTransform.anchorMax = new Vector2(1f, 1f);
        img_health_lerp.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        seq_ui = Sequence.Create()
            .Chain(Tween.Scale(img_health_root.transform, 0.75f, _uiDuration, Ease.OutBack))
            .Chain(Tween.UISizeDelta(img_health_root.rectTransform, new Vector2(730.3395f, 106.3343f), _uiDuration,
                Ease.InOutBack))
            .Group(Tween.Scale(img_nameplate.transform, 1.0f, _uiDuration * 0.8f, Ease.OutBack, startDelay: 0.25f))
            .ChainCallback(() =>
            {
                img_health_main.rectTransform.anchoredPosition = new Vector2(0, 0);
                img_health_main.rectTransform.sizeDelta = new Vector2(512, 58);
                img_health_main.rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
                img_health_main.rectTransform.anchorMax = new Vector2(0.0f, 0.5f);
                img_health_main.rectTransform.pivot = new Vector2(0.0f, 0.5f);

                img_health_lerp.rectTransform.anchoredPosition = new Vector2(0, 0);
                img_health_lerp.rectTransform.sizeDelta = new Vector2(512, 58);
                img_health_lerp.rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
                img_health_lerp.rectTransform.anchorMax = new Vector2(0.0f, 0.5f);
                img_health_lerp.rectTransform.pivot = new Vector2(0.0f, 0.5f);
            });
    }
    protected override void DeactivateUI()
    {
        base.DeactivateUI();
        seq_ui.Complete();
        img_health_root.rectTransform.localScale = GameManager.V3_One*0.75f;
        img_nameplate.rectTransform.localScale = GameManager.V3_One;
        img_health_root.rectTransform.sizeDelta = new Vector2(730.3395f, 106.3343f);

        seq_ui = Sequence.Create()
            .Chain(Tween.UISizeDelta(img_health_root.rectTransform, new Vector2(282, 106.3343f),
                _uiDuration * 0.875f, PrimeTween.Ease.InOutBack))
            .Group(Tween.Scale(img_nameplate.transform, 0.0f, _uiDuration * 0.75f, Ease.InBack))
            .Chain(Tween.Scale(img_health_root.transform, 0, _uiDuration, Ease.InBack, startDelay: 0.2f));
    }
    protected override void Core_Damage(int damage)
    {
        currenthp -= damage;
        if (currenthp > 0)
        {
            float ratio = (float)currenthp / (float)monsterInfo.hp;
            Vector2 targetVec = new Vector2(512 * ratio, 58);
            t_dmg_main.Stop();
            t_dmg_lerp.Stop();
            t_dmg_main = Tween.UISizeDelta(img_health_main.rectTransform, 
                targetVec, 0.5f, Ease.OutQuart, useUnscaledTime: true);
            t_dmg_main = Tween.UISizeDelta(img_health_lerp.rectTransform, 
                targetVec, 0.5f, Ease.OutQuart, useUnscaledTime: true,startDelay:2.5f);
        }
        else if(_isAlive)
        {
            _isAlive = false;
            Vector2 targetVec = new Vector2(0, 58);
            t_dmg_main.Stop();
            t_dmg_lerp.Stop();
            t_dmg_main = Tween.UISizeDelta(img_health_main.rectTransform, 
                targetVec, 0.2f, Ease.OutQuart, useUnscaledTime: true);
            t_dmg_main = Tween.UISizeDelta(img_health_lerp.rectTransform, 
                targetVec, 0.3f, Ease.OutQuart, useUnscaledTime: true);
            Despawn().Forget();
        }
        
        t_shake.Stop();
        t_shake = Tween.Custom(0, 1, 0.5f, useUnscaledTime: true, onValueChange: newVal =>
        {
            Vector2 RandomVec = Random.insideUnitCircle.normalized * Mathf.Clamp01(2 - 2 * newVal) * 6;
            rect_shake.anchoredPosition = _shakeAnchoredPos + RandomVec;
        });
    }
}
