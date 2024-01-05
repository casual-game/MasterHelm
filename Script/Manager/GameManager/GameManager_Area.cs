using System.Collections;
using System.Collections.Generic;
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
        
    }
    [Button]
    public void Test()
    {
        print("1");
        //카메라 첫 위치 설정
        SetCamT(cam_crossfade_3.transform,Room3,CamArm.instance.transform);
        SetCamT(cam_crossfade_2.transform,Room2,transform);
        SetCamT(cam_crossfade_1.transform,Room1,transform);
        //기타 설정
        _seqarea = Sequence.Create(useUnscaledTime: true);
        float duration = 3.0f;
        float transition = 1.0f;
        Color endColor = new Color(1, 1, 1, 0);
        raw_crossfade_3.color = Color.white;
        raw_crossfade_2.color = Color.white;
        raw_crossfade_1.color = Color.white;
        CamArm.instance.transform.position = Room1.overviewFin.position;
        CamArm.instance.transform.rotation = Quaternion.Euler(0,Room3.degree,0);
        //시퀸스 시작
        _seqarea = Sequence.Create();
        //Room3 -> Room2
        _seqarea.Chain(Tween.Position(CamArm.instance.transform,
            Room3.overviewFin.position, Room3.overviewBegin.position, duration,ease: Ease.InOutSine));
        _seqarea.Group(Tween.Custom(0, 1,transition, startDelay: duration-transition, 
            onValueChange: ratio => { raw_crossfade_3.color = Color.Lerp(Color.white,endColor, ratio); }));
        _seqarea.ChainCallback(() =>
        {
            cam_crossfade_3.transform.SetParent(transform);
            SetCamT(cam_crossfade_2.transform,Room2,CamArm.instance.transform);
        });
        //Room2 -> Room1
        _seqarea.Chain(Tween.Position(CamArm.instance.transform,
            Room2.overviewFin.position, Room2.overviewBegin.position, duration,ease: Ease.InOutSine));
        _seqarea.Group(Tween.Custom(0, 1,transition, startDelay: duration-transition, 
            onValueChange: ratio => { raw_crossfade_2.color = Color.Lerp(Color.white,endColor, ratio); }));
        _seqarea.ChainCallback(() =>
        {
            cam_crossfade_2.transform.SetParent(transform);
            SetCamT(cam_crossfade_1.transform,Room1,CamArm.instance.transform);
        });
        //Room1 -> 종료지점
        _seqarea.Chain(Tween.Position(CamArm.instance.transform,
            Room1.overviewFin.position, Room1.overviewBegin.position, duration,ease: Ease.InOutSine));
        _seqarea.Group(Tween.Custom(0, 1,transition, startDelay: duration-transition, 
            onValueChange: ratio => { raw_crossfade_1.color = Color.Lerp(Color.white,endColor, ratio); }));
        _seqarea.OnComplete(() =>
        {
            cam_crossfade_1.transform.SetParent(transform);
        });
        
        void SetCamT(Transform camT,Room_Area room,Transform finalParent)
        {
            CamArm.instance.transform.position = room.overviewFin.position;
            CamArm.instance.transform.rotation = Quaternion.Euler(0,room.degree,0);
            camT.SetParent(CamArm.instance.mainCam.transform);
            camT.SetLocalPositionAndRotation(V3_Zero,Q_Identity);
            camT.SetParent(finalParent);
        }
    }
    [TitleGroup("인게임 구역 인스펙터")]
    [TabGroup("인게임 구역 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public ParticleSystem areaParticle;
    [TabGroup("인게임 구역 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public RawImage raw_crossfade_1, raw_crossfade_2,raw_crossfade_3;
    [TabGroup("인게임 구역 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public Camera cam_crossfade_1, cam_crossfade_2,cam_crossfade_3;


    [TabGroup("인게임 구역 인스펙터/AreaUI", "구역 설정", SdfIconType.Map)]
    public Room_Area Room1, Room2, Room3;

    private Sequence _seqarea;
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
    [BoxGroup("points")] public Transform startPoint, overviewBegin, overviewFin;
	
    

    [HideInInspector] public float padding = 2;
}