using System;
using System.Collections;
using System.Collections.Generic;
using DuloGames.UI;
using Pathfinding;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
public partial class Enemy : MonoBehaviour
{
	[TitleGroup("Data")] [FoldoutGroup("Data/Audio")]
	public Data_Audio audio_attack, audio_damaged, audio_death,audio_create,audio_roar,audio_swing;
	
	private float lastVoiceTime, voiceDelay = 1.5f;

	protected virtual void Setting_Sound()
	{
		SoundManager.instance.Add(audio_attack);
		SoundManager.instance.Add(audio_damaged);
		SoundManager.instance.Add(audio_death);
		SoundManager.instance.Add(audio_create);
		SoundManager.instance.Add(audio_roar);
		SoundManager.instance.Add(audio_swing);
	}
	
	public void AttackVoice()
	{
		if(Time.unscaledTime-lastVoiceTime > voiceDelay)
		{
			lastVoiceTime = Time.unscaledTime;
			audio_attack.Play();
		}
	}
	public void RoarVoice()
	{
		audio_roar.Play();
	}
	private void HitVoice()
	{
		if (Time.unscaledTime > lastVoiceTime + voiceDelay)
		{
			audio_damaged.Play();
			lastVoiceTime = Time.unscaledTime;
		}
	}
	public void SwingSound()
	{
		audio_swing.Play();
	}
	
}
