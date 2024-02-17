using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 1)]
public class Item_Weapon : ScriptableObject
{
    public Data_WeaponPack weaponPack;
    public Sprite icon;
    public float left,right,top,bottom;
    public Vector3 scale = Vector3.one;
}
