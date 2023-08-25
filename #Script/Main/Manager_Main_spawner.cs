using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
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
	[Sirenix.OdinInspector.ReadOnly] public int areaIndex = 0;
	public void Setting_Spawner()
	{
		areaIndex = 0;
		Dictionary<EnemyRoot, int> enemiesCount = new Dictionary<EnemyRoot, int>();
		Dictionary<Prefab_Prop, int> propsCount = new Dictionary<Prefab_Prop, int>();
		//준비할 개수 구하기
		for (int i = 0; i < areas.Count-1; i++)
		{
			foreach (var spawn in areas[i].spawns)
			{
				for (int j = 0; j < spawn.singleSpwawns.Count-1; j++)
				{
					Dictionary<EnemyRoot, int> _enemiesCount = new Dictionary<EnemyRoot, int>();
					Dictionary<Prefab_Prop, int> _propsCount = new Dictionary<Prefab_Prop, int>();
					
					SingleSpawn ss1 = spawn.singleSpwawns[j],ss2 = spawn.singleSpwawns[j+1];
					EnemyRoot er1 = ss1.spawnPreset.root, er2 = ss2.spawnPreset.root;
					Prefab_Prop pp1 = ss1.spawnPreset.weaponL,
								pp2 = ss1.spawnPreset.weaponR,
								pp3 = ss1.spawnPreset.shield,
								pp4 = ss2.spawnPreset.weaponL,
								pp5 = ss2.spawnPreset.weaponR,
								pp6 = ss2.spawnPreset.shield;
					
					AddEnemy(er1); AddEnemy(er2);
					AddProp(pp1); AddProp(pp2); AddProp(pp3); AddProp(pp4); AddProp(pp5); AddProp(pp6);
					void AddEnemy(EnemyRoot er)
					{
						if (_enemiesCount.ContainsKey(er)) _enemiesCount[er] = _enemiesCount[er] + 1;
						else _enemiesCount[er] = 1;
					}
					void AddProp(Prefab_Prop pp)
					{
						if (pp != null)
						{
							if (_propsCount.ContainsKey(pp)) _propsCount[pp] = _propsCount[pp] + 1;
							else _propsCount[pp] = 1;
						}
					}

					foreach (var key in _enemiesCount.Keys)
					{
						if (enemiesCount.ContainsKey(key))
							enemiesCount[key] = Mathf.Max(enemiesCount[key], _enemiesCount[key]);
						else enemiesCount.Add(key,_enemiesCount[key]);
					}
					foreach (var key in _propsCount.Keys)
					{
						if (propsCount.ContainsKey(key))
							propsCount[key] = Mathf.Max(propsCount[key], _propsCount[key]);
						else propsCount.Add(key,_propsCount[key]);
					}
				}
			}
		}
		//pool에 등록
		foreach (var enemyPair in enemiesCount)
		{
			for (int i = 0; i < enemyPair.Value; i++)
			{
				manager_Enemy.AddEnemy(enemyPair.Key);
			}
			//print(enemyPair.Key.gameObject.name+": "+enemyPair.Value);
		}
		foreach (var propPair in propsCount)
		{
			for (int i = 0; i < propPair.Value; i++)
			{
				manager_Enemy.AddProp(propPair.Key);
			}
			//print(propPair.Key.gameObject.name+": "+propPair.Value);
		}
		
	}
	
	
	#region 흐름 설정(외부 호출은 전부 여기서)

	private bool lastSpawner = false;
	private List<EnemyRoot> spawnedEnemyRoots = new List<EnemyRoot>();
	public void Spawner_GameStart()
	{
		CamArm.instance.StartFOV();
		CamArm.instance.Cutscene(2.5f,0.2f,1.75f,startBarricade.pointT,startBarricade);
		Canvas_Player.instance.audio_EnterFin.Play();
		SoundManager.instance.Ingame_Main();
		Player.instance.audio_Ready.Play();
		Spawner_AreaStart();
	}
	public void Spawner_AreaStart()//바리케이드 닫히면 호출. 바리케이드 없을 경우 그냥 호출
	{
		if(c_spawner_areastart!=null) StopCoroutine(c_spawner_areastart);
		c_spawner_areastart = StartCoroutine(C_Spawner_AreaStart());
	}

	private Coroutine c_spawner_areastart = null;
	private IEnumerator C_Spawner_AreaStart()
	{
		Area currentArea = areas[areaIndex];
		while (true)
		{
			Transform playerT = Player.instance.transform;
			float playerX = playerT.position.x;
			float playerY = playerT.position.z;
			
			bool check = false;
			foreach (var c in currentArea.cells)
			{
				bool insideX = (startTile.x+c.x) * 6 <= playerX && playerX < (1 + startTile.x+ c.x) * 6;
				bool insideY = (startTile.y+c.y) * 6 <= playerY && playerY < (1 + startTile.y+c.y) * 6;

				if (insideX && insideY)
				{
					check = true;
					break;
				}
			}

			if (check) break;
			//print("Waiting for Player...");
			yield return null;
		}
		//print("Player founded!");
		for (int i = 0; i < currentArea.spawns.Count; i++)
		{
			Spawn spawn = currentArea.spawns[i];
			yield return new WaitForSeconds(spawn.delay);
			foreach (var singleSpawn in spawn.singleSpwawns)
			{
				Spawner_Spawn(spawn,singleSpawn);
			}
			lastSpawner = currentArea.spawns.Count - 1 <= i;
			while (spawnedEnemyRoots.Count > 0) yield return null;
			lastSpawner = false;
		}
		print("ALL SPAWNED!");
	}

	private void Spawner_Spawn(Spawn spawn,SingleSpawn singleSpawn)
	{
		EnemyRoot root = manager_Enemy.GetEnemy(singleSpawn.spawnPreset.root);
		float x = startTile.x*6+spawn.center.x + singleSpawn.x;
		float y = startTile.y*6+spawn.center.y + singleSpawn.y;

		Vector3 pos = new Vector3(x, singleSpawn.spawnHeight, y);
		Transform rootT = root.transform;
		rootT.SetPositionAndRotation(pos,Quaternion.identity);
		Prefab_Prop weaponL = singleSpawn.spawnPreset.weaponL == null?null: manager_Enemy.GetProp(singleSpawn.spawnPreset.weaponL);
		Prefab_Prop weaponR = singleSpawn.spawnPreset.weaponR == null?null: manager_Enemy.GetProp(singleSpawn.spawnPreset.weaponR);
		Prefab_Prop shield = singleSpawn.spawnPreset.shield == null?null: manager_Enemy.GetProp(singleSpawn.spawnPreset.shield);
		
		root.Enable(weaponL,weaponR,shield,singleSpawn.delay,singleSpawn.spawnPreset.root);
		spawnedEnemyRoots.Add(root);
	}
	[Button]
	public void Spawner_MoveNext(bool isBoss)
	{
		if (areas.Count - 1 < areaIndex) return;
		else if (areas.Count - 1 == areaIndex)
		{
			if(isBoss) Clear_Begin1(true);
			else Clear_Begin1(false);
			CamArm.instance.SpeedLine_Play(true);
			areaIndex += 1;
			return;
		}
		else
		{
			if (areas[areaIndex].exitBarricade != null)
			{
				CamArm.instance.Cutscene(2.75f,0.2f,1.75f,
					areas[areaIndex].exitBarricade.pointT,areas[areaIndex].exitBarricade);
				CamArm.instance.SpeedLine_Play(false);
				CamArm.instance.Impact(mainData.impact_Clear_Area);
				CamArm.instance.Production_Begin(1.0f);
				Player.instance.audio_AreaClear.Play();
				//print("clear");
			}
			else
			{
				CamArm.instance.SpeedLine_Play(false);
				CamArm.instance.Impact(mainData.impact_SpecialHit);
				//print("next!");
			}
			areaIndex += 1;
			Spawner_AreaStart();
		}
		

	}
	public void Spawner_EnemyKill(bool isBoss,Enemy enemy)
	{
		if (spawnedEnemyRoots.Contains(enemy.root)) spawnedEnemyRoots.Remove(enemy.root);
		//구역 클리어, 올클리어 
		if (Enemy.enemies.Count == 0 && lastSpawner)
		{
			if(IsLastArea())Text_Info("CLEAR");
			Spawner_MoveNext(isBoss);
		}
		//그냥 처치
		else
		{
			Text_Damage_Main();
			Text_Damage_Specific("execute");
			CamArm.instance.SpeedLine_Play(false);
			CamArm.instance.Impact(mainData.impact_SpecialHit);
			print("normal");
		}
		UpdateData();
	}
	private bool IsLastArea()
	{
		return areas.Count - 1 <= areaIndex;
	}
	#endregion
	
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
	[LabelText("나가는 문")] public Barricade exitBarricade;
	//셀 리스트
	[ListDrawerSettings(AddCopiesLastElement =  true,Expanded = false)][OnValueChanged("UpdateData")][OnCollectionChanged("UpdateData")]
	[LabelText("셀 리스트",SdfIconType.StickyFill,IconColor = "blue")][Space(16)][GUIColor(0.8f,0.8f,1.0f)]
	public List<Cell> cells = new List<Cell>();
	
	
	//스폰 리스트
	[ListDrawerSettings(AddCopiesLastElement =  true,Expanded = false)][OnValueChanged("UpdateData")][OnCollectionChanged("UpdateData")]
	[LabelText("스폰 리스트",SdfIconType.PersonFill,IconColor = "yellow")][GUIColor(1.0f,1.0f,0.8f)]
	public List<Spawn> spawns = new List<Spawn>();
	//씬 리스트
	[ListDrawerSettings(AddCopiesLastElement =  true,Expanded = false)][OnValueChanged("UpdateData")][OnCollectionChanged("UpdateData")]
	[LabelText("씬 리스트",SdfIconType.MapFill,IconColor = "red")][GUIColor(1.0f,0.8f,0.8f)]
	public List<SceneAsset> scenes = new List<SceneAsset>();

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
	[Sirenix.OdinInspector.ReadOnly][ShowInInspector][LabelText("스폰 높이")] public float spawnHeight;
	
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

