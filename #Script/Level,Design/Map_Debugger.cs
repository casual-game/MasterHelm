using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class Map_Debugger : MonoBehaviour
{
    //빌드 관리
    [TabGroup("tools", "빌드 관리", SdfIconType.HouseFill, TextColor = "blue")][LabelText("실시간 로딩(o),상호작용(o)")]
    [TitleGroup("tools/빌드 관리/폴더 설정")]
    public GameObject f_canload_main;
    [TabGroup("tools", "빌드 관리", SdfIconType.HouseFill, TextColor = "blue")][LabelText("실시간 로딩(o),상호작용(x)")]
    [TitleGroup("tools/빌드 관리/폴더 설정")]
    public GameObject f_canload_etc;
    [TabGroup("tools", "빌드 관리", SdfIconType.HouseFill, TextColor = "blue")][LabelText("파괴 오브젝트,나무")]
    [TitleGroup("tools/빌드 관리/폴더 설정")]
    public GameObject f_canload_etc_culling;
    [TabGroup("tools", "빌드 관리", SdfIconType.HouseFill, TextColor = "blue")][LabelText("Room")]
    [TitleGroup("tools/빌드 관리/폴더 설정")]
    public GameObject f_canload_etc_room;
    [TabGroup("tools", "빌드 관리",SdfIconType.HouseFill,TextColor = "blue")][LabelText("로딩 없이 항상 필요")]
    [TitleGroup("tools/빌드 관리/폴더 설정")]
    public GameObject f_alwaysloaded;

    [TitleGroup("tools/빌드 관리/세부 설정")][LabelText("기본 레이어")]
    public string defaultLayer = "Default";
    [TitleGroup("tools/빌드 관리/세부 설정")][LabelText("상호작용 레이어")]
    public string mapLayer = "Map";
    [TitleGroup("tools/빌드 관리/세부 설정")][LabelText("완전 스태틱")]
    public StaticEditorFlags static_main;
    [TitleGroup("tools/빌드 관리/세부 설정")][LabelText("파괴,나무 스태틱")]
    public StaticEditorFlags static_culling;
    [TitleGroup("tools/빌드 관리/세부 설정")][LabelText("룸 스태틱")]
    public StaticEditorFlags static_room;
    [TitleGroup("tools/빌드 관리/세부 설정")][LabelText("카피 스태틱")]
    public StaticEditorFlags static_copy;
    
    [TabGroup("tools", "빌드 관리",SdfIconType.HouseFill,TextColor = "blue")][Button][GUIColor("yellow")]
    [InfoBox("빌드 후에 수동으로 Water Layer설정을 해주어야 합니다!",InfoMessageType.Warning)]
    public void Build()
    {
        f_alwaysloaded.transform.SetParent(null);
        
        GameObject folder = new GameObject("Build");
        folder.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);
        folder.transform.localScale = Vector3.one;
        //각종 컴포넌트들 copy
        foreach (var area in buildAreas)
        {
	        GameObject root = CreateSubFolder(area.title, folder.transform);
	        GameObject mesh = CreateSubFolder("mesh",root.transform);
	        GameObject collider = CreateSubFolder("collider",root.transform);
	        GameObject sparkable = CreateSubFolder("sparkable",root.transform);
	        GameObject culling = CreateSubFolder("culling",root.transform);
	        GameObject room = CreateSubFolder("room",root.transform);
	        GameObject etc = CreateSubFolder("etc",root.transform);
	        
	        Copy_F_CanLoad_Main(mesh.transform,f_canload_main.GetComponentsInChildren<MeshRenderer>(false),area,true);
	        Copy_F_CanLoad_Main(collider.transform,f_canload_main.GetComponentsInChildren<BoxCollider>(false),area);
	        Copy_F_CanLoad_Main(sparkable.transform,f_canload_main.GetComponentsInChildren<Sparkable>(false),area);
	        Copy_F_CanLoad_Etc_Culling(culling.transform,area);
	        Copy_F_CanLoad_Etc_Room(room.transform,area);
	        Copy_F_CanLoad_Etc(etc.transform,area);
        }
        //복사 폴더 내부의 요소들 깊은 복사
        GameObject newFolder = Instantiate(f_alwaysloaded);
        newFolder.transform.SetParent(null);
        newFolder.transform.position = f_alwaysloaded.transform.position;
        newFolder.transform.rotation = f_alwaysloaded.transform.rotation;
        newFolder.transform.localScale = f_alwaysloaded.transform.lossyScale;
        newFolder.transform.SetParent(folder.transform);
        newFolder.name = "Copy";
        f_alwaysloaded.transform.SetParent(transform);
        
        GameObject CreateSubFolder(string folderName,Transform parent)
        {
            GameObject subFolder = new GameObject(folderName);
            subFolder.transform.SetParent(parent);
            subFolder.transform.localPosition = Vector3.zero;
            subFolder.transform.localRotation = Quaternion.identity;
            subFolder.transform.localScale = Vector3.one;
            return subFolder;
        }
        void Copy_F_CanLoad_Main(Transform folder, Component[] components,DebugArea area,bool useCenter = false)
        {
            foreach (var component in components)
            {
	            Vector3 pos = useCenter?getCenter(component.gameObject.transform):component.gameObject.transform.position;
	            bool insideX = area.range_x.x * 6 <= pos.x && pos.x < area.range_x.y * 6;
	            bool insideY = area.range_y.x * 6 <= pos.z && pos.z < area.range_y.y * 6;
	            if(!insideX || !insideY) continue;
	            
                GameObject g = Instantiate(component.gameObject);
                g.transform.SetParent(null);
                g.transform.localScale = component.transform.lossyScale;
                g.transform.position = component.transform.position;
                g.transform.rotation = component.transform.rotation;
                g.name = component.gameObject.name;
                g.transform.SetParent(folder);
            }
        }
        void Copy_F_CanLoad_Etc(Transform folder,DebugArea area)
        {
	        foreach (Transform t_1 in f_canload_etc.transform)
	        {
		        foreach (Transform gameobjT in t_1)
		        {
			        if(!gameobjT.gameObject.activeSelf) continue;
			        Vector3 pos = gameobjT.gameObject.transform.position;
			        bool insideX = area.range_x.x * 6 <= pos.x && pos.x < area.range_x.y * 6;
			        bool insideY = area.range_y.x * 6 <= pos.z && pos.z < area.range_y.y * 6;
			        if(!insideX || !insideY) continue;
	            
			        GameObject g = Instantiate(gameobjT.gameObject);
			        g.transform.SetParent(null);
			        g.transform.localScale = gameobjT.lossyScale;
			        g.transform.position = gameobjT.position;
			        g.transform.rotation = gameobjT.rotation;
			        g.name = gameobjT.gameObject.name;
			        g.transform.SetParent(folder);
		        }
	        }
        }
        void Copy_F_CanLoad_Etc_Culling(Transform folder,DebugArea area)
        {
	        foreach (Transform t_1 in f_canload_etc_culling.transform)
	        {
		        foreach (Transform gameobjT in t_1)
		        {
			        if(!gameobjT.gameObject.activeSelf) continue;
			        Vector3 pos = gameobjT.gameObject.transform.position;
			        bool insideX = area.range_x.x * 6 <= pos.x && pos.x < area.range_x.y * 6;
			        bool insideY = area.range_y.x * 6 <= pos.z && pos.z < area.range_y.y * 6;
			        if(!insideX || !insideY) continue;
	            
			        GameObject g = Instantiate(gameobjT.gameObject);
			        g.transform.SetParent(null);
			        g.transform.localScale = gameobjT.lossyScale;
			        g.transform.position = gameobjT.position;
			        g.transform.rotation = gameobjT.rotation;
			        g.name = gameobjT.gameObject.name;
			        g.transform.SetParent(folder);
		        }
	        }
        }
        void Copy_F_CanLoad_Etc_Room(Transform folder,DebugArea area)
        {
	        foreach (Transform t_1 in f_canload_etc_room.transform)
	        {
		        foreach (Transform gameobjT in t_1)
		        {
			        if(!gameobjT.gameObject.activeSelf) continue;
			        Vector3 pos = gameobjT.gameObject.transform.position;
			        bool insideX = area.range_x.x * 6 <= pos.x && pos.x < area.range_x.y * 6;
			        bool insideY = area.range_y.x * 6 <= pos.z && pos.z < area.range_y.y * 6;
			        if(!insideX || !insideY) continue;
	            
			        GameObject g = Instantiate(gameobjT.gameObject);
			        g.transform.SetParent(null);
			        g.transform.localScale = gameobjT.lossyScale;
			        g.transform.position = gameobjT.position;
			        g.transform.rotation = gameobjT.rotation;
			        g.name = gameobjT.gameObject.name;
			        g.transform.SetParent(folder);
		        }
	        }
        }
        //레이어
        List<GameObject> cullingList = new List<GameObject>();
        List<GameObject> roomList = new List<GameObject>();
        foreach (var t in folder.GetComponentsInChildren<Transform>())
        {
	        GameObjectUtility.SetStaticEditorFlags(t.gameObject,static_main);
	        
	        if (t.gameObject.name.Contains("메인")) t.gameObject.layer = LayerMask.NameToLayer(mapLayer);
	        else t.gameObject.layer = LayerMask.NameToLayer(defaultLayer);
	        if(t.gameObject.name == "culling" && !cullingList.Contains(t.gameObject)) cullingList.Add(t.gameObject);
	        if(t.gameObject.name == "room" && !roomList.Contains(t.gameObject)) roomList.Add(t.gameObject);
        }
        foreach (var c in cullingList)
        {
	        foreach (var t in c.GetComponentsInChildren<Transform>())
	        {
		        GameObjectUtility.SetStaticEditorFlags(t.gameObject,static_culling);
		        t.gameObject.layer = LayerMask.NameToLayer(defaultLayer);
	        }
        }
        foreach (var c in roomList)
        {
	        foreach (var t in c.GetComponentsInChildren<Transform>())
	        {
		        GameObjectUtility.SetStaticEditorFlags(t.gameObject,static_room);
		        t.gameObject.layer = LayerMask.NameToLayer(defaultLayer);
	        }
        }
        foreach (var t in folder.transform.Find("Copy").GetComponentsInChildren<Transform>())
        {
	        GameObjectUtility.SetStaticEditorFlags(t.gameObject,static_copy);
	        t.gameObject.layer = LayerMask.NameToLayer(defaultLayer);
        }
        Transform[] ts = folder.GetComponentsInChildren<Transform>(true);
        for (int i = ts.Length-1; i >=0 ; i--)
        {
	        if(ts[i]==null) continue;
	        GameObject g = ts[i].gameObject;
	        if (!g.activeInHierarchy)DestroyImmediate(g);
        }
    }
    Vector3 getCenter(Transform obj)
    {
	    Vector3 center = new Vector3();
	    if (obj.GetComponent<Renderer>() != null)
	    {
		    center = obj.GetComponent<Renderer>().bounds.center;
	    }
	    else
	    {
		    foreach (Transform subObj in obj)
		    {
			    center += getCenter(subObj);
		    }
		    center /= obj.childCount;
	    }
	    return center;
    }
    
    //구역 관리
    [TabGroup("tools", "구역 관리", SdfIconType.MapFill, TextColor = "orange")]
    [LabelText("타일 시작 위치")] public Vector2Int tile_begin;
    [TabGroup("tools", "구역 관리", SdfIconType.MapFill, TextColor = "orange")]
    [LabelText("타일 크기")] public Vector2Int tile_size = new Vector2Int(6,6);
    [TabGroup("tools", "구역 관리",SdfIconType.MapFill,TextColor = "orange")][PropertySpace(16)]
    [LabelText("빌드 구역 설정")] public List<DebugArea> buildAreas = new List<DebugArea>();
    
    //머티리얼 관리
    [TabGroup("tools", "머티리얼 관리",SdfIconType.ImageFill,TextColor = "green")]
    public List<Material> materials;
    [TabGroup("tools", "머티리얼 관리",SdfIconType.ImageFill,TextColor = "green")][Button][GUIColor("yellow")]
    public void FindMaterial()
    {
        materials = new List<Material>();
        foreach (var m in FindObjectsOfType<MeshRenderer>())
        {
            if(!m.gameObject.activeSelf) continue;
            if(!materials.Contains(m.sharedMaterial)) materials.Add(m.sharedMaterial);
        }
        foreach (var m in FindObjectsOfType<SkinnedMeshRenderer>())
        {
            if(!m.gameObject.activeSelf) continue;
            if(!materials.Contains(m.sharedMaterial)) materials.Add(m.sharedMaterial);
        }
    }
    
    //기즈모
    public void OnDrawGizmos()
    {
	    Quaternion currentRot = SceneView.lastActiveSceneView.camera.transform.rotation;
	    Quaternion targetRot = Quaternion.Euler(90,0,0);
	    bool isRot = Quaternion.Angle(currentRot, targetRot) < 0.1f;
	    if (!SceneView.lastActiveSceneView.camera.orthographic || !isRot) return;
	    var gui = new GUIStyle();
	    gui.fontStyle = FontStyle.Bold;
	    gui.normal.textColor = Color.white;
	    //격자 그리기
	    for (int i = 0; i < tile_size.x; i++)
	    {
		    for (int j = 0; j < tile_size.y; j++)
		    {
			    Vector2Int pos = tile_begin*6 + new Vector2Int(i * 6, j * 6);
			    Vector2Int size = new Vector2Int(6, 6);
			    DrawRectangle(pos,size,Color.gray);
		    }
	    }
	    //구역 그리기
	    foreach (var buildArea in buildAreas)
	    {
		    Vector2Int pos = new Vector2Int(buildArea.range_x.x*6, buildArea.range_y.x*6);
		    Vector2Int size = new Vector2Int(buildArea.range_x.y * 6, buildArea.range_y.y * 6) - pos;
		    DrawCell(gui,pos,size,buildArea.debugColor,buildArea.title);
	    }
    }

	private void DrawCell(GUIStyle gui,Vector2 startPos,Vector2 startSize,Color color,string title)
	{
		gui.fontSize = Mathf.RoundToInt(900/SceneView.lastActiveSceneView.camera.transform.position.y);
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
}
[System.Serializable]
public class DebugArea
{
	[TitleGroup("$title")]
	[HorizontalGroup("$title/data")] [HideLabel]
	public string title = "NULL";
	[HorizontalGroup("$title/data")][ColorPalette(PaletteName = "Country")][HideLabel]
	public Color debugColor;
    [MinMaxSlider("BeginX","EndX")]
    public Vector2Int range_x;
    [MinMaxSlider("BeginY","EndY")]
    public Vector2Int range_y;

    

    public int BeginX()
    {
        Map_Debugger debugger = GameObject.FindObjectOfType<Map_Debugger>();
        if (debugger == null) return 0;
        return debugger.tile_begin.x;
    }
    public int BeginY()
    {
        Map_Debugger debugger = GameObject.FindObjectOfType<Map_Debugger>();
        if (debugger == null) return 0;
        return debugger.tile_begin.y;
    }
    public int EndX()
    {
        Map_Debugger debugger = GameObject.FindObjectOfType<Map_Debugger>();
        if (debugger == null) return 0;
        return debugger.tile_begin.x + debugger.tile_size.x;
    }
    public int EndY()
    {
        Map_Debugger debugger = GameObject.FindObjectOfType<Map_Debugger>();
        if (debugger == null) return 0;
        return debugger.tile_begin.y + debugger.tile_size.y;
    }
}