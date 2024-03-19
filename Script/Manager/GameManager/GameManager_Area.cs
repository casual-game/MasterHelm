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
            Room1.startPoint.position + Room1.addVec,Quaternion.Euler(0,Room1.degree,0));
        _dragon.gameObject.SetActive(true);
        _dragon.Setting();
        _dragon.gameObject.SetActive(false);
        roomIndex = 0;
        heightFog.fogHeightEnd = Get_Room().startPoint.position.y - 3.75f;
        Directing_Ready();
    }

    [TitleGroup("인게임 구역 인스펙터")] public BgmData bgmMain, bgmSuccess, bgmFailed;
    [TitleGroup("인게임 구역 인스펙터")]
    [TabGroup("인게임 구역 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public HeightFogGlobal heightFog;
    [TabGroup("인게임 구역 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public Dragon _dragon;
    [TabGroup("인게임 구역 인스펙터/AreaUI", "구역 설정", SdfIconType.Map)]
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
        BgmManager.instance.PlayBGM(bgmSuccess,false);
        CamArm.instance.SetFinished(true);
        CamArm.instance.Set_FollowTarget(true);
        _dragon.FinalFlight(Get_Room(),CamArm.instance);
    }

    [Button]
    public void Directing_Failed()
    {
        BgmManager.instance.PlayBGM(bgmFailed,false);
        Hero.instance.Death();
    }
    public Room_Area Get_Room()
    {
        if (roomIndex == 0) return Room1;
        else if (roomIndex == 1) return Room2;
        else return Room3;
    }
    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Quaternion currentRot = SceneView.lastActiveSceneView.camera.transform.rotation;
        Quaternion targetRot = Quaternion.Euler(90,0,0);
        bool isRot = Quaternion.Angle(currentRot, targetRot) < 0.1f;
        if (!SceneView.lastActiveSceneView.camera.orthographic || !isRot) return;
		
        var gui = new GUIStyle();
		
        gui.fontStyle = FontStyle.Bold;
        gui.normal.textColor = Color.white;
        Room_Area[] Rooms = new Room_Area[3]{Room1, Room2, Room3};
        foreach (var area in Rooms)
        {
            gui.fontSize = Mathf.RoundToInt(900/SceneView.lastActiveSceneView.camera.transform.position.y);
            Vector3 center = new Vector3(area.startPos.x + area.size.x*0.5f
                , 0, area.startPos.y + area.size.y*0.5f);
            Vector3 size = new Vector3(area.size.x, 0, area.size.y);
            //외곽선
			
            Handles.color = Color.white;
            Vector3 upperLeft = center + new Vector3(-size.x * 0.5f, 0, size.z * 0.5f);
            Vector3 middleLeft = center + new Vector3(-size.x * 0.5f, 0, 0);
            Vector3 upperRight = center + new Vector3(size.x * 0.5f, 0, size.z * 0.5f);
            Vector3 lowerLeft = center + new Vector3(-size.x * 0.5f, 0, -size.z * 0.5f);
            Vector3 lowerRight = center + new Vector3(size.x * 0.5f, 0, -size.z * 0.5f);
            Vector3[] verts = new Vector3[]
            {
                lowerLeft,
                upperLeft,
                upperRight,
                lowerRight
            };
            Handles.DrawSolidRectangleWithOutline(verts, 
                new Color(area.color.r,area.color.g,area.color.b,0.5f), Color.clear);
            Handles.DrawLine(upperLeft,upperRight,2);
            Handles.DrawLine(upperLeft,lowerLeft,2);
            Handles.DrawLine(lowerLeft,lowerRight,2);
            Handles.DrawLine(lowerRight,upperRight,2);
            //글자
            gui.normal.textColor = Color.white;
            gui.alignment = TextAnchor.UpperLeft;
            Handles.Label(upperLeft,area.title,gui);
        }
    }
    #endif
}
[System.Serializable]
public class Room_Area
{
    [Title("$title")]
    [LabelText("디버깅용 이름")] public string title;
    [LabelText("카메라 각도")] public float degree;
    [LabelText("시작 좌표")] public Vector2Int startPos;
    [LabelText("구역 크기")] public Vector2Int size = new Vector2Int(6,6);
    [LabelText("추가 벡터")] public Vector3 addVec;
    [LabelText("구역 색상")][Space(12.0f)][ColorPalette] public Color color;
    [LabelText("시작 카메라 구역")] public Transform startPoint;
	
    

    [HideInInspector] public float padding = 2;
}