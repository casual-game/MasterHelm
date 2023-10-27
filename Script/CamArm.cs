using System;
using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CamArm : MonoBehaviour
{
    public float test_strength, test_duration;
    public int test_vibrato;
    public Ease test_ease1,test_ease2;
    //Public
    public static CamArm instance;
    public Camera mainCam;
    public Transform target;
    public float moveSpeed = 3.0f;
    public Vector3 addVec;
    //Private
    private Transform _camT;
    private Sequence s_stop_normal,s_shake_normal;
    private void Awake()
    {
        instance = this;
        _camT = transform.GetChild(0);
        mainCam = GetComponentInChildren<Camera>();
        transform.position = target.position + addVec;
        
        s_shake_normal = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity); })
            .Append(_camT.DOShakePosition(0.25f,0.375f,25).SetEase(Ease.OutQuint)).SetUpdate(true);
        
        s_stop_normal = DOTween.Sequence().SetAutoKill(false)
            .Append(DOTween.To(() => 0.05f, x => Time.timeScale = x, 
                1f, 0.3f).SetEase(Ease.OutQuad)).SetUpdate(true);
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,target.position + addVec,moveSpeed*Time.deltaTime);
    }
    [Button]
    public void Tween_ShakeNormal()
    {
        if(!s_shake_normal.IsInitialized()) s_shake_normal.Play();
        else s_shake_normal.Restart();


        if(!s_stop_normal.IsInitialized()) s_stop_normal.Play();
        else s_stop_normal.Restart();
        
        
    }
}
