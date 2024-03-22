using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class UI_Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool moveBG;
    private Sequence s_interact;
    private Image inner,bg;
    private RectTransform canvasRect;
    private Vector2 firstAnchoredPosition;
    public float tween_FadeDuration = 0.25f,tween_ScaleDuration = 0.4f;
    public void Start()
    {
        moveBG = GetComponent<OnScreenStick>().behaviour == OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin;
        inner = GetComponent<Image>();
        var parent = transform.parent;
        bg = parent.GetChild(0).GetComponent<Image>();
        canvasRect = parent.GetComponentInParent<RectTransform>();
        
        firstAnchoredPosition = bg.rectTransform.anchoredPosition;
        
        
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(moveBG) bg.rectTransform.anchoredPosition = firstAnchoredPosition;
        
        s_interact.Complete();
        bg.color = Color.clear;
        bg.rectTransform.localScale = GameManager.V3_One * 0.75f;
        bg.rectTransform.anchoredPosition = inner.rectTransform.anchoredPosition;

        s_interact = Sequence.Create(useUnscaledTime: true)
            .Chain(Tween.PunchScale(inner.transform, GameManager.V3_One * 0.1f, 
                tween_FadeDuration, 1))
            .Group(Tween.Scale(bg.transform,GameManager.V3_One,tween_ScaleDuration,Ease.OutBack))
            .Group(Tween.Color(bg, Color.white, tween_FadeDuration));
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (moveBG)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, 
                eventData.position, eventData.pressEventCamera, out var position);
            bg.rectTransform.anchoredPosition = position;
        }
        s_interact.Complete();
        inner.color = Color.clear;
        bg.rectTransform.anchoredPosition = inner.rectTransform.anchoredPosition;
        
        s_interact = Sequence.Create(useUnscaledTime: true)
            .Chain(Tween.PunchScale(inner.transform, GameManager.V3_One * 0.1f, 
                tween_FadeDuration, 1))
            .Group(Tween.Color(inner, Color.white, tween_FadeDuration));
    }

}
