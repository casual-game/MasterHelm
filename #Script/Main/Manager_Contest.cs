using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public partial class Manager_Main : MonoBehaviour
{
	private float totalDamage=0;
	private int maxAction=0;

	[BoxGroup("공모전")] [GUIColor("RGB(1,0.7,0.7)")]
	public TMP_Text tmp_TotalDamage, tmp_MaxAction, tmp_TimeSpend;


	public void CalculateAction(int action)
	{
		maxAction = Mathf.Max(action, maxAction);
	}

	public void AddDamage(float damage)
	{
		totalDamage += damage;
	}
	private void Update_Contest()
	{
		tmp_TotalDamage.text = Mathf.RoundToInt(totalDamage).ToString();
		tmp_MaxAction.text = maxAction.ToString();
		tmp_TimeSpend.text = Mathf.RoundToInt(Time.unscaledTime).ToString();
	}
}