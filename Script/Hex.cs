using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Hex : MonoBehaviour
{
    [Button]
    public void Test()
    {
        transform.GetChild(transform.childCount-1).GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
    }
}
