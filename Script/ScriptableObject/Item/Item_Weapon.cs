using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 1)]
public class Item_Weapon : ScriptableObject
{
    [TitleGroup("Setting")] public Data_WeaponPack weaponPack;
    [TitleGroup("Setting")] public Sprite icon;
    [TitleGroup("Inventory")] public float left,right,top,bottom;
    [TitleGroup("Inventory")] public Vector3 scale = Vector3.one;
    [TitleGroup("Shop")] public float sleft,sright,stop,sbottom;
    [TitleGroup("Shop")] public Vector3 sscale = Vector3.one;
    [TitleGroup("Shop")] public Color decoColor;
    [TitleGroup("Shop")] public Sprite star;
    
    [TitleGroup("Data")]public string title; 
    [TitleGroup("Data")][TextArea] public string info;

    [TitleGroup("Blueprint")] public Item_Resource bpResource1, bpResource2, bpResource3;
    [TitleGroup("Blueprint")] public Item_Weapon bpWeapon1,bpWeapon2,bpWeapon3;
    [TitleGroup("Blueprint")] public int bpCount1, bpCount2, bpCount3;
}
