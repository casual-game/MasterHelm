using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public partial class Canvas_Player : MonoBehaviour
{
	[FoldoutGroup("인벤토리")] public UI_NormalItemSlot[] inventory;
	[FoldoutGroup("인벤토리")] public UI_GetItem getItem;
	private Queue<DropTable> orb_Special_DropTable = new Queue<DropTable>();
	private Queue<DropTable> orb_Normal_DropTable = new Queue<DropTable>();
	[HideInInspector] public UI_ItemInfo itemInfo;
	private void Setting_Inventory()
	{
		inventory = GetComponentsInChildren<UI_NormalItemSlot>(true);
		itemInfo = GetComponentInChildren<UI_ItemInfo>(true);
		itemInfo.Setting();
		foreach (var slot in inventory)
		{
			slot.Setting();
		}
	}
	public void AddItem(Data_Item item)
	{
		if (death_Activated) return;
		UI_NormalItemSlot slot = null;
		foreach (var _slot in inventory)
		{
			if (_slot.item == item && _slot.num>0)
			{
				slot = _slot;
				break;
			}
		}

		if (slot == null)
		{
			foreach (var _slot in inventory)
			{
				if (_slot.num == 0)
				{
					slot = _slot;
					break;
				}
			}
		}
		slot.UpdateItem(slot.num+1,item);
		getItem.AddItem(item);
	}

	public void AddTable_NormalOrb(DropTable table)
	{
		orb_Normal_DropTable.Enqueue(table);
	}
	public void AddTable_SpecialOrb(DropTable table)
	{
		orb_Special_DropTable.Enqueue(table);
	}

	public DropTable GetOrbTable(Orb_Normal normal)
	{
		if (orb_Normal_DropTable.Count > 0) return orb_Normal_DropTable.Dequeue();
		else return null;
	}
	public DropTable GetOrbTable(Orb_Special special)
	{
		if (orb_Special_DropTable.Count > 0) return orb_Special_DropTable.Dequeue();
		else return null;
	}
}
