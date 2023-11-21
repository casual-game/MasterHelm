using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    //Public
    [FoldoutGroup("Equipment")] public Transform t_hand_l, t_hand_r, t_shield;
    
    //Private
    protected Prefab_Prop _weaponL, _weaponR, _shield;
    private TrailData _currentTrailData;
    //Get,Set
    public void Set_CurrentTrail(TrailData traildata)
    {
        _currentTrailData = traildata;
    }
    //Equipment
    public void Equipment_Equip()
    {
        if(_weaponL!=null) _weaponL.Attach();
        if(_weaponR!=null) _weaponR.Attach();
        if(_shield!=null) _shield.Attach();
    }
    public void Equipment_Unequip()
    {
        if (_weaponL != null)
        {
            _weaponL.SetTrail(false);
            _weaponL.Detach();
        }

        if (_weaponR != null)
        {
            _weaponR.SetTrail(false);
            _weaponR.Detach();
        }

        if (_shield != null)
        {
            _shield.SetTrail(false);
            _shield.Detach();
        }
    }
    public void Equipment_UpdateTrail(bool weaponL,bool weaponR,bool shield)
    {
        if (_weaponL != null) _weaponL.SetTrail(weaponL);
        if (_weaponR != null) _weaponR.SetTrail(weaponR);
        if (_shield != null) _shield.SetTrail(shield);
    }
    public void Equipment_Collision_Interact(bool weaponL,bool weaponR,bool useShield)
    {
        Transform t = transform;
        if (weaponL && _weaponL != null) _weaponL.Collision_Interact(_currentTrailData, t);
        if (weaponR && _weaponR != null) _weaponR.Collision_Interact(_currentTrailData, t);
        if (useShield && _shield != null) _shield.Collision_Interact(_currentTrailData, t);
    }
    public void Equipment_Collision_Reset()
    {
        if(_weaponL!=null) _weaponL.Collision_Reset();
        if (_weaponR != null) _weaponR.Collision_Reset();
        if(_shield != null) _shield.Collision_Reset();
    }
}
