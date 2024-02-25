using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
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
    public GameObject selectedFrame;
    public TMP_Text tmpCount,tmpIndex;
    public GameObject locked;
    public Image icon,bg;
    public Color colorActivated, colorDeactivated;
    public Color colorIconDeactivated = new Color(80.0f / 255.0f, 80.0f / 255.0f, 80.0f / 255.0f, 1);
    private Item_Weapon _itemWeapon;
    private Item_Resource _itemResource;
    private Tween _tSlot;
    private Sequence _seqSelect;
    private UI_Inventory _inventory;
    private Image _imgSelectedFrame,_imgSelectedFrame2;

    public void Setting(UI_Inventory inventory)
    {
        _inventory = inventory;
        _imgSelectedFrame = selectedFrame.GetComponent<Image>();
        _imgSelectedFrame2 = selectedFrame.transform.GetChild(0).GetComponent<Image>();
    }
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
        _itemWeapon = SaveManager.instance.GetWeapon(data.weaponIndex);
        icon.enabled = true;
        icon.sprite = _itemWeapon.icon;
        icon.rectTransform.offsetMin = new Vector2(_itemWeapon.left, _itemWeapon.bottom);
        icon.rectTransform.offsetMax = new Vector2(-_itemWeapon.right, -_itemWeapon.top);
        icon.rectTransform.localScale = _itemWeapon.scale;
        
        SetSlotState(data.slotState,data.count);
    }
    public void UpdateData(ResourceSaveData data)
    {
        _itemWeapon = null;
        _itemResource = SaveManager.instance.GetResource(data.resourceIndex);
        icon.enabled = true;
        icon.sprite = _itemResource.icon;
        icon.rectTransform.offsetMin = new Vector2(_itemResource.left, _itemResource.bottom);
        icon.rectTransform.offsetMax = new Vector2(-_itemResource.right, -_itemResource.top);
        icon.rectTransform.localScale = _itemResource.scale;
        
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

    public void Selected()
    {
        //스케일 조정
        _tSlot.Stop();
        Transform t = transform;
        t.localScale = Vector3.one;
        _tSlot = Tween.PunchScale(t, -Vector3.one * 0.2f, 0.2f, 2);
        
        //선택 Frame
        if (slotState == SlotState.Locked) return;
        if (selectedFrame.activeSelf&& !_seqSelect.isAlive) return;
        foreach (var slot in _inventory.slots) slot.Deselected();
        selectedFrame.SetActive(true);
        _seqSelect.Stop();
        _imgSelectedFrame.color = Color.clear;
        _imgSelectedFrame2.color = Color.clear;
        
        _seqSelect = Sequence.Create();
        _seqSelect.Group(Tween.Color(_imgSelectedFrame, Color.white, 0.25f));
        _seqSelect.Group(Tween.Color(_imgSelectedFrame2, Color.white, 0.25f));
        UI_Inventory.InventoryState inventoryState;
        if (_itemWeapon != null) inventoryState = UI_Inventory.InventoryState.Weapon;
        else if (_itemResource != null) inventoryState = UI_Inventory.InventoryState.Resource;
        else inventoryState = UI_Inventory.InventoryState.Weapon;
        _inventory.SetSelectedItem(inventoryState, _inventory.slots.IndexOf(this));
    }
    public void SelectedWithoutAnim()
    {
        if (selectedFrame.activeSelf && !_tSlot.isAlive && !_seqSelect.isAlive) return;
        _tSlot.Stop();
        transform.localScale = Vector3.one;
        _seqSelect.Stop();
        _imgSelectedFrame.color = Color.white;
        _imgSelectedFrame2.color = Color.white;
        selectedFrame.SetActive(true);
        UI_Inventory.InventoryState inventoryState;
        if (_itemWeapon != null) inventoryState = UI_Inventory.InventoryState.Weapon;
        else if (_itemResource != null) inventoryState = UI_Inventory.InventoryState.Resource;
        else inventoryState = UI_Inventory.InventoryState.Weapon;
        _inventory.SetSelectedItem(inventoryState, _inventory.slots.IndexOf(this));
    }
    public void Deselected()
    {
        _seqSelect.Stop();
        selectedFrame.SetActive(false);
    }
    public Item_Weapon GetWeapon()
    {
        return _itemWeapon;
    }
}
