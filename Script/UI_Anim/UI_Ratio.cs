using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class UI_Ratio : MonoBehaviour
{
    public RectTransform myRectT, parentRectT;
    [Button]
    public void FitUI()
    {
        var rect = parentRectT.rect;
        float radius = Mathf.Min(rect.width, rect.height);
        myRectT.sizeDelta = Vector2.one*radius*1.75f;
    }
}
