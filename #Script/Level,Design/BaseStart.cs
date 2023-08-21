using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[ExecuteAlways] public class BaseStart : MonoBehaviour
{
    public Color gizmoColor = Color.white*0.9f;
    void OnDrawGizmos(){
        Gizmos.color = gizmoColor;
        
        Gizmos.DrawCube(transform.position+Vector3.up*0.625f,new Vector3(0.75f,1.25f,0.75f));
        Gizmos.DrawCube(transform.position+Vector3.up*0.8f+transform.forward*0.5f,
            new Vector3(0.75f*0.25f,1.25f*0.25f,0.75f*0.25f));
        Gizmos.DrawSphere(transform.position+Vector3.up*1.75f,0.5f);
    }
}
