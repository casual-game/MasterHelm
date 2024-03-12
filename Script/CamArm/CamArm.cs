using System;
using System.Collections;
using System.Collections.Generic;
using AtmosphericHeightFog;
using Beautify.Universal;
using LeTai.TrueShadow;
using PrimeTween;
using Sirenix.OdinInspector;
using SSCS;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class CamArm : MonoBehaviour
{
    
    //Public
    public static CamArm instance;
    public float orthographicSize = 4;
    [HideInInspector] public Camera mainCam;
    [TitleGroup("Movement")]
    [FoldoutGroup("Movement/data")] public Transform target;
    [FoldoutGroup("Movement/data")] public float moveSpeed = 3.0f;
    [FoldoutGroup("Movement/data")] public Vector3 addVec;
    [FormerlySerializedAs("m_radialblur")]
    [TitleGroup("Effect")]
    [FoldoutGroup("Effect/data")] public Material mRadialblur,mSpeedline,mInvert;

    [FoldoutGroup("Effect/data")] public Color vignette_NormalColor, vignette_HitColor;
    [FoldoutGroup("Effect/data")] public CloudShadowsProfile cloudShadows;

    [TitleGroup("Sequence")] [FoldoutGroup("Sequence/data")] public Image i_title_line, i_titile_bg, iTitleBG;
    [TitleGroup("Sequence")] [FoldoutGroup("Sequence/data")] public TMP_Text tmp_title;
    [TitleGroup("Sequence")] [FoldoutGroup("Sequence/data")] public CanvasGroup cg1, cg2;
    [TitleGroup("Sequence")] [FoldoutGroup("Sequence/data")] public UIElement_Frame uiElementFrame;
    [TitleGroup("Sequence")] [FoldoutGroup("Sequence/data")] public UI_IngameResult uiIngameResult;
    //Private
    private Vector3 _camBossVec,_camAttackVec;
    private float _camAttackVecDist,_zoomAttackVecFinalRatio = 1;
    private Transform _camT;
    private Camera[] _cams;
    private Tween t_cambossvec,t_camattackvec,t_time,t_shake,t_chromatic,t_purkinjeLuminanceThreshold,
        tUIStopped,tInvert,tShakeDeathHero;
    private Sequence s_speedline, s_fade, s_cloud, s_angle, seqUIZoom, seqBloom,seqRadial;
    private Sequence s_frame,s_vignette,s_zoom;
    private int id_radial;
    private Hero _hero;
    private bool attackVecActivating = false;
    private bool followTarget = false;
    private bool _uiStopped = false, _uiZoom = false;
    public void Setting()
    {
        instance = this;
        _camT = transform.GetChild(0);
        mainCam = GetComponentInChildren<Camera>();
        _cams = GetComponentsInChildren<Camera>();
        transform.position = target.position + addVec;
        id_radial = Shader.PropertyToID(GameManager.s_bluramount);
        _hero = target.GetComponentInParent<Hero>();
        Setting_UI();
        uiElementFrame.Setting(false);
    }

    void LateUpdate()
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
    
    //연출
    public void Tween_GameReady()
    {
        s_fade.Stop();
        CanvasGroup cg = tmp_title.GetComponent<CanvasGroup>();
        Color tmpColor = new Color(250.0f / 255.0f, 185.0f / 255.0f, 122.0f / 255.0f, 1.0f);
        Color tmpFinColor = tmpColor;
        tmpFinColor.a = 0;
        iTitleBG.gameObject.SetActive(true);
        iTitleBG.color = Color.white;
        cloudShadows.cloudsOpacity = 0.5f;
        cloudShadows.coverage = 0.5f;
        _cams[0].orthographicSize = orthographicSize + 1.5f;
        _cams[1].orthographicSize = orthographicSize + 1.5f;
        i_title_line.transform.localScale = Vector3.one;
        i_titile_bg.transform.localScale = Vector3.one;
        tmp_title.transform.localScale = Vector3.one;
        i_title_line.color = Color.white;
        i_titile_bg.color = Color.white;
        tmp_title.color = tmpColor;
        cg.alpha = 1;
        cg1.alpha = 0;
        cg2.alpha = 0;
        cg1.enabled = true;
        cg2.enabled = true;
        tmp_title.gameObject.SetActive(true);
        cg1.transform.localScale = Vector3.one*1.5f;
        cg2.transform.localScale = Vector3.one*1.5f;
        iTitleBG.raycastTarget = true;
        Time.timeScale = 1.0f;
        uiElementFrame.RevealInstantly();
    }
    public void Tween_GameStart()
    {
        s_fade.Stop();
        CanvasGroup cg = tmp_title.GetComponent<CanvasGroup>();
        Color tmpColor = new Color(250.0f / 255.0f, 185.0f / 255.0f, 122.0f / 255.0f, 1.0f);
        Color tmpFinColor = tmpColor;
        tmpFinColor.a = 0;
        iTitleBG.gameObject.SetActive(true);
        iTitleBG.color = Color.white;
        cloudShadows.cloudsOpacity = 0.5f;
        cloudShadows.coverage = 0.5f;
        _cams[0].orthographicSize = orthographicSize + 1.5f;
        _cams[1].orthographicSize = orthographicSize + 1.5f;
        i_title_line.transform.localScale = Vector3.one;
        i_titile_bg.transform.localScale = Vector3.one;
        tmp_title.transform.localScale = Vector3.one;
        i_title_line.color = Color.white;
        i_titile_bg.color = Color.white;
        tmp_title.color = tmpColor;
        cg.alpha = 1;
        cg1.alpha = 0;
        cg2.alpha = 0;
        cg1.enabled = true;
        cg2.enabled = true;
        tmp_title.gameObject.SetActive(true);
        cg1.transform.localScale = Vector3.one*1.5f;
        cg2.transform.localScale = Vector3.one*1.5f;
        iTitleBG.raycastTarget = false;
        Time.timeScale = 1.0f;
        
        
        float duration = 1.5f;
        Ease ease = Ease.InOutQuart;
        s_fade = Sequence.Create(useUnscaledTime: true)
            //구름
            .Chain(Tween.Custom(0.5f, 0.0f, duration,
                onValueChange: opacity => cloudShadows.cloudsOpacity = opacity, ease))
            .Group(Tween.Custom(0.5f, 0.0f, duration * 0.8f,
                onValueChange: coverage => cloudShadows.coverage = coverage, ease))
            //카메라
            .Group(Tween.CameraOrthographicSize(_cams[0], orthographicSize, 1.55f, ease: Ease.InOutQuint))
            .Group(Tween.CameraOrthographicSize(_cams[1], orthographicSize, 1.55f, ease: Ease.InOutQuint))
            //배경
            .Group(Tween.Color(iTitleBG, Color.clear, 0.5f, startDelay: 0.5f));
        //글자
        float textRatio = 1.25f;
        float textAddDelay = 0;
        s_fade.Group(Tween.Scale(i_title_line.transform, new Vector3(0, 1, 1), 0.75f * textRatio, startDelay: 0.0f+textAddDelay,
                ease: Ease.InBack))
            .Group(Tween.Alpha(i_title_line, 0, 0.5f * textRatio, startDelay: 0.0f+textAddDelay))
            .Group(Tween.Scale(i_titile_bg.transform, 0.6f * textRatio, 0.875f, startDelay: 0.0f+textAddDelay, ease: Ease.InBack))
            .Group(Tween.Alpha(i_titile_bg, 0, 0.25f * textRatio, startDelay: 0.625f+textAddDelay))
            .Group(Tween.Scale(tmp_title.transform, 0.5f, 0.75f * textRatio, startDelay: 0.125f+textAddDelay, ease: Ease.InBack))
            .Group(Tween.Alpha(cg, 0, 0.25f * textRatio, startDelay: 0.625f+textAddDelay)); 
        //CanvasGroup
        float canvasRatio = 0.9f;
        float canvasAddDelay = -0.2f;
        s_fade
            .Group(Tween.Scale(cg1.transform, 1.0f, 0.875f*canvasRatio, startDelay: 0.625f+canvasAddDelay, ease: Ease.InOutQuint))
            .Group(Tween.Scale(cg2.transform, 1.0f, 0.875f*canvasRatio, startDelay: 0.625f+canvasAddDelay, ease: Ease.InOutQuint))
            .Group(Tween.Alpha(cg1, 1, 0.5f*canvasRatio, startDelay: 0.5f+canvasAddDelay))
            .Group(Tween.Alpha(cg2, 1, 0.5f*canvasRatio, startDelay: 0.5f+canvasAddDelay));
        s_fade.OnComplete(() =>
            {
                tmp_title.gameObject.SetActive(false);
                cg1.enabled = false;
                cg2.enabled = false;
            });
        //Tween_Chromatic(0.05f,1.55f,Ease.InOutQuint);
        Tween_Bloom(0.125f,0.25f,0.5f,20);
        uiElementFrame.Hide();
        Tween_Radial(0.25f,0.25f,0.75f,0.05f);
    }
    
    //복합 이펙트. 중복 불가
    public void Tween_ShakeStrong()
    {
        Tween_Stop(0.05f,0.6f,Ease.InSine);
        Tween_ShakeStrong_Core();
    }
    public void Tween_ShakeStrong_Core()
    {
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
    }
    public void Tween_ShakeSmooth()
    {
        Tween_Shake(0.5f,25,GameManager.V3_One * 0.125f,Ease.OutSine);
    }
    public void Tween_ShakeSuperArmor()
    {
        Tween_Vignette(0.074f,0.125f,0.5f,vignette_HitColor,vignette_NormalColor);
        Tween_Stop(0.05f,0.15f,Ease.InQuad);
        Tween_Shake(0.3f,25,GameManager.V3_One * 0.2f,Ease.OutSine);
        Tween_Chromatic(0.015f,1.5f,Ease.OutCirc);
    }
    public void Tween_ShakeStrong_Hero()
    {
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
        Tween_Speedline(0.075f,0.125f,0.5f,new Color(1,0,0,10.0f/255.0f),0.375f);
        Tween_Zoom(0.075f,0.125f,0.5f,0,orthographicSize-0.25f);
        Tween_Vignette(0.075f,0.125f,0.5f,vignette_HitColor,vignette_NormalColor);
        Tween_Stop(0.05f,0.4f,Ease.OutQuad);
        Tween_Shake(0.3f,25,GameManager.V3_One * 0.25f,Ease.OutSine);
        Tween_Chromatic(0.035f,1.5f,Ease.OutCirc);
    }

    public void Tween_ShakeDeath_Hero()
    {
        float begin = 0.1f, delay = 0.5f, fin = 2.0f;
        GameManager.Instance.Shockwave(transform.position + Vector3.up*3,1.0f,0.2f,0,Ease.OutSine);
        Tween_Vignette(0.15f,0.375f,0.5f,vignette_HitColor,vignette_NormalColor);
        Tween_Stop(0.05f,begin + delay,Ease.InQuad);
        Tween_Shake(begin + delay,Mathf.RoundToInt((begin + delay)*80),GameManager.V3_One * 0.35f,Ease.OutSine);
        Tween_Zoom(begin,delay,fin,0,orthographicSize-1.25f);
        Tween_Chromatic(0.1f,begin + delay + fin,Ease.OutCirc);
        Tween_Radial(begin,delay,fin,0.1f);
        Tween_Bloom(begin, delay, fin, 7.5f);


        uiIngameResult.Failed_Begin();
        return;
        tShakeDeathHero.Stop();
        tShakeDeathHero = Tween.Delay(begin + delay, () => uiIngameResult.Failed_Begin(),useUnscaledTime:true);
    }
    public void Tween_ShakeDown()
    {
        Tween_Shake(0.75f,20,Vector3.down * 0.175f,Ease.OutSine);
    }
    public void Tween_ShakeWeak()
    {
        Tween_Stop(0.05f,0.2f,Ease.OutQuad);
    }
    public void Tween_JustEvade()
    {
        Tween_Vignette(0.25f,0.5f,0.25f);
        Tween_Frame(0.25f,0.5f,0.25f);
        Tween_Speedline(0.25f,0.5f,0.25f,new Color(1.0f,1.0f,1.0f,6.0f/255.0f),0.675f);
        Tween_Zoom(0.25f,0.5f,0.25f,0,orthographicSize-0.5f);
        Tween_Chromatic(0.05f,1.0f,Ease.InQuad);
        Tween_Stop(0.4f,1.0f,Ease.InOutBack);
        Tween_Shake(1.0f,20,GameManager.V3_One * 0.15f,Ease.OutSine);
    }
    public void Tween_Skill()
    {
        Tween_Vignette(0.25f,0.4f,0.5f);
        Tween_Frame(0.25f,0.4f,0.5f);
        Tween_Zoom(0.4f,0.4f,0.75f,0,3.0f);
        Tween_Speedline(0.25f,0.15f,1.0f,new Color(1,1,1,10.0f/255.0f),0.675f);
        Tween_Stop(0.2f,0.65f,Ease.InExpo);
        Tween_Shake(1.0f,32,GameManager.V3_One * 0.1f,Ease.InExpo);
        Tween_Chromatic(0.035f,1.0f,Ease.InQuad);
    }
    public void Tween_SkillLong()
    {
        Tween_Vignette(0.25f,0.8f,0.5f);
        Tween_Frame(0.25f,0.8f,0.5f);
        Tween_Speedline(0.25f,0.3f,2.0f,new Color(1,1,1,10.0f/255.0f),0.675f);
        //Tween_Shake(2.0f,64,GameManager.V3_One * 0.1f,Ease.InExpo);
        Tween_Chromatic(0.035f,2.0f,Ease.InQuad);
    }
    public void Tween_ResetTimescale()
    {
        t_time.Complete();
    }
    //싱글 이펙트. 복합에서 호출한다.
    public void Tween_Radial(float begin,float delay,float fin,float intensity)
    {
        seqRadial.Stop();
        seqRadial = Sequence.Create(useUnscaledTime:true);
        seqRadial.Chain(Tween.MaterialProperty(mRadialblur, id_radial, intensity, begin));
        seqRadial.ChainDelay(delay);
        seqRadial.Chain(Tween.MaterialProperty(mRadialblur, id_radial, 0, fin));
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
    public void Tween_Zoom(float begin, float delay, float fin, float startDelay,float zoom,bool useUnscaledTime = true)
    {
        if (_uiZoom) return;
        seqUIZoom.Stop();
        s_zoom.Complete();
        s_zoom = Sequence.Create(useUnscaledTime:useUnscaledTime);
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
                mSpeedline.SetColor(GameManager.s_colour, Color.Lerp(clearColor, color, val));
                mSpeedline.SetFloat(GameManager.s_unscaledtime,  Time.unscaledTime*speed);
            }))
            .Group(Tween.Custom(0, 1, delay, onValueChange: val =>
            {
                mSpeedline.SetFloat(GameManager.s_unscaledtime,  Time.unscaledTime*speed);
            }))
            .Chain(Tween.Custom(1, 0, fin, onValueChange: val =>
            {
                mSpeedline.SetColor(GameManager.s_colour, Color.Lerp(clearColor, color, val));
                mSpeedline.SetFloat(GameManager.s_unscaledtime,  Time.unscaledTime*speed);
            }));
    }
    public void Tween_Stop(float timescale,float duration,Ease ease)
    {
        if (_uiStopped) return;
        tUIStopped.Stop();
        t_time.Stop();
        t_time = Tween.Custom(timescale, 1, duration, onValueChange: val =>
        {
            if(!_uiStopped)Time.timeScale = val;
        }, ease: ease,useUnscaledTime:true);
    }
    public void Tween_Shake(float shakeDuration,int shakeFrequency,Vector3 shakeVec,Ease ease)
    {
        t_shake.Stop();
        _camT.transform.localPosition =GameManager.V3_Zero;
        t_shake = Tween.ShakeLocalPosition(_camT, shakeVec, shakeDuration, shakeFrequency,
            easeBetweenShakes: ease, useUnscaledTime: true);
    }
    public void Tween_Chromatic(float strength,float duration,Ease ease,float startDelay)
    {
        t_chromatic.Stop();
        t_chromatic = Tween.Custom(strength, 0.001f, duration, ease: ease, useUnscaledTime: true,startDelay: startDelay,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
    }
    public void Tween_Chromatic(float strength,float duration,Ease ease)
    {
        t_chromatic.Stop();
        t_chromatic = Tween.Custom(strength, 0.001f, duration, ease: ease, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
    }
    public void Tween_Angle(float begin,float delay,float fin,float angle = -30)
    {
        s_frame.Stop();
        float startVal = BeautifySettings.settings.frameBandVerticalSize.value;
        s_frame = Sequence.Create(useUnscaledTime: false)
            .Chain(Tween.Custom(0, angle, begin, ease: Ease.InOutSine,
                onValueChange: newVal => _camT.localRotation = Quaternion.Euler(newVal,0, 0)))
            .ChainDelay(delay)
            .Chain(Tween.Custom(angle, 0, fin, ease: Ease.InOutSine,
                onValueChange: newVal => _camT.localRotation = Quaternion.Euler(newVal, 0, 0)));
    }
    public void Tween_Invert(float duration)
    {
        tInvert.Stop();
        mInvert.SetFloat(GameManager.s_screen_colour,0);
        tInvert = Tween.Delay(duration, () => mInvert.SetFloat(GameManager.s_screen_colour, 1));
        
        return;
        Tween_Stop(0.05f,0.5f,Ease.InSine);
        Tween_Shake(0.375f,45,GameManager.V3_One * 0.3f,Ease.OutSine);
        Tween_Zoom(0.15f,0.35f,1.0f,0,orthographicSize-0.5f);
        Tween_Chromatic(0.035f,1.5f,Ease.OutCirc,0.1f);
    }

    public void Tween_Bloom(float begin,float delay,float fin,float intensity)
    {
        float defaultBloom = 1.0f;
        seqBloom.Stop();
        BeautifySettings.settings.bloomIntensity.value = defaultBloom;
        
        seqBloom = Sequence.Create(useUnscaledTime:true);
        seqBloom.Chain(Tween.Custom(defaultBloom, intensity,begin, 
            onValueChange: val => BeautifySettings.settings.bloomIntensity.value = val));
        seqBloom.ChainDelay(delay);
        seqBloom.Chain(Tween.Custom(intensity, defaultBloom,fin, 
            onValueChange: val => BeautifySettings.settings.bloomIntensity.value = val));
    }
    //연출용. 씬 매니저 등에서만 호출한다. 함부로 막 쓰지 말것.
    public void Tween_UIPurkinje(float value,float duration,float delay=0)
    {
        t_purkinjeLuminanceThreshold.Stop();
        float startValue = BeautifySettings.settings.purkinjeLuminanceThreshold.value;
        t_purkinjeLuminanceThreshold = Tween.Custom(startValue, value,duration, onValueChange: val =>
        {
            BeautifySettings.settings.purkinjeLuminanceThreshold.value = val;
        },startDelay:delay,useUnscaledTime:true);

    }
    public void Tween_UIStopped(bool stop,float duration, float delay)
    {
        float startTimeScale = Time.timeScale;
        _uiStopped = stop;
        tUIStopped.Stop();
        tUIStopped = Tween.Custom(startTimeScale, stop ? 0.0f : 1.0f, duration, onValueChange: val =>
        {
            Time.timeScale = val;
        }, useUnscaledTime: true);
    }
    public void Tween_UIZoom(bool zoom,float duration,float delay,Ease ease,int zoomStrength = 1)
    {
        _uiZoom = zoom;
        s_zoom.Stop();
        t_shake.Stop();
        seqUIZoom.Stop();
        seqUIZoom = Sequence.Create(useUnscaledTime:true);
        foreach (var cam in _cams)seqUIZoom.Group(Tween.CameraOrthographicSize(cam,
            zoom ? orthographicSize + 0.125f*zoomStrength : orthographicSize, duration, ease,startDelay:delay));
    }
    //---
    public bool Get_FollowTarget()
    {
        return followTarget;
    }
    
}
