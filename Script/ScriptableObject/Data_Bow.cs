using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bow", menuName = "Data/Bow", order = 1)]
public class Data_Bow : ScriptableObject
{
    public Prefab_Prop_Bow bow;
    public float endRatio = 0.8f;
    public float moveRatio = 1.0f;
    public TrailData arrowData;
}
