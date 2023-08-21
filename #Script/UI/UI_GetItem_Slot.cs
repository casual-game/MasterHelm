using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UI_GetItem_Slot : MonoBehaviour
{
    private Vector2 firstPos;
    private Vector2 createPos, destroyPos;
    [HideInInspector]public RectTransform rectT;
    private CanvasGroup canvasGroup;
    private float moveDuration;
    private Data_Item item;
    [HideInInspector] public bool finished = true;
    public Image icon;
    public TMP_Text tmp_name;
    
    public void Setting(Vector2 firstPos,Vector2 createPos,Vector2 destroyPos,float moveDuration)
    {
        rectT = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        this.firstPos = firstPos;
        this.createPos = createPos;
        this.destroyPos = destroyPos;
        this.moveDuration = moveDuration;
        canvasGroup.alpha = 0;
    }
    public void Move(Vector2 startPos,Vector2 endPos,int nextIndex,Data_Item _item)
    {
        rectT.DOKill();
        finished = false;
        rectT.anchoredPosition = startPos;
        
        if (nextIndex == 0)
        {
            item = _item;
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, moveDuration*0.4f).SetUpdate(true);
            rectT.DOAnchorPos(destroyPos, moveDuration*0.4f).SetEase(Ease.OutExpo).SetUpdate(true)
                .OnComplete(MoveFirst);
        }
        else rectT.DOAnchorPos(endPos, moveDuration).SetEase(Ease.OutSine).SetUpdate(true).OnComplete(MoveNormal);

        
    }
    public void MoveFirst()
    {
        rectT.DOKill();
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        icon.sprite = item.icon;
        tmp_name.text = item.itemName;
        rectT.anchoredPosition = createPos;
        
        rectT.DOAnchorPos(firstPos, moveDuration*0.75f).SetEase(Ease.OutExpo).SetUpdate(true);
        canvasGroup.DOFade(1, moveDuration*0.75f).SetUpdate(true).OnComplete(Sleep);
        finished = true;
    }

    public void MoveNormal()
    {
        finished = true;
    }
    public void Sleep()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, moveDuration).SetDelay(3.0f).SetUpdate(true);
    }
}
