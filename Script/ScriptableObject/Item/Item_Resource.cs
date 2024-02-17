using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Resource", menuName = "Item/Resource", order = 1)]
public class Item_Resource : ScriptableObject
{
    public Sprite icon;
    public float left,right,top,bottom;
    public Vector3 scale = Vector3.one;
}
