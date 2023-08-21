using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "Scriptable/DropTable", order = 1)]
public class DropTable : ScriptableObject
{
	[System.Serializable]
	public class SingleData
	{
		[HorizontalGroup("아이템 정보")][HideLabel] public Data_Item item;
		[HorizontalGroup("아이템 정보")][HideLabel] public int weight = 1;
	}

   public List<SingleData> items = new List<SingleData>();

   public Data_Item[] GetItem(int count)
   {
	   List<Data_Item> singleItems = new List<Data_Item>();
	   Data_Item[] returnItem = new Data_Item[count];
	   foreach (var singleDrop in items)
	   {
		   for (int i = 0; i < singleDrop.weight; i++)
		   {
			   singleItems.Add(singleDrop.item);
		   }
	   }

	   for (int i = 0; i < count; i++) returnItem[i] = singleItems[Random.Range(0, singleItems.Count)];
	   return returnItem;
   }
}