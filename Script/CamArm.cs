using System;
using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using PrimeTween;
using Sirenix.OdinInspector;
using SSCS;
using UnityEngine;
using UnityEngine.Serialization;

public class CamArm : MonoBehaviour
{
    
    //Public
    public static CamArm instance;
    public float orthographicSize = 4;
    [HideInInspector] public Camera mainCam;
    [TitleGroup("Movement")]
    [FoldoutGroup("Movement/data")] public Transform target;
    [FoldoutGroup("Movement/data")] public float moveSpeed = 3.0f;
    [FoldoutGroup("Movement/data")] public Vector3 addVec;
    [TitleGroup("Effect")]
    [FoldoutGroup("Effect/data")] public Material m_radialblur,m_speedline;
    [FoldoutGroup("Effect/data")] public Color vignette_NormalColor, vignette_HitColor;
    [FoldoutGroup("Effect/data")] public CloudShadowsProfile cloudShadows;
    //Private
    private Vector3 _camBossVec,_camAttackVec;
    private float _camAttackVecDist,_zoomAttackVecFinalRatio = 1;
    private Transform _camT;
    private Camera[] _cams;
    private Tween t_radial,t_cambossvec,t_camattackvec,t_time,t_shake,t_chromatic;
    private Sequence s_impact,s_speedline,s_fade;
    private Sequence s_frame,s_vignette,s_zoom;
    private int id_radial,id_speedline;
    private Hero _hero;
    private bool attackVecActivating = false;
    private bool followTarget = false;
    public void Setting()
    {
        instance = this;
        _camT = transform.GetChild(0);
        mainCam = GetComponentInChildren<Camera>();
        _cams = GetComponentsInChildren<Camera>();
        transform.position = target.position + addVec;
        id_radial = Shader.PropertyToID(GameManager.s_bluramount);
        id_speedline = Shader.PropertyToID(GameManager.s_colour);
        _hero = target.GetComponentInParent<Hero>();
    }

    void Update()
    {
        if (!followTarget) return;
        _camAttackVec = Quaternion.Euler(0, _hero.Get_LookF(), 0) * Vector3.forward;
        transform.position = Vector3.Lerp(transform.position,
            target.position + addVec + _camBossVec + (_camAttackVec*_camAttackVecDist*_zoomAttackVecFinalRatio),moveSpeed*Time.unscaledDeltaTime);
    }
    public void Tween_CamBossVec(bool activateBossVec)
    {
        float height = 1.25f;
        if (activateBossVec)
        {
            t_cambossvec = Tween.Custom(0, height,0.75f, onValueChange: value =>
                { _camBossVec = new Vector3(0, value, 0); } ,ease: Ease.InOutCirc,startDelay:0.5f);
        }
        else
        {
            t_cambossvec = Tween.Custom(height, 0,0.75f, onValueChange: value =>
                { _camBossVec = new Vector3(0, value, 0); } ,ease: Ease.InOutCirc,startDelay:0.5f);
        }
    }
    public void Tween_CamAttackVec(bool activateAttackVec)
    {
        if (activateAttackVec == attackVecActivating) return;
        float distance = 1.25f;
        float distanceRatio = Vector3.Distance(_camAttackVec, transform.position - target.position) / distance;
        if (activateAttackVec)
        {
            attackVecActivating = true;
            t_cambossvec = Tween.Custom(_camAttackVecDist, distance,1.5f*distanceRatio, onValueChange: value =>
            { _camAttackVecDist = value; } ,ease: Ease.InOutSine);
        }
        else
        {
            attackVecActivating = false;
            t_cambossvec = Tween.Custom(_camAttackVecDist, 0,1.5f*distanceRatio, onValueChange: value =>
                { _camAttackVecDist = value; } ,ease: Ease.InOutSine);
        }
    }
    public void Set_FollowTarget(bool follow)
    {
        followTarget = follow;
    }
    //복합 이펙트. 중복 불가
    [Button]
    public void Tween_FadeIn()
    {
        s_fade.Stop();
        cloudShadows.cloudsThickness = 10.0f;
        cloudShadows.cloudsOpacity = 1.0f;
        cloudShadows.coverage = 1.0f;
        _cams[0].orthographicSize = orthographicSize + 1.5f;
        _cams[1].orthographicSize = orthographicSize + 1.5f;
        BeautifySettings.settings.purkinjeLuminanceThreshold.value = 0.0f;
        
        //Tween.Delay(this, 0.5f + delay, (() => Tween_Radial(0.5f)), true);
        Tween_Chromatic(0.03f,2.5f,Ease.OutCirc,0.5f);
        //Stop
        t_time.Stop();
        Time.timeScale = 1.0f;
        float duration = 1.5f;
        Ease ease = Ease.InOutQuart;
        s_fade = Sequence.Create(useUnscaledTime: true)
            .Chain(Tween.Custom(10.0f, 15.0f, duration, onValueChange: thickness =>
            {
                cloudShadows.cloudsThickness = thickness;
            }, ease))
            .Group(Tween.Custom(1.0f, 0.015f, duration, onValueChange: opacity =>
            {
                cloudShadows.cloudsOpacity = opacity;
            }, ease))
            .Group(Tween.Custom(1.0f, 0.5f, duration, onValueChange: coverage =>
            {
                cloudShadows.coverage = coverage;
            }, ease))
            .Group(Tween.CameraOrthographicSize(_cams[0], orthographicSize, duration, ease))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, duration, ease));
    }
    [Button]
    public void Tween_FadeOut()
    {
        s_fade.Stop();
        s_zoom.Complete();
        Tween_Radial(1.0f,0.2f);
        Tween_Vignette(0.15f,1.0f,1.5f,vignette_HitColor,vignette_NormalColor);
        Tween_Shake(0.375f,45,GameManager.V3_One * 0.25f,Ease.OutSine);
        Tween_Chromatic(0.04f,2.5f,Ease.OutCirc);
        
        float duration = 1.5f,cloudDelay = 0.2f;
        Ease ease = Ease.InOutQuad;
        s_fade = Sequence.Create(useUnscaledTime: true)
            .ChainDelay(1.0f)
            .Chain(Tween.Custom(15.0f, 10.0f, duration, onValueChange: thickness =>
            {
                cloudShadows.cloudsThickness = thickness;
            }, ease,startDelay:cloudDelay))
            .Group(Tween.Custom(0.015f, 1.0f, duration, onValueChange: opacity =>
            {
                cloudShadows.cloudsOpacity = opacity;
            }, ease,startDelay:cloudDelay))
            .Group(Tween.Custom(0.5f, 1.0f, duration, onValueChange: coverage =>
            {
                cloudShadows.coverage = coverage;
            }, ease,startDelay:cloudDelay))
            .Group(Tween.CameraOrthographicSize(_cams[0], orthographicSize + 1.5f, duration, ease))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize + 1.5f, duration, ease));

    }
    public void Tween_ShakeStrong()
    {
        Tween_Stop(0.05f,0.6f,Ease.InSine);
        Tween_ShakeStrong_Core();
    }
    public void Tween_ShakeStrong_Core()
    {
        Tween_Radial(0.5f);
        Tween_Shake(0.5f,45,GameManager.V3_One * 0.375f,Ease.OutSine);
        Tween_Zoom(0.15f,0.05f,1.0f,0,orthographicSize-0.75f);
        Tween_Chromatic(0.02f,1.5f,Ease.OutCirc);
    }
    public void Tween_ShakeNormal()
    {
        Tween_Stop(0.05f,0.3f,Ease.OutQuad);
        Tween_ShakeNormal_Core();
    }
    public void Tween_ShakeNormal_Core()
    {
        Tween_Shake(0.3f,25,GameManager.V3_One * 0.275f,Ease.Linear);
        Tween_Radial(0.3f,0.05f);
    }
    public void Tween_ShakeSmooth()
    {
        Tween_Shake(0.5f,25,GameManager.V3_One * 0.125f,Ease.OutSine);
    }
    public void Tween_ShakeSuperArmor()
    {
        Tween_Radial(0.15f);
        Tween_Vignette(0.074f,0.125f,0.5f,vignette_HitColor,vignette_NormalColor);
        Tween_Stop(0.05f,0.15f,Ease.InQuad);
        Tween_Shake(0.3f,25,GameManager.V3_One * 0.2f,Ease.OutSine);
        Tween_Chromatic(0.015f,1.5f,Ease.OutCirc);
    }
    public void Tween_ShakeStrong_Hero()
    {
        Tween_Radial(1.0f,0.2f);
        Tween_Vignette(0.15f,0.375f,0.5f,vignette_HitColor,vignette_NormalColor);
        Tween_Speedline(0.15f,0.375f,0.5f,new Color(1,0,0,10.0f/255.0f),0.375f);
        //Tween_Frame(0.15f,0.225f,0.65f);
        
        Tween_Stop(0.05f,0.3f,Ease.InQuad);
        Tween_Shake(0.375f,45,GameManager.V3_One * 0.35f,Ease.OutSine);
        Tween_Zoom(0.15f,0.25f,0.75f,0,orthographicSize-0.75f);
        Tween_Chromatic(0.035f,1.5f,Ease.OutCirc);
        GameManager.Instance.Shockwave(target.position);
    }
    public void Tween_ShakeNormal_Hero()
    {
        
        Tween_Radial(0.5f,0.125f);
        Tween_Speedline(0.075f,0.125f,0.5f,new Color(1,0,0,10.0f/255.0f),0.375f);
        Tween_Zoom(0.075f,0.125f,0.5f,0,orthographicSize-0.25f);
        Tween_Vignette(0.075f,0.125f,0.5f,vignette_HitColor,vignette_NormalColor);
        Tween_Stop(0.05f,0.4f,Ease.OutQuad);
        Tween_Shake(0.3f,25,GameManager.V3_One * 0.25f,Ease.OutSine);
        Tween_Chromatic(0.035f,1.5f,Ease.OutCirc);
    }
    
    public void Tween_ShakeDown()
    {
        Tween_Shake(0.75f,20,Vector3.down * 0.175f,Ease.OutSine);
    }
    public void Tween_ShakeWeak()
    {
        Tween_Stop(0.05f,0.2f,Ease.OutQuad);
    }
    public void Tween_Impact(Transform t ,float particleScale,int damage)
    {
        return;
        float impactDuration = 0.075f;
        Tween_Radial(0.5f);
        Tween_Stop(0.05f,0.5f,Ease.InSine);
        Tween_Shake(0.375f,45,GameManager.V3_One * 0.3f,Ease.OutSine);
        Tween_Zoom(0.15f,0.35f,1.0f,0,orthographicSize-0.5f);
        Tween_Speedline(0.05f,impactDuration,0.25f,Color.white);
        Tween_Chromatic(0.035f,1.5f,Ease.OutCirc,0.1f);
        //Impact
        s_impact.Stop();
    }
    public void Tween_JustEvade()
    {
        Tween_Vignette(0.25f,0.5f,0.25f);
        Tween_Frame(0.25f,0.5f,0.25f);
        Tween_Speedline(0.25f,0.5f,0.25f,new Color(1.0f,1.0f,1.0f,6.0f/255.0f),0.675f);
        Tween_Zoom(0.25f,0.5f,0.25f,0,orthographicSize-0.5f);
        Tween_Chromatic(0.05f,1.0f,Ease.InQuad);
        Tween_Radial(1.00f,0.15f);
        Tween_Stop(0.4f,1.0f,Ease.InOutBack);
        Tween_Shake(1.0f,20,GameManager.V3_One * 0.15f,Ease.OutSine);
    }
    public void Tween_Skill()
    {
        Tween_Radial(0.5f,0.075f);
        Tween_Vignette(0.25f,0.4f,0.5f);
        Tween_Frame(0.25f,0.4f,0.5f);
        Tween_Zoom(0.4f,0.4f,0.75f,0,3.25f);
        Tween_Speedline(0.25f,0.15f,1.0f,new Color(1,1,1,10.0f/255.0f),0.675f);
        Tween_Stop(0.2f,0.65f,Ease.InExpo);
        Tween_Shake(1.0f,32,GameManager.V3_One * 0.1f,Ease.InExpo);
        Tween_Chromatic(0.035f,1.0f,Ease.InQuad);
    }
    public void Tween_ResetTimescale()
    {
        t_time.Complete();
    }
    //싱글 이펙트. 복합에서 호출한다.
    public void Tween_Radial(float duration,float bluramount =0.1f)
    {
    }
    public void Tween_Frame(float begin,float delay,float fin)
    {
        s_frame.Stop();
        float startVal = BeautifySettings.settings.frameBandVerticalSize.value;
        s_frame = Sequence.Create(useUnscaledTime:true)
            .Chain(Tween.Custom(startVal, 0.5f, begin, ease: Ease.OutCirc,
                onValueChange: newVal => BeautifySettings.settings.frameBandVerticalSize.value = newVal))
            .ChainDelay(delay)
            .Chain(Tween.Custom(0.5f, 0.0f, fin, ease: Ease.OutCirc,
                onValueChange: newVal => BeautifySettings.settings.frameBandVerticalSize.value = newVal));
    }
    public void Tween_Vignette(float begin,float delay,float fin)
    {
        s_vignette.Complete();
        s_vignette = Sequence.Create(useUnscaledTime:true)
            .Chain(Tween.Custom(0.75f, 1.0f, begin, ease: Ease.OutCirc,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(1.0f, 0.75f, fin, ease: Ease.OutCirc,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal));
    }
    public void Tween_Vignette(float begin,float delay,float fin,Color hitColor,Color normalColor)
    {
        s_vignette = Sequence.Create(useUnscaledTime:true)
            .Chain(Tween.Custom(0.75f, 1.0f, begin, ease: Ease.OutCirc,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(normalColor, hitColor, begin,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(1.0f, 0.75f, fin, ease: Ease.OutCirc,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(hitColor, normalColor, fin,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal));
    }
    public void Tween_Zoom(float begin, float delay, float fin, float startDelay,float zoom)
    {
        //Zoom
        s_zoom.Complete();
        s_zoom = Sequence.Create(useUnscaledTime:true);
        if (startDelay > 0.01f) s_zoom.ChainDelay(startDelay);
        s_zoom
            .Chain(Tween.CameraOrthographicSize(_cams[0], zoom, begin,
                ease: Ease.OutCirc))
            .Group(Tween.CameraOrthographicSize(_cams[1], zoom, begin,
                ease: Ease.OutCirc))
            .Group(Tween.Custom(1, 0, begin, onValueChange: val =>
            {
                _zoomAttackVecFinalRatio = val;
            }))
            .ChainDelay(delay, true)
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize, fin, Ease.OutCirc))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, fin, Ease.OutCirc))
            .Group(Tween.Custom(0, 1, fin, onValueChange: val =>
            {
                _zoomAttackVecFinalRatio = val;
            }));

    }
    public void Tween_Speedline(float begin, float delay, float fin,Color color,float speed = 0.675f)
    {
        Color clearColor = color;
        clearColor.a = 0;
        float duration = begin + delay + fin;
        s_speedline.Stop();
        s_speedline = Sequence.Create(useUnscaledTime:true)
            .Group(Tween.Custom(0, 1, begin, onValueChange: val =>
            {
                m_speedline.SetColor(GameManager.s_colour, Color.Lerp(clearColor, color, val));
                m_speedline.SetFloat(GameManager.s_unscaledtime,  Time.unscaledTime*speed);
            }))
            .Group(Tween.Custom(0, 1, delay, onValueChange: val =>
            {
                m_speedline.SetFloat(GameManager.s_unscaledtime,  Time.unscaledTime*speed);
            }))
            .Chain(Tween.Custom(1, 0, fin, onValueChange: val =>
            {
                m_speedline.SetColor(GameManager.s_colour, Color.Lerp(clearColor, color, val));
                m_speedline.SetFloat(GameManager.s_unscaledtime,  Time.unscaledTime*speed);
            }));
    }
    public void Tween_Stop(float timescale,float duration,Ease ease)
    {
        t_time.Stop();
        t_time = Tween.Custom(timescale, 1, duration, onValueChange: val =>
        {
            Time.timeScale = val;
        }, ease: ease,useUnscaledTime:true);
    }
    public void Tween_Shake(float shakeDuration,int shakeFrequency,Vector3 shakeVec,Ease ease)
    {
        t_shake.Stop();
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, shakeVec, shakeDuration, shakeFrequency,
            easeBetweenShakes: ease, useUnscaledTime: true);
    }
    public void Tween_Chromatic(float strength,float duration,Ease ease,float startDelay)
    {
        t_chromatic.Complete();
        t_chromatic = Tween.Custom(strength, 0.001f, duration, ease: ease, useUnscaledTime: true,startDelay: startDelay,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
    }
    public void Tween_Chromatic(float strength,float duration,Ease ease)
    {
        t_chromatic.Complete();
        t_chromatic = Tween.Custom(strength, 0.001f, duration, ease: ease, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
    }
}
