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
        CamArm.instance.transform.SetPositionAndRotation(
            Room1.startPoint.position + Room1.addVec,Quaternion.Euler(0,Room1.degree,0));
    }
    
    [TitleGroup("인게임 구역 인스펙터")]
    [TabGroup("인게임 구역 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public ParticleSystem areaParticle;

    [TabGroup("인게임 구역 인스펙터/AreaUI", "구역 설정", SdfIconType.Map)]
    public Room_Area Room1, Room2, Room3;

    private Sequence _s_areaparticle;

    [Button]
    public void Enter_Title()
    {
        
    }
    [Button]
    public void Title_Ingame()
    {
        Hero.instance.gameObject.SetActive(false);
        CamArm.instance.Set_FollowTarget(false);
        areaParticle.transform.position = Room1.startPoint.position + Vector3.up*1.0f;
        areaParticle.Play();
        CamArm.instance.transform.SetPositionAndRotation(Room1.startPoint.position,Room1.startPoint.rotation);
        CamArm.instance.Tween_FadeIn();
        _s_areaparticle = Sequence.Create(useUnscaledTime: true)
            .ChainDelay(0.9f)
            .ChainCallback(() =>
            {
                Hero.instance.Spawn(Room1.startPoint.position, Quaternion.Euler(0, Room1.degree+45, 0));
                CamArm.instance.Set_FollowTarget(true);
                CamArm.instance.Tween_ShakeNormal();
                areaParticle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            });
    }

    [Button]
    public void Area1_Area2()
    {
        float duration = 2.5f;
        CamArm.instance.Set_FollowTarget(false);
        
        Hero.instance.Despawn_Move().Forget();
        Vector3 startPos = Hero.instance.transform.position + Vector3.up;
        Vector3 endPos = Room2.startPoint.position;
        Quaternion camEndRot = Quaternion.Euler(0,Room2.degree,0);
        
        areaParticle.transform.position = startPos;
        areaParticle.Play();
        _s_areaparticle.Stop();
        _s_areaparticle = Sequence.Create(useUnscaledTime:true)
            .ChainDelay(0.5f)
            .Chain(Tween.Position(areaParticle.transform, endPos + Vector3.up, duration, Ease.InOutQuart))
            .Group(Tween.Position(CamArm.instance.transform, endPos, duration, Ease.InOutCubic))
            .Group(Tween.Rotation(CamArm.instance.transform, camEndRot, duration, Ease.InOutCubic))
            .OnComplete(() =>
            {
                Hero.instance.Spawn(endPos, camEndRot);
                CamArm.instance.Set_FollowTarget(true);
                CamArm.instance.Tween_ShakeNormal();
                areaParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            });
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