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
    public SaveManager saveManager;
    
    
    private Item_Weapon _itemWeapon;
    private Sequence _seqSlot;

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
            //1
            if (weapon.bpWeapon1 != null) currentNum += 
                Mathf.Min(saveManager.weaponDataLinker[weapon.bpWeapon1].count,weapon.bpCount1);
            else currentNum += 
                Mathf.Min(saveManager.resourceDataLinker[weapon.bpResource1].count,weapon.bpCount1);
            //2
            if (weapon.bpWeapon2 != null) currentNum += 
                Mathf.Min(saveManager.weaponDataLinker[weapon.bpWeapon2].count,weapon.bpCount2);
            else currentNum += 
                Mathf.Min(saveManager.resourceDataLinker[weapon.bpResource2].count,weapon.bpCount2);
            //3
            if (weapon.bpWeapon3 != null) currentNum += 
                Mathf.Min(saveManager.weaponDataLinker[weapon.bpWeapon3].count,weapon.bpCount3);
            else currentNum += 
                Mathf.Min(saveManager.resourceDataLinker[weapon.bpResource3].count,weapon.bpCount3);
            
            int percentage = Mathf.RoundToInt(100 * ((float)currentNum) / ((float)requireNum));
            if (percentage < 100) tmpMakeRatio.text = Mathf.Clamp(percentage, 0, 100) + "%";
            else tmpMakeRatio.text = "준비됨";
        }
    }

    public bool CheckWeapon(Item_Weapon target)
    {
        return target == _itemWeapon;
    }
}
