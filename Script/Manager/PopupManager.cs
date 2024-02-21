using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public Image imgPurchase, imgSuccess, imgDebugError;
    public CanvasGroup cgPurchase, cgSuccess, cgDebugError;
    public Transform tPurchaseText, tSuccessText, tDebugErrorText;
    private Sequence _seqPurchase, _seqSuccess, _seqDebugError;
    private Vector2 v2Purchase, v2Success, v2DebugError;

    public void Awake()
    {
        v2Purchase = imgPurchase.rectTransform.sizeDelta;
        v2Success = imgSuccess.rectTransform.sizeDelta;
        v2DebugError = imgDebugError.rectTransform.sizeDelta;
    }

    [Button]
    public void Purchase_Begin()
    {
        _seqPurchase.Stop();
        cgPurchase.gameObject.SetActive(true);
        Vector2 sizeDelta = v2Purchase;
        sizeDelta.y *= 0.5f;
        imgPurchase.rectTransform.sizeDelta = sizeDelta;
        tPurchaseText.localScale = Vector3.one*1.5f;
        cgPurchase.alpha = 0;
        
        _seqPurchase = Sequence.Create();
        _seqPurchase.timeScale = 1.5f;
        _seqPurchase.Chain(Tween.Alpha(cgPurchase, 1, 0.25f));
        _seqPurchase.Group(Tween.UISizeDelta(imgPurchase.rectTransform, v2Purchase, 0.5f,Ease.OutCirc));
        _seqPurchase.Group(Tween.Scale(tPurchaseText, 1.0f, 0.5f, Ease.OutCirc));
        
    }

    [Button]
    public void Purchase_Fin()
    {
        _seqPurchase.Stop();
        cgPurchase.gameObject.SetActive(true);
        Vector2 sizeDelta = v2Purchase;
        sizeDelta.y *= 0.5f;
        imgPurchase.rectTransform.sizeDelta = v2Purchase;
        tPurchaseText.localScale = Vector3.one;
        cgPurchase.alpha = 1;
        
        _seqPurchase = Sequence.Create();
        _seqPurchase.timeScale = 1.5f;
        _seqPurchase.Chain(Tween.Alpha(cgPurchase, 0, 0.2f,startDelay:0.2f));
        _seqPurchase.Group(Tween.UISizeDelta(imgPurchase.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqPurchase.OnComplete(() => cgPurchase.gameObject.SetActive(false));
    }

    [Button]
    public void Success()
    {
        _seqSuccess.Stop();
        cgSuccess.gameObject.SetActive(true);
        Vector2 sizeDelta = v2Success;
        sizeDelta.y *= 0.5f;
        imgSuccess.rectTransform.sizeDelta = sizeDelta;
        tSuccessText.localScale = Vector3.one*1.5f;
        cgSuccess.alpha = 0;
        
        _seqSuccess = Sequence.Create();
        _seqSuccess.timeScale = 1.5f;
        _seqSuccess.Chain(Tween.Alpha(cgSuccess, 1, 0.25f));
        _seqSuccess.Group(Tween.UISizeDelta(imgSuccess.rectTransform, v2Success, 0.5f,Ease.OutCirc));
        _seqSuccess.Group(Tween.Scale(tSuccessText, 1.0f, 0.5f, Ease.OutCirc));
        _seqSuccess.ChainDelay(1.5f);
        _seqSuccess.Chain(Tween.Alpha(cgSuccess, 0, 0.2f,startDelay:0.2f));
        _seqSuccess.Group(Tween.UISizeDelta(imgSuccess.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqSuccess.OnComplete(() => cgSuccess.gameObject.SetActive(false));
    }
    [Button]
    public void DebugError()
    {
        _seqDebugError.Stop();
        cgDebugError.gameObject.SetActive(true);
        Vector2 sizeDelta = v2DebugError;
        sizeDelta.y *= 0.5f;
        imgDebugError.rectTransform.sizeDelta = sizeDelta;
        tDebugErrorText.localScale = Vector3.one*1.5f;
        cgDebugError.alpha = 0;
        
        _seqDebugError = Sequence.Create();
        _seqDebugError.timeScale = 1.5f;
        _seqDebugError.Chain(Tween.Alpha(cgDebugError, 1, 0.25f));
        _seqDebugError.Group(Tween.UISizeDelta(imgDebugError.rectTransform, v2DebugError, 0.5f,Ease.OutCirc));
        _seqDebugError.Group(Tween.Scale(tDebugErrorText, 1.0f, 0.5f, Ease.OutCirc));
        _seqDebugError.ChainDelay(1.5f);
        _seqDebugError.Chain(Tween.Alpha(cgDebugError, 0, 0.2f,startDelay:0.2f));
        _seqDebugError.Group(Tween.UISizeDelta(imgDebugError.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqDebugError.OnComplete(() => cgDebugError.gameObject.SetActive(false));
    }
}
