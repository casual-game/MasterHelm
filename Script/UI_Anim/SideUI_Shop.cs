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
    [FoldoutGroup("Shop")] public ForgeSaved forgeSaved;
    [FoldoutGroup("Shop")] public ForgeBlueprint forgeBP;
    [FoldoutGroup("Shop")] public RawImage rtHero;
    [FoldoutGroup("Shop")] public RectTransform rtMoney;
    [FoldoutGroup("Shop")] public TMP_Text tmpCoin, tmpGem;
    //Private
    private Sequence _seqShop,_seqShopBanner,_seqForge;
    private float bottomBanner1, bottomBanner2, bottomBanner3;
    private static string _strFadeAmount = "_FadeAmount", _strShadowAlpha = "_ShadowAlpha";
    private bool _checkShop = false;
    private int _selection = 0;
    private Vector2 _anchoredPosMoneyNorm, _anchoredPosMoneyForge;

    
    private void Setting_Shop()
    {
        banner1.Setting();
        banner2.Setting();
        banner3.Setting();
        banner1.SetItem();
        banner2.SetItem();
        banner3.SetItem();
        _anchoredPosMoneyNorm = new Vector2(0, -70);
        _anchoredPosMoneyForge = new Vector2(0, 20);
        UpdateMoney();
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
        forgeSaved.Setting();
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_page_open,0.75f);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_shop,0.975f);
        _checkShop = true;
        cgShop.gameObject.SetActive(true);
        cgShop.transform.localScale = Vector3.one * 1.5f;
        cgShop.alpha = 0;
        matHero.SetFloat(_strFadeAmount, 1);
        matHero.SetFloat(_strShadowAlpha, 0);
        rtHero.color = Color.white;
        if (_selection <2)
        {
            banner1.Open(delay-0.2f);
            banner2.Open(delay-0.1f);
            banner3.Open(delay);
        }
        else
        {
            Forage_Show(delay);
        }
        
        
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
        
        Forage_Hide();
    }

    public void UpdateMoney()
    {
        tmpCoin.text = SaveManager.instance.coin.ToString();
        tmpGem.text = SaveManager.instance.gem.ToString();
    }
    public void Change()
    {
        banner1.Change(0.0f);
        banner2.Change(0.0f);
        banner3.Change(0.0f);
    }
    public void ShowBanner()
    {
        banner1.Show();
        banner2.Show();
        banner3.Show();
    }
    public void HideBanner()
    {
        banner1.Hide();
        banner2.Hide();
        banner3.Hide();
    }

    public void Button_Limited()
    {
        _selection = 0;
        rtHero.color = Color.white;
        banner1.SetItem();
        banner2.SetItem();
        banner3.SetItem();
        ShowBanner();
        Change();
        if(twTitle.isHidingText) twTitle.StopDisappearingText();
        if(twTitle.isShowingText) twTitle.StopShowingText();
        tmpShopTitle.text = String.Empty;
        twTitle.ShowText("한정\n판매");
        
        Forage_Hide();
    }
    public void Button_Always()
    {
        _selection = 1;
        rtHero.color = Color.white;
        banner1.SetPackage();
        banner2.SetPackage();
        banner3.SetPackage();
        ShowBanner();
        Change();
        if(twTitle.isHidingText) twTitle.StopDisappearingText();
        if(twTitle.isShowingText) twTitle.StopShowingText();
        tmpShopTitle.text = String.Empty;
        twTitle.ShowText("상시\n판매");

        Forage_Hide();
    }
    public void Button_Forge()
    {
        _selection = 2;
        HideBanner();
        Change();
        if(twTitle.isHidingText) twTitle.StopDisappearingText();
        if(twTitle.isShowingText) twTitle.StopShowingText();
        tmpShopTitle.text = String.Empty;
        twTitle.ShowText("대장간");
        
        Forage_Show(0.125f);
    }

    private void Forage_Show(float delay)
    {
        if (forgeSaved.forgeOpened && forgeBP.forgeOpened) return;
        forgeSaved.Show(delay);
        forgeBP.Show(delay);
        _seqForge.Stop();
        _seqForge = Sequence.Create();
        _seqForge.timeScale = 1.0f;
        _seqForge.Group(Tween.Custom(1, 0, 0.75f, onValueChange: ratio =>
        {
            matHero.SetFloat(_strFadeAmount, curveHeroFade.Evaluate(ratio));
            matHero.SetFloat(_strShadowAlpha, curveHeroShadow.Evaluate(ratio));
        }));
        _seqForge.Group(Tween.Color(rtHero, Color.clear, 0.25f));
        _seqForge.Group(Tween.UIAnchoredPosition(rtMoney, _anchoredPosMoneyForge, 0.5f, Ease.InOutCubic));
    }

    private void Forage_Hide()
    {
        if (!forgeSaved.forgeOpened && !forgeBP.forgeOpened) return;
        forgeSaved.Hide();
        forgeBP.Hide();
        _seqForge.Stop();
        _seqForge = Sequence.Create();
        _seqForge.timeScale = 1.5f;
        _seqForge.Group(Tween.Custom(0, 1, 0.5f, onValueChange: ratio =>
        {
            matHero.SetFloat(_strFadeAmount, curveHeroFade.Evaluate(ratio));
            matHero.SetFloat(_strShadowAlpha, curveHeroShadow.Evaluate(ratio));
        }));
        _seqForge.Group(Tween.UIAnchoredPosition(rtMoney, _anchoredPosMoneyNorm, 0.75f, Ease.InOutCubic));
    }
}
