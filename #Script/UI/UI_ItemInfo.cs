using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemInfo : MonoBehaviour
{
    [TitleGroup("사전 설정")] public TMP_Text itemName, itemInfo;
    [TitleGroup("사전 설정")] public Image icon;
    private CanvasGroup canvasGroup;
    private bool activated = false;
    private Vector3 originalScale;
    public void Setting()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        itemName.text = string.Empty;
        itemInfo.text = string.Empty;
        originalScale = transform.localScale;
    }

    [Button]
    public void UpdateData(UI_NormalItemSlot slot)
    {
        Data_Item item = slot == null ? null : slot.item;
        if (item == null)
        {
            Deactivate();
            return;
        }
        float delay = 0.0f;
        if(activated && item == null) Deactivate();
        else if (!activated && item != null)
        {
            Activate();
            delay = 0.05f;
        }
        icon.sprite = item.icon;
        itemInfo.DOKill();
        itemName.DOKill();
        transform.DOKill();
        icon.DOKill();
        itemInfo.text = "";
        itemName.text = "";
        icon.color = Color.clear;
        itemInfo.DOText(item.itemInfo, 1.2f, false).SetUpdate(true).SetDelay(delay);
        itemName.DOText(item.itemName, 0.4f, false).SetUpdate(true).SetDelay(delay);
        icon.DOColor(Color.white*0.9f,1.0f).SetEase(Ease.OutBack).SetUpdate(true).SetDelay(delay);
        transform.localScale = originalScale;
        transform.DOPunchScale(Vector3.one * 0.02f, 0.2f, 2);
    }

    private void Activate()
    {
        if (UI_NormalItemSlot.slot != null && UI_NormalItemSlot.slot.num > 0)
        {
            activated = true;
            canvasGroup.DOKill();
            canvasGroup.DOFade(1, 0.2f);
        }
    }
    private void Deactivate()
    {
        if (UI_NormalItemSlot.slot == null || UI_NormalItemSlot.slot.num < 1)
        {
            activated = false;
            transform.DOKill();
            canvasGroup.DOKill();
            transform.localScale = originalScale;
            transform.DOPunchScale(Vector3.one * 0.02f, 0.2f, 2);
            canvasGroup.DOFade(0, 0.2f);
        }
    }
}
