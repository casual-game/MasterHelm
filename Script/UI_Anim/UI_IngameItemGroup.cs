using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class UI_IngameItemGroup : MonoBehaviour
{
    public CanvasGroup cgSlot1, cgSlot2, cgSlot3;

    private RectTransform _rtSlot1, _rtSlot2, _rtSlot3;
    private Vector2 _anchoredPosSlot1, _anchoredPosSlot2, _anchoredPosSlot3;
    private Sequence _seqMain;
    private bool awaked = false;
    public void Awake()
    {
        if (awaked) return;
        awaked = true;
        _rtSlot1 = cgSlot1.GetComponent<RectTransform>();
        _rtSlot2 = cgSlot2.GetComponent<RectTransform>();
        _rtSlot3 = cgSlot3.GetComponent<RectTransform>();
        _anchoredPosSlot1 = _rtSlot1.anchoredPosition;
        _anchoredPosSlot2 = _rtSlot2.anchoredPosition;
        _anchoredPosSlot3 = _rtSlot3.anchoredPosition;
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
    }
    [Button]
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
