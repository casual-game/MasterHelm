using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

public class Debug_ShadowUpdater : MonoBehaviour
{
    public ShadowCastingMode mode;
    public bool receiveShadows = true;
    [Button]
    public void Execution()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            r.shadowCastingMode = mode;
            r.receiveShadows = receiveShadows;
        }
    }
}
