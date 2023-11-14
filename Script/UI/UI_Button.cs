using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Sequence s_interact;
    private Image inner;
    public float tween_FadeDuration = 0.25f,tween_ScaleDuration = 0.4f;
    public void Start()
    {
        inner = GetComponent<Image>();
        var parent = transform.parent;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        s_interact.Complete();
        s_interact = Sequence.Create()
            .Chain(Tween.PunchScale(inner.transform, GameManager.V3_One * -3.0f,
                tween_FadeDuration, 1, useUnscaledTime: true))
            .Group(Tween.Color(inner, Color.white, tween_FadeDuration, useUnscaledTime: true));
    }

}
