using System.Collections;
using System.Collections.Generic;
using AtmosphericHeightFog;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public partial class GameManager : MonoBehaviour
{
    private void Setting_Area()
    {
        CamArm.instance.Set_FollowTarget(false);
        CamArm.instance.transform.SetPositionAndRotation(
            Room1.startPoint.position,Quaternion.Euler(0,Room1.degree,0));
        _dragon.gameObject.SetActive(true);
        _dragon.Setting();
        _dragon.gameObject.SetActive(false);
        roomIndex = 0;
        heightFog.fogHeightEnd = Get_Room().startPoint.position.y - 3.75f;
        Directing_Ready();
    }
    [TitleGroup("메인 설정 인스펙터")]
    [TabGroup("메인 설정 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public BgmData bgmMain, bgmSuccess, bgmFailed,bgmGameOver;
    [TabGroup("메인 설정 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public HeightFogGlobal heightFog;
    [TabGroup("메인 설정 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public Dragon _dragon;
    [TabGroup("메인 설정 인스펙터/AreaUI", "구역 설정", SdfIconType.Map)]
    public Room_Area Room1, Room2, Room3;
    private int roomIndex = 0;

    private Sequence _seqIngame;
    private Tween _tFog;

    [Button]
    public void Directing_Ready()
    {
        CamArm.instance.SetFinished(false);
        CamArm.instance.Tween_GameReady();
        CamArm.instance.Set_FollowTarget(false);
        CamArm.instance.transform.SetPositionAndRotation(Room1.startPoint.position,Room1.startPoint.rotation);
        roomIndex = 0;
    }
    [Button]
    public void Directing_Start()
    {
        BgmManager.instance.fadeDuration = 0.0f;
        
        SoundManager.Play(SoundContainer_Ingame.instance.sound_stage_clear);
        
        
        CamArm.instance.SetFinished(false);
        CamArm.instance.Set_FollowTarget(false);
        CamArm.instance.transform.SetPositionAndRotation(Room1.startPoint.position,Room1.startPoint.rotation);
        CamArm.instance.Tween_GameStart();
        _seqIngame.Stop();
        _seqIngame = Sequence.Create(useUnscaledTime: true);
        _seqIngame
            .ChainDelay(0.5f)
            .ChainCallback(() =>
            {
                _dragon.Call();
                Hero.instance.SpawnInstantly();
                Hero.instance.MountInstantly();
            })
            .ChainDelay(0.375f)
            .ChainCallback(() => BgmManager.instance.PlayBGM(bgmMain, false))
            .ChainDelay(0.5f)
            .ChainCallback(() => BgmManager.instance.ChangeLayer(1));


    }
    [Button]
    public void Directing_MoveRoom()
    {
        CamArm.instance.UI_Clear();
        CamArm.instance.Set_FollowTarget(false);
        Room_Area startRoom = Get_Room();
        roomIndex++;
        Room_Area endRoom = Get_Room();
        _dragon.MoveDestination(startRoom,endRoom);
        //HeightFog 높이 조절
        _tFog.Stop();
        _tFog = Tween.Custom(heightFog.fogHeightEnd, endRoom.startPoint.position.y - 3.75f,
            2.0f, onValueChange: heightEnd => heightFog.fogHeightEnd = heightEnd);

    }
    [Button]
    public void Directing_Success()
    {
        BgmManager.instance.fadeDuration = 1.0f;
        float delay = BgmManager.instance.ChangeLayer(2);
        //SoundManager.Play(SoundContainer_Ingame.instance.sound_stage_begin);
        CamArm.instance.SetFinished(true);
        CamArm.instance.Set_FollowTarget(true);
        _dragon.FinalFlight(Get_Room(),CamArm.instance);
    }

    [Button]
    public void Directing_Failed()
    {
        SoundManager.Play(SoundContainer_Ingame.instance.sound_hit_smash);
        SoundManager.Play(Hero.instance.sound_voice_death,2.25f);
        BgmManager.instance.fadeDuration = 0.0f;
        BgmManager.instance.PlayBGM(bgmFailed,false);
        Hero.instance.Death();
    }
    public Room_Area Get_Room()
    {
        if (roomIndex == 0) return Room1;
        else if (roomIndex == 1) return Room2;
        else return Room3;
    }
}
[System.Serializable]
public class Room_Area
{
    [LabelText("카메라 각도")] public float degree;
    [LabelText("시작 카메라 구역")] public Transform startPoint;
	
    

    [HideInInspector] public float padding = 2;
}