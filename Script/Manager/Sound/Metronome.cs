using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    [ReadOnly]public List<AudioSource> sources = new List<AudioSource>();
    [ReadOnly]public int index;
    public AudioClip beatClip;
    [Button]
    public void Setting()
    {
        sources.Clear();
        while (GetComponent<AudioSource>()!=null) DestroyImmediate(GetComponent<AudioSource>());
        for (int i = 0; i < 100; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            source.clip = beatClip;
            sources.Add(source);
        }
    }

    public void PlayBeat(float delay)
    {
        index = (index + 1) % sources.Count;
        sources[index].PlayDelayed(delay);
    }

    public void StopBeat()
    {
        foreach (var source in sources) source.Stop();
    }
}
