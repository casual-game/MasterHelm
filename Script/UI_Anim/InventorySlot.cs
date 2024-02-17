using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public enum SlotState
    {
        Locked=0,Found=1,Opened=2
    }

    [ReadOnly] public SlotState slotState;
    public TMP_Text tmpCount,tmpIndex;
    public GameObject locked;
    public Image icon,bg;
    public Color colorActivated, colorDeactivated;
    public Color colorIconDeactivated = new Color(80.0f / 255.0f, 80.0f / 255.0f, 80.0f / 255.0f, 1);
    private Item_Weapon _itemWeapon;
    private Item_Resource _itemResource;
    
    
    private void SetSlotState(SlotState state,int count)
    {
        slotState = state;
        switch (slotState)
        {
            case SlotState.Locked:
                locked.SetActive(true);
                icon.gameObject.SetActive(false);
                bg.color = colorActivated;
                tmpCount.text = String.Empty;
                break;
            case SlotState.Found:
                locked.SetActive(false);
                icon.gameObject.SetActive(true);
                icon.color = colorIconDeactivated;
                bg.color = colorDeactivated;
                tmpCount.text = "X";
                tmpCount.color = Color.red;
                break;
            case SlotState.Opened:
                locked.SetActive(false);
                icon.gameObject.SetActive(true);
                icon.color = Color.white;
                bg.color = colorActivated;
                tmpCount.text = count.ToString();
                tmpCount.color = Color.white;
                break;
        }
    }
    public void UpdateData(WeaponSaveData data)
    {
        _itemResource = null;
        _itemWeapon = data.weapon;
        icon.enabled = true;
        icon.sprite = data.weapon.icon;
        icon.rectTransform.offsetMin = new Vector2(data.weapon.left, data.weapon.bottom);
        icon.rectTransform.offsetMax = new Vector2(-data.weapon.right, -data.weapon.top);
        icon.rectTransform.localScale = data.weapon.scale;
        
        SetSlotState(data.slotState,data.count);
    }
    public void UpdateData(ResourceSaveData data)
    {
        _itemWeapon = null;
        _itemResource = data.resource;
        icon.enabled = true;
        icon.sprite = data.resource.icon;
        icon.rectTransform.offsetMin = new Vector2(data.resource.left, data.resource.bottom);
        icon.rectTransform.offsetMax = new Vector2(-data.resource.right, -data.resource.top);
        icon.rectTransform.localScale = data.resource.scale;
        
        SetSlotState(data.slotState,data.count);
    }
    public void ClearData()
    {
        _itemWeapon = null;
        _itemResource = null;
        tmpCount.text= String.Empty;
        icon.enabled = false;
        SetSlotState(SlotState.Locked,0);
    }
}
