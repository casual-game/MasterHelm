using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class UI_ReturnItemSlot : MonoBehaviour
{
    public static int maxSlot = 3,slotCount=0;
    private Data_Item item;
    public void Setting()
    {
        icon = transform.Find("icon").GetComponent<Image>();
        icon.color = Color.black;
        selectedInner = transform.Find("SelectedInner").GetComponent<Image>();
        selectedOuter = GetComponent<Image>();
        
        selectedRatio = 0;
        selectedInner.color = Color.clear;
        selectedOuter.color = Color.clear;
        tmp_num = transform.GetComponentInChildren<TMP_Text>(true);
        isSelected = true;
        Selected();
        slotCount = 0;
    }
    //Selected
    [BoxGroup("선택 효과")] public Color selectedInnerColor, selectedOuterColor;
    [BoxGroup("선택 효과")] public float selectedSpeed = 1.0f;
    [BoxGroup("선택 효과")] public AnimationCurve selectedCurve;
    private Image selectedInner, selectedOuter,icon;
    private Coroutine c_selected;
    private float selectedRatio = 0;
    private bool isSelected = false;
    public void Selected()
    {
        if (Canvas_Player.instance.returnPressed)
        {
            return;
        }
        if (!isSelected)
        {
            if (slotCount >= Mathf.Min(Canvas_Player.instance.Death_ItemCount, maxSlot) || item == null)
            {
                return;
            }
            else slotCount++;
        }
        else slotCount--;
        
        isSelected = !isSelected;
        if (gameObject.activeInHierarchy)
        {
            if(c_selected!=null) StopCoroutine(c_selected);
            c_selected = StartCoroutine(C_Selected(isSelected));
        }
        else
        {
            if (isSelected) selectedRatio = 1;
            else selectedRatio = 0;
            ColorRatio();
        }
        
        Canvas_Player.instance.Death_UpdateNum(slotCount);
    }
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
    private int num = 0;
    public void CopySlot(UI_NormalItemSlot slot)
    {
        int num = slot.num;
        this.num = num;
        if (num > 0)
        {
            tmp_num.transform.parent.gameObject.SetActive(true);
            tmp_num.text = num.ToString();
        }
        else tmp_num.transform.parent.gameObject.SetActive(false);

        if (slot.item == null || slot.num < 1)
        {
            icon.color = Color.black;
            item = null;
        }
        else
        {
            item = slot.item;
            icon.sprite = slot.item.icon;
            icon.color = Color.white * 0.9f;
        }
    }
}
