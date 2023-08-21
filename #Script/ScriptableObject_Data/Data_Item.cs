using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Data_Item", order = 1)]
public class Data_Item : ScriptableObject
{
    public Sprite icon;
    public string itemName;
    [TextArea]
    public string itemInfo;
}