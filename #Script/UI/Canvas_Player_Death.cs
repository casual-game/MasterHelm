using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
	[FoldoutGroup("You Died")] public TMP_Text TMP_ItemLeft;
	[FoldoutGroup("You Died")] public CanvasGroup CG_ItemLeft, CG_ReturnButton;
	[FoldoutGroup("You Died")] public float Death_TextFadeDuration = 0.4f,Death_ButtonFadeDuration = 0.25f,Death_FadeDelay = 0.25f;
	[HideInInspector] public bool returnPressed = false;
	private UI_ReturnItemSlot[] returnItemSlots;
	private string str_count = "개";
	private int lastNum = 0;
	[HideInInspector]public int Death_ItemCount = 0;
	[HideInInspector]public bool death_Activated = false;
	private void Setting_Death()
	{
		returnItemSlots = GetComponentsInChildren<UI_ReturnItemSlot>(true);
		foreach (var returnItemSlot in returnItemSlots)
		{
			returnItemSlot.Setting();
		}
	}
	public void Activate_Death()
	{
		if (death_Activated) return;
		death_Activated = true;
		for (int i = 0; i < inventory.Length; i++)
		{
			if (inventory[i].item != null && inventory[i].num > 0) Death_ItemCount++;
			returnItemSlots[i].CopySlot(inventory[i]);
		}
		if(Death_ItemCount==0)
		{
			CG_ItemLeft.alpha = 0;
			CG_ReturnButton.alpha = 1;
			CG_ReturnButton.blocksRaycasts = true;
		}
		StartCoroutine(C_Death());

	}
	public void Death_UpdateNum(int num)
	{
		if (returnPressed) return;
		if (lastNum != num)
		{
			if (num ==Mathf.Min(Death_ItemCount,UI_ReturnItemSlot.maxSlot))
			{
				CG_ItemLeft.DOKill();
				CG_ReturnButton.DOKill();
				CG_ItemLeft.DOFade(0, Death_TextFadeDuration).SetUpdate(true);
				CG_ReturnButton.DOFade(1, Death_ButtonFadeDuration).SetUpdate(true)
					.SetDelay(Death_TextFadeDuration+Death_FadeDelay);
				CG_ReturnButton.blocksRaycasts = true;
			}
			else
			{
				CG_ItemLeft.DOKill();
				CG_ReturnButton.DOKill();
				CG_ItemLeft.DOFade(1, Death_TextFadeDuration).SetUpdate(true)
					.SetDelay(Death_ButtonFadeDuration+Death_FadeDelay);
				CG_ReturnButton.DOFade(0, Death_ButtonFadeDuration).SetUpdate(true);
				CG_ReturnButton.blocksRaycasts = false;
			}
		}

		lastNum = Mathf.Max(0,num);
		TMP_ItemLeft.text = (UI_ReturnItemSlot.maxSlot-lastNum) + str_count;
	}

	private string s_death = "Death";
	public void Death_ReturnButton()
	{
		print(returnPressed);
		if (returnPressed) return;
		returnPressed = true;
		anim.SetBool(s_death,true);
		//Manager_Main.instance.ResetScene();
	}
	private IEnumerator C_Death()
	{
		yield return new WaitForSecondsRealtime(3.5f);
		anim.CrossFade(s_death,0.0f,0);
		float currentTimeScale = Time.timeScale;
		while (currentTimeScale>0)
		{
			Time.timeScale = currentTimeScale;
			currentTimeScale -= Time.deltaTime;
			yield return null;
		}

		Time.timeScale = 0;
		
	}

	

}
