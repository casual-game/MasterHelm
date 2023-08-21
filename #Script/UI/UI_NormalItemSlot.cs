using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;


public class UI_NormalItemSlot : MonoBehaviour
{
    
    public static UI_NormalItemSlot slot;
    
    [BoxGroup("사전 설정")] public Image icon,selectedInner, selectedOuter;
    [BoxGroup("사전 설정")] public Transform numSlot;
    [BoxGroup("인게임 데이터")] public Data_Item item = null;
    [BoxGroup("인게임 데이터")] public int num = 0;
    private Vector3 numSlot_Scale;
    public void Setting()
    {
        numSlot_Scale = numSlot.localScale;
        selectedInner = transform.Find("SelectedInner").GetComponent<Image>();
        icon = transform.Find("icon").GetComponent<Image>();
        selectedOuter = GetComponent<Image>();
        selectedRatio = 0;
        selectedInner.color = Color.clear;
        selectedOuter.color = Color.clear;
        tmp_num = transform.GetComponentInChildren<TMP_Text>(true);
        Deselected();
        slot = null;
        num = 1;
        UpdateItem(0,null);
    }
    //Selected
    [BoxGroup("선택 효과")] public Color selectedInnerColor, selectedOuterColor;
    [BoxGroup("선택 효과")] public float selectedSpeed = 1.0f;
    [BoxGroup("선택 효과")] public AnimationCurve selectedCurve;
    private Coroutine c_selected;
    private bool selected = false;
    private float selectedRatio = 0;
    
    private IEnumerator C_Selected(bool result)
    {
        if (result)
        {
            while (selectedRatio < 1)
            {
                selectedRatio += selectedSpeed * Time.unscaledDeltaTime;
                ColorRatio();
                yield return null;
            }
            selectedRatio = 1;
            ColorRatio();
        }
        else
        {
            while (selectedRatio > 0)
            {
                selectedRatio -= selectedSpeed * Time.unscaledDeltaTime;
                ColorRatio();
                yield return null;
            }  
            selectedRatio = 0;
            ColorRatio();
        }
        
        
    }
    void ColorRatio()
    {
        float ratio = selectedCurve.Evaluate(selectedRatio);
        selectedInner.color = Color.Lerp(Color.clear, selectedInnerColor, ratio);
        selectedOuter.color = Color.Lerp(Color.clear, selectedOuterColor, ratio);
    }
    //Num
    private TMP_Text tmp_num;
    [BoxGroup("아이템 개수")][Button("아이템 개수/숫자 변경")] 
    public void SetNum(int num)
    {
        if (num>0)
        {
            DOTween.Kill(numSlot);
            numSlot.transform.localScale = Vector3.zero;
            numSlot.DOScale(Vector3.one * 0.714f, 0.55f).SetEase(Ease.OutBack).SetUpdate(true);
            if (icon != null)
            {
                DOTween.Kill(icon);
                icon.DOColor(Color.white*0.9f,0.35f).SetEase(Ease.OutBack).SetUpdate(true);
            }
        }
        else if (this.num > 0 && num == 0)
        {
            DOTween.Kill(numSlot);
            numSlot.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetUpdate(true);
            if (icon != null)
            {
                DOTween.Kill(icon);
                icon.DOColor(Color.clear,0.2f).SetEase(Ease.InBack).SetUpdate(true);
            }
            Deselected();
        }
        this.num = num;
        if (num > 0) tmp_num.text = num.ToString();
    }
    //메인 함수들!
    public void Selected()
    {
        if (selected)
        {
            Deselected();
            return;
        }
        if (num ==0) return;
        if (slot != null)
        {
            if (slot != this) slot.Deselected();
            else return;
        }
        slot = this;

        if (gameObject.activeInHierarchy)
        {
            if(c_selected!=null) StopCoroutine(c_selected);
            c_selected = StartCoroutine(C_Selected(true));
        }
        else
        {
            selectedRatio = 1;
            ColorRatio();
        }
        selected = true;
        Canvas_Player.instance.itemInfo.UpdateData(slot);
    }
    public void Deselected()
    {
        if (!gameObject.activeInHierarchy)
        {
            selectedRatio = 0;
            ColorRatio();
            return;
        }
        
        
        if (slot == this) slot = null;
        if(c_selected!=null) StopCoroutine(c_selected);
        c_selected = StartCoroutine(C_Selected(false));
        selected = false;
        Canvas_Player.instance.itemInfo.UpdateData(slot);
    }

    public (int count,Data_Item item) CurrentItem()
    {
        return (num,item);
    }
    [Button]
    public void UpdateItem(int _count, Data_Item _item)
    {
        if (_count == 0 || _item == null)
        {
            //if (icon.sprite == null) icon.enabled = false;
            SetNum(0);
            return;
        }

        //if (icon.sprite != null && !icon.enabled) icon.enabled = true;
        icon.sprite = _item.icon;
        SetNum(_count);
        item = _item;
        num = _count;
    }
}
