using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Shield", menuName = "Data/Data_Shield", order = 1)]
public class Data_Shield : ScriptableObject
{
    public Prefab_Prop prefab=null;
    public ElementalAttributes elementalAttributes;
    [TitleGroup("AttachBack")] public Vector3 localPos, localRot, localScale;
}

