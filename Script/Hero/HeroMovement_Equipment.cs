using System.Collections;
using System.Collections.Generic;
using RPGCharacterAnims.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public partial class HeroMovement : MonoBehaviour
{
    [FoldoutGroup("Equipment")] public Data_WeaponPack weaponPack_Normal,weaponPack_StrongL,weaponPack_StrongR;

    [FoldoutGroup("Equipment")] public Prefab_Prop shield;
    [FoldoutGroup("Equipment")] public Transform t_hand_l, t_hand_r, t_shield,t_back;
    [ShowInInspector]
    private int leftEnterIndex, rightEnterIndex;
    private Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield,List<ParticleSystem> attackParticles)> weapondata;
    private Data_WeaponPack _currentWeaponPack = null,_lastWeaponPack = null;

    
    private void Setting_Equipment()
    {
        weapondata = new Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield,List<ParticleSystem> attackParticles)>();
        AddWeaponPack(weaponPack_Normal,t_back,true);
        AddWeaponPack(weaponPack_StrongL,folder,false);
        AddWeaponPack(weaponPack_StrongR,folder,false);
        shield = Instantiate(shield);
        shield.Setting_Hero(outlinable,true,t_shield,t_back);
        void AddWeaponPack(Data_WeaponPack weaponPack,Transform detachT,bool canKeep)
        {
            Prefab_Prop l = weaponPack.wepaon_L == null? null : Instantiate(weaponPack.wepaon_L);
            Prefab_Prop r = weaponPack.weapon_R == null? null : Instantiate(weaponPack.weapon_R);
            List<ParticleSystem> attackParticles = new List<ParticleSystem>();
            foreach (var p in weaponPack.attackEffects)
            {
                ParticleSystem attackParticle = Instantiate(p,folder);
                attackParticles.Add(attackParticle);
            }
            if(l!=null) l.Setting_Hero(outlinable,canKeep,t_hand_l,detachT);
            if(r!=null) r.Setting_Hero(outlinable,canKeep,t_hand_r,detachT);
            weapondata.Add(weaponPack,(l,r,weaponPack.useShield,attackParticles));
        }
        //Animator의 ChargeEnterIndex용 변수 설정
        bool foundLeft = false, foundRight = false;
        for (int i = 0; i < weaponPack_Normal.PlayerAttackMotionDatas_Normal.Count; i++)
        {
            var data = weaponPack_Normal.PlayerAttackMotionDatas_Normal[i];
            if (!foundLeft && data.playerAttackType_End == PlayerAttackType.LeftState)
            {
                foundLeft = true;
                leftEnterIndex = i;
            }
            if (!foundRight && data.playerAttackType_End == PlayerAttackType.RightState)
            {
                foundRight = true;
                rightEnterIndex = i;
            }
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

        _lastWeaponPack = _currentWeaponPack;
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

    public void UpdateTrail(Data_WeaponPack weaponPack,bool weaponL,bool weaponR,bool shield)
    {
        (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield,List<ParticleSystem> attackParticles) data = weapondata[weaponPack];
        if(data.weaponL!=null) data.weaponL.SetTrail(weaponL);
        if(data.weaponR!=null) data.weaponR.SetTrail(weaponR); 
        if(this.shield!=null) this.shield.SetTrail(shield);
    }
    
    public Data_WeaponPack Get_LastWeaponPack()
    {
        return _lastWeaponPack;
    }
    public Data_WeaponPack Get_CurrentWeaponPack()
    {
        return _currentWeaponPack;
    }
    public int Get_LeftEnterIndex()
    {
        return leftEnterIndex;
    }

    public int Get_RightEnterIndex()
    {
        return rightEnterIndex;
    }
}
