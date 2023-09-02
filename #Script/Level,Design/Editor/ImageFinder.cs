using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityEngine.UI;
public class ImageFinder : MonoBehaviour
{
    #if UNITY_EDITOR
    public List<Sprite> sprites = new List<Sprite>();
    public SpriteAtlas atlas;
    [Button]
    public void UpdateSprites()
    {
        sprites.Clear();
        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (var image in images)
        {
            if (image.sprite != null && !sprites.Contains(image.sprite) && !atlas.CanBindTo(image.sprite)) sprites.Add(image.sprite);
        }
        atlas.Add(sprites.ToArray());
    }
    #endif
}
