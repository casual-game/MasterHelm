using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIElement_Frame : MonoBehaviour
{
    public RectTransform rectTFrameDown, rectTFrameUp;
    public TMP_Text tmpTitle;
    public Image imgTitle;
    public CanvasGroup cgTitle,cgMain;
    public TypewriterCore typewriterCore;
    private Sequence seqFrame;
    private Vector2 apUp, apDown,apUpHide,apDownHide;
    public void Setting()
    {
        tmpTitle.text = String.Empty;
        apUp = rectTFrameUp.anchoredPosition;
        apDown = rectTFrameDown.anchoredPosition;
        apUpHide = apUp;
        apUpHide.y = 0;
        apDownHide = apDown;
        apDownHide.y = 0;
        gameObject.SetActive(false);
    }
    public void Reveal(int stage = 1)
    {
        gameObject.SetActive(true);
        seqFrame.Stop();
        cgTitle.transform.localScale = Vector3.one*0.8f;
        cgTitle.alpha = 0;
        cgMain.alpha = 0;
        tmpTitle.text = String.Empty;
        imgTitle.rectTransform.sizeDelta = new Vector2(250,imgTitle.rectTransform.sizeDelta.y);
        rectTFrameUp.anchoredPosition = apUpHide;
        rectTFrameDown.anchoredPosition = apDownHide;
        
        seqFrame = Sequence.Create();
        seqFrame.Chain(Tween.Alpha(cgMain, 1,0.25f));
        seqFrame.Group(Tween.UIAnchoredPosition(rectTFrameUp, apUp, 0.5f, Ease.OutCirc));
        seqFrame.Group(Tween.UIAnchoredPosition(rectTFrameDown, apDown, 0.5f, Ease.OutCirc));

        seqFrame.Group(Tween.Scale(cgTitle.transform, 1, 0.375f, Ease.OutBack));
        seqFrame.Group(Tween.Alpha(cgTitle, 1,0.5f));
        seqFrame.Group(Tween.UISizeDelta(imgTitle.rectTransform, new Vector2(500,
        imgTitle.rectTransform.sizeDelta.y), 0.5f, Ease.InOutBack,startDelay:0.125f));
        seqFrame.Group(Tween.Delay(0.325f, () => typewriterCore.ShowText("WORLD " + stage)));
    }
    public void Hide()
    {
        seqFrame.Stop();
        seqFrame = Sequence.Create();
        seqFrame.ChainCallback(() => typewriterCore.StartDisappearingText());
        seqFrame.Group(Tween.UISizeDelta(imgTitle.rectTransform, new Vector2(250,
            imgTitle.rectTransform.sizeDelta.y), 0.5f, Ease.InOutBack,startDelay:0.125f));
        seqFrame.Group(Tween.Scale(cgTitle.transform, 1, 0.375f, Ease.OutBack,startDelay:0.25f));
        seqFrame.Group(Tween.Alpha(cgTitle, 0,0.5f,startDelay:0.5f));
        
        seqFrame.Group(Tween.UIAnchoredPosition(rectTFrameUp, apUpHide, 0.5f, Ease.InCirc,startDelay:0.375f));
        seqFrame.Group(Tween.UIAnchoredPosition(rectTFrameDown, apDownHide, 0.5f, Ease.InCirc,startDelay:0.375f));
        seqFrame.Group(Tween.Alpha(cgMain, 0,0.25f,startDelay:0.875f));
        seqFrame.ChainCallback(() => gameObject.SetActive(false));
    }
}
