using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_IngameItemGroup : MonoBehaviour
{
    public CanvasGroup cgSlot1, cgSlot2, cgSlot3;
    public Material matDynamic;

    public ParticleImage
        piItemSuccess1,
        piItemSuccess2,
        piItemSuccess3;

    public Image imgIcon1, imgIcon2, imgIcon3, imgFailed1, imgFailed2, imgFailed3;
    public Color failedColor;
    public RectTransform _rtSlot1, _rtSlot2, _rtSlot3,_rtMain;
    private Vector2 _anchoredPosSlot1, _anchoredPosSlot2, _anchoredPosSlot3,_anchoredPosMain;
    private Sequence _seqMain, _seqItem1, _seqItem2, _seqItem3,_seqBlink;
    private Tween _tShake;
    private bool awaked = false;
    public void Awake()
    {
        if (awaked) return;
        awaked = true;
        _anchoredPosSlot1 = _rtSlot1.anchoredPosition;
        _anchoredPosSlot2 = _rtSlot2.anchoredPosition;
        _anchoredPosSlot3 = _rtSlot3.anchoredPosition;
        _anchoredPosMain = _rtMain.anchoredPosition;
    }

    [Button]
    public void Activate()
    {
        gameObject.SetActive(true);
        Awake();
        _seqMain.Stop();
        
        cgSlot1.alpha = 0;
        cgSlot2.alpha = 0;
        cgSlot3.alpha = 0;
        Vector2 addVec = new Vector2(164,0);
        _rtSlot1.anchoredPosition = _anchoredPosSlot1 + addVec;
        _rtSlot2.anchoredPosition = _anchoredPosSlot2 + addVec;
        _rtSlot3.anchoredPosition = _anchoredPosSlot3 + addVec;
        
        _seqMain = Sequence.Create(useUnscaledTime:true);
        _seqMain.Group(Tween.Alpha(cgSlot1, 1, 0.2f,startDelay:0.0f));
        _seqMain.Group(Tween.Alpha(cgSlot2, 1, 0.2f,startDelay:0.1f));
        _seqMain.Group(Tween.Alpha(cgSlot3, 1, 0.2f,startDelay:0.2f));
        _seqMain.Group(Tween.UIAnchoredPosition(_rtSlot1, _anchoredPosSlot1, 0.5f, Ease.OutBack,startDelay:0.0f));
        _seqMain.Group(Tween.UIAnchoredPosition(_rtSlot2, _anchoredPosSlot2, 0.5f, Ease.OutBack,startDelay:0.1f));
        _seqMain.Group(Tween.UIAnchoredPosition(_rtSlot3, _anchoredPosSlot3, 0.5f, Ease.OutBack,startDelay:0.2f));

        imgIcon1.color = Color.clear;
        imgIcon2.color = Color.clear;
        imgIcon3.color = Color.clear;
        imgFailed1.color = Color.clear;
        imgFailed2.color = Color.clear;
        imgFailed3.color = Color.clear;
    }
    [Button]
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
    [Button]
    public void Item1(float delay,bool getItem)
    {
        Image useImg = getItem ? imgIcon1 : imgFailed1;
        Color useColor = getItem ? Color.white : failedColor;
        _seqItem1.Stop();
        imgIcon1.color = Color.clear;
        imgFailed1.color = Color.clear;
        _seqItem1 = Sequence.Create(useUnscaledTime:true);
        _seqItem1.Group(Tween.Color(useImg,useColor, 0.2f,startDelay:delay));
        _seqItem1.Group(Tween.Scale(useImg.transform, Vector3.one * (getItem? 2.5f : 4.0f), 
            Vector3.one, 0.5f, Ease.InBack,startDelay:delay));
        _seqItem1.Group(Tween.Delay(delay + 0.5f, () =>
        {
            if (getItem) piItemSuccess1.Play();
            Blink(getItem? Color.white : Color.red);
            Shake();
        }));
    }
    public void Item2(float delay,bool getItem)
    {
        Image useImg = getItem ? imgIcon2 : imgFailed2;
        Color useColor = getItem ? Color.white : failedColor;
        _seqItem2.Stop();
        imgIcon2.color = Color.clear;
        imgFailed2.color = Color.clear;
        _seqItem2 = Sequence.Create(useUnscaledTime:true);
        _seqItem2.Group(Tween.Color(useImg,useColor, 0.2f,startDelay:delay));
        _seqItem2.Group(Tween.Scale(useImg.transform, Vector3.one * (getItem? 2.5f : 4.0f), 
            Vector3.one, 0.5f, Ease.InBack,startDelay:delay));
        _seqItem2.Group(Tween.Delay(delay + 0.5f, () =>
        {
            if (getItem) piItemSuccess2.Play();
            Blink(getItem? Color.white : Color.red);
            Shake();
        }));
    }
    public void Item3(float delay,bool getItem)
    {
        Image useImg = getItem ? imgIcon3 : imgFailed3;
        Color useColor = getItem ? Color.white : failedColor;
        _seqItem3.Stop();
        imgIcon3.color = Color.clear;
        imgFailed3.color = Color.clear;
        _seqItem3 = Sequence.Create(useUnscaledTime:true);
        _seqItem3.Group(Tween.Color(useImg,useColor, 0.2f,startDelay:delay));
        _seqItem3.Group(Tween.Scale(useImg.transform, Vector3.one * (getItem? 2.5f : 4.0f), 
            Vector3.one, 0.5f, Ease.InBack,startDelay:delay));
        _seqItem3.Group(Tween.Delay(delay + 0.5f, () =>
        {
            if (getItem) piItemSuccess3.Play();
            Blink(getItem? Color.white : Color.red);
            Shake();
        }));
    }
    public void Blink(Color color,float begin = 0.1f,float  delay = 0.2f,float  fin = 0.5f)
    {
        matDynamic.SetColor(GameManager.s_alphaoutlinecolor,color);
        _seqBlink.Stop();
        _seqBlink = Sequence.Create(useUnscaledTime:true);
        _seqBlink.Chain(Tween.Custom(0, 1, begin,
            onValueChange: val => matDynamic.SetFloat(GameManager.s_alphaoutlineblend, val)));
        _seqBlink.ChainDelay(delay);
        _seqBlink.Chain(Tween.Custom(1, 0, fin,ease:Ease.OutSine,
            onValueChange: val => matDynamic.SetFloat(GameManager.s_alphaoutlineblend, val)));
    }
    public void Shake(float duration = 0.5f)
    {
        _tShake.Stop();
        _tShake = Tween.Custom(1, 0, duration,ease:Ease.OutCubic,useUnscaledTime:true,
            onValueChange: val => _rtMain.anchoredPosition = _anchoredPosMain + Random.insideUnitCircle * 30 * val);
    }
}
