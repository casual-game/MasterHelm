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
        weapondata = new Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield)>();
        //무기 추가
        AddWeaponPack(weaponPack_Normal,t_back,true);
        AddWeaponPack(weaponPack_StrongL,GameManager.Folder_Hero,false);
        AddWeaponPack(weaponPack_StrongR,GameManager.Folder_Hero,false);
        //사운드,파티클 추가
        foreach (var motionData in weaponPack_Normal.PlayerAttackMotionDatas_Normal)
        {
            foreach (var td in motionData.TrailDatas)
            {
                SoundManager.Add(td.soundData);
                if (td.useCustomParticle)
                {
                    if(td.customParticle_Sound!=null) SoundManager.Add(td.customParticle_Sound);
                    if(td.customParticle_Particle!=null) ParticleManager.Add(td.customParticle_Particle);
                    if(!_attackParticles.Contains(td.customParticle_Particle)) _attackParticles.Add(td.customParticle_Particle);
                }
            }
        }
        foreach (var td in weaponPack_StrongL.playerAttackMotionData_Strong.TrailDatas)
        {
            SoundManager.Add(td.soundData);
            if (td.useCustomParticle)
            {
                if(td.customParticle_Sound!=null) SoundManager.Add(td.customParticle_Sound);
                if(td.customParticle_Particle!=null) ParticleManager.Add(td.customParticle_Particle);
                if(!_attackParticles.Contains(td.customParticle_Particle)) _attackParticles.Add(td.customParticle_Particle);
            }
        }
        foreach (var td in weaponPack_StrongR.playerAttackMotionData_Strong.TrailDatas)
        {
            SoundManager.Add(td.soundData);
            if (td.useCustomParticle)
            {
                if(td.customParticle_Sound!=null) SoundManager.Add(td.customParticle_Sound);
                if(td.customParticle_Particle!=null) ParticleManager.Add(td.customParticle_Particle);
                if(!_attackParticles.Contains(td.customParticle_Particle)) _attackParticles.Add(td.customParticle_Particle);
            }
        }
        shield = Instantiate(shield);
        shield.Setting_Hero(_outlinable,true,t_shield,t_back);
        void AddWeaponPack(Data_WeaponPack weaponPack,Transform detachT,bool canKeep)
        {
            Prefab_Prop l = weaponPack.wepaon_L == null? null : Instantiate(weaponPack.wepaon_L);
            Prefab_Prop r = weaponPack.weapon_R == null? null : Instantiate(weaponPack.weapon_R);
            if(l!=null) l.Setting_Hero(_outlinable,canKeep,t_hand_l,detachT);
            if(r!=null) r.Setting_Hero(_outlinable,canKeep,t_hand_r,detachT);
            weapondata.Add(weaponPack,(l,r,weaponPack.useShield));
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
        //플레이어 AnimatorOverrideController 설정
        var overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        for (int i = 0; i < weaponPack_Normal.PlayerAttackMotionDatas_Normal.Count; i++)
        {
            var motionData = weaponPack_Normal.PlayerAttackMotionDatas_Normal[i];
            var attackClipName = "Combo_" + i;
            var chargeClipName = attackClipName + "_Charge";
            overrideController[attackClipName] = motionData.clip_attack;
            overrideController[chargeClipName] = motionData.clip_charge;
        }
        overrideController["StrongAttack_L"] = weaponPack_StrongL.playerAttackMotionData_Strong.clip_attack;
        overrideController["StrongAttack_R"] = weaponPack_StrongR.playerAttackMotionData_Strong.clip_attack;
        _animator.runtimeAnimatorController = overrideController;
    }
    
    //Public
    [FoldoutGroup("Equipment")] public Data_WeaponPack weaponPack_Normal,weaponPack_StrongL,weaponPack_StrongR;
    [FoldoutGroup("Equipment")] public Prefab_Prop shield;
    [FoldoutGroup("Equipment")] public Transform t_hand_l, t_hand_r, t_shield,t_back;
    
    //Private
    private int leftEnterIndex, rightEnterIndex;
    private Dictionary<Data_WeaponPack, (Prefab_Prop weaponL, Prefab_Prop weaponR, bool useShield)> weapondata;
    private Data_WeaponPack _currentWeaponPack = null,_lastWeaponPack = null;
    private RangeSensor _sensor;
    private TrailData _currentTrailData;
    private List<ParticleData> _attackParticles = new List<ParticleData>();

    
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

    public TrailData Get_TrailData()
    {
        return _currentTrailData;
    }
    //Setter
    public void Set_CurrentTrail(TrailData traildata)
    {
        if (traildata != null)
        {
            SoundManager.Play(traildata.soundData);
            if (traildata.attackType_ground == AttackType.Normal)
            {
                Sound_Voice_Attack_Normal();
            }
            else
            {
                Sound_Voice_Attack_Strong();
            }
        }
        _currentTrailData = traildata;
    }
    //Equipment
    public void Equipment_Equip(Data_WeaponPack weaponPack,bool useDeactivateParticle = true,float activateSpeed = 2.0f,float deactivateSpeed = 1.25f)
    {
        if (_currentWeaponPack == weaponPack) return;
        bool useShield = false;
        //이전 무기 해제
        if (_currentWeaponPack != null)
        {
            Sound_Weapon_Despawn();
            var past = weapondata[_currentWeaponPack];
            if (past.weaponL != null)
            {
                past.weaponL.Set_UpdateSpeed(activateSpeed,deactivateSpeed);
                past.weaponL.Detach(useDeactivateParticle);
                //past.weaponL.Collision_Reset();
            }

            if (past.weaponR != null)
            {
                past.weaponR.Set_UpdateSpeed(activateSpeed,deactivateSpeed);
                past.weaponR.Detach(useDeactivateParticle);
                //past.weaponR.Collision_Reset();
            }
        }
        _lastWeaponPack = _currentWeaponPack;
        _currentWeaponPack = weaponPack;
        //현 무기 장착
        if (weaponPack != null)
        {
            Sound_Weapon_Spawn();
            var current = weapondata[_currentWeaponPack];
            if (current.weaponL != null)
            {
                current.weaponL.Set_UpdateSpeed(activateSpeed,deactivateSpeed);
                current.weaponL.Attach();
            }

            if (current.weaponR != null)
            {
                current.weaponR.Set_UpdateSpeed(activateSpeed,deactivateSpeed);
                current.weaponR.Attach();
            }
            useShield = current.useShield;
        }
        
        shield.Set_UpdateSpeed(activateSpeed,deactivateSpeed);
        if(useShield) shield.Attach();
        else shield.Detach(useDeactivateParticle);
    }
    public void Equipment_UpdateTrail(Data_WeaponPack weaponPack,bool weaponL,bool weaponR,bool shield)
    {
        if (weaponPack == null) return;
        var data = weapondata[weaponPack];
        if (data.weaponL != null) data.weaponL.SetTrail(weaponL);
        if (data.weaponR != null) data.weaponR.SetTrail(weaponR);
        if (this.shield != null) this.shield.SetTrail(shield);
    }
    
    public void Equipment_Collision_Interact(Data_WeaponPack weaponPack,bool weaponL,bool weaponR,bool useShield)
    {
        var data = weapondata[weaponPack];
        Transform t = transform;
        bool collided = false;
        int count = 0;
        if (weaponL && data.weaponL != null)
        {
            var hitData = data.weaponL.Collision_Interact_Hero(_currentTrailData, t);
            if (hitData.collided) collided = true;
            count += hitData.count;
        }

        if (weaponR && data.weaponR != null)
        {
            var hitData = data.weaponR.Collision_Interact_Hero(_currentTrailData, t);
            if (hitData.collided) collided = true;
            count += hitData.count;
        }

        if (useShield && data.useShield)
        {
            var hitData = shield.Collision_Interact_Hero(_currentTrailData, t);
            if(hitData.collided) collided = true;
            count += hitData.count;
        }

        if (collided)
        {
            frameMain.Charge_MP(_currentTrailData.charge_mp);
        }
        for (int i = 0; i < count; i++)
        {
            frameMain.HP_Regain(_currentTrailData.regain);
        }
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
        Transform t = transform;
        bool collided = false;
        foreach (var signal in _sensor.GetSignals())
        {
            signal.Object.TryGetComponent<Monster>(out var monster);
            if(monster.AI_Hit(t,t,_currentTrailData)) collided = true;
        }

        if (collided)
        {
            frameMain.HP_Regain(_currentTrailData.regain);
            frameMain.Charge_MP(_currentTrailData.charge_mp);
        }
    }
    public void Equipment_CancelEffect()
    {
        foreach (var _ap in _attackParticles)
        {
            ParticleManager.Stop(_ap);
        }
    }
    
}
