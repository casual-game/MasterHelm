using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundContainer_StageSelect : SoundContainer
{
    public static SoundContainer_StageSelect instance;
    public override void Setting()
    {
        instance = this;
    }

    public SoundData
        sound_sideui_open,
        sound_sideui_close,
        sound_click,
        sound_clickfailed,
        sound_page_open,
        sound_page_close,
        sound_shop,
        sound_book,
        sound_forge,
        sound_stage_selected,
        sound_popup_success,
        sound_popup_positive,
        sound_popup_negative,
        sound_popup_create;
}
