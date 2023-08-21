using System.Collections;
using System.Collections.Generic;
using ES3Internal;
using LeTai.TrueShadow;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Audio", menuName = "Data/Data_Audio", order = 1)]
public class Data_Audio : ScriptableObject
{
	[LabelText("오브젝트 풀링 사용")] public bool usePool = false;
	[LabelText("반복 여부")] public bool isLoop;
	[LabelText("전부 플레이")] public bool playTogether = false;
	[LabelText("오디오 믹서")] public AudioMixerGroup mixerGroup;
	#if UNITY_EDITOR
	[ShowIf("playTogether")][Button][GUIColor(1.0f,0.5f,0.5f)][ShowInInspector]
	private void PlayTogether()
	{
		SoundManager soundManager = MonoBehaviour.FindObjectOfType<SoundManager>();
		if(soundManager == null) Debug.LogError("SoundManager가 존재하지 않습니다!");
		else
		{
			soundManager.DebugPlay(this);
		}
	}
	#endif
	public void Play(float _volume = 1.0f)
	{
		if (SoundManager.instance == null)
		{
			Debug.LogError("사운드매니저가 없습니다!");
			return;
		}

		SoundManager.instance.Play(this,_volume);
	}
	[LabelText("오디오 클립")][PropertySpace(32)]
	[ListDrawerSettings(DraggableItems = false,AddCopiesLastElement =  true,ShowFoldout = false)]
	public List<Data_AudioClip> clips = new List<Data_AudioClip>();
}
[System.Serializable]
public class Data_AudioClip
{
	
	
	[TitleGroup("$TitleName")][HorizontalGroup("$TitleName/horizontal")][HideLabel]
	public AudioClip clip;
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
		AudioSource[] sources = MonoBehaviour.FindObjectsOfType<AudioSource>();
		if(sources.Length ==0 ) Debug.LogError("AudioSource가 하나도 존재하지 않습니다!");
		else
		{
			foreach (var source in sources)
			{
				source.Stop();
			}
		}
	}
	#endif
	
	[TitleGroup("$TitleName")][LabelText("시작시간 설정")][MinMaxSlider(0.0f,1.0f,true)]  
	public Vector2 clipRange = new Vector2(0,1);
	[TitleGroup("$TitleName")][LabelText("렌덤 pitch")][MinMaxSlider(-0.15f,0.15f,true)] 
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
}