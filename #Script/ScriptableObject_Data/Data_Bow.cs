using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Bow", menuName = "Data/Data_Bow", order = 1)]
public class Data_Bow : ScriptableObject
{
	public Prefab_Bow prefab=null;
}