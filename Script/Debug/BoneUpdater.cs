using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoneUpdater : MonoBehaviour
{
    [Button]
    public void Bone()
    {
        var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var smr in skinnedMeshRenderers)
        {
            //smr.bones = smr.rootBone;
        }
    }
}
