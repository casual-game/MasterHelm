using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Impact", menuName = "Data/Data_Impact", order = 1)]
public class Data_Impact : ScriptableObject
{
	[BoxGroup("카메라 이펙트")] [TitleGroup("카메라 이펙트/Etc")] public bool isImportant = false;
	[TitleGroup("카메라 이펙트/Stop")] public float stopDuration = 0.15f, stopScale = 0.05f, stopDuration2 = 0,stopScale2 = 0.3f;
    
	[TitleGroup("카메라 이펙트/Shake")][LabelText("세기")] public float pos_shakeStrength = 0.25f;
	[TitleGroup("카메라 이펙트/Shake")][LabelText("주기")] public float pos_shakeDuration = 0.25f;
	[TitleGroup("카메라 이펙트/Shake")][LabelText("진동수")] public int pos_shakeVibrato=10;
	[TitleGroup("카메라 이펙트/Shake")][LabelText("커브")] public Ease pos_shakeEase;

	[TitleGroup("카메라 이펙트/Chromatic")][LabelText("세기")] public float chromaticStrength = 0.035f;
	[TitleGroup("카메라 이펙트/Chromatic")][LabelText("증가 시간")] public float chromatic_increase_duration;
	[TitleGroup("카메라 이펙트/Chromatic")][LabelText("감소 시간")] public float chromatic_dercrease_duration;
	
	[TitleGroup("카메라 이펙트/Hit")][LabelText("증가 시간")] public float hit_increase_duration;
	[TitleGroup("카메라 이펙트/Hit")][LabelText("감소 시간")] public float hit_dercrease_duration;
	[TitleGroup("카메라 이펙트/Hit")] [LabelText("색상")] public Color hit_Color;
	[TitleGroup("카메라 이펙트/Hit")] [LabelText("세기")] public float hit_Intensity;
	

	[BoxGroup("히트 효과")] [TitleGroup("카메라 이펙트/Etc")]
	[TitleGroup("히트 효과/밀리기")] [LabelText("가드백 속도 범위")] public Vector2 guardback_normal_speedRange = new Vector2(0.5f,1.2f);
	[TitleGroup("히트 효과/밀리기")] [LabelText("가드백 속도 범위")] public float guardback_normal_targetDist = 1.5f;
	[TitleGroup("히트 효과/밀리기")] [LabelText("특수공격 모션 속도")] public float guardback_special = 1.0f;
}