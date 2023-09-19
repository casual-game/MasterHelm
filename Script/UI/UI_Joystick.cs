using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class UI_Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool moveBG;
    private Sequence s_Pressed,s_Released;
    private Image inner,bg;
    private RectTransform canvasRect;
    private Vector2 firstAnchoredPosition;
    public float tween_FadeDuration = 0.25f,tween_ScaleDuration = 0.4f;
    public void Start()
    {
        moveBG = GetComponent<OnScreenStick>().behaviour == OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin;
        inner = GetComponent<Image>();
        bg = transform.parent.GetChild(0).GetComponent<Image>();
        canvasRect = transform.parent?.GetComponentInParent<RectTransform>();
        RectTransform innerT = inner.rectTransform, bgT = bg.rectTransform;
        firstAnchoredPosition = bgT.anchoredPosition;
        
        s_Pressed = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .OnStart(() =>
            {
                inner.color = Color.clear;
                bgT.anchoredPosition = innerT.anchoredPosition;
            })
            .Append(innerT.DOPunchScale(Vector3.one*0.1f,tween_FadeDuration,1))
            .Join(inner.DOColor(Color.white,tween_FadeDuration));
        s_Released = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .OnStart(() =>
            {
                bg.color = Color.clear;
                bgT.localScale = Vector3.one * 0.75f;
                bgT.anchoredPosition = innerT.anchoredPosition;
            })
            .Append(innerT.DOPunchScale(Vector3.one*0.1f,tween_FadeDuration,1))
            .Join(bgT.DOScale(Vector3.one,tween_ScaleDuration).SetEase(Ease.OutBack))
            .Join(bg.DOColor(Color.white,tween_FadeDuration));
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(moveBG) bg.rectTransform.anchoredPosition = firstAnchoredPosition;
        s_Released.Restart();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (moveBG)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, 
                eventData.position, eventData.pressEventCamera, out var position);
            bg.rectTransform.anchoredPosition = position;
        }
        
        s_Pressed.Restart();
    }

    public void OnDestroy()
    {
        s_Pressed.Kill();
        s_Released.Kill();
    }
}
