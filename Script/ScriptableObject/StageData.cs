using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "StageData", menuName = "Data/StageData", order = 1)]
public class StageData : ScriptableObject
{
	public int targetTime,targetScore,targetCombo;
	[TitleGroup("아이템")][FoldoutGroup("아이템/data")]
	public EarnableItem item0, item1, item2, item3, item4, item5, item6, item7, item8;

	public EarnableItem GetItem(int index)
	{
		switch (index)
		{
			case 0:
				return item0;
				break;
			case 1:
				return item1;
				break;
			case 2:
				return item2;
				break;
			case 3:
				return item3;
				break;
			case 4:
				return item4;
				break;
			case 5:
				return item5;
				break;
			case 6:
				return item6;
				break;
			case 7:
				return item7;
				break;
			case 8:
				return item8;
				break;
			default:
				return null;
				break;
		}
	}
}
[System.Serializable]
public class EarnableItem
{
	public int weights = 1;
	[HorizontalGroup("Item")][HideLabel] public Item_Weapon weapon;
	[HorizontalGroup("Item")][HideLabel]  public Item_Resource resource;
}