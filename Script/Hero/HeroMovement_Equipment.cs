using System.Collections;
using System.Collections.Generic;
using RPGCharacterAnims.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class HeroMovement : MonoBehaviour
{
    [FoldoutGroup("Equipment")] public Data_WeaponPack weaponPack_Main,weaponPack_SkillL,weaponPack_SkillR,weaponPack_Ranged;

    [FoldoutGroup("Equipment")] public Prefab_Prop shield;
    [FoldoutGroup("Equipment")] public Transform t_hand_l, t_hand_r, t_shield,t_back;
    
    private Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield)> weapondata;
    private Data_WeaponPack _currentWeaponPack = null;
    private void Setting_Equipment()
    {
        weapondata = new Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield)>();
        AddWeaponPack(weaponPack_Main,t_back,true);
        AddWeaponPack(weaponPack_SkillL,folder,false);
        AddWeaponPack(weaponPack_SkillR,folder,false);
        AddWeaponPack(weaponPack_Ranged,folder,false);
        shield = Instantiate(shield);
        shield.Setting_Hero(outlinable,true,t_shield,t_back);
        void AddWeaponPack(Data_WeaponPack weaponPack,Transform detachT,bool canKeep)
        {
            Prefab_Prop l = weaponPack.wepaon_L == null? null : Instantiate(weaponPack.wepaon_L);
            Prefab_Prop r = weaponPack.weapon_R == null? null : Instantiate(weaponPack.weapon_R); 
            if(l!=null) l.Setting_Hero(outlinable,canKeep,t_hand_l,detachT);
            if(r!=null) r.Setting_Hero(outlinable,canKeep,t_hand_r,detachT);
            weapondata.Add(weaponPack,(l,r,weaponPack.useShield));
        }
    }
    [Button]
    public void Equip(Data_WeaponPack weaponPack)
    {
        if (_currentWeaponPack == weaponPack) return;
        bool useShield = false;
        //이전 무기 해제
        if (_currentWeaponPack != null)
        {
            var past = weapondata[_currentWeaponPack];
            if(past.weaponL!=null) past.weaponL.Detach();
            if(past.weaponR!=null) past.weaponR.Detach();
        }
        _currentWeaponPack = weaponPack;
        //현 무기 장착
        if (weaponPack != null)
        {
            var current = weapondata[_currentWeaponPack];
            if(current.weaponL!=null) current.weaponL.Attach();
            if(current.weaponR!=null) current.weaponR.Attach();
            useShield = current.useShield;
        }
        if(useShield) shield.Attach();
        else shield.Detach();
        
    
    }
}
