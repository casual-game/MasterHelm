using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    //Public
    [FoldoutGroup("Equipment")] public Transform t_hand_l, t_hand_r, t_shield;
    //Private
    private TrailData_Monster _currentTrailData;
    //Get,Set
    public TrailData_Monster Get_CurrentTrail()
    {
        return _currentTrailData;
    }
    public void Set_CurrentTrail(TrailData_Monster data)
    {
        _currentTrailData = data;
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
            _weaponL.Detach(true);
        }

        if (_weaponR != null)
        {
            _weaponR.SetTrail(false);
            _weaponR.Detach(true);
        }

        if (_shield != null)
        {
            _shield.SetTrail(false);
            _shield.Detach(true);
        }
    }
    public void Equipment_UpdateTrail(bool weaponL,bool weaponR,bool shield)
    {
        bool newL = _weaponL!=null && !_weaponL.GetTrail() && weaponL;
        bool newR = _weaponR!=null && !_weaponR.GetTrail() && weaponR;
        bool newS = _shield!=null && !_shield.GetTrail() && shield;
        if ((newL || newR || newS) && Get_CurrentTrail().soundData!=null) SoundManager.Play(Get_CurrentTrail().soundData);
        
        if (_weaponL != null) _weaponL.SetTrail(weaponL);
        if (_weaponR != null) _weaponR.SetTrail(weaponR);
        if (_shield != null) _shield.SetTrail(shield);
    }
    public void Equipment_Collision_Interact(bool weaponL,bool weaponR,bool useShield)
    {
        Transform t = transform;
        if (weaponL && _weaponL != null) _weaponL.Collision_Interact_Monster(_currentTrailData, t);
        if (weaponR && _weaponR != null) _weaponR.Collision_Interact_Monster(_currentTrailData, t);
        if (useShield && _shield != null) _shield.Collision_Interact_Monster(_currentTrailData, t);
    }
    public void Equipment_Collision_Skip()
    {
        Transform t = transform;
        if (_weaponL != null) _weaponL.Collision_Skip();
        if (_weaponR != null) _weaponR.Collision_Skip();
        if (_shield != null) _shield.Collision_Skip();
    }
    public void Equipment_Collision_Reset()
    {
        if(_weaponL!=null) _weaponL.Collision_Reset();
        if (_weaponR != null) _weaponR.Collision_Reset();
        if(_shield != null) _shield.Collision_Reset();
    }
}
