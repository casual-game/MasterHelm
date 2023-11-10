using System;
using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using PrimeTween;
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
    private Camera[] _cams;
    private Tween t_stop,t_shake,t_chromatic;
    private Sequence s_zoom;
    private void Awake()
    {
        instance = this;
        _camT = transform.GetChild(0);
        mainCam = GetComponentInChildren<Camera>();
        _cams = GetComponentsInChildren<Camera>();
        transform.position = target.position + addVec;
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,target.position + addVec,moveSpeed*Time.deltaTime);
    }

    public void Tween_ShakeStrong()
    {
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Stop();
        s_zoom.Complete();
        //Zoom
        s_zoom = Sequence.Create()
            .Chain(Tween.CameraOrthographicSize(_cams[0], 3.25f, 0.15f, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], 3.25f, 0.15f, useUnscaledTime: true))
            .Chain(Tween.CameraOrthographicSize(_cams[0], 4.0f, 1.0f, Ease.OutCirc, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], 4.0f, 1.0f, Ease.OutCirc, useUnscaledTime: true));
        //Chromatic
        BeautifySettings.settings.chromaticAberrationIntensity.value = 0.02f;
        t_chromatic = Tween.Custom(0.02f, 0.001f, 1.5f, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
        //Shake Strong
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.25f, 0.375f, 45
            , easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        //Stop
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.3f, ease: Ease.InSine);
    }
    public void Tween_ShakeNormal()
    {
        t_shake.Complete();
        t_stop.Complete();
        //Shake Normal
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.2f, 0.3f, 25,
            easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        
        //Stop Normal
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.3f, ease: Ease.OutQuad);
    }
    public void Tween_ShakeWeak()
    {
        t_stop.Complete();
        
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.2f, ease: Ease.OutQuad);
    }
}
