using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundContainer_Ingame : SoundContainer
{
    public static SoundContainer_Ingame instance;
    public override void Setting()
    {
        instance = this;
    }

    public SoundData
        sound_hit_normal,
        sound_hit_smash,
        sound_combat_sparkable,
        sound_interact_wood_normal,
        sound_interact_wood_strong,
        sound_falldown,
        sound_friction_cloth,
        sound_stage_clear,
        sound_stage_begin,
        sound_stage_click;
}
