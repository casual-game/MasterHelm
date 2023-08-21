using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public partial class Manager_Main: MonoBehaviour
{
	[TitleGroup("에디터 툴")]
	[TabGroup("에디터 툴/tools", "실내 설정",SdfIconType.HouseFill,TextColor = "orange")]
	[Button("기즈모 그리기", ButtonSizes.Large,Stretch = false,Icon = SdfIconType.Stars,ButtonAlignment = 0.0f),GUIColor(0,1,0)]
	public void DrawGizmos_Rooms()
	{
		gizmoTab = 2;
	}

	[TabGroup("에디터 툴/tools", "실내 설정",SdfIconType.HouseFill,TextColor = "orange")]
	public float room_padding = 0.5f,room_padding2 = 0.5f,seeThroughChangeDuration = 5.0f,seeThroughRadius = 3.5f;

	[TabGroup("에디터 툴/tools", "실내 설정",SdfIconType.HouseFill,TextColor = "orange")]
	public AnimationCurve room_curve;
	[TabGroup("에디터 툴/tools","실내 설정",SdfIconType.HouseFill,TextColor = "orange")]
	[ListDrawerSettings(CustomAddFunction = "AddRoom",CustomRemoveElementFunction = "RemoveRoom", DraggableItems = false)]
	public List<Room_Area> Rooms = new List<Room_Area>();
	private void AddRoom()
	{
		Room_Area area = new Room_Area();
		Rooms.Add(area);
		UpdateRooms();
		
	}
	private void RemoveRoom(Room_Area area)
	{
		Rooms.Remove(area);
		UpdateRooms();
	}
	private void UpdateRooms()
	{
		for (int i = 0; i < Rooms.Count; i++)
		{
			Rooms[i].count = i+1;
			Rooms[i].title = (i+1).ToString() + "번째";
			if (!Rooms[i].created)
			{
				Rooms[i].color = Random.ColorHSV();
				Rooms[i].created = true;
			}
		}
	}
	
	
	
	private Room_Area currentRoom = null;
	public bool UpdateCurrentRoom()
	{
		if (Player.instance == null) return false;
		Vector3 playerPos = Player.instance.transform.position;
		//현재 룸 있는지 확인
		if (currentRoom != null)
		{
			bool checkX = (currentRoom.startPos.x-room_padding-room_padding2) <= playerPos.x 
			              && playerPos.x < (currentRoom.startPos.x + currentRoom.size.x+room_padding+room_padding2);
			bool checkY = (currentRoom.startPos.y-room_padding-room_padding2) <= playerPos.z 
			              && playerPos.z < (currentRoom.startPos.y + currentRoom.size.y+room_padding+room_padding2);
			if (checkX && checkY)
			{
				return false;
			}
		}
		//새로운 룸 설정
		foreach (var area in Rooms)
		{
			bool checkX = area.startPos.x-room_padding <= playerPos.x && playerPos.x < (area.startPos.x + area.size.x+room_padding);
			bool checkY = area.startPos.y-room_padding <= playerPos.z && playerPos.z < (area.startPos.y + area.size.y+room_padding);
			if (checkX && checkY)
			{
				currentRoom = area;
				return true;
			}
		}

		if (currentRoom != null)
		{
			currentRoom = null;
			return true;
		}
		currentRoom = null;
		return false;
	}
	public void UpdateSeeThrough()
	{
		if (currentRoom != null)
		{
			Vector3 playerPos = Player.instance.transform.position;
			playerPos.x = Mathf.Clamp(playerPos.x, currentRoom.startPos.x, currentRoom.startPos.x + currentRoom.size.x);
			playerPos.z = Mathf.Clamp(playerPos.z, currentRoom.startPos.y, currentRoom.startPos.y + currentRoom.size.y);
			dissolve_start.position = playerPos;
			dissolve_end.position = CamArm.instance.mainCam.transform.position;
		}
	}
	//룸 체크, see through 상태 설정
	public IEnumerator C_Room_Update()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.25f);
			if (UpdateCurrentRoom())
			{
				if (seethrough_change != null) StopCoroutine(seethrough_change);
				seethrough_change = StartCoroutine(C_SeeTrough_Change(currentRoom != null));
				Canvas_Player.instance.OnLateUpdate.AddListener(UpdateSeeThrough);
			}
			else if (currentRoom == null)
			{
				Canvas_Player.instance.OnLateUpdate.RemoveListener(UpdateSeeThrough);
			}
		}
	}

	
	private Coroutine seethrough_change;
	private IEnumerator C_SeeTrough_Change(bool activate)
	{
		float beginRadius = dissolveController.target1Radius;
		float endRadius = activate ? seeThroughRadius : 0;
		float beginOuterRing = BeautifySettings.settings.vignettingOuterRing.value;
		float endOuterRing = activate ? 0.7f : 0.5f;
		float beginFade = BeautifySettings.settings.vignettingFade.value;
		float endFade= activate ? 0.503f : 0.3f;
		
		float beginTime = Time.unscaledTime;
		float endTime = beginTime + Mathf.Abs(endRadius - dissolveController.target1Radius) * seeThroughChangeDuration;
		float ratio = 0;
		while (ratio<1)
		{
			ratio = (Time.unscaledTime-beginTime) / (endTime - beginTime);
			float _ratio = room_curve.Evaluate(ratio);
			dissolveController.target1Radius = beginRadius * (1 - _ratio) + endRadius * _ratio;
			
			BeautifySettings.settings.vignettingOuterRing.Override(Mathf.Lerp(beginOuterRing,endOuterRing,ratio));
			BeautifySettings.settings.vignettingFade.Override(Mathf.Lerp(beginFade,endFade,ratio));
			yield return null;
		}

		dissolveController.target1Radius = endRadius;
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmos_Room()
	{
		Quaternion currentRot = SceneView.lastActiveSceneView.camera.transform.rotation;
		Quaternion targetRot = Quaternion.Euler(90,0,0);
		bool isRot = Quaternion.Angle(currentRot, targetRot) < 0.1f;
		if (!SceneView.lastActiveSceneView.camera.orthographic || !isRot) return;
		
		var gui = new GUIStyle();
		
		gui.fontStyle = FontStyle.Bold;
		gui.normal.textColor = Color.white;
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
	[HideInInspector] public int count;
	[HideInInspector] public string title;
	[HideInInspector] public bool created = false;
	[Title("$title")]
	[LabelText("시작 좌표")] public Vector2Int startPos;
	[LabelText("구역 크기")] public Vector2Int size = new Vector2Int(6,6);
	
	[LabelText("구역 색상")][Space(12.0f)][ColorPalette] public Color color;
}