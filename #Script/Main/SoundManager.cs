using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [FoldoutGroup("BGM")]
    public AudioMixerSnapshot snapshot_Ingame_Enter,snapshot_Ingame_Main,snapshot_Ingame_StageClear,snapshot_Ingame_Result;
    [FoldoutGroup("BGM")]
    public Data_Audio bgm_ambience, bgm_main, bgm_clear;
    [FoldoutGroup("BGM")]
    public float snapshot_TransitionSpeed_Enter = 0.5f,snapshot_TransitionSpeed_Main = 0.5f,
        snapshot_TransitionSpeed_StageClear = 0.5f,snapshot_TransitionSpeed_Result = 0.5f;

    private class ManagedAudioData
    {
        private Queue<AudioSource> sources;
        private bool playAll;
        private bool isLoop = false;
        private int playIndex;
        public ManagedAudioData(Queue<AudioSource> sources,bool playAll,bool isLoop)
        {
            this.sources = sources;
            this.playAll = playAll;
            this.isLoop = isLoop;
            playIndex = Random.Range(0,sources.Count);
        }

        public void Play(Data_Audio dataAudio, float _volume)
        {
            if (playAll)
            {
                int count = sources.Count;
                for (int i = 0; i < count; i++)
                {
                    AudioSource audio = sources.Dequeue();
                    sources.Enqueue(audio);
                    PlayAudioSource(audio, dataAudio.clips[i], dataAudio.mixerGroup);
                }
            }
            else
            {
                AudioSource audio = sources.Dequeue();
                sources.Enqueue(audio);
                PlayAudioSource(audio, dataAudio.clips[playIndex], dataAudio.mixerGroup);
                playIndex = (playIndex + 1) % sources.Count;
            }

            void PlayAudioSource(AudioSource source, Data_AudioClip dataAudioClip, AudioMixerGroup mixerGroup)
            {
                source.clip = dataAudioClip.clip;
                source.loop = isLoop;
                source.outputAudioMixerGroup = mixerGroup;
                source.pitch = 1.0f + Random.Range(dataAudioClip.pitch.x, dataAudioClip.pitch.y);
                //시작 시간
                source.time = dataAudioClip.clipRange.x * source.clip.length;
                source.volume = dataAudioClip.volume*_volume;
                //종료 시간
                double curDspTime = AudioSettings.dspTime;
                double clipDuration = (source.clip.samples * 1.0f) / (source.clip.frequency * 1.0f);
                source.PlayScheduled(curDspTime + dataAudioClip.delay);
                
                if (!isLoop)
                {
                    source.SetScheduledEndTime(curDspTime + dataAudioClip.delay
                     + clipDuration * (dataAudioClip.clipRange.y - dataAudioClip.clipRange.x));
                }
                
            }
        }
    }
    private Dictionary<Data_Audio, ManagedAudioData> playData;
    public void Setting()
    {
        if (instance != null && instance!=this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        playData = new Dictionary<Data_Audio, ManagedAudioData>();
        if (debugT != null)
        {
            #if UNITY_EDITOR
            Debug.LogWarning("디버그용 사운드T 가 존재합니다.\n 플레이 종료 후 직접 제거해주세요!");
            #endif
            Destroy(debugT.gameObject);
        }
        
        Add(bgm_ambience);
        Add(bgm_main);
        Add(bgm_clear);
    }
    public void Add(params Data_Audio[] audios)
    {
        foreach (var data_audio in audios)
        {
            if (!playData.ContainsKey(data_audio))
            {
                Queue<AudioSource> audioSources = new Queue<AudioSource>();
                GameObject root = new GameObject(data_audio.name);
                root.transform.SetParent(transform);
                
                for (int i = 0; i < data_audio.clips.Count; i++)
                {
                    GameObject child = new GameObject(data_audio.clips[i].clip.name);
                    child.transform.SetParent(root.transform);
                    
                    AudioSource audioSource = child.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                    audioSources.Enqueue(audioSource);
                }
                
                playData.Add(data_audio,new ManagedAudioData(audioSources,data_audio.playTogether,data_audio.isLoop));
            }
        }
    }
    
    public void Play(Data_Audio audio,float _volume)
    {
        if(!playData.ContainsKey(audio)) Add(audio);
        playData[audio].Play(audio,_volume);
    }
    //Mixer-------------------------------------------------------------------------------------------------------------
    public void Ingame_Enter()
    {
        bgm_ambience.Play();
        snapshot_Ingame_Enter.TransitionTo(snapshot_TransitionSpeed_Enter);
    }
    public void Ingame_Main()
    {
        bgm_main.Play();
        snapshot_Ingame_Main.TransitionTo(snapshot_TransitionSpeed_Main);
    }
    public void Ingame_StageClear()
    {
        snapshot_Ingame_StageClear.TransitionTo(snapshot_TransitionSpeed_StageClear);
    }
    public void Ingame_Result()
    {
        bgm_clear.Play();
        snapshot_Ingame_Result.TransitionTo(snapshot_TransitionSpeed_Result);
    }
    //DEBUG-------------------------------------------------------------------------------------------------------------
    public void DebugPlay(Data_AudioClip audioClip)
    {
        Create_DebugAudio();
        AudioSource source;
        if (debugT.childCount == 0)
        {
            GameObject newG = new GameObject(debugAudioName + "_" + 0);
            newG.transform.SetParent(debugT);
            source = newG.AddComponent<AudioSource>();
        }
        else source = debugT.GetChild(0).GetComponent<AudioSource>();
        
        source.pitch = 1.0f + Random.Range(audioClip.pitch.x, audioClip.pitch.y);
        //시작 시간
        source.clip = audioClip.clip;
        source.time = audioClip.clipRange.x * audioClip.clip.length;
        source.volume = 1;
        //종료 시간
        double curDspTime = AudioSettings.dspTime;
        double clipDuration = (audioClip.clip.samples*1.0f) / (audioClip.clip.frequency*1.0f);
        source.PlayScheduled(curDspTime + audioClip.delay);
        source.SetScheduledEndTime(curDspTime + audioClip.delay + clipDuration*(audioClip.clipRange.y-audioClip.clipRange.x));
    }
    public void DebugPlay(Data_Audio data)
    {
        Create_DebugAudio();
        int childCount = debugT.childCount;
        for (int i = 0; i < data.clips.Count-childCount; i++)
        {
            GameObject newG = new GameObject(debugAudioName + "_" + (childCount+i));
            newG.transform.SetParent(debugT);
            newG.AddComponent<AudioSource>();
        }
        for (int i=0; i<data.clips.Count; i++)
        {
            Data_AudioClip audioClip = data.clips[i];
            AudioSource source = debugT.GetChild(i).GetComponent<AudioSource>();
            source.pitch = 1.0f + Random.Range(audioClip.pitch.x, audioClip.pitch.y);
            //시작 시간
            source.clip = audioClip.clip;
            source.time = audioClip.clipRange.x * audioClip.clip.length;
            source.volume = 1;
            //종료 시간
            double curDspTime = AudioSettings.dspTime;
            double clipDuration = (audioClip.clip.samples*1.0f) / (audioClip.clip.frequency*1.0f);
            source.PlayScheduled(curDspTime + audioClip.delay);
            source.SetScheduledEndTime(curDspTime + audioClip.delay + clipDuration*(audioClip.clipRange.y-audioClip.clipRange.x));
        }
    }
    private string debugAudioName = "Debug";
    [HideInInspector] public Transform debugT;
        private void Create_DebugAudio()
    {
        debugT = transform.Find(debugAudioName);
        if (debugT == null)
        {
            GameObject newG = new GameObject(debugAudioName);
            newG.transform.SetParent(transform);
        }
    }
}

