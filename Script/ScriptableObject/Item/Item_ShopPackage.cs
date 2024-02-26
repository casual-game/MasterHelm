using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "ShopPackage", menuName = "Item/ShopPackage", order = 1)]
public class Item_ShopPackage : ScriptableObject
{
    [TextArea]
    public string title;
    public Sprite sprite;
    public int price = 1500;

    public float left, right, top, bottom;
    public Vector3 scale;
}
