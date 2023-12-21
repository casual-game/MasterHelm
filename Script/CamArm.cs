using System;
using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using MirzaBeig.LightningVFX;
using PrimeTween;
using Sirenix.OdinInspector;
using SSCS;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.Serialization;

public class CamArm : MonoBehaviour
{
    
    //Public
    public static CamArm instance;
    public TransitionProfile transition_main_fadein;
    public float orthographicSize = 4;
    [HideInInspector] public Camera mainCam;
    [TitleGroup("Movement")]
    [FoldoutGroup("Movement/data")] public Transform target;
    [FoldoutGroup("Movement/data")] public float moveSpeed = 3.0f;
    [FoldoutGroup("Movement/data")] public Vector3 addVec;
    [TitleGroup("Effect")]
    [FoldoutGroup("Effect/data")] public Material m_radialblur,m_speedline;
    [FoldoutGroup("Effect/data")] public CustomPostProcessingInstance cp_invert, cp_radial;
    [FoldoutGroup("Effect/data")] public Color vignette_NormalColor, vignette_HitColor;
    [FoldoutGroup("Effect/data")] public CloudShadowsProfile cloudShadows;
    [FoldoutGroup("Effect/data")] public Color cloud_FadeColor, cloud_IngameColor;
    //Private
    private Vector3 _camBossVec,_camAttackVec;
    private float _camAttackVecDist;
    private Transform _camT;
    private Camera[] _cams;
    private Tween t_stop,t_shake,t_chromatic,t_radial,t_cambossvec,t_camattackvec;
    private Sequence s_impact,s_speedline,s_fade;
    private Sequence s_frame,s_vignette,s_zoom;
    private int id_radial,id_speedline;
    private TransitionAnimator _transitionAnimator;
    private Hero _hero;
    private bool attackVecActivating = false;
    private void Awake()
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
        _camAttackVec = Quaternion.Euler(0, _hero.Get_LookF(), 0) * Vector3.forward;
        
        transform.position = Vector3.Lerp(transform.position,
            target.position + addVec + _camBossVec + (_camAttackVec*_camAttackVecDist),moveSpeed*Time.unscaledDeltaTime);
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
    //복합 이펙트. 중복 불가
    public void Tween_FadeIn()
    {
        float delay = 0.5f;
        s_fade.Stop();
        cloudShadows.cloudsThickness = 10.0f;
        cloudShadows.cloudsOpacity = 1.0f;
        cloudShadows.coverage = 1.0f;
        cloudShadows.sunColor = cloud_FadeColor;
        _cams[0].orthographicSize = orthographicSize + 1.5f;
        _cams[1].orthographicSize = orthographicSize + 1.5f;
        BeautifySettings.settings.purkinjeLuminanceThreshold.value = 0.0f;
        if(_transitionAnimator!=null) Destroy(_transitionAnimator.gameObject);
        _transitionAnimator = TransitionAnimator.Start(transition_main_fadein,autoDestroy:false);
        //Chromatic
        t_chromatic.Stop();
        t_chromatic = Tween.Custom(0.03f, 0.001f, 2.5f,startDelay:0.5f + delay, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
        //Shake Strong
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.25f, 0.375f, 45
            , easeBetweenShakes: Ease.OutSine, useUnscaledTime: true,startDelay:0.5f + delay);
        Tween.Delay(this, 0.5f + delay, (() => Tween_Radial(0.5f)), true);
        //Stop
        t_stop.Stop();
        Time.timeScale = 1.0f;
        //
        float duration = 1.0f;
        
        Ease ease = Ease.InOutCubic;
        s_fade = Sequence.Create()
            .ChainDelay(delay)
            .Chain(Tween.Custom(10.0f, 15.0f, duration, onValueChange: thickness =>
            {
                cloudShadows.cloudsThickness = thickness;
            }, ease))
            .Group(Tween.Custom(1.0f, 0.035f, duration, onValueChange: opacity =>
            {
                cloudShadows.cloudsOpacity = opacity;
            }, ease))
            .Group(Tween.Custom(1.0f, 0.5f, duration, onValueChange: coverage =>
            {
                cloudShadows.coverage = coverage;
            }, ease))
            .Group(Tween.Custom(cloud_FadeColor,cloud_IngameColor,duration,onValueChange: sunColor =>
            {
                cloudShadows.sunColor = sunColor;
            }, ease))
            .Group(Tween.CameraOrthographicSize(_cams[0], orthographicSize, duration, ease))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, duration, ease));
    }
    public void Tween_FadeOut()
    {
        s_fade.Stop();
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Stop();
        s_zoom.Complete();
        cloudShadows.cloudsThickness = 15.0f;
        cloudShadows.cloudsOpacity = 0.025f;
        cloudShadows.coverage = 0.5f;
        cloudShadows.sunColor = cloud_IngameColor;
        _cams[0].orthographicSize = orthographicSize;
        _cams[1].orthographicSize = orthographicSize;
        BeautifySettings.settings.purkinjeLuminanceThreshold.value = 0.0f;
        Tween_Radial(0.75f,0.2f);
        Tween_Vignette(0.15f,1.0f,1.5f,vignette_HitColor,vignette_NormalColor);
        //Chromatic
        BeautifySettings.settings.chromaticAberrationIntensity.value = 0.02f;
        t_chromatic = Tween.Custom(0.04f, 0.001f, 3.5f, ease: Ease.OutCirc,useUnscaledTime:true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
        //Shake Strong
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.25f, 0.375f, 45
            , easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        //
        float duration = 2.125f;
        float delay = 2.5f;
        Ease ease = Ease.InOutCubic;
        s_fade = Sequence.Create()
            .Group(Tween.Custom(15.0f, 10.0f, duration, onValueChange: thickness =>
            {
                cloudShadows.cloudsThickness = thickness;
            }, ease, startDelay: delay,useUnscaledTime:true))
            .Group(Tween.Custom(0.035f, 1.0f, duration, onValueChange: opacity =>
            {
                cloudShadows.cloudsOpacity = opacity;
            }, ease, startDelay: delay,useUnscaledTime:true))
            .Group(Tween.Custom(0.5f, 1.0f, duration, onValueChange: coverage =>
            {
                cloudShadows.coverage = coverage;
            }, ease, startDelay: delay,useUnscaledTime:true))
            .Group(Tween.Custom(cloud_IngameColor,cloud_FadeColor,duration,onValueChange: sunColor =>
            {
                cloudShadows.sunColor = sunColor;
            }, ease, startDelay: delay,useUnscaledTime:true))
            .Group(Tween.Custom(0.0f, 1.0f, 1.5f, onValueChange: threshold =>
            {
                BeautifySettings.settings.purkinjeLuminanceThreshold.value = threshold;
            }, ease, startDelay: delay-1,useUnscaledTime:true))
            .Group(Tween.Custom(1.0f, 0.0f, 1.5f, onValueChange: progress =>
            {
                _transitionAnimator.SetProgress(progress);
            }, ease: Ease.InOutQuad, startDelay: delay+0.5f,useUnscaledTime:true));
        //Zoom
        s_zoom = Sequence.Create()
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize-0.75f, 0.1f,useUnscaledTime:true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize-0.75f, 0.1f,useUnscaledTime:true))
            .ChainDelay(.25f,true)
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize, 1.0f, Ease.OutCirc,useUnscaledTime:true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, 1.0f, Ease.OutCirc,useUnscaledTime:true))
            .Group(Tween.CameraOrthographicSize(_cams[0], orthographicSize+1.5f, duration, ease, startDelay: delay-.5f,useUnscaledTime:true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize+1.5f, duration, ease, startDelay: delay-.5f,useUnscaledTime:true));
        //Stop
        t_stop = Tween.GlobalTimeScale(0.25f, 0.0f, 2.5f, ease: Ease.InExpo);

    }
    public void Tween_ShakeStrong()
    {
        Tween_Radial(0.5f);
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Stop();
        s_zoom.Complete();
        //Zoom
        s_zoom = Sequence.Create()
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize-0.75f, 0.15f, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize-0.75f, 0.15f, useUnscaledTime: true))
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize, 1.0f, Ease.OutCirc, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, 1.0f, Ease.OutCirc, useUnscaledTime: true));
        //Chromatic
        BeautifySettings.settings.chromaticAberrationIntensity.value = 0.02f;
        t_chromatic = Tween.Custom(0.02f, 0.001f, 1.5f, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
        //Shake Strong
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.375f, 0.5f, 45
            , easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        //Stop
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.6f, ease: Ease.InSine);
    }
    public void Tween_ShakeNormal()
    {
        t_shake.Complete();
        t_stop.Complete();
        //Shake Normal
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.275f, 0.3f, 25,
            easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        
        //Stop Normal
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.3f, ease: Ease.OutQuad);
    }
    public void Tween_ShakeSuperArmor()
    {
        Tween_Radial(0.15f);
        Tween_Vignette(0.074f,0.125f,0.5f,vignette_HitColor,vignette_NormalColor);
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Complete();
        //Shake Normal
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.2f, 0.3f, 25,
            easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        //Stop Normal
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.15f, ease: Ease.InQuad);
        //Chromatic
        BeautifySettings.settings.chromaticAberrationIntensity.value = 0.02f;
        t_chromatic = Tween.Custom(0.015f, 0.001f, 1.5f, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
    }

    public void Tween_ShakeStrong_Hero()
    {
        Tween_Radial(0.5f);
        Tween_Vignette(0.15f,0.25f,0.5f,vignette_HitColor,vignette_NormalColor);
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Stop();
        s_zoom.Complete();
        //Zoom
        s_zoom = Sequence.Create()
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize-0.75f, 0.15f, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize-0.75f, 0.15f, useUnscaledTime: true))
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize, 1.0f, Ease.OutCirc, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, 1.0f, Ease.OutCirc, useUnscaledTime: true));
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
    public void Tween_ShakeNormal_Hero()
    {
        Tween_Radial(0.25f);
        Tween_Vignette(0.075f,0.125f,0.5f,vignette_HitColor,vignette_NormalColor);
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Stop();
        //Shake Normal
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.2f, 0.3f, 25,
            easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        
        //Stop Normal
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.4f, ease: Ease.OutQuad);
        //Chromatic
        BeautifySettings.settings.chromaticAberrationIntensity.value = 0.025f;
        t_chromatic = Tween.Custom(0.02f, 0.001f, 1.5f, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);

    }
    
    public void Tween_ShakeDown()
    {
        t_shake.Complete();
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, Vector3.down * 0.175f, 0.75f, 20,
            easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
    }
    public void Tween_ShakeWeak()
    {
        t_stop.Complete();
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.2f, ease: Ease.OutQuad);
    }
    public void Tween_Impact(ParticleSystem particle,int damage)
    {
        float impactDuration = 0.075f;
        Tween_Radial(0.5f);
        s_impact.Stop();
        t_stop.Stop();
        
        t_chromatic.Stop();
        //Speedline
        s_speedline.Stop();
        m_speedline.SetColor(GameManager.s_colour, Color.white);
        s_speedline = Sequence.Create()
            .Chain(Tween.Custom(0.0f, 1.0f, impactDuration, onValueChange: val =>
            {
                m_speedline.SetFloat(GameManager.s_unscaledtime, Time.unscaledTime);
            }))
            .ChainCallback(() =>
            {
                m_speedline.SetColor(GameManager.s_colour, Color.clear);
            });
        //Impact
        cp_invert.Activate();
        s_impact = Sequence.Create()
            .ChainDelay(impactDuration).ChainCallback(() =>
            {
                cp_invert.Deactivate();
                particle.Play();
                Vector3 pos = particle.transform.position;
                GameManager.Instance.Shockwave(pos);
                GameManager.Instance.dmp_strong.Spawn(pos + Vector3.up * 1.2f, damage);
                Tween_Radial(0.75f);
                //Shake Strong
                t_shake.Stop();
                _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
                t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.25f, 0.65f, 40
                    , easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
            });
        //Chromatic
        BeautifySettings.settings.chromaticAberrationIntensity.value = 0.02f;
        t_chromatic = Tween.Custom(0.035f, 0.001f, 1.5f, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal,startDelay:0.1f);
        //Shake Strong
        t_shake.Stop();
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.3f, 0.375f, 45
            , easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        //Stop
        t_stop = Tween.GlobalTimeScale(0.05f, 1.0f, 0.5f, ease: Ease.InSine);
        //Zoom
        s_zoom = Sequence.Create()
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize-0.5f, 0.15f, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize-0.5f, 0.15f, useUnscaledTime: true))
            .ChainDelay(0.35f,useUnscaledTime:true)
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize, 1.0f, Ease.OutCirc, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, 1.0f, Ease.OutCirc, useUnscaledTime: true));
        
        
    }
    public void Tween_JustEvade()
    {
        Tween_Vignette(0.75f,0.75f,0.75f);
        Tween_Frame(0.75f,0.75f,0.75f);
        Tween_Radial(1.05f,0.15f);
        //Stop
        t_stop.Stop();
        t_stop = Tween.GlobalTimeScale(0.375f, 1.0f, 1.2f, ease: Ease.InOutCirc);
        //Shake Normal
        float shakeDuration = 0.5f;
        t_shake.Stop();
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.25f, shakeDuration, shakeDuration*25,
            easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        //Chromatic
        t_chromatic.Stop();
        t_chromatic = Tween.Custom(0.035f, 0.001f, 2.5f, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
        
    }
    public void Tween_Skill()
    {
        float duration = 0.5f;
        Tween_Radial(duration,0.075f);
        //Tween_Vignette(0.25f,0.4f,0.5f);
        Tween_Vignette(0.25f,0.4f,0.5f);
        Tween_Frame(0.25f,0.4f,0.5f);
        Tween_Zoom(0.25f,0.4f,0.5f,0,3.5f);
        //Stop
        t_stop.Complete();
        t_stop = Tween.GlobalTimeScale(0.2f, 1.0f, duration, ease: Ease.InExpo);
        //Shake Normal
        t_shake.Complete();
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.15f, 0.75f, 25,
            easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        
        //Chromatic
        t_chromatic.Complete();
        t_chromatic = Tween.Custom(0.035f, 0.001f, duration, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
    }
    public void Tween_ResetTimescale()
    {
        t_stop.Complete();
    }
    //싱글 이펙트. 복합에서 호출한다.
    public void Tween_Radial(float duration,float bluramount =0.1f)
    {
        t_radial.Stop();
        cp_radial.Activate();
        m_radialblur.SetFloat(GameManager.s_bluramount,bluramount);
        t_radial = Tween.MaterialProperty(m_radialblur, id_radial, 0.0f, duration,useUnscaledTime: true)
            .OnComplete(target:this, target=> target.cp_radial.Deactivate());
    }
    public void Tween_Frame(float begin,float delay,float fin)
    {
        s_frame.Stop();
        float startVal = BeautifySettings.settings.frameBandVerticalSize.value;
        s_frame = Sequence.Create()
            .Chain(Tween.Custom(startVal, 0.5f, begin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.frameBandVerticalSize.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(0.5f, 0.0f, fin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.frameBandVerticalSize.value = newVal));
    }
    public void Tween_Vignette(float begin,float delay,float fin)
    {
        s_vignette.Complete();
        s_vignette = Sequence.Create()
            .Chain(Tween.Custom(0.75f, 1.0f, begin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(1.0f, 0.75f, fin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal));
    }
    public void Tween_Vignette(float begin,float delay,float fin,Color hitColor,Color normalColor)
    {
        s_vignette = Sequence.Create()
            .Chain(Tween.Custom(0.75f, 1.0f, begin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(normalColor, hitColor, begin, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(1.0f, 0.75f, fin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(hitColor, normalColor, fin, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal));
    }
    public void Tween_Zoom(float begin, float delay, float fin, float startDelay,float zoom)
    {
        //Zoom
        s_zoom.Complete();
        s_zoom = Sequence.Create();
        if (startDelay > 0.01f) s_zoom.ChainDelay(startDelay, true);
        s_zoom
            .Chain(Tween.CameraOrthographicSize(_cams[0], zoom, begin, useUnscaledTime: true,
                ease: Ease.OutCirc))
            .Group(Tween.CameraOrthographicSize(_cams[1], zoom, begin, useUnscaledTime: true,
                ease: Ease.OutCirc))
            .ChainDelay(delay, true)
            .Chain(Tween.CameraOrthographicSize(_cams[0], orthographicSize, fin, Ease.OutCirc, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, fin, Ease.OutCirc, useUnscaledTime: true));
    }
    
}
