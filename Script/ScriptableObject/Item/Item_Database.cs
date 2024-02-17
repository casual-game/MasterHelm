using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Database", menuName = "Item/Database", order = 1)]
public class Item_Database : ScriptableObject
{
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<Item_Weapon> weapons;
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<Item_Resource> resources;
}
