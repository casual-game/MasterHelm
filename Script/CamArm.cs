using System;
using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using MirzaBeig.LightningVFX;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class CamArm : MonoBehaviour
{
    //Public
    public static CamArm instance;
    [HideInInspector] public Camera mainCam;
    [TitleGroup("Movement")]
    [FoldoutGroup("Movement/data")] public Transform target;
    [FoldoutGroup("Movement/data")] public float moveSpeed = 3.0f;
    [FoldoutGroup("Movement/data")] public Vector3 addVec;
    [TitleGroup("Effect")]
    [FoldoutGroup("Effect/data")] public Material m_radialblur,m_speedline;
    [FoldoutGroup("Effect/data")] public CustomPostProcessingInstance cp_invert, cp_radial;
    //Private
    private Transform _camT;
    private Camera[] _cams;
    private Tween t_stop,t_shake,t_chromatic,t_radial;
    private Sequence s_zoom,s_impact;
    private int id_radial,id_speedline;
    private void Awake()
    {
        instance = this;
        _camT = transform.GetChild(0);
        mainCam = GetComponentInChildren<Camera>();
        _cams = GetComponentsInChildren<Camera>();
        transform.position = target.position + addVec;
        id_radial = Shader.PropertyToID(GameManager.s_bluramount);
        id_speedline = Shader.PropertyToID(GameManager.s_colour);
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
    public void Tween_Radial(float duration)
    {
        t_radial.Stop();
        cp_radial.Activate();
        m_radialblur.SetFloat(GameManager.s_bluramount,0.1f);
        t_radial = Tween.MaterialProperty(m_radialblur, id_radial, 0.0f, duration,useUnscaledTime: true)
            .OnComplete(target:this, target=> target.cp_radial.Deactivate());
    }

    [Button]
    public void Tween_Impact(float duration = 0.0075f)

    {
        Tween_ShakeStrong();
        Tween_Radial(0.5f);
        s_impact.Stop();
        t_stop.Stop();
        m_speedline.SetColor(GameManager.s_colour, Color.white);
        cp_invert.Activate();
        BeautifySettings.settings.bloomThreshold.value = 0.3f;
        Time.timeScale = 0.05f;
        s_impact = Sequence.Create()
            .ChainDelay(duration,useUnscaledTime: true).ChainCallback(() =>
            {
                m_speedline.SetColor(GameManager.s_colour, Color.clear);
                cp_invert.Deactivate();
                BeautifySettings.settings.bloomThreshold.value = 0.7f;
                
            })
            .ChainDelay(0.015f,useUnscaledTime: true).ChainCallback(() =>
            {
                Time.timeScale = 1.0f;
            });
    }

}
