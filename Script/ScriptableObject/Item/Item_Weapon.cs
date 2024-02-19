using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 1)]
public class Item_Weapon : ScriptableObject
{
    [TitleGroup("Setting")] public Data_WeaponPack weaponPack;
    [TitleGroup("Setting")] public Sprite icon;
    [TitleGroup("Setting")] public float left,right,top,bottom;
    [TitleGroup("Setting")] public Vector3 scale = Vector3.one;
    
    [TitleGroup("Data")]public string title; 
    [TitleGroup("Data")][TextArea] public string info;
}
