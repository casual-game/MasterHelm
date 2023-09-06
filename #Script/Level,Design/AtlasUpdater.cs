using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.U2D;
using UnityEditor.U2D;
#endif
using UnityEngine.UI;
public class AtlasUpdater : MonoBehaviour
{
    #if UNITY_EDITOR
    public List<Sprite> excludeList = new List<Sprite>();
    public SpriteAtlas atlas;
    [Button]
    public void UpdateSprites()
    {
        List<Sprite> sprites = new List<Sprite>();
        Image[] images = FindObjectsOfType<Image>(true);
        foreach (var image in images)
        {
            if (image.sprite != null && !sprites.Contains(image.sprite) && !atlas.CanBindTo(image.sprite) 
                && !excludeList.Contains(image.sprite)) sprites.Add(image.sprite);
        }
        atlas.Add(sprites.ToArray());
    }
    #endif
}
