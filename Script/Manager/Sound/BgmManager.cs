using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;
    private static string _strBgmLowpassCutoff = "BgmLowpassCutoff";
    private Tween _tweenLowpass;
    //Setting--------------------------------------------------------------
    [TitleGroup("Setting")] public AudioMixerGroup bgmMixer;
    [TitleGroup("Setting")]public float fadeDuration = 0.75f;
    [TitleGroup("Setting")]public AnimationCurve fadeCurve;
    [TitleGroup("Setting")]public List<BgmData> BGMs = new List<BgmData>();
    public Vector2Int currentData = Vector2Int.zero;
    [TitleGroup("Setting")][Button]
    public void UpdateBGM()
    {
        while (transform.childCount>0) DestroyImmediate(transform.GetChild(0).gameObject);
        foreach (var bgm in BGMs)
        {
            GameObject g = new GameObject(bgm.name);
            g.transform.SetParent(transform);
            foreach (var layer in bgm.bgmLayer) UpdateLayer(layer,g.transform);
            UpdateLayer(bgm.intro,g.transform);
            UpdateLayer(bgm.outro,g.transform);
        }


        void UpdateLayer(BgmLayer layer, Transform parent)
        {
            if (layer.clip == null) return;

            GameObject g = new GameObject(layer.clip.name);
            g.transform.SetParent(parent);
            AudioSource source = g.AddComponent<AudioSource>();
            source.clip = layer.clip;
            source.loop = layer.loop;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = bgmMixer;
        }
    }
    public Dictionary<BgmLayer, AudioSource> layerSources;
    public void Setting()
    {
        layerSources = new Dictionary<BgmLayer, AudioSource>();
        instance = this;
        foreach (var bgm in BGMs)
        {
            Transform t = transform.Find(bgm.name);
            if(bgm.intro.clip!=null) layerSources.Add(bgm.intro,t.Find(bgm.intro.clip.name).GetComponent<AudioSource>());
            if(bgm.outro.clip!=null) layerSources.Add(bgm.outro,t.Find(bgm.outro.clip.name).GetComponent<AudioSource>());
            foreach (var layer in bgm.bgmLayer)
            {
                Transform t2 = t.Find(layer.clip.name);
                layerSources.Add(layer,t2.GetComponent<AudioSource>());
            }
        }
    }
    public AudioSource GetSource(BgmLayer layer)
    {
        if (!layerSources.ContainsKey(layer)) return null;
        return layerSources[layer];
    }
    //메트로놈--------------------------------------------------------------
    [TitleGroup("Metronome")]public Metronome metronome;
    [TitleGroup("Metronome")]public Vector2Int metronomeTarget;
    [TitleGroup("Metronome")] public bool useIntro;
    [HorizontalGroup("Metronome/H")][Button,GUIColor(0.75f,1.0f,0.75f)]
    public void Metronome_Activate()
    {
        if (useIntro)
        {
            
            GetSource(BGMs[metronomeTarget.x].intro).Play();
            if (BGMs[metronomeTarget.x].bgmLayer.Count > metronomeTarget.y)
            {
                AudioSource source = GetSource(BGMs[metronomeTarget.x].bgmLayer[metronomeTarget.y]);
                source.PlayDelayed(GetSource(BGMs[metronomeTarget.x].intro).clip.length);
            }
                
        }
        else
        {
            if(GetSource(BGMs[metronomeTarget.x].intro)!=null) GetSource(BGMs[metronomeTarget.x].intro).Stop();
            if (BGMs[metronomeTarget.x].bgmLayer.Count > metronomeTarget.y)
            {
                
                print(GetSource(BGMs[metronomeTarget.x].bgmLayer[metronomeTarget.y]).clip.name);
                GetSource(BGMs[metronomeTarget.x].bgmLayer[metronomeTarget.y]).volume = 1;
                GetSource(BGMs[metronomeTarget.x].bgmLayer[metronomeTarget.y]).Play();
            }
        }
        float lastBeat = 60.0f/(BGMs[metronomeTarget.x].bpm);
        for (int i = 0; i < 100; i++)
        {
            metronome.PlayBeat(lastBeat*i + BGMs[0].beatDelay);
        }
    }
    [HorizontalGroup("Metronome/H")][Button,GUIColor(1.0f,0.75f,0.75f)]
    public void Metronome_Deactivate()
    {
        metronome.StopBeat();
        foreach (var bgm in BGMs)
        {
            if(GetSource(bgm.intro)!=null) GetSource(bgm.intro).Stop();
            if(GetSource(bgm.outro)!=null) GetSource(bgm.outro).Stop();
            foreach(var layer in bgm.bgmLayer) GetSource(layer).Stop();
        }
        
    }
    //BGM------------------------------------------------------------------
    [TitleGroup("Control")][Button]
    public void PlayBGM(BgmData data,bool useIntro)
    {
        int bgmIndex = BGMs.IndexOf(data);
        if (BGMs.Count <= currentData.x) return;
        if (BGMs.Count <= bgmIndex) return;
        
        //기존 BGM 페이드 아웃
        double dspTime = AudioSettings.dspTime;
        AudioSource pastSource = GetSource(BGMs[currentData.x].intro);
        if(pastSource!=null && pastSource.isPlaying) AddFadeOut(pastSource,0);
        if (BGMs[currentData.x].bgmLayer.Count > currentData.y)
        {
            AudioSource source = GetSource(BGMs[currentData.x].bgmLayer[currentData.y]);
            if (source.isPlaying) AddFadeOut(source,0);
        }
        currentData = new Vector2Int(bgmIndex, 0);
        //새로 플레이 
        if (useIntro)
        {
            //인트로 있는 경우
            if (BGMs[currentData.x].intro.clip != null)
            {
                BgmLayer intro = BGMs[currentData.x].intro;
                AudioSource introSource = GetSource(intro);
                float startDelay = fadeDuration*0.65f;
                introSource.time = 0;
                introSource.volume = 1;
                introSource.PlayDelayed(startDelay);
                introSource.SetScheduledEndTime(AudioSettings.dspTime+introSource.clip.length+startDelay);
                float delay = intro.clip.length;
                if (BGMs[currentData.x].bgmLayer.Count > 0)
                {
                    AudioSource source = GetSource(BGMs[currentData.x].bgmLayer[currentData.y]);
                    source.time = 0;
                    source.volume = 1;
                    source.PlayDelayed(delay+startDelay);
                } 
            }
            //인트로 없는 경우
            else
            {
                if (BGMs[currentData.x].bgmLayer.Count > 0)
                {
                    float startDelay = fadeDuration*0.65f;
                    AudioSource source = GetSource(BGMs[currentData.x].bgmLayer[currentData.y]);
                    source.time = 0;
                    source.volume = 1;
                    source.PlayDelayed(startDelay);
                } 
            }
        }
        //새로 플레이 - 인트로 미포함
        else
        {
            float startDelay = fadeDuration*0.65f;
            if (BGMs[currentData.x].bgmLayer.Count > 0)
            {
                AudioSource source = GetSource(BGMs[currentData.x].bgmLayer[currentData.y]);
                
                if(source.isPlaying) AddFadeIn(source,startDelay);
                else
                {
                    source.volume = 1;
                    source.PlayDelayed(startDelay);
                }
            }
        }
    }
    [TitleGroup("Control")][Button]
    public void ChangeLayer(int layerIndex)
    {
        //범위 밖,이미 플레이중이면 return
        if (BGMs.Count <= currentData.x) return;
        if (BGMs[currentData.x].bgmLayer.Count <= currentData.y) return;
        if (BGMs[currentData.x].bgmLayer.Count <= layerIndex) return;
        if (GetSource(BGMs[currentData.x].bgmLayer[layerIndex]).isPlaying &&
            GetSource(BGMs[currentData.x].bgmLayer[layerIndex]).volume>0.25f) return;
        
        //기존 사운드 설정
        BgmLayer lastLayer;
        var intro = GetSource(BGMs[currentData.x].intro);
        if(intro!=null && intro.isPlaying) lastLayer = BGMs[currentData.x].intro;
        else lastLayer = BGMs[currentData.x].bgmLayer[currentData.y];
        float section = (60.0f/BGMs[currentData.x].bpm) * lastLayer.beatSectionCount;
        float lastBGMTime = (GetSource(lastLayer).time % lastLayer.clip.length);
        float targetTime = section * (Mathf.FloorToInt(lastBGMTime / section) + 1);
        float delay = targetTime - lastBGMTime;
        AddFadeOut(GetSource(lastLayer),delay + BGMs[currentData.x].beatDelay);
        //새로운 사운드 설정
        currentData.y = layerIndex;
        AudioSource source = GetSource(BGMs[currentData.x].bgmLayer[currentData.y]);
        source.time = 0;
        source.volume = 1;
        source.PlayDelayed(delay);
    }
    public void BgmLowpass(bool activate,float speed = 1.0f)
    {
        _tweenLowpass.Stop();
        if (activate)
        {
            _tweenLowpass = Tween.Custom(6000, 1000, 1.0f/speed,useUnscaledTime:true, 
                onValueChange: lowpass =>bgmMixer.audioMixer.SetFloat(_strBgmLowpassCutoff, lowpass));
        }
        else
        {
            _tweenLowpass = Tween.Custom(700, 6000, 1.0f/speed,useUnscaledTime: true, 
                onValueChange: lowpass =>bgmMixer.audioMixer.SetFloat(_strBgmLowpassCutoff, lowpass));
        }

    }
    //함수------------------------------------------------------------------
    private Dictionary<AudioSource, (double dspDelay, double dspFade)> dspFadeOutCall =
        new Dictionary<AudioSource, (double dspDelay, double dspFade)>();
    private Dictionary<AudioSource, (double dspDelay, double dspFade)> dspFadeInCall =
        new Dictionary<AudioSource, (double dspDelay, double dspFade)>();
    private void FixedUpdate()
    {
        List<AudioSource> removeInTarget = new List<AudioSource>();
        List<AudioSource> removeOutTarget = new List<AudioSource>();
        //페이드 아웃
        foreach (var fadeCall in dspFadeOutCall)
        {
            double dspTime = AudioSettings.dspTime;
            if (dspTime < fadeCall.Value.dspDelay) continue;
            else if (dspTime < fadeCall.Value.dspFade)
            {
                float ratio = (float)((fadeCall.Value.dspFade - dspTime) /
                                      (fadeCall.Value.dspFade - fadeCall.Value.dspDelay));
                float volume = Mathf.Lerp(0, 1,fadeCurve.Evaluate(ratio));
                
                fadeCall.Key.volume = volume;
            }
            else
            {
                fadeCall.Key.volume = 0;
                removeOutTarget.Add(fadeCall.Key);
            }
        }
        foreach (var target in removeOutTarget)
        {
            dspFadeOutCall.Remove(target);
        }
        //페이드 인
        foreach (var fadeCall in dspFadeInCall)
        {
            double dspTime = AudioSettings.dspTime;
            if (dspTime < fadeCall.Value.dspDelay) continue;
            else if (dspTime < fadeCall.Value.dspFade)
            {
                float ratio = (float)((fadeCall.Value.dspFade - dspTime) /
                                      (fadeCall.Value.dspFade - fadeCall.Value.dspDelay));
                float volume = Mathf.Lerp(1, 0,fadeCurve.Evaluate(ratio));
                
                fadeCall.Key.volume = volume;
            }
            else
            {
                fadeCall.Key.volume = 1;
                removeInTarget.Add(fadeCall.Key);
            }
        }
        foreach (var target in removeInTarget)
        {
            dspFadeInCall.Remove(target);
        }
    }
    private void AddFadeOut(AudioSource source,float delay)
    {
        double dspTime = AudioSettings.dspTime;
        dspFadeOutCall.Add(source,(dspTime + delay,fadeDuration + dspTime));
    }
    private void AddFadeIn(AudioSource source,float delay)
    {
        double dspTime = AudioSettings.dspTime;
        dspFadeInCall.Add(source,(dspTime + delay,fadeDuration + dspTime));
    }
}