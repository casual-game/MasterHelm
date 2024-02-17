using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MasterHelm_TestCode : MonoBehaviour
{
    public Transform point;
    public Transform target;
    [Button]
    public void LookCam()
    {
        target.rotation = point.rotation;
        target.position = point.position;
    }
    [Button]
    public void MountRot()
    {
        float dist = 1.5f;
        target.rotation = point.rotation * Quaternion.Euler(0,135,0);
        target.position = point.position + point.rotation * Quaternion.Euler(0, 45, 0)*Vector3.back*dist;
    }
}
