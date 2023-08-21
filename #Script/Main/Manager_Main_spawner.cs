using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
//using Beautify.Universal;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Manager_Main : MonoBehaviour
{
	//public
	[TitleGroup("에디터 툴")]
	[TabGroup("에디터 툴/tools", "레벨 설정",SdfIconType.MapFill,TextColor = "green")]
	[LabelText("시작 바리케이드")] public Barricade startBarricade;
	[TabGroup("에디터 툴/tools", "레벨 설정", SdfIconType.MapFill, TextColor = "green")]
	[LabelText("적 생성 레이어")] public LayerMask enemyCreateDetectLayer;
	[TabGroup("에디터 툴/tools", "레벨 설정",SdfIconType.MapFill,TextColor = "green")][OnValueChanged("UpdateData")] 
	[LabelText("시작 타일")] public Vector2Int startTile = new Vector2Int(0, 0);
	[TabGroup("에디터 툴/tools", "레벨 설정",SdfIconType.MapFill,TextColor = "green")][OnValueChanged("UpdateData")] 
	[LabelText("타일 크기")] public Vector2Int tileSize = new Vector2Int(2, 2);
	[TabGroup("에디터 툴/tools", "레벨 설정",SdfIconType.MapFill,TextColor = "green")][OnValueChanged("UpdateData")] 
	[LabelText("레벨")][Space(32)][OnCollectionChanged("UpdateData")]
	public List<Area> areas = new List<Area>();
	//메인 코드
	[ReadOnly] public int areaIndex = 0;
	public void Setting_Spawner()
	{
		areaIndex = 0;
	}
	public void Spawner_UpdateData()//적이 죽을때,바리케이드 상태변경 등의 이벤트마다 호출되어 현 상태를 업데이트한다.
	{
		
	}
	#region Clear
	private Coroutine c_clear = null;
	public void Clear_Begin1(bool isBoss)
	{
		if(c_clear!=null) StopCoroutine(c_clear);
		if(isBoss) c_clear = StartCoroutine(C_Boss_Clear_Begin1());
		else c_clear = StartCoroutine(C_Clear_Begin1());
	}
	public void Clear_Begin2()
	{
		if(c_clear!=null) StopCoroutine(c_clear);
		c_clear = StartCoroutine(C_Clear_Begin2());
	}
	private IEnumerator C_Boss_Clear_Begin1()
	{
		SoundManager.instance.Ingame_StageClear();
		Player.instance.clear = true;
		instance.mainData.audio_Effecting_ImpactSection.Play();
		CamArm.instance.Impact(mainData.impact_Boss_Clear_Area);
		CamArm.instance.Production_Begin(1.0f);
		yield return new WaitForSecondsRealtime(3.75f);
		CamArm.instance.SpeedLine_Stop();
		CamArm.instance.Production_Fin(1.5f);
		yield return new WaitForSecondsRealtime(1.0f);
		Player.instance.audio_BossFin.Play();
		yield return new WaitForSecondsRealtime(3.25f);
		Player.instance.animator.CrossFade("Exit_Begin",0.5f,0);
		Canvas_Player.instance.anim.CrossFade("Menu_None",0,0);
		
		//CamArm.instance.StartFOVNow();
	}
	private IEnumerator C_Clear_Begin1()
	{
		SoundManager.instance.Ingame_StageClear();
		Player.instance.clear = true;
		instance.mainData.audio_Effecting_ImpactSection.Play();
		CamArm.instance.Impact(mainData.impact_Clear_Area);
		CamArm.instance.Production_Begin(1.0f);
		yield return new WaitForSecondsRealtime(2.0f);
		CamArm.instance.SpeedLine_Stop();
		CamArm.instance.Production_Fin(1.5f);
		yield return new WaitForSecondsRealtime(3.25f);
		Player.instance.animator.CrossFade("Exit_Begin",0.5f,0);
		Canvas_Player.instance.anim.CrossFade("Menu_None",0,0);
		
		//CamArm.instance.StartFOVNow();
	}
	private IEnumerator C_Clear_Begin2()
	{
		SoundManager.instance.Ingame_Result();
		CamArm.instance.Production_Fin(1.25f);
		CamArm.instance.ClearFOV(2.25f,6,0.425f);
		yield return new WaitForSeconds(0.75f);
		Text_Info_Fin();
		Canvas_Player.instance.Level_Clear();
		//NextScene();
		yield break;
	}
	
	public bool IsLastArea()
	{
		return false;
	}
	#endregion
	#region Gizmos,Editor
	public void OnDrawGizmos_Spawner()
	{
		Quaternion currentRot = SceneView.lastActiveSceneView.camera.transform.rotation;
		Quaternion targetRot = Quaternion.Euler(90,0,0);
		bool isRot = Quaternion.Angle(currentRot, targetRot) < 0.1f;
		if (!SceneView.lastActiveSceneView.camera.orthographic || !isRot) return;
		var gui = new GUIStyle();
		gui.fontStyle = FontStyle.Bold;
		gui.normal.textColor = Color.white;
		//격자 그리기
		for (int i = 0; i < tileSize.x; i++)
		{
			for (int j = 0; j < tileSize.y; j++)
			{
				Vector2Int pos = startTile*6 + new Vector2Int(i * 6, j * 6);
				Vector2Int size = new Vector2Int(6, 6);
				DrawRectangle(pos,size,Color.gray);
			}
		}
		//Area 그리기
		foreach (var area in areas)
		{
			//cell
			foreach (var cell in area.cells)
			{
				Vector2Int pos = startTile*6 + new Vector2Int(cell.x * 6, cell.y * 6);
				Vector2Int size = new Vector2Int(6, 6);
				DrawCell(gui,pos,size,area.color,area.title);
			}
			
			//spawn
			foreach (var spawn in area.spawns)
			{
				foreach (var singleSpawn in spawn.singleSpwawns)
				{
					float x = startTile.x*6+spawn.center.x + singleSpawn.x;
					float y = startTile.y*6+spawn.center.y + singleSpawn.y;
					
					DrawPoint(new Vector2(x,y),spawn.gizmosColor,singleSpawn.spawnHeight<Mathf.NegativeInfinity+1);
				}
			}
		}
	}
	public void UpdateData()
	{
		foreach (var area in areas)
		{
			area.UpdateData();
			foreach (var spawn in area.spawns)
			{
				foreach (var singleSpawner in spawn.singleSpwawns)
				{
					float x = startTile.x*6+spawn.center.x + singleSpawner.x;
					float y = startTile.y*6+spawn.center.y + singleSpawner.y;
					singleSpawner.UpdateHeight(new Vector3(x,10,y),enemyCreateDetectLayer);
				}
			}
		}
	}
	private void DrawCell(GUIStyle gui,Vector2 startPos,Vector2 startSize,Color color,string title)
	{
		gui.fontSize = Mathf.RoundToInt(450/SceneView.lastActiveSceneView.camera.transform.position.y);
		Vector3 center = new Vector3(startPos.x +startSize.x*0.5f
			, 0, startPos.y + startSize.y*0.5f);
		Vector3 size = new Vector3(startSize.x, 0, startSize.y);
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
			new Color(color.r,color.g,color.b,0.5f), Color.clear);
		Handles.DrawLine(upperLeft,upperRight,2);
		Handles.DrawLine(upperLeft,lowerLeft,2);
		Handles.DrawLine(lowerLeft,lowerRight,2);
		Handles.DrawLine(lowerRight,upperRight,2);
		//글자
		gui.normal.textColor = Color.white;
		gui.alignment = TextAnchor.UpperLeft;
		Handles.Label(upperLeft,title,gui);
	}
	private void DrawRectangle(Vector2 startPos,Vector2 startSize,Color color)
	{
		Vector3 center = new Vector3(startPos.x +startSize.x*0.5f
			, 0, startPos.y + startSize.y*0.5f);
		Vector3 size = new Vector3(startSize.x, 0, startSize.y);
		//외곽선
		Handles.color = color;
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
		float thickness = 1.2f;
		Handles.DrawLine(upperLeft,upperRight,thickness);
		Handles.DrawLine(upperLeft,lowerLeft,thickness);
		Handles.DrawLine(lowerLeft,lowerRight,thickness);
		Handles.DrawLine(lowerRight,upperRight,thickness);
	}
	private void DrawPoint(Vector2 _center,Color _color,bool hasValue)
	{
		float _size = 0.75f;
		float height = 5.0f;
		Vector3 center = new Vector3(_center.x,0,_center.y);
		Vector3 size = new Vector3(_size, 0, _size);
		//외곽선
		Handles.color = Color.white;
		Vector3 upperLeft = center + new Vector3(-size.x * 0.5f, height, size.z * 0.5f);
		Vector3 middleLeft = center + new Vector3(-size.x * 0.5f, height, 0);
		Vector3 upperRight = center + new Vector3(size.x * 0.5f, height, size.z * 0.5f);
		Vector3 lowerLeft = center + new Vector3(-size.x * 0.5f, height, -size.z * 0.5f);
		Vector3 lowerRight = center + new Vector3(size.x * 0.5f, height, -size.z * 0.5f);
		Vector3[] verts = new Vector3[]
		{
			lowerLeft,
			upperLeft,
			upperRight,
			lowerRight
		};
		Handles.DrawSolidRectangleWithOutline(verts, 
			new Color(_color.r,_color.g,_color.b,0.5f), Color.clear);
		float thickness = 2.0f;
		Handles.color = hasValue?Color.white:Color.red;
		Handles.DrawLine(upperLeft,upperRight,thickness);
		Handles.DrawLine(upperLeft,lowerLeft,thickness);
		Handles.DrawLine(lowerLeft,lowerRight,thickness);
		Handles.DrawLine(lowerRight,upperRight,thickness);
	}
	#endregion
	#region Scene

	private IEnumerator LoadScene(string sceneName)
	{
		var mAsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		yield return mAsyncOperation;
	}
	

	#endregion
}
[System.Serializable]
public class Area
{
	[TitleGroup("$title","$subtitle")][HideLabel][HorizontalGroup("$title/maindata")]
	public string title = "Title";
	private string subtitle()
	{
		return "셀 개수: " + cells.Count;
	}
	[TitleGroup("$title","$subtitle")][ColorPalette(PaletteName = "Country")][HideLabel][HorizontalGroup("$title/maindata")]
	public Color color;
	//셀 리스트
	[ListDrawerSettings(AddCopiesLastElement =  true,Expanded = false)][OnValueChanged("UpdateData")][OnCollectionChanged("UpdateData")]
	[LabelText("셀 리스트",SdfIconType.StickyFill,IconColor = "blue")][Space(16)][GUIColor(0.8f,0.8f,1.0f)]
	public List<Cell> cells = new List<Cell>();
	
	public void UpdateData()
	{
		foreach (var cell in cells)
		{
			cell.UpdateData();
		}
		foreach (var spawn in spawns)
		{
			spawn.UpdateData(cells);
		}
	}
	//스폰 리스트
	[ListDrawerSettings(AddCopiesLastElement =  true,Expanded = false)][OnValueChanged("UpdateData")][OnCollectionChanged("UpdateData")]
	[LabelText("스폰 리스트",SdfIconType.PersonFill,IconColor = "yellow")][GUIColor(1.0f,1.0f,0.8f)]
	public List<Spawn> spawns = new List<Spawn>();
	//씬 리스트
	[ListDrawerSettings(AddCopiesLastElement =  true,Expanded = false)]
	[LabelText("씬 리스트",SdfIconType.MapFill,IconColor = "red")][GUIColor(1.0f,0.8f,0.8f)]
	public List<SceneAsset> scenes = new List<SceneAsset>();

	
}
[System.Serializable]
public class Cell
{
	[HideInInspector] public int maxX,maxY;
	[PropertyRange(0,"maxX")] public int x;
	[PropertyRange(0,"maxY")] public int y;

	public void UpdateData()
	{
		Manager_Main main = GameObject.FindObjectOfType<Manager_Main>();
		if (main == null) return;
		maxX = main.tileSize.x-1;
		maxY = main.tileSize.y-1;
	}
}

[System.Serializable]
public class Spawn
{
	[HideInInspector] public Vector2 center;
	[HideInInspector] public List<Cell> cells = new List<Cell>();
	
	[HideLabel][ColorPalette("RGB")] public Color gizmosColor;
	[LabelText("전체 딜레이")] public float delay;
	[ListDrawerSettings(AddCopiesLastElement =  true,Expanded = false)]
	[LabelText("개별 스폰")][GUIColor(1.0f,1.0f,1.0f)][OnValueChanged("UpdateData2")][OnCollectionChanged("UpdateData2")]
	public List<SingleSpawn> singleSpwawns = new List<SingleSpawn>();

	public void UpdateData(List<Cell> cells)
	{
		this.cells = cells;
		UpdateData2();
	}
	public void UpdateData2()
	{
		int minX = int.MaxValue,
			minY = int.MaxValue,
			maxX = int.MinValue,
			maxY = int.MinValue;
		
		foreach (var cell in cells)
		{
			minX = Mathf.Min(minX, cell.x*6);
			minY = Mathf.Min(minY, cell.y*6);
			maxX = Mathf.Max(maxX, (cell.x+1)*6);
			maxY = Mathf.Max(maxY, (cell.y+1)*6);
		}
		center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
		foreach (var spawner in singleSpwawns)
		{
			spawner.UpdateData(minX-center.x, minY-center.y, maxX-center.x, maxY-center.y);
		}
	}
}
[System.Serializable]
public class SingleSpawn
{
	[HideInInspector] public float minX, minY, maxX, maxY;
	[HideInInspector] public LayerMask layerMask;
	[HideInInspector] public Vector3 origin;
	
	[LabelText("프리셋")] public SpawnPreset spawnPreset;
	[LabelText("생성 X 좌표")][PropertyRange("minX","maxX")][OnValueChanged("RecalculateAll")] public float x;
	[LabelText("생성 Y 좌표")][PropertyRange("minY","maxY")][OnValueChanged("RecalculateAll")] public float y;
	[LabelText("딜레이")] public float delay;
	[ReadOnly][ShowInInspector][LabelText("스폰 높이")] public float spawnHeight;
	
	public void UpdateData(float minX,float minY ,float maxX, float maxY)
	{
		this.minX = minX;
		this.minY = minY;
		this.maxX = maxX;
		this.maxY = maxY;
	}
	public void UpdateHeight(Vector3 origin,LayerMask layerMask)
	{
		RaycastHit hit;
		Physics.Raycast(origin, Vector3.down, out hit, 100, layerMask);
		if (hit.collider == null) spawnHeight = Mathf.NegativeInfinity;
		else spawnHeight = hit.point.y;
	}
	public void RecalculateAll()
	{
		Manager_Main main = GameObject.FindObjectOfType<Manager_Main>();
		if (main == null) return;
		main.UpdateData();
	}
}
