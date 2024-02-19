using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using PrimeTween;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    //인벤토리 관련 변수
    public enum InventoryState
    {
        Weapon=0,Resource=1,Badge=2
    }

    public UIElement_Tip tip;
    public SaveManager saveManager;
    public CanvasGroup cgInfo;
    public TypewriterCore twItemInfo;
    public ContentSizeFitter fitterItemTitle;
    public TMP_Text tmpItemTitle,tmpItemInfo;
    public RectTransform rectT;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public List<Image> _slotImages = new List<Image>();
    public List<CanvasGroup> _slotCanvasGroups = new List<CanvasGroup>();
    private Sequence _seqInventory,_seqInfo;
    private InventoryState _state;
    private (InventoryState state, int index) _selectedItem;
    public void Setting()
    {
        gameObject.SetActive(false);
        _state = InventoryState.Weapon;
        foreach (var slot in slots) slot.Setting(this);
        cgInfo.alpha = 0;
        isBtnGroupOrigin = true;
        isBtnForgeOrigin = true;
        isBGOriginSelectActivated = false;
        isBGForgeForgeActivated = false;
        //장비 저장 데이터 반영
        slotWeaponMain.ChangeData(saveManager.equipWeaponMain);
        slotWeaponSkillL.ChangeData(saveManager.equipWeaponSkillL);
        slotWeaponSkillR.ChangeData(saveManager.equipWeaponSkillR);
    }
    //인벤토리 부분 제어
    public void FitUI()
    {
        var rect = rectT.rect;
        float a, b, x, width = rect.width, height = rect.height;
        x = Mathf.Min(width / 3.0f, height / 4.0f) * 0.75f;
        a = (width - 3 * x) * 0.25f;
        b = (height - 4 * x) * 0.2f;
        
        for (int w = 0; w < 3; w++)
        {
            for (int h = 0; h < 4; h++)
            {
                RectTransform rt = transform.GetChild(3 * h + w).GetComponent<RectTransform>();
                rt.anchoredPosition= new Vector2((w+1)*a + w*x + 0.5f*x,(h+1)*b + h*x - height + 0.5f*x);
                rt.sizeDelta = Vector2.one*x;
            }
        }
    }
    public void Open(float duration = 0.15f,float delay = 0.05f)
    {
        UpdateItemIndex(0);
        UpdateState(InventoryState.Weapon);
        _selectedItem.index = -1;
        SetSelectedItem(InventoryState.Weapon,0);
        slots[0].Selected();
        _seqInventory.Stop();
        foreach (var slot in _slotImages) slot.transform.localScale = Vector3.one*0.75f;
        foreach (var slot in _slotCanvasGroups) slot.alpha = 0;
        gameObject.SetActive(true);
        //인벤토리 시퀸스
        _seqInventory = Sequence.Create();
        float startDelay = 0;
        for (int h = 3; h >= 0; h--)
        {
            for (int w = 0; w < 3; w++)
            {
                _seqInventory.Group(Tween.Scale(_slotImages[h * 3 + w].transform, 1, 
                    duration, Ease.OutCubic,startDelay: startDelay));
                //_seqInventory.Group(Tween.Color(_slotImages[h * 3 + w],Color.white,
                    //duration, Ease.OutSine,startDelay: startDelay));
                _seqInventory.Group(Tween.Alpha(_slotCanvasGroups[h * 3 + w],1,
                    duration, Ease.OutCubic,startDelay: startDelay));
            }

            startDelay += delay;
        }
        //Info 시퀸스
        _seqInfo.Stop();
        Transform tInfo = cgInfo.transform;
        cgInfo.alpha = 0;
        tInfo.localScale = Vector3.one*0.75f;
        _seqInfo = Sequence.Create();
        _seqInfo.Chain(Tween.Alpha(cgInfo, 1, 0.375f));
        _seqInfo.Group(Tween.Scale(tInfo, 0.95f, 0.375f, Ease.OutCubic));

    }
    public void Reroll_Weapon()
    {
        Reroll(InventoryState.Weapon);
    }
    public void Reroll_Resource()
    {
        Reroll(InventoryState.Resource);
    }
    public void Reroll_Badge()
    {
        Reroll(InventoryState.Badge);
    }
    private void Reroll(InventoryState inventoryState)
    {
        Close(0.15f,0.05f,true);
        float startDelay = 0.3f;
        for (int h = 3; h >= 0; h--)
        {
            for (int w = 0; w < 3; w++)
            {
                _seqInventory.Group(Tween.Delay(startDelay, () => UpdateState(inventoryState)));
                _seqInventory.Group(Tween.Scale(_slotImages[h * 3 + w].transform, 1, 
                    0.15f, Ease.OutCubic,startDelay: startDelay));
                //_seqInventory.Group(Tween.Color(_slotImages[h * 3 + w],Color.white,
                  //  0.15f, Ease.OutSine,startDelay: startDelay));
                _seqInventory.Group(Tween.Alpha(_slotCanvasGroups[h * 3 + w],1,
                    0.15f, Ease.OutCubic,startDelay: startDelay));
            }
            startDelay += 0.05f;
        }
    }
    private void Page()
    {
        _seqInventory.Stop();
        transform.localScale = Vector3.one;

        _seqInventory = Sequence.Create();
        Tween.PunchScale(transform, Vector3.down*0.15f, 0.15f, 1);
    }
    public void Close(float duration = 0.15f,float delay = 0.05f,bool activate = false)
    {
        _seqInventory.Stop();
        foreach (var slot in _slotImages) slot.transform.localScale = Vector3.one;
        foreach (var slot in _slotCanvasGroups) slot.alpha = 1;
        
        //인벤토리 시퀸스
        _seqInventory = Sequence.Create();
        float startDelay = 0;
        for (int h = 3; h >= 0; h--)
        {
            for (int w = 0; w < 3; w++)
            {
                _seqInventory.Group(Tween.Scale(_slotImages[h * 3 + w].transform, 0.75f, 
                    duration, Ease.InCubic,startDelay: startDelay));
                _seqInventory.Group(Tween.Color(_slotImages[h * 3 + w],Color.clear,
                    duration, Ease.InSine,startDelay: startDelay));
                _seqInventory.Group(Tween.Alpha(_slotCanvasGroups[h * 3 + w],0,
                    duration, Ease.InCubic,startDelay: startDelay));
            }

            startDelay += delay;
        }

        if (!activate)
        {
            _seqInventory.OnComplete(() => gameObject.SetActive(false));
            //Info 시퀸스
            _seqInfo.Stop();
            Transform tInfo = cgInfo.transform;
            cgInfo.alpha = 1;
            tInfo.localScale = Vector3.one*0.95f;
            _seqInfo = Sequence.Create();
            _seqInfo.Chain(Tween.Alpha(cgInfo, 0, 0.375f));
            _seqInfo.Group(Tween.Scale(tInfo, 0.75f, 0.375f, Ease.InCubic));
        }
    }
    private void UpdateItemIndex(int page = 0)
    {
        for(int i=0; i<slots.Count; i++)
        {
            slots[i].tmpIndex.text = ((page + 1) * (i + 1)).ToString();
        }
    }
    public void UpdateState(InventoryState inventoryState)
    {
        foreach (var slot in slots) slot.Deselected();
        
        _state = inventoryState;
        switch (_state)
        {
            case InventoryState.Weapon:
                for (int i = 0; i < slots.Count; i++)
                {
                    var slot = slots[i];
                    if(saveManager.weaponSaveDatas.Count>i) slot.UpdateData(saveManager.weaponSaveDatas[i]);
                    else slot.ClearData();
                }
                break;
            case InventoryState.Resource:
                for (int i = 0; i < slots.Count; i++)
                {
                    var slot = slots[i];
                    if(saveManager.resourceSaveDatas.Count>i) slot.UpdateData(saveManager.resourceSaveDatas[i]);
                    else slot.ClearData();
                }
                break;
            case InventoryState.Badge:
                for (int i = 0; i < slots.Count; i++)
                {
                    var slot = slots[i];
                    slot.ClearData();
                }
                break;
        }
        if(_selectedItem.state == inventoryState)  slots[_selectedItem.index].SelectedWithoutAnim();
    }
    public void SetSelectedItem(InventoryState state, int index)
    {
        if (_selectedItem.state == state && _selectedItem.index == index) return;
        _selectedItem = (state, index);
        //Info 교체
        if (state == InventoryState.Weapon)
        {
            tmpItemTitle.text = saveManager.weaponSaveDatas[index].weapon.title;
            tmpItemInfo.text = String.Empty;
            twItemInfo.ShowText(saveManager.weaponSaveDatas[index].weapon.info);
        }
        else if (state == InventoryState.Resource)
        {
            tmpItemTitle.text = saveManager.resourceSaveDatas[index].resource.title;
            tmpItemInfo.text = String.Empty;
            twItemInfo.ShowText(saveManager.resourceSaveDatas[index].resource.info);
        }
        fitterItemTitle.SetLayoutHorizontal();
        //Info 시퀸스
        _seqInfo.Stop();
        Transform tInfo = cgInfo.transform;
        tInfo.localScale = Vector3.one*0.95f;
        cgInfo.alpha = 1;
        _seqInfo = Sequence.Create();
        _seqInfo.Group(Tween.PunchScale(tInfo, Vector3.one*-0.1f, 0.2f, 2));
        //착용 버튼 그룹
        if (state == InventoryState.Weapon)
        {
            var data = saveManager.weaponSaveDatas[index];
            if (data.count > 0)
            {
                ShowBtnGroup_Origin();
                RemoveBtnGroup_ForgeForge();
            }
            else
            {
                if(!saveManager.forgeWeaponDatas.Contains(data.weapon)) ShowBtnGroup_AddForge();
                else ShowBtnGroup_GoForge();
                RemoveBtnGroup_OriginSelect();
            }
        }
        else
        {
            RemoveBtnGroup_ForgeForge();
            RemoveBtnGroup_OriginSelect();
        }
    }
    //착용 관련 변수
    public EauipmentSlot_Weapon slotWeaponMain, slotWeaponSkillL, slotWeaponSkillR;
    public List<CanvasGroup> cgBtnGroupsOrigin = new List<CanvasGroup>();
    public List<CanvasGroup> cgBtnGroupSelectSlot = new List<CanvasGroup>();
    public CanvasGroup cgBtnAddForge,cgBtnGoForge;
    public Transform equipFlagT;
    private bool isBtnGroupOrigin,isBtnForgeOrigin;
    private bool isBGOriginSelectActivated, isBGForgeForgeActivated;
    private Sequence seqBtnGroups_OriginSelect,seqBtnGroups_ForgeForge,seqEquip;
    //착용 부분 제어
    [Button]
    public void ShowBtnGroup_Origin()
    {
        if (isBtnGroupOrigin && isBGOriginSelectActivated) return;
        isBtnGroupOrigin = true;
        isBGOriginSelectActivated = true;
        seqBtnGroups_OriginSelect.Stop();
        seqBtnGroups_OriginSelect = Sequence.Create();
        //생성
        foreach (var cgBtn in cgBtnGroupsOrigin)
        {
            cgBtn.gameObject.SetActive(true);
            seqBtnGroups_OriginSelect.Group(Tween.Alpha(cgBtn, 1,0.2f));
            seqBtnGroups_OriginSelect.Group(Tween.Scale(cgBtn.transform, 1.0f, 0.2f, Ease.OutCubic));
        } 
        //제거1
        foreach (var cgBtn in cgBtnGroupSelectSlot)
        {
            cgBtn.gameObject.SetActive(true);
            seqBtnGroups_OriginSelect.Group(Tween.Alpha(cgBtn, 0,0.2f));
            seqBtnGroups_OriginSelect.Group(Tween.Scale(cgBtn.transform, 0.8f, 0.2f, Ease.OutCubic));
        }
        seqBtnGroups_OriginSelect.OnComplete(() =>
        {
            foreach (var cgBtn in cgBtnGroupsOrigin) cgBtn.gameObject.SetActive(true);
            foreach (var cgBtn in cgBtnGroupSelectSlot) cgBtn.gameObject.SetActive(false);
        });
    }
    [Button]
    public void ShowBtnGroup_SelectSlot()
    {
        if (!isBtnGroupOrigin && isBGOriginSelectActivated) return;
        isBGOriginSelectActivated = true;
        isBtnGroupOrigin = false;
        seqBtnGroups_OriginSelect.Stop();
        seqBtnGroups_OriginSelect = Sequence.Create();
        //생성
        foreach (var cgBtn in cgBtnGroupSelectSlot)
        {
            cgBtn.gameObject.SetActive(true);
            seqBtnGroups_OriginSelect.Group(Tween.Alpha(cgBtn, 1,0.2f));
            seqBtnGroups_OriginSelect.Group(Tween.Scale(cgBtn.transform, 1.0f, 0.2f, Ease.OutCubic));
        } 
        //제거
        foreach (var cgBtn in cgBtnGroupsOrigin)
        {
            cgBtn.gameObject.SetActive(true);
            seqBtnGroups_OriginSelect.Group(Tween.Alpha(cgBtn, 0,0.2f));
            seqBtnGroups_OriginSelect.Group(Tween.Scale(cgBtn.transform, 0.8f, 0.2f, Ease.OutCubic));
        } 
        
        seqBtnGroups_OriginSelect.OnComplete(() =>
        {
            foreach (var cgBtn in cgBtnGroupSelectSlot) cgBtn.gameObject.SetActive(true);
            foreach (var cgBtn in cgBtnGroupsOrigin) cgBtn.gameObject.SetActive(false);
        });
        
        //Info 시퀸스
        _seqInfo.Stop();
        Transform tInfo = cgInfo.transform;
        tInfo.localScale = Vector3.one*0.95f;
        cgInfo.alpha = 1;
        _seqInfo = Sequence.Create();
        _seqInfo.Group(Tween.PunchScale(tInfo, Vector3.one*-0.1f, 0.2f, 2));
    }
    [Button]
    public void RemoveBtnGroup_OriginSelect()
    {
        if (!isBGOriginSelectActivated) return;
        seqBtnGroups_OriginSelect = Sequence.Create();
        isBGOriginSelectActivated = false;
        //제거1
        foreach (var cgBtn in cgBtnGroupSelectSlot)
        {
            cgBtn.gameObject.SetActive(true);
            seqBtnGroups_OriginSelect.Group(Tween.Alpha(cgBtn, 0,0.2f));
            seqBtnGroups_OriginSelect.Group(Tween.Scale(cgBtn.transform, 0.8f, 0.2f, Ease.OutCubic));
        } 
        //제거2
        foreach (var cgBtn in cgBtnGroupsOrigin)
        {
            cgBtn.gameObject.SetActive(true);
            seqBtnGroups_OriginSelect.Group(Tween.Alpha(cgBtn, 0,0.2f));
            seqBtnGroups_OriginSelect.Group(Tween.Scale(cgBtn.transform, 0.8f, 0.2f, Ease.OutCubic));
        }  
        //끝나고
        seqBtnGroups_OriginSelect.OnComplete(() =>
        {
            foreach (var cgBtn in cgBtnGroupSelectSlot) cgBtn.gameObject.SetActive(false);
            foreach (var cgBtn in cgBtnGroupsOrigin) cgBtn.gameObject.SetActive(false);
        });
    }
    [Button]
    public void ShowBtnGroup_AddForge()
    {
        if (isBtnForgeOrigin && isBGForgeForgeActivated) return;
        isBGForgeForgeActivated = true;
        isBtnForgeOrigin = true;
        seqBtnGroups_ForgeForge.Stop();
        seqBtnGroups_ForgeForge = Sequence.Create();
        //생성
        cgBtnAddForge.gameObject.SetActive(true);
        seqBtnGroups_ForgeForge.Group(Tween.Alpha(cgBtnAddForge, 1,0.2f));
        seqBtnGroups_ForgeForge.Group(Tween.Scale(cgBtnAddForge.transform, 1.0f, 0.2f, Ease.OutCubic));
        //제거1
        cgBtnGoForge.gameObject.SetActive(true);
        seqBtnGroups_ForgeForge.Group(Tween.Alpha(cgBtnGoForge, 0,0.2f));
        seqBtnGroups_ForgeForge.Group(Tween.Scale(cgBtnGoForge.transform, 0.8f, 0.2f, Ease.OutCubic));
        //콜백
        seqBtnGroups_ForgeForge.OnComplete(() =>
        {
            cgBtnAddForge.gameObject.SetActive(true);
            cgBtnGoForge.gameObject.SetActive(false);
        });
    }
    [Button]
    public void ShowBtnGroup_GoForge()
    {
        if (!isBtnForgeOrigin&& isBGForgeForgeActivated) return;
        isBGForgeForgeActivated = true;
        isBtnForgeOrigin = false;
        seqBtnGroups_ForgeForge.Stop();
        seqBtnGroups_ForgeForge = Sequence.Create();
        //생성
        cgBtnGoForge.gameObject.SetActive(true);
        seqBtnGroups_ForgeForge.Group(Tween.Alpha(cgBtnGoForge, 1,0.2f));
        seqBtnGroups_ForgeForge.Group(Tween.Scale(cgBtnGoForge.transform, 1.0f, 0.2f, Ease.OutCubic));
        //제거1
        cgBtnAddForge.gameObject.SetActive(true);
        seqBtnGroups_ForgeForge.Group(Tween.Alpha(cgBtnAddForge, 0,0.2f));
        seqBtnGroups_ForgeForge.Group(Tween.Scale(cgBtnAddForge.transform, 0.8f, 0.2f, Ease.OutCubic));
        //콜백
        seqBtnGroups_ForgeForge.OnComplete(() =>
        {
            cgBtnAddForge.gameObject.SetActive(false);
            cgBtnGoForge.gameObject.SetActive(true);
        });
    }
    [Button]
    public void RemoveBtnGroup_ForgeForge()
    {
        if (!isBGForgeForgeActivated) return;
        isBGForgeForgeActivated = false;
        seqBtnGroups_ForgeForge = Sequence.Create();
        //제거1
        cgBtnAddForge.gameObject.SetActive(true);
        seqBtnGroups_ForgeForge.Group(Tween.Alpha(cgBtnAddForge, 0,0.2f));
        seqBtnGroups_ForgeForge.Group(Tween.Scale(cgBtnAddForge.transform, 0.8f, 0.2f, Ease.OutCubic)); 
        //제거2
        cgBtnGoForge.gameObject.SetActive(true);
        seqBtnGroups_ForgeForge.Group(Tween.Alpha(cgBtnGoForge, 0,0.2f));
        seqBtnGroups_ForgeForge.Group(Tween.Scale(cgBtnGoForge.transform, 0.8f, 0.2f, Ease.OutCubic)); 
        //끝나고
        seqBtnGroups_ForgeForge.OnComplete(() =>
        {
            cgBtnAddForge.gameObject.SetActive(false);
            cgBtnGoForge.gameObject.SetActive(false);
        });
    }

    public void Equip_WeaponMain()
    {
        if (_selectedItem.state != InventoryState.Weapon) return;
        Item_Weapon currentWeapon = slots[_selectedItem.index].GetWeapon();
        Item_Weapon currentSlotWeapon = slotWeaponMain.weapon;

        if (slotWeaponSkillL.weapon == currentWeapon) slotWeaponSkillL.ChangeData(currentSlotWeapon);
        if (slotWeaponSkillR.weapon == currentWeapon) slotWeaponSkillR.ChangeData(currentSlotWeapon);
        slotWeaponMain.ChangeData(currentWeapon);
        saveManager.EquipUpdate(slotWeaponMain.weapon,slotWeaponSkillL.weapon,slotWeaponSkillR.weapon);
        //시퀸스
        seqEquip.Stop();
        seqEquip = Sequence.Create();
        seqEquip.Chain(Tween.PunchScale(equipFlagT, Vector3.one * -0.1f,0.25f,2));
    }
    public void Equip_SkillL()
    {
        if (_selectedItem.state != InventoryState.Weapon) return;
        var currentWeapon = slots[_selectedItem.index].GetWeapon();
        var currentSlotWeapon = slotWeaponSkillL.weapon;
        
        if (slotWeaponMain.weapon == currentWeapon) slotWeaponMain.ChangeData(currentSlotWeapon);
        if (slotWeaponSkillR.weapon == currentWeapon) slotWeaponSkillR.ChangeData(currentSlotWeapon);
        slotWeaponSkillL.ChangeData(currentWeapon);
        saveManager.EquipUpdate(slotWeaponMain.weapon,slotWeaponSkillL.weapon,slotWeaponSkillR.weapon);
        //시퀸스
        seqEquip.Stop();
        seqEquip = Sequence.Create();
        seqEquip.Chain(Tween.PunchScale(equipFlagT, Vector3.one * -0.1f,0.25f,2));
    }
    public void Equip_SkillR()
    {
        if (_selectedItem.state != InventoryState.Weapon) return;
        var currentWeapon = slots[_selectedItem.index].GetWeapon();
        var currentSlotWeapon = slotWeaponSkillR.weapon;
        
        if (slotWeaponMain.weapon == currentWeapon) slotWeaponMain.ChangeData(currentSlotWeapon);
        if (slotWeaponSkillL.weapon == currentWeapon) slotWeaponSkillL.ChangeData(currentSlotWeapon);
        slotWeaponSkillR.ChangeData(currentWeapon);
        saveManager.EquipUpdate(slotWeaponMain.weapon,slotWeaponSkillL.weapon,slotWeaponSkillR.weapon);
        //시퀸스
        seqEquip.Stop();
        seqEquip = Sequence.Create();
        seqEquip.Chain(Tween.PunchScale(equipFlagT, Vector3.one * -0.1f,0.25f,2));
    }
    //기타
    public void Tip_NotReady()
    {
        tip.Tip_NotReady();
    }
}
