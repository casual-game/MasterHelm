 using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class SideUI : MonoBehaviour
{
    
    [FoldoutGroup("Common")] public Transform decoProp, decoRoot, decoShadow;
    [FoldoutGroup("Common")] public RectTransform rtBtnShop,rtBtnBook;
    [FoldoutGroup("Common")] public Button btnShop, btnBook;
    [FoldoutGroup("Common")] public CanvasGroup cgDeco;
    [FoldoutGroup("Common")] public UnityEvent eFrameHide, eFrameActiavte,eMoveCam;
    [FoldoutGroup("Common")] public List<Camera> cams = new List<Camera>();
    [FoldoutGroup("Audio")] public BgmManager bgmManager;
    [FoldoutGroup("Audio")] public BgmData bgmSelect, bgmSideUI;
    private bool _firstSideUI;
    private Sequence _seqDeco,_seqCam,_seqIngame;
    private Tween _tBlur;
    private bool _deco = false;
    
    private Vector2 shopAnchoredPos,shopAnchoredPosHide, bookAnchoredPos,bookAnchoredPosHide;
    public void Setting()
    {
        shopAnchoredPos = rtBtnShop.anchoredPosition;
        bookAnchoredPos = rtBtnBook.anchoredPosition;
        shopAnchoredPosHide = shopAnchoredPos;
        shopAnchoredPosHide.x = 0;
        bookAnchoredPosHide = bookAnchoredPos;
        bookAnchoredPosHide.x = 0;
        Setting_Shop();
        Setting_Book();
        _firstSideUI = true;
        bgmManager.Setting();
        bgmManager.PlayBGM(bgmSelect,true);
    }

    public bool IsUsing()
    {
        return _deco;
    }
    public void Ingame_Activate(float delay)
    {
        _seqIngame.Stop();
        _seqIngame = Sequence.Create();
        _seqIngame.ChainDelay(delay);
        _seqIngame.Chain(Tween.UIAnchoredPosition(rtBtnBook, bookAnchoredPos, 0.5f, Ease.InOutQuart));
        _seqIngame.Group(Tween.UIAnchoredPosition(rtBtnShop, shopAnchoredPos, 0.5f, Ease.InOutQuart,startDelay:0.125f));
        btnShop.interactable = true;
        btnBook.interactable = true;
    }
    public void Ingame_Deactivate(float delay)
    {
        _seqIngame.Stop();
        _seqIngame = Sequence.Create();
        _seqIngame.ChainDelay(delay);
        _seqIngame.Chain(Tween.UIAnchoredPosition(rtBtnBook, bookAnchoredPosHide, 0.5f, Ease.InOutQuart));
        _seqIngame.Group(Tween.UIAnchoredPosition(rtBtnShop, shopAnchoredPosHide, 0.5f, Ease.InOutQuart,startDelay:0.125f));
        btnShop.interactable = false;
        btnBook.interactable = false;
    }
    private void Deco_Activate()
    {
        if (_deco || _seqDeco.isAlive) return;
        _deco = true;
        _seqDeco.Stop();
        cgDeco.alpha = 0;
        decoShadow.localScale = Vector3.one*1.2f;
        decoRoot.localScale = Vector3.one*1.4f;
        decoProp.localScale = Vector3.one*1.75f;
        cgDeco.gameObject.SetActive(true);
        TweenSide_MoveOrtho(5.75f,2.0f/1.25f);
        Tween_Blur(1.5f,0.75f,0.0f);
        
        
        _seqDeco = Sequence.Create();
        _seqDeco.timeScale = 1.25f;
        _seqDeco.ChainCallback(() => eFrameHide.Invoke());
        _seqDeco.Chain(Tween.Alpha(cgDeco, 1,1.0f, Ease.OutCubic));
        _seqDeco.Group(Tween.Scale(decoShadow, 1.0f, 1.75f, startDelay: 0.5f, ease: Ease.InOutCubic));
        _seqDeco.Group(Tween.Scale(decoRoot, 1.0f, 1.75f, startDelay: 0.25f, ease: Ease.InOutCubic));
        _seqDeco.Group(Tween.Scale(decoProp, 1.0f, 2.0f, startDelay: 0.0f, ease: Ease.InOutCubic));
        
    }
    private void Deco_Deactivate()
    {
        if (!_deco || _seqDeco.isAlive) return;
        _seqDeco.Stop();
        cgDeco.alpha = 1;
        decoShadow.localScale = Vector3.one;
        decoRoot.localScale = Vector3.one;
        decoProp.localScale = Vector3.one;
        Tween_Blur(0.5f,0.0f,0.5f);
        TweenSide_MoveOrtho(5.0f,2.0f/1.5f);
        
        
        
        _seqDeco = Sequence.Create();
        _seqDeco.timeScale = 1.5f;
        _seqDeco.Chain(Tween.Scale(decoShadow, 1.2f, 2.0f, startDelay: 0.0f, ease: Ease.InOutCubic));
        _seqDeco.Group(Tween.Scale(decoRoot, 1.4f, 1.75f, startDelay: 0.25f, ease: Ease.InOutCubic));
        _seqDeco.Group(Tween.Scale(decoProp, 1.75f, 1.75f, startDelay: 0.5f, ease: Ease.InOutCubic));
        _seqDeco.Group(Tween.Alpha(cgDeco, 0,1.0f, Ease.OutCubic,startDelay:1.25f));
        _seqDeco.Group(Tween.Delay(0.75f,()=>eFrameActiavte.Invoke()));
        _seqDeco.OnComplete(() =>
        {
            _deco = false;
            cgDeco.gameObject.SetActive(false);
        });
    }
    
    public void Tween_Blur(float duration,float strength,float delay)
    {
        _tBlur.Complete();
        _tBlur = Tween.Custom(BeautifySettings.settings.blurIntensity.value, strength, duration, 
            onValueChange:blurIntensity => BeautifySettings.settings.blurIntensity.value = blurIntensity,startDelay:delay);
    }
    private void TweenSide_MoveOrtho(float targetOrtho,float duration)
    {
        float startOrthosize = cams[0].orthographicSize;
        _seqCam.Stop();
        _seqCam = Sequence.Create();
        _seqCam.Group(Tween.Custom(startOrthosize,targetOrtho,duration,onValueChange: os =>
        {
            foreach (var cam in cams) cam.orthographicSize = os;
            eMoveCam.Invoke();
        },Ease.InOutCubic));
    }
}
