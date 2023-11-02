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
    private Camera[] _cams;
    private Sequence s_stop_normal,s_shake_normal,s_stop_strong,s_shake_strong,s_chromatic,s_zoom;
    private void Awake()
    {
        instance = this;
        _camT = transform.GetChild(0);
        mainCam = GetComponentInChildren<Camera>();
        _cams = GetComponentsInChildren<Camera>();
        transform.position = target.position + addVec;
        
        s_shake_normal = DOTween.Sequence().SetAutoKill(false)
            .PrependCallback(() =>
            {
                _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
            })
            .Append(_camT.DOShakePosition(0.25f,0.375f,25).SetEase(Ease.OutQuint)).SetUpdate(true);
        
        s_stop_normal = DOTween.Sequence().SetAutoKill(false)
            .Append(DOTween.To(() => 0.05f, x => Time.timeScale = x, 
                1f, 0.3f).SetEase(Ease.OutQuad)).SetUpdate(true);
        
        s_shake_strong = DOTween.Sequence().SetAutoKill(false)
            .PrependCallback(() =>
            {
                _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
                BeautifySettings.settings.chromaticAberrationIntensity.value = 0.025f;
            })
            .Append(_camT.DOShakePosition(0.25f,0.6f,45).SetEase(Ease.OutQuint)).SetUpdate(true);
        
        s_stop_strong = DOTween.Sequence().SetAutoKill(false)
            .Append(DOTween.To(() => 0.05f, x => Time.timeScale = x, 
                1f, 0.3f).SetEase(Ease.InExpo)).SetUpdate(true);

        s_chromatic = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .PrependCallback(() =>
            {
                BeautifySettings.settings.chromaticAberrationIntensity.value = 0.02f;
            })
            .Join(DOTween.To(() => 0.02f, x => BeautifySettings.settings.chromaticAberrationIntensity.value = x,
                0.001f, 1.5f).SetEase(Ease.OutCirc));

        s_zoom = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .Append(_cams[0].DOOrthoSize(3.25f, 0.15f))
            .Join(_cams[0].DOOrthoSize(3.25f, 0.15f))
            .AppendInterval(0.15f)
            .Append(_cams[0].DOOrthoSize(4.0f, 1.0f).SetEase(Ease.OutCirc))
            .Join(_cams[0].DOOrthoSize(4.0f, 1.0f).SetEase(Ease.OutCirc));
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,target.position + addVec,moveSpeed*Time.deltaTime);
    }

    public void Tween_ShakeStrong()
    {
        if(s_shake_normal.IsPlaying()) s_shake_normal.Pause();
        if(s_stop_normal.IsPlaying()) s_stop_normal.Pause();
        
        if(!s_zoom.IsInitialized()) s_zoom.Play();
        else s_zoom.Restart();
        if(!s_chromatic.IsInitialized()) s_chromatic.Play();
        else s_chromatic.Restart();
        if(!s_shake_strong.IsInitialized()) s_shake_strong.Play();
        else s_shake_strong.Restart();
        if(!s_stop_strong.IsInitialized()) s_stop_strong.Play();
        else s_stop_strong.Restart();
    }
    public void Tween_ShakeNormal()
    {
        if(s_shake_strong.IsPlaying()) s_shake_strong.Pause();
        if(s_stop_strong.IsPlaying()) s_stop_strong.Pause();

        s_stop_normal.timeScale = 1.0f;
        if(!s_shake_normal.IsInitialized()) s_shake_normal.Play();
        else s_shake_normal.Restart();
        if(!s_stop_normal.IsInitialized()) s_stop_normal.Play();
        else s_stop_normal.Restart();
    }
    public void Tween_ShakeWeak()
    {
        if(s_stop_strong.IsPlaying()) s_stop_strong.Pause();
        
        s_stop_normal.timeScale = 1.5f;
        if(!s_stop_normal.IsInitialized()) s_stop_normal.Play();
        else s_stop_normal.Restart();
    }
}
