using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [TableList(AlwaysExpanded = true,ShowIndexLabels = true)][TitleGroup("수동 추가 사운드")]
    public List<SoundGroup> soundGroups = new List<SoundGroup>();

    [TitleGroup("필수 사운드")] public SoundData
        sound_hit_normal,
        sound_hit_smash,
        sound_combat_sparkable,
        sound_interact_wood_normal,
        sound_interact_wood_strong,
        sound_falldown,
        sound_friction_cloth;

    private Dictionary<SoundData, SoundGroup> ingameData = new Dictionary<SoundData, SoundGroup>();
    public void Setting()
    {
        foreach (var group in soundGroups) ingameData.Add(group.soundData, group);
        instance = this;
    }
    public static void Play(SoundData soundData,float delay = 0)
    {
        if (!instance.ingameData.ContainsKey(soundData))
        {
            Debug.Log("해당 SoundData가 없습니다.");
            return;
        }
        instance.ingameData[soundData].Play(delay);
    }
    public static void Stop(SoundData soundData)
    {
        if (!instance.ingameData.ContainsKey(soundData))
        {
            Debug.Log("해당 SoundData가 없습니다.");
            return;
        }
        instance.ingameData[soundData].Stop();
    }
    public static void Add(SoundData soundData,int count = 1)
    {
        if (soundData == null) return;
        if (instance.ingameData.ContainsKey(soundData))
        {
            SoundGroup soundGroup = instance.ingameData[soundData];
            Transform parent = soundGroup.audioSources[0].transform.parent;

            for (int i = 0; i < count; i++)
            {
                var singleSound = soundData.sounds[i% soundData.sounds.Count];
                GameObject g_singleSound = new GameObject((i+1)+"_"+soundData.name);
                g_singleSound.transform.parent = parent;
                var source = g_singleSound.AddComponent<AudioSource>();

                source.playOnAwake = false;
                source.clip = singleSound.clip;
                source.volume = singleSound.volume;
                source.loop = singleSound.isLoop;
                soundGroup.audioSources.Add(source);
            }
        }
        else
        {
            GameObject g_soundGroup = new GameObject(soundData.name);
            g_soundGroup.transform.parent = instance.transform;
            int length = Mathf.Max(count, soundData.sounds.Count);
            SoundGroup soundGroup = new SoundGroup();
            soundGroup.soundData = soundData;
            soundGroup.audioIndex = 0;
            soundGroup.audioSources = new List<AudioSource>();
            soundGroup.poolLength = count;
        
            for (int i = 0; i < length; i++)
            {
                var singleSound = soundData.sounds[i% soundData.sounds.Count];
                GameObject g_singleSound = new GameObject((i+1)+"_"+soundData.name);
                g_singleSound.transform.parent = g_soundGroup.transform;
                var source = g_singleSound.AddComponent<AudioSource>();

                source.playOnAwake = false;
                source.clip = singleSound.clip;
                source.volume = singleSound.volume;
                source.loop = singleSound.isLoop;
                soundGroup.audioSources.Add(source);
            }

            instance.soundGroups.Add(soundGroup);
            instance.ingameData.Add(soundData, soundGroup);
        }
    }
    //DEBUG
    public void DebugPlay(SingleSound clip)
    {
        AudioSource source = GetComponent<AudioSource>();
        
        source.pitch = 1.0f + Random.Range(clip.pitch.x, clip.pitch.y);
        //시작 시간
        source.clip = clip.clip;
        source.time = clip.clipRange.x * clip.clip.length;
        source.volume = clip.volume;
        //종료 시간
        double curDspTime = AudioSettings.dspTime;
        double clipDuration = (clip.clip.samples*1.0f) / (clip.clip.frequency*1.0f);
        source.PlayScheduled(curDspTime + clip.delay);
        source.SetScheduledEndTime(curDspTime + clip.delay + clipDuration*(clip.clipRange.y-clip.clipRange.x));
    }
    [Button]
    public void UpdateManager()
    {
        //모든 데이터 초기화
        while (transform.childCount>0) DestroyImmediate(transform.GetChild(0).gameObject);
        foreach (var sg in soundGroups)
        {
            sg.audioSources.Clear();
            sg.audioIndex = 0;
        }
        //새로 제작
        for (int i = 0; i < soundGroups.Count; i++)
        {
            if (soundGroups[i].soundData == null)
            {
                Debug.Log("Null인 값이 있습니다!");
                break;
            }
            GameObject soundGroup = new GameObject(soundGroups[i].soundData.name);
            soundGroup.transform.parent = transform;
            int length = Mathf.Max(soundGroups[i].poolLength, soundGroups[i].soundData.sounds.Count);
            for (int j = 0; j < length; j++)
            {
                var soundData = soundGroups[i].soundData.sounds[j% soundGroups[i].soundData.sounds.Count];
                GameObject g_soundData = new GameObject((j+1)+"_"+soundData.clip.name);
                g_soundData.transform.parent = soundGroup.transform;
                var source = g_soundData.AddComponent<AudioSource>();

                source.playOnAwake = false;
                source.clip = soundData.clip;
                source.volume = soundData.volume;
                source.loop = soundData.isLoop;
                soundGroups[i].audioSources.Add(source);
            }
        }
    }
}
[System.Serializable]
public class SoundGroup
{
    public SoundData soundData;
    [MinValue(1)]
    public int poolLength = 1;

    [HideInInspector] public List<AudioSource> audioSources = new List<AudioSource>();
    [HideInInspector] public int audioIndex = 0;
    [Button]
    public void Play(float delay)
    {
        audioIndex = (audioIndex + 1) % audioSources.Count;
        AudioSource source = audioSources[audioIndex];
        if (source.isPlaying) source.Stop();
        var singleSound = soundData.sounds[audioIndex % soundData.sounds.Count];
        //시작 시간
        source.time = singleSound.clipRange.x * (singleSound.clip.length + delay);
        source.pitch = 1.0f + Random.Range(singleSound.pitch.x, singleSound.pitch.y);
        //종료 시간
        double curDspTime = AudioSettings.dspTime;
        double clipDuration = (singleSound.clip.samples * 1.0f) / (singleSound.clip.frequency * 1.0f);
        source.PlayScheduled(curDspTime + singleSound.delay + delay);
        source.SetScheduledEndTime(curDspTime + singleSound.delay + delay +
                                   clipDuration * (singleSound.clipRange.y - singleSound.clipRange.x));
        source.Play();
    }
    public void Stop()
    {
        foreach (var source in audioSources)
        {
            if(source.isPlaying) source.Stop();
        }
    }
}
