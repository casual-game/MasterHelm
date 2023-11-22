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
    [FoldoutGroup("Effect/data")] public Color vignette_NormalColor, vignette_HitColor,vignette_SuperArmorColor;
    //Private
    private Transform _camT;
    private Camera[] _cams;
    private Tween t_stop,t_shake,t_chromatic,t_radial;
    private Sequence s_zoom,s_impact,s_vignette,s_speedline;
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
        transform.position = Vector3.Lerp(transform.position,target.position + addVec,moveSpeed*Time.unscaledDeltaTime);
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
    public void Tween_ShakeSuperArmor()
    {
        Tween_Radial(0.15f);
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Complete();
        s_vignette.Complete();
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
        //Vignette
        float begin = 0.075f, delay = 0.125f, fin = 0.5f;
        s_vignette = Sequence.Create()
            .Chain(Tween.Custom(0.75f, 1.0f, begin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(vignette_NormalColor, vignette_HitColor, begin, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(1.0f, 0.75f, fin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(vignette_HitColor, vignette_NormalColor, fin, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal));
    }

    public void Tween_ShakeStrong_Hero()
    {
        Tween_Radial(0.5f);
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Stop();
        s_zoom.Complete();
        s_vignette.Complete();
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
        //Vignette
        float begin = 0.15f, delay = 0.25f, fin = 0.5f;
        
        s_vignette = Sequence.Create()
            .Chain(Tween.Custom(0.75f, 1.0f, begin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(vignette_NormalColor, vignette_HitColor, begin, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(1.0f, 0.75f, fin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(vignette_HitColor, vignette_NormalColor, fin, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal));

    }
    public void Tween_ShakeNormal_Hero()
    {
        Tween_Radial(0.25f);
        t_shake.Complete();
        t_stop.Complete();
        t_chromatic.Stop();
        s_vignette.Complete();
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
        //Vignette
        float begin = 0.075f, delay = 0.125f, fin = 0.5f;
        s_vignette = Sequence.Create()
            .Chain(Tween.Custom(0.75f, 1.0f, begin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(vignette_NormalColor, vignette_HitColor, begin, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(1.0f, 0.75f, fin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .Group(Tween.Custom(vignette_HitColor, vignette_NormalColor, fin, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingColor.value = newVal));

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
    public void Tween_Radial(float duration,float bluramount =0.1f)
    {
        t_radial.Stop();
        cp_radial.Activate();
        m_radialblur.SetFloat(GameManager.s_bluramount,bluramount);
        t_radial = Tween.MaterialProperty(m_radialblur, id_radial, 0.0f, duration,useUnscaledTime: true)
            .OnComplete(target:this, target=> target.cp_radial.Deactivate());
    }
    public void Tween_Impact(ParticleSystem particle,int damage)
    {
        Tween_Radial(0.5f);
        s_impact.Stop();
        t_stop.Stop();
        
        t_chromatic.Stop();
        //Speedline
        s_speedline.Stop();
        m_speedline.SetColor(GameManager.s_colour, Color.white);
        s_speedline = Sequence.Create().ChainDelay(0.1f,true).ChainCallback(() =>
        {
            m_speedline.SetColor(GameManager.s_colour, Color.clear);
        });
        //Impact
        cp_invert.Activate();
        s_impact = Sequence.Create()
            .ChainDelay(0.1f,useUnscaledTime: true).ChainCallback(() =>
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
            .Chain(Tween.CameraOrthographicSize(_cams[0], 3.5f, 0.15f, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], 3.5f, 0.15f, useUnscaledTime: true))
            .ChainDelay(0.35f,useUnscaledTime:true)
            .Chain(Tween.CameraOrthographicSize(_cams[0], 4.0f, 1.0f, Ease.OutCirc, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], 4.0f, 1.0f, Ease.OutCirc, useUnscaledTime: true));
        
        
    }
    public void Tween_JustEvade()
    {
        float duration = 0.5f;
        Tween_Radial(duration,0.075f);
        //Stop
        t_stop.Stop();
        t_stop = Tween.GlobalTimeScale(0.2f, 1.0f, 0.5f, ease: Ease.InExpo);
        //Shake Normal
        t_shake.Stop();
        _camT.transform.SetLocalPositionAndRotation(GameManager.V3_Zero,GameManager.Q_Identity);
        t_shake = Tween.ShakeLocalPosition(_camT, GameManager.V3_One * 0.25f, duration, duration*25,
            easeBetweenShakes: Ease.OutSine, useUnscaledTime: true);
        //Chromatic
        t_chromatic.Stop();
        t_chromatic = Tween.Custom(0.035f, 0.001f, 1.0f, ease: Ease.OutCirc, useUnscaledTime: true,
            onValueChange: newVal => BeautifySettings.settings.chromaticAberrationIntensity.value = newVal);
        //Vignette
        s_vignette.Complete();
        float begin = 0.075f, delay = 0.5f, fin = 0.4f;
        s_vignette = Sequence.Create()
            .Chain(Tween.Custom(0.75f, 1.0f, begin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .ChainDelay(delay, true)
            .Chain(Tween.Custom(1.0f, 0.75f, fin, ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange: newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal));
    }
    public void Tween_Skill()
    {
        float duration = 0.5f;
        Tween_Radial(duration,0.075f);
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

        float begin = 0.25f, delay = 0.4f, fin = 0.5f;
        //Vignette
        s_vignette.Complete();
        s_vignette = Sequence.Create()
            .Chain(Tween.Custom(0.75f,1.0f,begin,ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange:newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal))
            .ChainDelay(delay,true)
            .Chain(Tween.Custom(1.0f,0.75f,fin,ease: Ease.OutCirc, useUnscaledTime: true,
                onValueChange:newVal => BeautifySettings.settings.vignettingInnerRing.value = newVal));
        //Zoom
        s_zoom.Complete();
        s_zoom = Sequence.Create()
            .Chain(Tween.CameraOrthographicSize(_cams[0], 3.5f, begin, useUnscaledTime: true,ease: Ease.OutCirc))
            .Group(Tween.CameraOrthographicSize(_cams[1], 3.5f, begin, useUnscaledTime: true,ease: Ease.OutCirc))
            .ChainDelay(delay,true)
            .Chain(Tween.CameraOrthographicSize(_cams[0], 4.0f, fin, Ease.OutCirc, useUnscaledTime: true))
            .Group(Tween.CameraOrthographicSize(_cams[1], 4.0f, fin, Ease.OutCirc, useUnscaledTime: true));
        /*
         //Speedline
        s_speedline.Stop();
        s_speedline = Sequence.Create()
            .Chain(Tween.Custom(Color.clear, Color.white*0.65f, 0.2f,
                useUnscaledTime: true, ease: Ease.OutQuad, onValueChange: newVal =>
                {
                    m_speedline.SetColor(GameManager.s_colour, newVal);
                    m_speedline.SetFloat(GameManager.s_unscaledtime, Time.unscaledTime);
                }))
            .Chain(Tween.Custom(0.0f,1.0f,0.3f,useUnscaledTime:true,onValueChange:val
                => m_speedline.SetFloat(GameManager.s_unscaledtime, Time.unscaledTime)))
            .Chain(Tween.Custom(Color.white*0.65f, Color.clear, 0.5f,
                useUnscaledTime: true, ease: Ease.InCirc, onValueChange: newVal =>
                {
                    m_speedline.SetColor(GameManager.s_colour, newVal);
                    m_speedline.SetFloat(GameManager.s_unscaledtime, Time.unscaledTime);
                }));
         */
    }
}
