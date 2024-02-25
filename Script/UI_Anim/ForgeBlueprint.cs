using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeBlueprint : MonoBehaviour
{
    public SaveManager saveManager;
    public ForgeSaved forgeSaved;
    public TMP_Text tmpTitle;
    public Color cBtnActivated, cBtnmDeactivated,cResLack,cResFull;
    public List<Graphic> btnCoinGraphics = new List<Graphic>();
    public List<Graphic> btnGemGraphics = new List<Graphic>();
    public RectTransform rtBtnCoin, rtBtnGem;
    public List<ForgeBlueprintSlot> slots = new List<ForgeBlueprintSlot>();
    public CanvasGroup cgTitle, cgMain;
    public TypewriterCore tyTitle;
    public Transform hanger;
    public SideUI sideUI;

    private Sequence _seqForgeBP,_seqPunch;
    private Tween _tButtonPunch;
    private int _createCoin = 750, _createGem = 10;
    [HideInInspector] public bool forgeOpened = false;
    private Item_Weapon _currentWeapon = null;

    public void Start()
    {
        SetItem(null);
        gameObject.SetActive(false);
    }
    //세팅,애니메이션
    public void SetItem(Item_Weapon weapon)
    {
        _currentWeapon = weapon;
        _seqPunch.Stop();
        hanger.localScale = Vector3.one;
        _seqPunch = Sequence.Create();
        _seqPunch.Chain(Tween.PunchScale(hanger, Vector3.one * -0.1f, 0.2f, 2));
        if (weapon == null)
        {
            SetBPResource(null, slots[0],0,0);
            SetBPResource(null, slots[1],0,0);
            SetBPResource(null, slots[2],0,0);
        }
        else
        {
            if(weapon.bpWeapon1 != null) SetBPWeapon(weapon.bpWeapon1,slots[0],weapon.bpCount1
            ,saveManager.weaponDataLinker[weapon.bpWeapon1].count);
            else if(weapon.bpResource1 != null) SetBPResource(weapon.bpResource1,slots[0],weapon.bpCount1
                ,saveManager.resourceDataLinker[weapon.bpResource1].count);
            if(weapon.bpWeapon2 != null) SetBPWeapon(weapon.bpWeapon2,slots[1],weapon.bpCount2
                ,saveManager.weaponDataLinker[weapon.bpWeapon2].count);
            else if(weapon.bpResource2 != null)  SetBPResource(weapon.bpResource2,slots[1],weapon.bpCount2
                ,saveManager.resourceDataLinker[weapon.bpResource2].count);
            if(weapon.bpWeapon3 != null) SetBPWeapon(weapon.bpWeapon3,slots[2],weapon.bpCount3
                ,saveManager.weaponDataLinker[weapon.bpWeapon3].count);
            else if(weapon.bpResource3 != null)  SetBPResource(weapon.bpResource3,slots[2],weapon.bpCount3
                ,saveManager.resourceDataLinker[weapon.bpResource3].count);
        }
        
        
        void SetBPResource(Item_Resource item,ForgeBlueprintSlot slot,int requireNum,int currentNum)
        {
            if (item != null)
            {
                slot.imgIcon.gameObject.SetActive(true);
                slot.imgIcon.sprite = item.icon;
                slot.imgIcon.rectTransform.offsetMin = new Vector2(item.left, item.bottom);
                slot.imgIcon.rectTransform.offsetMax = new Vector2(-item.right, -item.top);
                slot.imgIcon.rectTransform.localScale = item.scale;
                slot.tmpTitle.text = item.title;
                slot.tmpRequirement.text = currentNum+"/"+requireNum;
                slot.locked.SetActive(false);
                if (currentNum >= requireNum) slot.tmpRequirement.color = cResFull;
                else slot.tmpRequirement.color = cResLack;
            }
            else
            {
                slot.imgIcon.gameObject.SetActive(false);
                slot.locked.SetActive(true);
                slot.tmpTitle.text = "없음";
                slot.tmpRequirement.text = "0";
                slot.tmpRequirement.color = cResLack;
            }
        }
        void SetBPWeapon(Item_Weapon item,ForgeBlueprintSlot slot,int requireNum,int currentNum)
        {
            if (item != null)
            {
                slot.imgIcon.gameObject.SetActive(true);
                slot.imgIcon.sprite = item.icon;
                slot.imgIcon.rectTransform.offsetMin = new Vector2(item.left, item.bottom);
                slot.imgIcon.rectTransform.offsetMax = new Vector2(-item.right, -item.top);
                slot.imgIcon.rectTransform.localScale = item.scale;
                slot.tmpTitle.text = item.title;
                slot.tmpRequirement.text = currentNum+"/"+requireNum;
                slot.locked.SetActive(false);
            }
            else
            {
                slot.imgIcon.gameObject.SetActive(false);
                slot.locked.SetActive(true);
                slot.tmpTitle.text = "없음";
                slot.tmpRequirement.text = "0";
            }
        }

        if(tyTitle.isHidingText) tyTitle.StopDisappearingText();
        if(tyTitle.isShowingText) tyTitle.StopShowingText();
        
        if (weapon != null)
        {
            tmpTitle.text = String.Empty;
            tyTitle.ShowText("-"+weapon.title+"-");
        }
        else tmpTitle.text = "-대상을 선택하세요-";

        if (saveManager.gem >= _createGem) foreach (var graphic in btnGemGraphics) graphic.color = cBtnActivated;
        else foreach (var graphic in btnGemGraphics) graphic.color = cBtnmDeactivated;
        
        if (saveManager.coin >= _createCoin) foreach (var graphic in btnCoinGraphics) graphic.color = cBtnActivated;
        else foreach (var graphic in btnCoinGraphics) graphic.color = cBtnmDeactivated;
    }
    public void Show(float delay)
    {
        if (forgeOpened) return;
        forgeOpened = true;
        
        gameObject.SetActive(true);
        _seqForgeBP.Stop();
        cgTitle.alpha = 0;
        cgMain.alpha = 0;
        Transform tTitle = cgTitle.transform;
        Transform tMain = cgMain.transform;
        tTitle.localScale = Vector3.one*0.8f;
        tMain.localScale = Vector3.one*0.85f;

        _seqForgeBP = Sequence.Create();
        _seqForgeBP.timeScale = 1.0f;
        delay += 0.1f;
        _seqForgeBP.Group(Tween.Alpha(cgMain, 1, 0.2f,startDelay: delay));
        _seqForgeBP.Group(Tween.Scale(tMain, 1, 0.5f, Ease.OutCubic,startDelay: delay));
        _seqForgeBP.Group(Tween.Alpha(cgTitle, 1, 0.2f,startDelay:0.125f + delay));
        _seqForgeBP.Group(Tween.Scale(tTitle, 1, 0.375f, Ease.OutCubic,startDelay:0.125f + delay));
    }
    public void Hide()
    {
        if (!forgeOpened) return;
        forgeOpened = false;
        
        _seqForgeBP.Stop();
        cgTitle.alpha = 1;
        cgMain.alpha = 1;
        Transform tTitle = cgTitle.transform;
        Transform tMain = cgMain.transform;
        tTitle.localScale = Vector3.one;
        tMain.localScale = Vector3.one;

        _seqForgeBP = Sequence.Create();
        _seqForgeBP.timeScale = 1.5f;
        _seqForgeBP.Group(Tween.Alpha(cgMain, 0, 0.2f,startDelay:0.0f+0.05f));
        _seqForgeBP.Group(Tween.Scale(tMain, 0.85f, 0.3f, Ease.InCubic,startDelay:0.0f+0.05f));
        _seqForgeBP.Group(Tween.Alpha(cgTitle, 0, 0.2f,startDelay:0.175f));
        _seqForgeBP.Group(Tween.Scale(tTitle, 0.8f, 0.375f, Ease.InCubic));
        _seqForgeBP.OnComplete(() => gameObject.SetActive(false));
    }
    //제작
    public void CreateBtn_Coin()
    {
        print("1");
        if (_currentWeapon == null)
        {
            PopupManager.instance.Negative("먼저 제작 대상을 선택하세요!");
            print("2");
            return;
        }
        bool canCreate = _currentWeapon.CanCreate(saveManager) && saveManager.coin >= _createCoin;
        if (canCreate)
        {
            PopupManager.instance.Create_Begin(false,_currentWeapon);
        }
        else
        {
            PopupManager.instance.Negative("재료 혹은 재화가 부족합니다!");
        }
    }
    public void CreateBtn_Gem()
    {
        
    }
    public void Create_WithCoin()
    {
        if (_currentWeapon == null) return;
        if (saveManager.coin < _createCoin) return;

        bool created = _currentWeapon.Create(saveManager);
        if (created)
        {
            forgeSaved.UpdateData();
            SetItem(null);
            saveManager.Coin_Remove(_createCoin);
            saveManager.Forge_Remove(_currentWeapon);
            sideUI.UpdateMoney();
        }
        
        _tButtonPunch.Stop();
        rtBtnCoin.localScale = Vector3.one*0.45f;
        rtBtnGem.localScale = Vector3.one*0.45f;
        _tButtonPunch = Tween.PunchScale(rtBtnCoin, Vector3.one * -0.2f, 0.25f, 2);
    }
    public void Create_WithGem()
    {
        if (_currentWeapon == null) return;
        if (saveManager.gem < _createGem) return;
        
        bool created = _currentWeapon.Create(saveManager);
        if (created)
        {
            foreach (var slot in forgeSaved.slots)
            {
                if (slot.CheckWeapon(_currentWeapon))
                {
                    slot.SetItem(null);
                    break;
                }
            }
            SetItem(null);
            saveManager.Coin_Remove(_createCoin);
        }
        
        _tButtonPunch.Stop();
        rtBtnCoin.localScale = Vector3.one*0.45f;
        rtBtnGem.localScale = Vector3.one*0.45f;
        _tButtonPunch = Tween.PunchScale(rtBtnGem, Vector3.one * -0.2f, 0.25f, 2);
    }
}
[System.Serializable]
public class ForgeBlueprintSlot
{
    public Image imgIcon;
    public TMP_Text tmpTitle;
    public TMP_Text tmpRequirement;
    public GameObject locked;
}
