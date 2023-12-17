using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/SoundData", order = 1)]
public class SoundData : ScriptableObject
{
	
	public AudioMixerGroup mixerGroup;
	public List<SingleSound> sounds;
}
[System.Serializable]
public class SingleSound
{
	[TitleGroup("$TitleName")]public AudioClip clip;
	[TitleGroup("$TitleName")]public bool isLoop;
	[TitleGroup("$TitleName")][LabelText("시작시간 설정")][MinMaxSlider(0.0f,1.0f,true)]  
	public Vector2 clipRange = new Vector2(0,1);
	[TitleGroup("$TitleName")][LabelText("렌덤 pitch")][MinMaxSlider(-0.075f,0.075f,true)] 
	public Vector2 pitch = new Vector2(-0.025f, 0.025f);
	[TitleGroup("$TitleName")] [LabelText("볼륨")][Range(0,1)]
	public float volume = 0.8f;
	[TitleGroup("$TitleName")] [LabelText("딜레이")]
	public float delay;
	public string TitleName()
	{
		if (clip == null) return "NULL";
		else return clip.name;
	}
	#if UNITY_EDITOR
	[HorizontalGroup("$TitleName/horizontal")][Button][GUIColor(0.5f,1.0f,0.5f)]
	public void Play()
	{
		SoundManager soundManager = MonoBehaviour.FindObjectOfType<SoundManager>();
		if(soundManager == null) Debug.LogError("SoundManager가 존재하지 않습니다!");
		else
		{
			soundManager.DebugPlay(this);
		}
	}
	[HorizontalGroup("$TitleName/horizontal")][Button][GUIColor(1.0f,0.5f,0.5f)]
	public void Stop()
	{
		var sm = MonoBehaviour.FindObjectOfType<SoundManager>();
		if(sm!=null) sm.GetComponent<AudioSource>().Stop();
	}
	#endif
}
