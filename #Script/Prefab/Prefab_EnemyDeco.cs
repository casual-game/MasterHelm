using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefab_EnemyDeco : MonoBehaviour
{
    private Transform attachT;

    public void Setting(Transform parent)
    {
        attachT = transform.Find("attachT");
        transform.SetParent(parent);
        transform.localPosition = attachT.localPosition;
        transform.localRotation = attachT.localRotation;
        transform.localScale = attachT.localScale;
    }
}
