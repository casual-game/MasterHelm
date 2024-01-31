using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CustomTilemap : MonoBehaviour
{
    public Transform coastT;
    public Transform deco;
    [Button]
    public void UpdateTilemap()
    {
        foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>(true))
        {
            int additional =  0;
            if (spriteRenderer.transform.parent == coastT) additional = -1;
            else if (spriteRenderer.transform.parent == deco) additional = 225;
            spriteRenderer.sortingOrder = Mathf.FloorToInt(-spriteRenderer.transform.localPosition.y * 100)+additional;
        }
    }
}
