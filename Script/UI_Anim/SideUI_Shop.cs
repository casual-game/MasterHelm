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
    public void Shop_Activate()
    {
        if (_seqShop.isAlive || _seqBook.isAlive || _seqDeco.isAlive) return;
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        if (_checkBook)
        {
            if (_checkShop|| !_checkBook) return;
            Book_JustDeactivate(false);
            Shop_JustActivate(false,0.75f);
            SoundManager.Play(SoundContainer_StageSelect.instance.sound_page_close,0.0625f);
        }

        if (_deco) return;
        Shop_JustActivate(true,1.0f);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_sideui_open);
        bgmManager.PlayBGM(bgmSideUI,_firstSideUI);
        _firstSideUI = false;
    }
    public void Shop_Deactivate()
    {
        if (!_deco || _seqDeco.isAlive) return;
        Shop_JustDeactivate(true);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_sideui_close);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_page_close,0.5f);
        bgmManager.PlayBGM(bgmSelect,false);
    }
    public void Shop_JustActivate(bool controlDeco ,float delay)
    {
        if (_seqShop.isAlive || _checkShop) return;
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_page_open,0.75f);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_shop,0.975f);
        _checkShop = true;
        cgShop.gameObject.SetActive(true);
        cgShop.transform.localScale = Vector3.one * 1.5f;
        cgShop.alpha = 0;
        matHero.SetFloat(_strFadeAmount, 1);
        matHero.SetFloat(_strShadowAlpha, 0);
        banner1.Open(delay-0.2f);
        banner2.Open(delay-0.1f);
        banner3.Open(delay);
        
        
        if(controlDeco) Deco_Activate();
        cgShop.gameObject.SetActive(true);
        _seqShop.Stop();
        _seqShop = Sequence.Create();
        //Hero렌더 텍스쳐
        _seqShop.timeScale = 1.25f;
        _seqShop.ChainDelay(delay);
        _seqShop.Chain(Tween.Scale(cgShop.transform, 1, 1.2f, Ease.OutCubic));
        _seqShop.Group(Tween.Alpha(cgShop, 1, 0.75f));
        _seqShop.Group(Tween.Custom(0, 1, 0.75f, onValueChange: ratio =>
        {
            matHero.SetFloat(_strFadeAmount, curveHeroFade.Evaluate(ratio));
            matHero.SetFloat(_strShadowAlpha, curveHeroShadow.Evaluate(ratio));
        }, startDelay: 0.125f));
    }
    public void Shop_JustDeactivate(bool controlDeco)
    {
        if (_seqShop.isAlive || !_checkShop) return;
        
        _checkShop = false;
        cgShop.transform.localScale = Vector3.one * 1.0f;
        cgShop.alpha = 1;
        matHero.SetFloat(_strFadeAmount, 0);
        matHero.SetFloat(_strShadowAlpha, 1);
        
        
        if(controlDeco) Deco_Deactivate();
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
        twTitle.ShowText("모험\n상점");
    }
}
