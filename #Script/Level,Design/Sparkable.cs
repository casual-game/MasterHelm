using System;
using System.Collections;
using System.Collections.Generic;
using Dest.Math;
using Sirenix.OdinInspector;
using UnityEngine;

public class Sparkable : MonoBehaviour
{
    public static List<Sparkable> Sparkables = null;
    [ShowInInspector]
    public Box3 box;

    public void Setting2()
    {
        box = CreateBox3(transform);
        Sparkables.Add(this);
    }
    #if UNITY_EDITOR
    private Box3 gizmoBox;
    private void OnDrawGizmosSelected()
    {
        gizmoBox = CreateBox3(transform);
        DrawBox(gizmoBox);
    }
    #endif
    private void DrawBox(Box3 box)
    {
        Vector3 v0, v1, v2, v3, v4, v5, v6, v7;
        box.CalcVertices(out v0, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
        Gizmos.DrawLine(v0, v1);
        Gizmos.DrawLine(v1, v2);
        Gizmos.DrawLine(v2, v3);
        Gizmos.DrawLine(v3, v0);
        Gizmos.DrawLine(v4, v5);
        Gizmos.DrawLine(v5, v6);
        Gizmos.DrawLine(v6, v7);
        Gizmos.DrawLine(v7, v4);
        Gizmos.DrawLine(v0, v4);
        Gizmos.DrawLine(v1, v5);
        Gizmos.DrawLine(v2, v6);
        Gizmos.DrawLine(v3, v7);
    }
    private Box3 CreateBox3(Transform box)
    {
        return new Box3(box.position, box.right, box.up, box.forward, box.lossyScale);
    }
}
