using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Setting_SkillGauge : MonoBehaviour
{
    [BoxGroup("기본 설정")] [Range(0,1)] public float startFill = 1;
    [BoxGroup("기본 설정")] public float max = 100; 
    [BoxGroup("애니메이션")] public float blinkDelay = 0.35f,blinkDuration=0.25f,followDuration = 0.2f;
    [BoxGroup("애니메이션")] public AnimationCurve followCurve;

    private string s_Fill_Amount = "Fill_Amount";
    private Image image_Fill;
    private Color currentColor;
    private Color color_Uncharged,color_Charged,color_Highlighted;
    private Coroutine c_follow = null;
    private Image image_Effect;
    private float fillAmount = 0;
    private ElementalAttributes elementalAttributes;
    public void Setting(Color uncharged,Color charged,Color highlighted,ElementalAttributes _elemental)
    {
        elementalAttributes = _elemental;
        
        color_Uncharged = uncharged;
        color_Charged = charged;
        color_Highlighted = highlighted;
        
        image_Effect = GetComponent<Image>();
        image_Effect.material = new Material(image_Effect.material);
        image_Fill = transform.parent.GetComponent<Image>();
        image_Fill.color = color_Uncharged;
        

        currentColor = color_Uncharged;
        
        
        SetValue(Mathf.RoundToInt(startFill*max));
    }

    //메인 세팅
    [HideInInspector] public bool fullCharged = false;
    [HideInInspector] public float current = 100;

    public void Use()
    {
        SetValue(0);
        Canvas_Player.instance.ElementalParticle(elementalAttributes);
    }
    public void SetValue(float value)
    {
        if (max - value < 1)
        {
            if (!fullCharged) image_Fill.color = color_Highlighted;
            fullCharged = true;
            current = max;
            FillAmount(1);
            currentColor = color_Charged;
        }
        else
        {
            fullCharged = false;
            current = value;
            if(c_follow!=null) StopCoroutine(c_follow);
            c_follow = StartCoroutine(C_Follow(current / max));
            
            currentColor = color_Uncharged;
        }

        if (DOTween.IsTweening(image_Fill)) image_Fill.DOKill();
        image_Fill.DOColor(currentColor, blinkDuration).SetDelay(blinkDelay);

    }

    private IEnumerator C_Follow(float fillAmount)
    {
        float beginTime = Time.unscaledTime;
        float beginFillAmount = 1-image_Effect.material.GetFloat(s_Fill_Amount);
        while (Time.unscaledTime-beginTime<followDuration)
        {
            float ratio = (Time.unscaledTime - beginTime) / followDuration;
            float fa = Mathf.Lerp(beginFillAmount, fillAmount,followCurve.Evaluate(ratio));
            FillAmount(fa);
            yield return null;
        }

        FillAmount(fillAmount);
    }

    private void FillAmount(float amount)
    {
        image_Effect.material.SetFloat(s_Fill_Amount,1-amount);
    }
}
