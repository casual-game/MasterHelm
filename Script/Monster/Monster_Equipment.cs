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
    
    //Equipment
    public void Equip()
    {
        if(_weaponL!=null) _weaponL.Attach();
        if(_weaponR!=null) _weaponR.Attach();
        if(_shield!=null) _shield.Attach();
    }

    public void Unequip()
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
}
