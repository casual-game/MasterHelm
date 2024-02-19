using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElement_Tip : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Image image;
    public TMP_Text tmp_title;
    private Sequence _seqTip;
    
    private void Tip(float delay)
    {
        gameObject.SetActive(true);
        _seqTip.Stop();
        transform.localScale = Vector3.one*0.8f;
        canvasGroup.alpha = 0;
        
        _seqTip = Sequence.Create(cycles:2,cycleMode: CycleMode.Yoyo);
        _seqTip.ChainDelay(delay);
        _seqTip.Chain(Tween.Scale(transform, 1.0f, 0.375f, Ease.OutBack));
        _seqTip.Group(Tween.Alpha(canvasGroup, 1, 0.25f));
        _seqTip.ChainDelay(0.5f);
        _seqTip.OnComplete(() => gameObject.SetActive(false));
    }
    [Button]
    public void Tip_RequireClear()
    {
        image.color = new Color(217.0f / 255.0f, 0, 0, 1);
        tmp_title.text = "이전 스테이지 클리어 필요!";
        
        Tip(0.375f);
    }
    [Button]
    public void Tip_NotReady()
    {
        image.color = new Color(217.0f / 255.0f, 0, 0, 1);
        tmp_title.text = "강화는 아직 미구현 상태입니다!";
        
        Tip(0);
    }
}
