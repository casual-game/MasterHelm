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
    [FoldoutGroup("Book")] public UI_Ratio uiHeroRatio;
    [FoldoutGroup("Book")] public UI_Inventory inventory;
    [FoldoutGroup("Book")] public CanvasGroup cgBook;
    [FoldoutGroup("Book")] public List<Image> animSlotsImage = new List<Image>();
    [FoldoutGroup("Book")] public List<CanvasGroup> animSlotsCanvasGroup = new List<CanvasGroup>();
    private Sequence _seqBook;
    private bool _checkBook = false;
    private void Setting_Book()
    {
        uiHeroRatio.FitUI();
        inventory.Setting();
        inventory.FitUI();
    }
    //함수
    public void Book_Activate()
    {
        if (_seqShop.isAlive || _seqBook.isAlive || _seqDeco.isAlive) return;
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        if (_checkShop)
        {
            if (_checkBook || !_checkShop) return;
            Shop_JustDeactivate(false);
            Book_JustActivate(false,0.75f);
            SoundManager.Play(SoundContainer_StageSelect.instance.sound_page_close,0.0625f);
        }
        
        if (_deco) return;
        Book_JustActivate(true,1.0f);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_sideui_open);
        bgmManager.PlayBGM(bgmSideUI,_firstSideUI);
        _firstSideUI = false;
    }
    public void Book_Deactivate()
    {
        if (!_deco || _seqDeco.isAlive) return;
        Book_JustDeactivate(true);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_sideui_close);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_page_close,0.5f);
        bgmManager.PlayBGM(bgmSelect,false);
    }
    public void Book_JustActivate(bool controlDeco,float delay)
    {
        if (_seqBook.isAlive || _checkBook) return;
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_page_open,0.75f);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_book,1.0f);
        _checkBook = true;
        _seqBook.Stop();
        cgBook.gameObject.SetActive(true);
        cgBook.transform.localScale = Vector3.one * 1.25f;
        cgBook.alpha = 0;
        matHero.SetFloat(_strFadeAmount, 1);
        matHero.SetFloat(_strShadowAlpha, 0);
        foreach (var cg in animSlotsCanvasGroup)
        {
            cg.transform.localScale = Vector3.one*0.75f;
            cg.alpha = 0;
        }
        if(controlDeco) Deco_Activate();
        cgBook.gameObject.SetActive(true);
        
        
        
        _seqBook = Sequence.Create();
        //Hero렌더 텍스쳐
        _seqBook.timeScale = 1.25f;
        _seqBook.ChainDelay(delay);
        _seqBook.Chain(Tween.Scale(cgBook.transform, 1, 1.2f, Ease.OutCubic));
        _seqBook.Group(Tween.Alpha(cgBook, 1, 0.75f));
        _seqBook.Group(Tween.Delay(0.25f,()=>
        {
            inventory.Open(0.5f, 0.075f);
        }));
        _seqBook.Group(Tween.Custom(0, 1, 0.75f, onValueChange: ratio =>
        {
            matHero.SetFloat(_strFadeAmount, curveHeroFade.Evaluate(ratio));
            matHero.SetFloat(_strShadowAlpha, curveHeroShadow.Evaluate(ratio));
        }, startDelay: 0.125f));
        float startDelay = 0.25f;
        foreach (var cg in animSlotsCanvasGroup)
        {
            _seqBook.Group(Tween.Scale(cg.transform, Vector3.one, 0.5f, Ease.OutExpo,startDelay: startDelay));
            _seqBook.Group(Tween.Alpha(cg, 1, 0.25f, Ease.OutCubic,startDelay: startDelay));
            startDelay += 0.1f;
        }
    }
    public void Book_JustDeactivate(bool controlDeco)
    {
        if (_seqBook.isAlive || !_checkBook) return;
        _checkBook = false;
        cgBook.transform.localScale = Vector3.one * 1.0f;
        cgBook.alpha = 1;
        matHero.SetFloat(_strFadeAmount, 0);
        matHero.SetFloat(_strShadowAlpha, 1);
        inventory.Close();
        foreach (var cg in animSlotsCanvasGroup)
        {
            cg.transform.localScale = Vector3.one;
            cg.alpha = 1;
        }

        if(controlDeco) Deco_Deactivate();
        _seqBook.Stop();
        _seqBook = Sequence.Create();
        //Hero렌더 텍스쳐
        _seqBook.timeScale = 1.5f;
        _seqBook.Chain(Tween.Scale(cgBook.transform, 1.2f, 1.2f, Ease.InCubic));
        _seqBook.Group(Tween.Alpha(cgBook, 0, 0.5f, startDelay: 0.25f));
        _seqBook.Group(Tween.Custom(1, 0, 1.2f, onValueChange: ratio =>
        {
            matHero.SetFloat(_strFadeAmount, curveHeroFade.Evaluate(ratio));
            matHero.SetFloat(_strShadowAlpha, curveHeroShadow.Evaluate(ratio));
        }));
        float startDelay = 0.0f;
        foreach (var cg in animSlotsCanvasGroup)
        {
            _seqBook.Group(Tween.Scale(cg.transform, Vector3.one * 0.75f, 0.375f, Ease.InCubic,
                startDelay: startDelay));
            _seqBook.Group(Tween.Alpha(cg, 0, 0.25f, Ease.InCubic, startDelay: startDelay));
            startDelay += 0.1f;
        }

        _seqBook.OnComplete(() => cgBook.gameObject.SetActive(false));
    }
}
