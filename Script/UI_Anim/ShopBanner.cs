using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ShopBanner : MonoBehaviour
{
    public RectTransform rt;
    public TMP_Text tmpTitle,tmpPrice;
    public TypewriterCore twTitle,twPrice;
    public CanvasGroup cgPrice;
    
    private float _bottom;
    private Sequence _seq;
    public void Setting()
    {
        tmpTitle.text = string.Empty;
        tmpPrice.text = string.Empty;
        _bottom = rt.rect.size.y*0.375f;
    }
    public void Open(float delay)
    {
        _seq.Stop();
        if(twTitle.isHidingText)twTitle.StopDisappearingText();
        if(twTitle.isShowingText)twTitle.StopShowingText();
        if(twPrice.isHidingText)twPrice.StopDisappearingText();
        if(twPrice.isShowingText)twPrice.StopShowingText();
        tmpTitle.text = string.Empty;
        tmpPrice.text = string.Empty;
        transform.localScale = Vector3.one;
        cgPrice.transform.localScale = Vector3.one*0.75f;
        cgPrice.alpha = 0;
        
        rt.offsetMin= new Vector2(rt.offsetMin.x,_bottom);
        
        _seq = Sequence.Create();
        _seq.Chain(Tween.Delay(delay,() => twTitle.ShowText("상점 아이템\n예시")));
        _seq.Group(Tween.UIOffsetMinY(rt, 0, 0.75f, Ease.InOutBack,startDelay:delay));
        _seq.Group(Tween.Delay(delay+0.375f,() => twPrice.ShowText("2000 krw")));
        _seq.Group(Tween.Scale(cgPrice.transform, 1.0f, 0.5f, Ease.OutBack, startDelay: delay + 0.25f));
        _seq.Group(Tween.Alpha(cgPrice, 1.0f, 0.25f, startDelay: delay + 0.25f));
    }
    public void Close(float delay)
    {
        _seq.Stop();
        rt.offsetMin= new Vector2(rt.offsetMin.x,0);
        
        _seq = Sequence.Create();
        _seq.ChainDelay(delay);
        _seq.Chain(Tween.UIOffsetMinY(rt, _bottom, 0.75f, Ease.InOutBack,startDelay:0.0f));
    }

    public void Change(float delay)
    {
        _seq.Stop();
        tmpTitle.text = string.Empty;
        tmpPrice.text = string.Empty;
        transform.localScale = Vector3.one;
        cgPrice.transform.localScale = Vector3.one;
        
        _seq = Sequence.Create();
        _seq.Chain(Tween.PunchScale(transform, Vector3.up * -0.05f, 0.375f, 2, startDelay: delay));
        _seq.Group(Tween.Delay(delay,() => twTitle.ShowText("상점 아이템\n예시")));
        _seq.Group(Tween.PunchScale(cgPrice.transform, Vector3.one*-0.375f,0.375f, 2, startDelay: delay));
        _seq.Group(Tween.Delay(delay+0.125f,() => twPrice.ShowText("2000 krw")));
    }
}
