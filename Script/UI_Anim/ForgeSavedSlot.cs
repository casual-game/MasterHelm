using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeSavedSlot : MonoBehaviour
{
    public Image imgSelected;
    public Image imgMakeRatio;
    public Color cImgMakeSelected, cImgMakeDeselected;
    public TMP_Text tmpMakeRatio;
    public Image imgIcon,imgLocked;
    public ForgeBlueprint forgeBlueprint;
    public List<ForgeSavedSlot> slots = new List<ForgeSavedSlot>();
    public Item_Weapon debugWeapon;
    public SaveManager saveManager;
    
    
    private Item_Weapon _itemWeapon;
    private Sequence _seqSlot;

    public void Start()
    {
        SetItem(debugWeapon);
    }
    public void Selected()
    {
        _seqSlot.Stop();
        transform.localScale = Vector3.one*0.8f;
        _seqSlot = Sequence.Create();
        _seqSlot.Chain(Tween.PunchScale(transform, Vector3.one * -0.25f, 0.2f, 2));
        //if (_itemWeapon == null) return;
        imgSelected.gameObject.SetActive(true);
        imgMakeRatio.color = cImgMakeSelected;
        foreach (var slot in slots) if(slot!=this) slot.Deselected();
        forgeBlueprint.SetItem(_itemWeapon);
    }
    public void Deselected()
    {
        imgSelected.gameObject.SetActive(false);
        imgMakeRatio.color = cImgMakeDeselected;
    }
    [Button]
    public void SetItem(Item_Weapon weapon)
    {
        if (weapon == null)
        {
            imgLocked.gameObject.SetActive(true);
            imgIcon.gameObject.SetActive(false);
            tmpMakeRatio.text = "없음";
        }
        else
        {
            _itemWeapon = weapon;
            imgLocked.gameObject.SetActive(false);
            imgIcon.gameObject.SetActive(true);
            imgIcon.sprite = weapon.icon;
            imgIcon.rectTransform.offsetMin = new Vector2(weapon.left, weapon.bottom);
            imgIcon.rectTransform.offsetMax = new Vector2(-weapon.right, -weapon.top);
            imgIcon.rectTransform.localScale = weapon.scale;
            int requireNum = weapon.bpCount1 + weapon.bpCount2 + weapon.bpCount3;
            int currentNum=0;
            if (weapon.bpWeapon1 != null) currentNum += saveManager.weaponDataLinker[weapon.bpWeapon1].count;
            else currentNum += saveManager.resourceDataLinker[weapon.bpResource1].count;
            if (weapon.bpWeapon2 != null) currentNum += saveManager.weaponDataLinker[weapon.bpWeapon2].count;
            else currentNum += saveManager.resourceDataLinker[weapon.bpResource2].count;
            if (weapon.bpWeapon3 != null) currentNum += saveManager.weaponDataLinker[weapon.bpWeapon3].count;
            else currentNum += saveManager.resourceDataLinker[weapon.bpResource3].count;
            tmpMakeRatio.text = Mathf.RoundToInt(100 * ((float)currentNum) / ((float)requireNum)).ToString()+"%";
        }
    }

}
