using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Canvas_Player: MonoBehaviour
{
    [FoldoutGroup("Audio")] public Data_Audio audio_Enter, audio_EnterFin;
    public void Setting_Sound()
    {
        SoundManager.instance.Add(audio_Enter);
        SoundManager.instance.Add(audio_EnterFin);
    }
}
