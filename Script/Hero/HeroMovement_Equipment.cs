using System.Collections;
using System.Collections.Generic;
using RPGCharacterAnims.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class HeroMovement : MonoBehaviour
{
    [FoldoutGroup("Equipment")] public Data_WeaponPack weaponPack_Main,weaponPack_SkillL,weaponPack_SkillR
                                                        ,weaponPack_SubL,weaponPack_SubR,weaponPack_Ranged;
    [FoldoutGroup("Equipment")] public Transform t_lhand, t_rhand, t_shield;
    
    private Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, Prefab_Prop shield)> weapondata;
    private Data_WeaponPack _currentWeaponPack = null;
    private void Setting_Equipment()
    {
        weapondata = new Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, Prefab_Prop shield)>();
        AddWeaponPack(weaponPack_Main);
        AddWeaponPack(weaponPack_SkillL);
        AddWeaponPack(weaponPack_SkillR);
        AddWeaponPack(weaponPack_SubL);
        AddWeaponPack(weaponPack_SubR);
        AddWeaponPack(weaponPack_Ranged);
        Equip(weaponPack_Main);
        
        void AddWeaponPack(Data_WeaponPack weaponPack)
        {
            Prefab_Prop l = Instantiate(weaponPack.wepaon_L);
            Prefab_Prop r = Instantiate(weaponPack.weapon_R);
            Prefab_Prop s = Instantiate(weaponPack.shield);
            s.gameObject.SetActive(false);
            r.gameObject.SetActive(false);
            s.gameObject.SetActive(false);
            weapondata.Add(weaponPack,(l,r,s));
        }
    }
    public void Equip(Data_WeaponPack weaponPack)
    {
        if (weaponPack != null && _currentWeaponPack == weaponPack) return;
        //이전 무기 해제
        weapondata[_currentWeaponPack].weaponL.Unarm();
        weapondata[_currentWeaponPack].weaponR.Unarm();
        weapondata[_currentWeaponPack].shield.Unarm();
        _currentWeaponPack = weaponPack;
        
        //현 무기 장착
        weapondata[_currentWeaponPack].weaponL.Equip(t_lhand);
        weapondata[_currentWeaponPack].weaponR.Equip(t_rhand);
        weapondata[_currentWeaponPack].shield.Equip(t_shield);
    }
}
