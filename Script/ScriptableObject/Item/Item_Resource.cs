using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Resource", menuName = "Item/Resource", order = 1)]
public class Item_Resource : ScriptableObject
{
    [TitleGroup("Setting")] public Sprite icon;
    [TitleGroup("Inventory")] public float left,right,top,bottom;
    [TitleGroup("Inventory")] public Vector3 scale = Vector3.one;
    [TitleGroup("Shop")] public float sleft,sright,stop,sbottom;
    [TitleGroup("Shop")] public Vector3 sscale = Vector3.one;
    [TitleGroup("Shop")] public bool isSpecial = false;
    
    [TitleGroup("Data")]public string title; 
    [TitleGroup("Data")][TextArea] public string info;
}
