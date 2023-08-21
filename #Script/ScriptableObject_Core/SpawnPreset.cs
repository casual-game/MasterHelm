using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "SpawnPreset", menuName = "Scriptable/SpawnPreset", order = 1)]
public class SpawnPreset : ScriptableObject
{
	public EnemyRoot root;
	public Prefab_Prop weaponL, weaponR, shield;
	public DropTable dropTable;
}