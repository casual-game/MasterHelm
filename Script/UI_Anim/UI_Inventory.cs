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
    public ForgeSaved forgeSaved;
    public ForgeBlueprint forgeBlueprint;
    public SideUI sideUI;
    private Sequence _seqInventory,_seqInfo;
    private InventoryState _state;
    private (InventoryState state, int index) _selectedItem;
    private bool isFirst;
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
        //착용 장비 저장 데이터 반영
        slotWeaponMain.ChangeData(saveManager.GetWeapon(saveManager.equipWeaponMain));
        slotWeaponSkillL.ChangeData(saveManager.GetWeapon(saveManager.equipWeaponSkillL));
        slotWeaponSkillR.ChangeData(saveManager.GetWeapon(saveManager.equipWeaponSkillR));
        isFirst = true;
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
        
        if (isFirst)
        {
            isFirst = false;
            UpdateState(InventoryState.Weapon);
            _selectedItem.index = -1;
            SetSelectedItem(InventoryState.Weapon,0,false);
            slots[0].Selected();
        }
        else UpdateState(_state);
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
        sideUI.EquipWeapon(saveManager.GetWeapon(saveManager.equipWeaponMain),true);
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
        _state = inventoryState;
        Close(0.15f,0.05f,true);
        float startDelay = 0.3f;
        for (int h = 3; h >= 0; h--)
        {
            for (int w = 0; w < 3; w++)
            {
                _seqInventory.Group(Tween.Delay(startDelay, () => UpdateState(inventoryState)));
                _seqInventory.Group(Tween.Scale(_slotImages[h * 3 + w].transform, 1, 
                    0.15f, Ease.OutCubic,startDelay: startDelay));
                _seqInventory.Group(Tween.Alpha(_slotCanvasGroups[h * 3 + w],1,
                    0.15f, Ease.OutCubic,startDelay: startDelay));
            }
            startDelay += 0.05f;
        }
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_book,0.15f);
        sideUI.EquipWeapon(saveManager.GetWeapon(saveManager.equipWeaponMain),true);
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
    private void UpdateState(InventoryState inventoryState)
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
    public void SetSelectedItem(InventoryState state, int index,bool anim)
    {
        if (_selectedItem.state == state && _selectedItem.index == index) return;
        _selectedItem = (state, index);
        //Info 교체
        if (state == InventoryState.Weapon)
        {
            var weapon = saveManager.GetWeapon(saveManager.weaponSaveDatas[index].weaponIndex);
            tmpItemTitle.text = weapon.title;
            tmpItemInfo.text = String.Empty;
            twItemInfo.ShowText(weapon.info);
            if(anim) sideUI.EquipWeapon(weapon,false);
        }
        else if (state == InventoryState.Resource)
        {
            var resource = saveManager.GetResource(saveManager.resourceSaveDatas[index].resourceIndex);
            tmpItemTitle.text = resource.title;
            tmpItemInfo.text = String.Empty;
            twItemInfo.ShowText(resource.info);
            if(anim) sideUI.EquipWeapon(saveManager.GetWeapon(saveManager.equipWeaponMain),true);
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
            ShowBtnGroup_Origin();
        }
        else
        {
            RemoveBtnGroup_OriginSelect();
        }
    }
    //착용 관련 변수
    public EauipmentSlot_Weapon slotWeaponMain, slotWeaponSkillL, slotWeaponSkillR;
    public List<CanvasGroup> cgBtnGroupsOrigin = new List<CanvasGroup>();
    public List<CanvasGroup> cgBtnGroupSelectSlot = new List<CanvasGroup>();
    public TMP_Text tmpBtnForge;
    public Transform equipFlagT;
    private bool isBtnGroupOrigin,isBtnForgeOrigin;
    private bool isBGOriginSelectActivated, isBGForgeForgeActivated;
    private Sequence seqBtnGroups_OriginSelect,seqBtnGroups_ForgeForge,seqEquip;
    //착용 부분 제어
    public void ShowBtnGroup_Origin()
    {
        UpdateForgeButton();
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
    public void ShowBtnGroup_SelectSlot()
    {
        if (!isBtnGroupOrigin && isBGOriginSelectActivated) return;
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
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

    public void Equip_WeaponMain()
    {
        if (_selectedItem.state != InventoryState.Weapon) return;
        Item_Weapon currentWeapon = slots[_selectedItem.index].GetWeapon();
        Item_Weapon currentSlotWeapon = slotWeaponMain.weapon;

        if (slotWeaponSkillL.weapon == currentWeapon) slotWeaponSkillL.ChangeData(currentSlotWeapon);
        if (slotWeaponSkillR.weapon == currentWeapon) slotWeaponSkillR.ChangeData(currentSlotWeapon);
        slotWeaponMain.ChangeData(currentWeapon);
        saveManager.EquipUpdate(slotWeaponMain.weapon,slotWeaponSkillL.weapon,slotWeaponSkillR.weapon);
        PopupManager.instance.Positive("무기를 메인 슬롯에 장착했습니다.",1.5f);
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
        PopupManager.instance.Positive("무기를 스킬L 슬롯에 장착했습니다.",1.5f);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
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
        PopupManager.instance.Positive("무기를 스킬R 슬롯에 장착했습니다.",1.5f);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        //시퀸스
        seqEquip.Stop();
        seqEquip = Sequence.Create();
        seqEquip.Chain(Tween.PunchScale(equipFlagT, Vector3.one * -0.1f,0.25f,2));
    }
    public void Btn_Forge()
    {
        Item_Weapon weapon = saveManager.GetWeapon(_selectedItem.index);
        //도면 삭제
        if (saveManager.Forge_Contain(weapon))
        {
            saveManager.Forge_Remove(weapon);
            PopupManager.instance.Negative("대장간에서 도면이 삭제되었습니다.",1.5f);
        }
        //도면 저장
        else
        {
            if (saveManager.forgeWeaponDatas.Count >= 6)
            {
                PopupManager.instance.Negative("저장된 도면이 너무 많습니다!",1.5f);
                return;
            }
            saveManager.Forge_Add(weapon);
            PopupManager.instance.Positive("대장간에 도면이 저장되었습니다!",1.5f);
        }
        if(_state == InventoryState.Weapon) slots[_selectedItem.index].UpdateData(saveManager.weaponDataLinker[weapon]);
        UpdateForgeButton();
        forgeSaved.UpdateData();
        forgeBlueprint.SetItem(null);
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        //Info 시퀸스
        _seqInfo.Stop();
        Transform tInfo = cgInfo.transform;
        tInfo.localScale = Vector3.one*0.95f;
        cgInfo.alpha = 1;
        _seqInfo = Sequence.Create();
        _seqInfo.Group(Tween.PunchScale(tInfo, Vector3.one*-0.1f, 0.2f, 2));
    }
    private void UpdateForgeButton()
    {
        if (!saveManager.forgeWeaponDatas.Contains(_selectedItem.index)) tmpBtnForge.text = "도면 저장";
        else tmpBtnForge.text = "도면 삭제";
    }
    //기타
    public void Popup_Dev()
    {
        PopupManager.instance.Popup_Dev();
    }
}
