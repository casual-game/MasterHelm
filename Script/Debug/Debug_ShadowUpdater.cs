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
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>(true);
        foreach (var r in mrs)
        {
            r.shadowCastingMode = mode;
            r.receiveShadows = receiveShadows;
        }
        SkinnedMeshRenderer[] smrs = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var r in smrs)
        {
            r.shadowCastingMode = mode;
            r.receiveShadows = receiveShadows;
        }
    }
}
