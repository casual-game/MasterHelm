using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "Main", menuName = "Data/Data_Main", order = 1)]
public class Data_Main : ScriptableObject
{
	//Main
	public float weaponPhysicsCalculateMin = 0.05f;
	public float ai_period = 0.15f;
	[ColorUsage(true, true)] public Color enemy_HitColor,enemy_ExecutedColor,player_RollColor;
	//원소
	[TitleGroup("원소")][FoldoutGroup("원소/파티클 색")]
	public Color EA_Color_Fire, EA_Color_Water, EA_Color_Wind, EA_Color_Ground;
	[TitleGroup("원소")] [FoldoutGroup("원소/UI 색")][TitleGroup("원소/UI 색/Fire")]
	public Color EA_Color_Fire_Highlight,EA_Color_Fire_Charged,EA_Color_Fire_Uncharged;
	[TitleGroup("원소")] [FoldoutGroup("원소/UI 색")][TitleGroup("원소/UI 색/Water")]
	public Color EA_Color_Water_Highlight,EA_Color_Water_Charged,EA_Color_Water_Uncharged;
	[TitleGroup("원소")] [FoldoutGroup("원소/UI 색")][TitleGroup("원소/UI 색/Wind")]
	public Color EA_Color_Wind_Highlight,EA_Color_Wind_Charged,EA_Color_Wind_Uncharged;
	[TitleGroup("원소")] [FoldoutGroup("원소/UI 색")][TitleGroup("원소/UI 색/Ground")]
	public Color EA_Color_Ground_Highlight,EA_Color_Ground_Charged,EA_Color_Ground_Uncharged;
	public Color ElementalColor(ElementalAttributes ea)
	{
		switch (ea)
		{
			case ElementalAttributes.Fire:
				return EA_Color_Fire;
				break;
			case ElementalAttributes.Water:
				return EA_Color_Water;
				break;
			case ElementalAttributes.Wind:
				return EA_Color_Wind;
				break;
			case ElementalAttributes.Ground:
				return EA_Color_Ground;
				break;
			default:
				return EA_Color_Fire;
		}
	}
	public Color ElementalColor_Highlight(ElementalAttributes ea)
	{
		switch (ea)
		{
			case ElementalAttributes.Fire:
				return EA_Color_Fire_Highlight;
				break;
			case ElementalAttributes.Water:
				return EA_Color_Water_Highlight;
				break;
			case ElementalAttributes.Wind:
				return EA_Color_Wind_Highlight;
				break;
			case ElementalAttributes.Ground:
				return EA_Color_Ground_Highlight;
				break;
			default:
				return EA_Color_Fire_Highlight;
		}
	}
	public Color ElementalColor_Charged(ElementalAttributes ea)
	{
		switch (ea)
		{
			case ElementalAttributes.Fire:
				return EA_Color_Fire_Charged;
				break;
			case ElementalAttributes.Water:
				return EA_Color_Water_Charged;
				break;
			case ElementalAttributes.Wind:
				return EA_Color_Wind_Charged;
				break;
			case ElementalAttributes.Ground:
				return EA_Color_Ground_Charged;
				break;
			default:
				return EA_Color_Fire_Charged;
		}
	}
	public Color ElementalColor_Uncharged(ElementalAttributes ea)
	{
		switch (ea)
		{
			case ElementalAttributes.Fire:
				return EA_Color_Fire_Uncharged;
				break;
			case ElementalAttributes.Water:
				return EA_Color_Water_Uncharged;
				break;
			case ElementalAttributes.Wind:
				return EA_Color_Wind_Uncharged;
				break;
			case ElementalAttributes.Ground:
				return EA_Color_Ground_Uncharged;
				break;
			default:
				return EA_Color_Fire_Uncharged;
		}
	}
	
	
	//HighlightFX
	[TitleGroup("HighlightFX")] [FoldoutGroup("HighlightFX/Blink")]
	public Color blink_Damage;
	[TitleGroup("HighlightFX")] [FoldoutGroup("HighlightFX/HitFX")]
	public Color hitFX_Default,hitFX_Hide,hitFX_Hit_Normal;
	[TitleGroup("HighlightFX")] [FoldoutGroup("HighlightFX/Outline")]
	public Color outline_normal, outline_superarmor;
	
	//Impact
	[TitleGroup("Impact")] [FoldoutGroup("Impact/list")]
	public Data_Impact impact_Hit, impact_Smash,impact_PreSpecial ,impact_SpecialHit , impact_Guard 
		, impact_Smooth,impact_SpecialSmooth,impact_Death,impact_Clear_Area,impact_Clear_Stage
		,impact_Boss_SpecialHit,impact_Boss_Clear_Area;
	//Audio
	[TitleGroup("Audio")] [FoldoutGroup("Audio/list")]
	public Data_Audio Orb_Create, Orb_Remove, audio_Effecting_Impact,audio_Effecting_ImpactSection;
	public void Setting_Audio()
	{
		SoundManager.instance.Add(Orb_Create);
		SoundManager.instance.Add(Orb_Remove);
		SoundManager.instance.Add(audio_Effecting_Impact);
		SoundManager.instance.Add(audio_Effecting_ImpactSection);
	}
}

public enum ElementalAttributes
{
	Fire = 0,Water=1,Wind=2,Ground=3
}
