using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "BgmData", menuName = "Data/BgmData", order = 1)]
public class BgmData : ScriptableObject
{
    public int bpm=170;
    public float beatDelay = 0.0f;
    public BgmLayer intro, outro;
    public List<BgmLayer> bgmLayer = new List<BgmLayer>();
}

[System.Serializable]
public class BgmLayer
{
    public AudioClip clip;
    public bool loop = true;
    public int beatSectionCount = 8;
    public int beatSectionDelayCount = 0;

    private double dspDelay,dspFade;
    public void FadeOut(double delay,double duration)
    {
        dspDelay = AudioSettings.dspTime + delay;
        dspFade = dspDelay + duration;
    }


    private AudioSource FindSource()
    {
        var manager = GameObject.FindObjectOfType<BgmManager>();
        BgmData data = null;
        foreach (var bgm in manager.BGMs)
        {
            if (bgm.bgmLayer.Contains(this) ||
                bgm.intro == this ||
                bgm.outro == this)
            {
                data = bgm;
                break;
            }
        }

        if (data == null)
        {
            return null;
        }
        return manager.transform.Find(data.name).Find(clip.name).GetComponent<AudioSource>();
    }
    [ShowInInspector][Button,GUIColor(0.75f,1.0f,0.75f)]
    private void Play()
    {
        FindSource().Play();
    }
    [ShowInInspector][Button,GUIColor(1.0f,0.75f,0.75f)]
    private void Stop()
    {
        FindSource().Stop();
    }
}