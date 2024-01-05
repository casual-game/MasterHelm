using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    public void Setting_Sound()
    {
        SoundManager.Add(sd_hit);
        SoundManager.Add(sd_death);
        SoundManager.Add(sd_attack);
        SoundManager.Add(sd_spawn);
    }
    //Public
    [FoldoutGroup("Sound")] public SoundData sd_hit, sd_death,sd_attack,sd_spawn;
    //Private
    private float voiceDelay = 2.0f;
    private float lastVoicedTime = -100;

    
    public void Voice_Hit(bool force = false)
    {
        if (!Get_IsAlive())
        {
            SoundManager.Play(sd_death);
            return;
        }
        if (!force && Time.unscaledTime - lastVoicedTime < voiceDelay) return;
        
        lastVoicedTime = Time.unscaledTime;
        SoundManager.Play(sd_hit);
    }
    public void Voice_Attack(bool force = false)
    {
        if (!force && Time.unscaledTime - lastVoicedTime < voiceDelay) return;
        lastVoicedTime = Time.unscaledTime;
        SoundManager.Play(sd_attack);
    }
    
    public void Voice_Death()
    {
        lastVoicedTime = Time.unscaledTime;
        SoundManager.Play(sd_death);
    }
}
