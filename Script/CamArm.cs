using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamArm : MonoBehaviour
{
    public static CamArm instance;
    public Camera mainCam;
    public Transform target;
    public float moveSpeed = 3.0f;
    public Vector3 addVec;
    // Update is called once per frame

    private void Awake()
    {
        instance = this;
        mainCam = GetComponentInChildren<Camera>();
        transform.position = target.position + addVec;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,target.position + addVec,moveSpeed*Time.deltaTime);
    }
}
