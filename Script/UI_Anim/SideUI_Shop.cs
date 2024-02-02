using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class SideUI : MonoBehaviour
{
    [FoldoutGroup("Shop")] public CanvasGroup cgShop;
    [FoldoutGroup("Shop")] public Material matHero;

    [FoldoutGroup("Shop")] public AnimationCurve curveHeroShadow, curveHeroFade;
    [FoldoutGroup("Shop")] public ShopBanner banner1, banner2, banner3;
    [FoldoutGroup("Shop")] public TMP_Text tmpShopTitle;
    [FoldoutGroup("Shop")] public TypewriterCore twTitle;
    //Private
    private Sequence _seqShop,_seqShopBanner;
    private float bottomBanner1, bottomBanner2, bottomBanner3;
    private static string _strFadeAmount = "_FadeAmount", _strShadowAlpha = "_ShadowAlpha";
    private bool _checkShop = false;

    private void Setting_Shop()
    {
        banner1.Setting();
        banner2.Setting();
        banner3.Setting();
    }
    [FoldoutGroup("Shop")] [Button] [HorizontalGroup("Shop/ShopBtn")]
    public void Shop_Activate()
    {
        if (_seqShop.isAlive || _checkShop || _deco || _seqDeco.isAlive) return;
        _checkShop = true;
        cgShop.gameObject.SetActive(true);
        cgShop.transform.localScale = Vector3.one * 1.5f;
        cgShop.alpha = 0;
        matHero.SetFloat(_strFadeAmount, 1);
        matHero.SetFloat(_strShadowAlpha, 0);
        banner1.Open(0.8f);
        banner2.Open(0.9f);
        banner3.Open(1.0f);
        
        
        Deco_Activate();
        cgShop.gameObject.SetActive(true);
        _seqShop.Stop();
        _seqShop = Sequence.Create();
        //Hero렌더 텍스쳐
        _seqShop.timeScale = 1.25f;
        _seqShop.ChainDelay(1.0f);
        _seqShop.Chain(Tween.Scale(cgShop.transform, 1, 1.2f, Ease.OutCubic));
        _seqShop.Group(Tween.Alpha(cgShop, 1, 0.75f));
        _seqShop.Group(Tween.Custom(0, 1, 0.75f, onValueChange: ratio =>
        {
            matHero.SetFloat(_strFadeAmount, curveHeroFade.Evaluate(ratio));
            matHero.SetFloat(_strShadowAlpha, curveHeroShadow.Evaluate(ratio));
        }, startDelay: 0.125f));
    }
    [FoldoutGroup("Shop")] [Button] [HorizontalGroup("Shop/ShopBtn")]
    public void Shop_Deactivate()
    {
        if (_seqShop.isAlive || !_checkShop || !_deco || _seqDeco.isAlive) return;
        _checkShop = false;
        cgShop.transform.localScale = Vector3.one * 1.0f;
        cgShop.alpha = 1;
        matHero.SetFloat(_strFadeAmount, 0);
        matHero.SetFloat(_strShadowAlpha, 1);
        
        
        Deco_Deactivate();
        _seqShop.Stop();
        _seqShop = Sequence.Create();
        //Hero렌더 텍스쳐
        _seqShop.timeScale = 1.5f;
        //_seqShop.ChainDelay(1.0f);
        _seqShop.Chain(Tween.Scale(cgShop.transform, 1.5f, 1.2f, Ease.InCubic));
        _seqShop.Group(Tween.Alpha(cgShop, 0, 0.75f));
        _seqShop.Group(Tween.Custom(1, 0, 1.2f, onValueChange: ratio =>
        {
            matHero.SetFloat(_strFadeAmount, curveHeroFade.Evaluate(ratio));
            matHero.SetFloat(_strShadowAlpha, curveHeroShadow.Evaluate(ratio));
        }));
        _seqShop.OnComplete(() => cgShop.gameObject.SetActive(false));
    }

    public void Change()
    {
        banner1.Change(0.00f);
        banner2.Change(0.0f);
        banner3.Change(0.00f);
    }

    public void Button_Limited()
    {
        Change();
        if(twTitle.isHidingText) twTitle.StopDisappearingText();
        if(twTitle.isShowingText) twTitle.StopShowingText();
        tmpShopTitle.text = String.Empty;
        twTitle.ShowText("한정\n판매");
    }

    public void Button_Always()
    {
        Change();
        if(twTitle.isHidingText) twTitle.StopDisappearingText();
        if(twTitle.isShowingText) twTitle.StopShowingText();
        tmpShopTitle.text = String.Empty;
        twTitle.ShowText("상시\n판매");
    }

    public void Button_Forge()
    {
        Change();
        if(twTitle.isHidingText) twTitle.StopDisappearingText();
        if(twTitle.isShowingText) twTitle.StopShowingText();
        tmpShopTitle.text = String.Empty;
        twTitle.ShowText("대장간");
    }
}
