using System.Collections;
using System.Collections.Generic;
using GPUInstancer;
using LeTai.TrueShadow;
using Sirenix.OdinInspector;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MasterHelm_DevTool : MonoBehaviour
{
    #if UNITY_EDITOR
    [ToggleGroup("use_0",0,"GPUInstanceRemover")]public bool use_0 = false;
    [ToggleGroup("use_0",0,"GPUInstanceRemover")][Button]
    public void GPUInstanceRemover()
    {
        var array = GetComponentsInChildren<GPUInstancerPrefab>();
        for (int i = 0; i < array.Length; i++)
        {
            print(array[i].gameObject.name);
            DestroyImmediate(array[i]);
        }
    }
    [ToggleGroup("use_1",0,"TrueShadowFinder")]public bool use_1 = false;
    [ToggleGroup("use_1",0,"TrueShadowFinder")][Button]
    public void CanvasShadowFinder()
    {
        var array = GetComponentsInChildren<TrueShadow>();
        foreach (var arr in array)
        {
            print(arr.transform.parent.gameObject.name + " -> " + arr.gameObject.name);
        }
    }
    [ToggleGroup("use_2",0,"AtlasUpdater")] public bool use_2 = false;
    [ToggleGroup("use_2",0,"AtlasUpdater")] public List<Sprite> excludeList = new List<Sprite>();
    [ToggleGroup("use_2",0,"AtlasUpdater")] public List<GameObject> searchList = new List<GameObject>();
    [ToggleGroup("use_2",0,"AtlasUpdater")] public SpriteAtlas atlas;
    [ToggleGroup("use_2",0,"AtlasUpdater")] [Button]
    public void UpdateSprites()
    {
        List<Sprite> sprites = new List<Sprite>();
        List<Image> images = new List<Image>();
        foreach (var search in searchList)
        {
            var searchData = search.GetComponentsInChildren<Image>();
            if(searchData!= null && searchData.Length>0) images.AddRange(searchData);
        }
        foreach (var image in images)
        {
            if (image.sprite != null && !sprites.Contains(image.sprite) && !atlas.CanBindTo(image.sprite) 
                && !excludeList.Contains(image.sprite)) sprites.Add(image.sprite);
        }
        atlas.Add(sprites.ToArray());
    }
    [ToggleGroup("use_3", 0, "DeactivateRaycastTarget")] public bool use_3 = false;
    [ToggleGroup("use_3", 0, "DeactivateRaycastTarget")] [Button]
    public void DeactivateRaycastTarget()
    {
        int count = 0;
        foreach (var image in GetComponentsInChildren<Image>(true))
        {
            count++;
            image.raycastTarget = false;
        }
        print(count+"개의 이미지의 RaycastTarget 비활성화!");
    }
    #endif
}
