using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Resource", menuName = "Item/Resource", order = 1)]
public class Item_Resource : ScriptableObject
{
    [TitleGroup("세팅")]
    [FoldoutGroup("세팅/Setting")] public Sprite icon;
    [FoldoutGroup("세팅/Setting")]public string title; 
    [FoldoutGroup("세팅/Setting")][TextArea] public string info;
    [TitleGroup("인벤토리")]
    [FoldoutGroup("인벤토리/Inventory")] public float left,right,top,bottom;
    [FoldoutGroup("인벤토리/Inventory")] public Vector3 scale = Vector3.one;
    [TitleGroup("상점")]
    [FoldoutGroup("상점/Shop")] public float sleft,sright,stop,sbottom;
    [FoldoutGroup("상점/Shop")] public Vector3 sscale = Vector3.one;
    [FoldoutGroup("상점/Shop")] public bool isSpecial = false;
    [TitleGroup("가격")]
    [FoldoutGroup("가격/Price")] public int price;
    [FoldoutGroup("가격/Price")] public bool isGem;
}
