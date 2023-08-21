using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyAttack", menuName = "Data/Data_EnemyAttack", order = 1)]
public class Data_EnemyMotion : ScriptableObject
{
	public float animMoveSpeed = 0.75f;
	public List<SingleAttackData> attackData = new List<SingleAttackData>();
	
	[System.Serializable]
	public class SingleAttackData
	{
		public enum AttackType { Normal1 =0,Normal2=1,Strong =2}
		

		[TitleGroup("MainSetting")] public AttackType attackType;
		[TitleGroup("MainSetting")]public bool isGuardBreak = false;
		[TitleGroup("MainSetting")][MinMaxSlider(0,1,true)] public Vector2 turnRange;
		
		[TitleGroup("Trail")] public bool left, right, shield;
		[TitleGroup("Trail")][MinMaxSlider(0,1,true)] public Vector2 trailRange;

		[TitleGroup("Effect")] [BoxGroup("Effect/Shake", true)][LabelText("intensity")] public float shakeIntensity = 1.0f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Shake", true)][LabelText("duration")] public float shakeDuration = 0.3f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Stop", true)][LabelText("timeScale")] public float stopTimeScale = 0.05f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Stop", true)][LabelText("duration")] public float stopDuration = 0.2f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Chromatic", true)][LabelText("intensity")] public float chromaticIntensity = 0.05f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Chromatic", true)][LabelText("beginDuration")] public float chromaticBeginDuration = 0.05f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Chromatic", true)][LabelText("finDuration")] public float chromaticFinDuration = 0.5f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Hit", true)][LabelText("beginDuration")] public float hitBeginDuration = 0.05f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Hit", true)][LabelText("finDuration")] public float hitFinDuration = 0.5f;
		[TitleGroup("Effect")] [BoxGroup("Effect/Hit", true)] [LabelText("intensity")] public float hitIntensity = 0.6f;
		
	}
}