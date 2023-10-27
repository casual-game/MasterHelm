using System.Collections;
using System.Collections.Generic;
using Micosmo.SensorToolkit;
using RPGCharacterAnims.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public partial class Hero : MonoBehaviour
{
    private void Setting_Equipment()
    {
        _sensor = GetComponentInChildren<RangeSensor>();
        weapondata = new Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield,List<ParticleSystem> attackParticles)>();
        AddWeaponPack(weaponPack_Normal,t_back,true);
        AddWeaponPack(weaponPack_StrongL,GameManager.Folder_Hero,false);
        AddWeaponPack(weaponPack_StrongR,GameManager.Folder_Hero,false);
        shield = Instantiate(shield);
        shield.Setting_Hero(_outlinable,true,t_shield,t_back);
        void AddWeaponPack(Data_WeaponPack weaponPack,Transform detachT,bool canKeep)
        {
            Prefab_Prop l = weaponPack.wepaon_L == null? null : Instantiate(weaponPack.wepaon_L);
            Prefab_Prop r = weaponPack.weapon_R == null? null : Instantiate(weaponPack.weapon_R);
            List<ParticleSystem> attackParticles = new List<ParticleSystem>();
            foreach (var p in weaponPack.attackEffects)
            {
                ParticleSystem attackParticle = Instantiate(p,GameManager.Folder_Hero);
                attackParticles.Add(attackParticle);
            }
            if(l!=null) l.Setting_Hero(_outlinable,canKeep,t_hand_l,detachT);
            if(r!=null) r.Setting_Hero(_outlinable,canKeep,t_hand_r,detachT);
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
    
    //Public
    [FoldoutGroup("Equipment")] public Data_WeaponPack weaponPack_Normal,weaponPack_StrongL,weaponPack_StrongR;
    [FoldoutGroup("Equipment")] public Prefab_Prop shield;
    [FoldoutGroup("Equipment")] public Transform t_hand_l, t_hand_r, t_shield,t_back;
    
    //Private
    private int leftEnterIndex, rightEnterIndex;
    private Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield,List<ParticleSystem> attackParticles)> weapondata;
    private Data_WeaponPack _currentWeaponPack = null,_lastWeaponPack = null;
    private RangeSensor _sensor;
    private TrailData _currentTrailData;

    
    //Getter
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
    //Setter
    public void Set_CurrentTrail(TrailData traildata)
    {
        _currentTrailData = traildata;
    }
    //Equipment
    public void Equipment_Equip(Data_WeaponPack weaponPack)
    {
        if (_currentWeaponPack == weaponPack) return;
        bool useShield = false;
        //이전 무기 해제
        if (_currentWeaponPack != null)
        {
            var past = weapondata[_currentWeaponPack];
            if (past.weaponL != null)
            {
                past.weaponL.Detach();
                //past.weaponL.Collision_Reset();
            }

            if (past.weaponR != null)
            {
                past.weaponR.Detach();
                //past.weaponR.Collision_Reset();
            }
            
            //if(past.useShield) shield.Collision_Reset();
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
    public void Equipment_UpdateTrail(Data_WeaponPack weaponPack,bool weaponL,bool weaponR,bool shield)
    {
        var data = weapondata[weaponPack];
        if (data.weaponL != null)
        {
            data.weaponL.SetTrail(weaponL);
            //if(weaponL) data.weaponL.Collision_Interact();
        }

        if (data.weaponR != null)
        {
            data.weaponR.SetTrail(weaponR);
            //if(weaponR) data.weaponR.Collision_Interact();
        }

        if (this.shield != null)
        {
            this.shield.SetTrail(shield);
            //if(shield) this.shield.Collision_Interact();
        }
    }
    
    public void Equipment_Collision_Interact(Data_WeaponPack weaponPack,bool weaponL,bool weaponR,bool useShield)
    {
        var data = weapondata[weaponPack];
        if(weaponL && data.weaponL!=null) data.weaponL.Collision_Interact(_currentTrailData.attackType_ground,
            _currentTrailData.isAirSmash,_currentTrailData.attackType_extra);
        if(weaponR && data.weaponR!=null) data.weaponR.Collision_Interact(_currentTrailData.attackType_ground,
            _currentTrailData.isAirSmash,_currentTrailData.attackType_extra);
        if(useShield && data.useShield) shield.Collision_Interact(_currentTrailData.attackType_ground,
            _currentTrailData.isAirSmash,_currentTrailData.attackType_extra);
    }
    public void Equipment_Collision_Reset(Data_WeaponPack weaponPack)
    {
        var data = weapondata[weaponPack];
        if(data.weaponL!=null) data.weaponL.Collision_Reset();
        if (data.weaponR != null) data.weaponR.Collision_Reset();
        if(data.useShield) shield.Collision_Reset();
    }
    public void Equipment_HitScan()
    {
        if (_animator.IsInTransition(0) || _currentTrailData == null) return;
        _sensor.transform.SetLocalPositionAndRotation(
            _currentTrailData.hitscan_pos,Quaternion.Euler(_currentTrailData.hitscan_rot));
        _sensor.Box.HalfExtents = _currentTrailData.hitscan_scale;
        _sensor.Pulse();
        foreach (var signal in _sensor.GetSignals())
        {
            signal.Object.TryGetComponent<Monster>(out var monster);
            monster.Core_Hit_Strong(transform,_currentTrailData.attackType_ground,
                _currentTrailData.isAirSmash,_currentTrailData.attackType_extra);
        }
    }
    
}
