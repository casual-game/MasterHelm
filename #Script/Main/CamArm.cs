using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beautify.Universal;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using DG.Tweening;
using DynamicFogAndMist2;

public class CamArm : MonoBehaviour
{
    
    [TitleGroup("Main Setting")][BoxGroup("Main Setting/Base")] public static CamArm instance;
    [TitleGroup("Main Setting")][BoxGroup("Main Setting/Base")] public Transform target;
    [TitleGroup("Main Setting")][BoxGroup("Main Setting/Base")] public float moveDamp = 5.0f,maxDistance = 2.0f;
    [TitleGroup("Main Setting")] [BoxGroup("Main Setting/Base")] public AudioMixerGroup mixerGroup;
    [TitleGroup("Main Setting")] [BoxGroup("Main Setting/Base")] public float mainTargetFollowRatio = 1.0f;
    [TitleGroup("Main Setting")] [BoxGroup("Main Setting/Base")] 
    public ParticleSystem speedline_loop,speedline_once,speedline_special;

    [TitleGroup("Main Setting")] [BoxGroup("Main Setting/Base")]
    public DynamicFog fog;
    
    [HideInInspector] public UnityEvent cameraEffects;
    [HideInInspector] public Camera mainCam, uiCam;
    private Vector2 fovRange_Horizonal;
    private Transform productionT;
    private float startFixedDeltaTime,startMaxDeltaTime;
    #region Chromatic

    private float chromatic_Default = 0.003f,chromatic_Intensity;
    private float chromatic_Increase_BeginTime,chromatic_Increase_FinTime, chromatic_Decrease_FinTime;
    private void Chromatic(float intensity,float increase_Duration,float decrease_Duration)
    {
        if (Player.instance.death) return;
        cameraEffects.RemoveListener(Chromatic_Increase);
        cameraEffects.RemoveListener(Chromatic_Decrease);
        chromatic_Intensity = intensity;
        chromatic_Increase_BeginTime = Time.unscaledTime;
        chromatic_Increase_FinTime = chromatic_Increase_BeginTime + increase_Duration;
        chromatic_Decrease_FinTime = chromatic_Increase_FinTime + decrease_Duration;
        
        
        cameraEffects.AddListener(Chromatic_Increase);
    }

    private void Chromatic_Increase()
    {
        float ratio = Ratio(Time.unscaledTime, chromatic_Increase_BeginTime, chromatic_Increase_FinTime);
        BeautifySettings.settings.chromaticAberrationIntensity.Override(Mathf.Lerp(chromatic_Default,chromatic_Intensity,ratio));
        if (ratio > 0.99f)
        {
            cameraEffects.RemoveListener(Chromatic_Increase);
            cameraEffects.AddListener(Chromatic_Decrease);
        }
    }

    private void Chromatic_Decrease()
    {
        float ratio = Ratio(Time.unscaledTime, chromatic_Increase_FinTime, chromatic_Decrease_FinTime);
        BeautifySettings.settings.chromaticAberrationIntensity.Override(Mathf.Lerp(chromatic_Intensity,chromatic_Default,ratio));
        if (ratio > 0.99f) cameraEffects.RemoveListener(Chromatic_Decrease);
    }
    #endregion
    #region Impact
    private Transform shakeT;
    private Vector3 shakeT_OriginalPos;
    private Quaternion shakeT_OriginalRot;
    private float lastImpactTime,lastImpactStrength;
    private Data_Impact lastImpact;
    private float impact_importantTime;
    public void Impact(Data_Impact impact,bool chromatic = true,bool stop = true,bool hit = true)
    {
        if (!impact.isImportant && Time.unscaledTime < impact_importantTime) return;
        if (Player.instance.death) return;
        if (impact.isImportant)
        {

            float maxDuration = Mathf.Max(impact.pos_shakeDuration,
                impact.hit_increase_duration + impact.hit_dercrease_duration
                , impact.chromatic_increase_duration + impact.chromatic_dercrease_duration, impact.stopDuration);
            impact_importantTime = Time.unscaledTime + maxDuration;
        }
        float lastImpactEndTime = lastImpact !=null?lastImpactTime+ lastImpact.pos_shakeDuration:0;
        float currentImpactEndTime = Time.unscaledTime+ impact.pos_shakeDuration;
        if (currentImpactEndTime < lastImpactEndTime && impact.pos_shakeStrength-0.1f<lastImpactStrength) return;

        lastImpact = impact;
        lastImpactStrength = impact.pos_shakeStrength;
        //Shake
        DOTween.Kill(shakeT);
        shakeT.transform.localPosition = shakeT_OriginalPos;
        shakeT.transform.localRotation = shakeT_OriginalRot;
        shakeT.DOShakePosition(impact.pos_shakeDuration,impact.pos_shakeStrength,impact.pos_shakeVibrato)
            .SetEase(impact.pos_shakeEase).SetUpdate(true);
        //Chromatic
        if (chromatic)
        {
            if(impact.chromatic_dercrease_duration>0.01f)
                Chromatic(impact.chromaticStrength,impact.chromatic_increase_duration,impact.chromatic_dercrease_duration);
        }
        //Stop
        if (stop)
        {
            if (impact.stopDuration > 0.01f)
                Stop(impact.stopDuration, impact.stopScale, 
                    impact.stopDuration2, impact.stopScale2);
        }
        //Hit
        if (hit)
        {
            if(impact.hit_dercrease_duration>0.01f)
                Hit(impact.hit_increase_duration,impact.hit_dercrease_duration,impact.hit_Color,impact.hit_Intensity);
        }
    }

    #endregion
    #region Stop
    private float stopEndTime,stopScale;
    private Coroutine c_stop;
    private void Stop(float duration,float scale,float duration2,float scale2)
    {
        if (Player.instance.death) return;
        if (Canvas_Player.instance.death_Activated) return;
        Manager_Main.instance.Vibrate(duration,1);
        if(c_stop!=null) StopCoroutine(c_stop);
        c_stop = StartCoroutine(C_Stop(duration, scale, duration2, scale2));
    }

    private IEnumerator C_Stop(float duration,float scale,float duration2,float scale2)
    {
        stopEndTime = Time.unscaledTime + duration+duration2;
        stopScale = scale;
        Time.timeScale = stopScale;
        Time.fixedDeltaTime = Time.timeScale*startFixedDeltaTime;
        Time.maximumDeltaTime = Time.timeScale*startMaxDeltaTime;
        yield return new WaitForSecondsRealtime(duration);
        if (duration2 < 0.01f)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = Time.timeScale*startFixedDeltaTime;
            Time.maximumDeltaTime = Time.timeScale*startMaxDeltaTime;
            yield break;
        }
        
        stopScale = scale2;
        Time.timeScale = stopScale;
        Time.fixedDeltaTime = Time.timeScale*startFixedDeltaTime;
        Time.maximumDeltaTime = Time.timeScale*startMaxDeltaTime;
        yield return new WaitForSecondsRealtime(duration2);
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale*startFixedDeltaTime;
        Time.maximumDeltaTime = Time.timeScale*startMaxDeltaTime;
    }
    #endregion
    #region Hit

    private Color hitColor = new Color(50.0f / 255, 0, 0);
    private float hit_IncreaseBeginTime,hit_DecreaseBeginTime,hit_FinTime,hit_Intensity =1.0f;
    private Color hit_OriginalColor = new Color(0,0,0,244.0f/255.0f);
    private void Hit(float increaseDuration,float decreaseDuration,Color hitColor,float intensity = 1.0f)
    {
        if (Player.instance.death) return;
        this.hitColor = hitColor;
        cameraEffects.RemoveListener(Hit_Increase);
        cameraEffects.RemoveListener(Hit_Decrease);
        hit_Intensity = intensity;
        hit_IncreaseBeginTime = Time.unscaledTime;
        hit_DecreaseBeginTime = hit_IncreaseBeginTime + increaseDuration;
        hit_FinTime = hit_DecreaseBeginTime + decreaseDuration;
        cameraEffects.AddListener(Hit_Increase);
    }

    private void Hit_Increase()
    {
        float ratio = Ratio(Time.unscaledTime, hit_IncreaseBeginTime, hit_DecreaseBeginTime);
        BeautifySettings.settings.vignettingColor.Override(Color.Lerp(hit_OriginalColor,hitColor * hit_Intensity, ratio));
        if (ratio > 0.99f)
        {
            cameraEffects.RemoveListener(Hit_Increase);
            cameraEffects.AddListener(Hit_Decrease);
        }
    }
    private void Hit_Decrease()
    {
        float ratio = Ratio(Time.unscaledTime, hit_DecreaseBeginTime, hit_FinTime);
        BeautifySettings.settings.vignettingColor.Override(Color.Lerp(hitColor * hit_Intensity, hit_OriginalColor, ratio));
        if (ratio > 0.99f) cameraEffects.RemoveListener(Hit_Decrease);
    }
    #endregion
    #region Particle
    public void SpeedLine_Play(bool loop , float deg)
    {
        while (deg <= 0) deg += 360;
        if (deg < 90) deg = 45;
        else if (deg < 180) deg = 135;
        else if (deg < 270) deg = 225;
        else deg = 315;
        speedline_loop.transform.localRotation = Quaternion.Euler(0,deg,0);
        speedline_once.transform.localRotation = Quaternion.Euler(0,deg,0);
        if(loop) speedline_loop.Play();
        else speedline_once.Play();
    }
    public void SpeedLine_Play(bool loop)
    {
        float deg= Player.instance.transform.rotation.eulerAngles.y-45;
        SpeedLine_Play(loop,deg);
    }
    public void SpeedLine_Play(bool loop,Transform targetT)
    {
        
        float  deg;
        Vector3 lookVec = targetT.position - Player.instance.transform.position;
        lookVec.y = 0;
        deg = Quaternion.LookRotation(lookVec).eulerAngles.y - 45;
        SpeedLine_Play(loop,deg);
    }
    public void SpeedLine_Play(bool loop,Vector3 lookVec)
    {
        lookVec.y = 0;
        float deg = Quaternion.LookRotation(lookVec).eulerAngles.y - 45;
        SpeedLine_Play(loop,deg);
    }
    [Button]
    public void SpeedLine_Stop()
    {
        speedline_loop.Stop(true,ParticleSystemStopBehavior.StopEmitting);
    }

    public void SpeedLine_Special()//게임 종료시 나오는 그 섬광 줄기이다.
    {
        speedline_special.Play();
    }
    #endregion
    #region Production
    
    private float pd_frame_begin = 0, pd_frame_fin = 0.08f, pd_blur_begin = 0.15f, pd_blur_fin = 0.75f
        ,pd_temp_begin = 0.425f,pd_temp_fin = 1.0f;
    private Vector3 pd_pos_begin = Vector3.zero, pd_pos_fin = new Vector3(0,-4.94f,1.73f);
    private Vector3 pd_rot_begin = Vector3.zero, pd_rot_fin = new Vector3(-20, 0, 0);
    public AnimationCurve pd_curve;
    private Coroutine c_production = null;

    
    [Button]
    public void Production_Begin(float pd_duration_begin = 1.4f)
    {
        if(c_production!= null) StopCoroutine(c_production);
        c_production = StartCoroutine(C_Production_Begin(pd_duration_begin));
    }
    [Button]
    public void Production_Fin(float pd_duration_fin = 1.0f)
    {
        if(c_production!= null) StopCoroutine(c_production);
        c_production = StartCoroutine(C_Production_Fin(pd_duration_fin));
    }
    public void Production_FinNow()
    {
        BeautifySettings.settings.frameBandVerticalSize.value = pd_frame_begin;
        BeautifySettings.settings.blurIntensity.value = pd_blur_begin;
        productionT.localPosition =pd_pos_begin;
        productionT.localRotation = Quaternion.Euler(pd_rot_begin);
    }
    public void Production_BeginNow()
    {
        BeautifySettings.settings.frameBandVerticalSize.value = pd_frame_fin;
        BeautifySettings.settings.blurIntensity.value = pd_blur_fin;
        productionT.localPosition =pd_pos_fin;
        productionT.localRotation = Quaternion.Euler(pd_rot_fin);
    }
    IEnumerator C_Production_Begin(float pd_duration_begin)
    {
        float startTime = Time.unscaledTime;
        Quaternion rot_begin = Quaternion.Euler(pd_rot_begin), rot_fin = Quaternion.Euler(pd_rot_fin);
        while (true)
        {
            float ratio = (Time.unscaledTime - startTime) / pd_duration_begin;
            if (ratio > 0.99f) break;
            float evaluatedRatio = pd_curve.Evaluate(ratio);
            float frame = pd_frame_begin*(1-evaluatedRatio) + pd_frame_fin*evaluatedRatio;
            float blur = pd_blur_begin*(1-evaluatedRatio) + pd_blur_fin*evaluatedRatio;
            float temp = pd_temp_begin*(1-evaluatedRatio) + pd_temp_fin*evaluatedRatio;
            Vector3 pos = pd_pos_begin*(1-evaluatedRatio) + pd_pos_fin*evaluatedRatio;
            Quaternion rot = Quaternion.Lerp(rot_begin,rot_fin,evaluatedRatio);

            BeautifySettings.settings.frameBandVerticalSize.value = frame;
            BeautifySettings.settings.blurIntensity.value = blur;
            BeautifySettings.settings.colorTempBlend.value = temp;
            productionT.localPosition =pos;
            productionT.localRotation = rot;
            yield return null;
        }
        
        BeautifySettings.settings.frameBandVerticalSize.value = pd_frame_fin;
        BeautifySettings.settings.blurIntensity.value = pd_blur_fin;
        BeautifySettings.settings.colorTempBlend.value = pd_temp_fin;
        productionT.localPosition =pd_pos_fin;
        productionT.localRotation = rot_fin;
        
    }
    IEnumerator C_Production_Fin(float pd_duration_fin = 1.0f)
    {
        float startTime = Time.unscaledTime;
        Quaternion rot_begin = Quaternion.Euler(pd_rot_begin), rot_fin = Quaternion.Euler(pd_rot_fin);
        while (true)
        {
            float ratio = (Time.unscaledTime - startTime) / pd_duration_fin;
            if (ratio > 0.99f) break;
            float evaluatedRatio = pd_curve.Evaluate(ratio);
            float frame = pd_frame_fin*(1-evaluatedRatio) + pd_frame_begin*evaluatedRatio;
            float blur = pd_blur_fin*(1-evaluatedRatio) + pd_blur_begin*evaluatedRatio;
            float temp = pd_temp_fin*(1-evaluatedRatio) + pd_temp_begin*evaluatedRatio;
            Vector3 pos = pd_pos_fin*(1-evaluatedRatio) + pd_pos_begin*evaluatedRatio;
            Quaternion rot = Quaternion.Lerp(rot_fin,rot_begin,evaluatedRatio);

            BeautifySettings.settings.frameBandVerticalSize.value = frame;
            BeautifySettings.settings.blurIntensity.value = blur;
            BeautifySettings.settings.colorTempBlend.value = temp;
            productionT.localPosition =pos;
            productionT.localRotation = rot;
            
            yield return null;
        }
        
        BeautifySettings.settings.frameBandVerticalSize.value = pd_frame_begin;
        BeautifySettings.settings.blurIntensity.value = pd_blur_begin;
        BeautifySettings.settings.colorTempBlend.value = pd_temp_begin;
        productionT.localPosition =pd_pos_begin;
        productionT.localRotation = rot_begin;
        
    }
    #endregion

    #region Cutscene

    public AnimationCurve cutsceneCurve;
    public float cutsceneMaxRatio = 0.5f;
    private float cutsceneRatio = 0.0f;
    private float cutscene_duration = 1.0f;
    private Transform cutsceneT = null;
    private Coroutine c_cutscene = null;

    public void Cutscene(float startDelay, float delay1,float delay2, Transform t,Barricade b = null)
    {
        if(c_cutscene!=null) StopCoroutine(c_cutscene);
        c_cutscene = StartCoroutine(C_Cutscene(startDelay, delay1,delay2, t,b));
    }
    IEnumerator C_Cutscene(float startDelay, float delay1,float delay2, Transform t,Barricade b = null)
    {
        
        cutsceneT = t;
        yield return new WaitForSecondsRealtime(startDelay);
        float startTime = Time.unscaledTime;
        while (true)
        {
            float ratio = (Time.unscaledTime - startTime) / (cutscene_duration*1.5f);
            if (ratio > 0.99f) break;
            cutsceneRatio = cutsceneCurve.Evaluate(ratio)*cutsceneMaxRatio;
            yield return null;
        }
        cutsceneRatio = cutsceneMaxRatio;
        yield return new WaitForSecondsRealtime(delay1);
        if(b!=null) b.Open();
        
        yield return new WaitForSecondsRealtime(delay2);
        
        startTime = Time.unscaledTime;
        while (true)
        {
            float ratio = (Time.unscaledTime - startTime) / cutscene_duration;
            if (ratio > 0.99f) break;
            cutsceneRatio = (1-cutsceneCurve.Evaluate(ratio))*cutsceneMaxRatio;
            yield return null;
        }
        CamArm.instance.Production_Fin();
        cutsceneRatio = 0;
        yield return null;
    }

    #endregion
    #region FOV
    //startFOV
    private Coroutine c_fov;
    public AnimationCurve startFOVCurve;
    private float startFOV_begin = 7, startFOV_fin = 4,
        startFOVFog_begin = 1.0f,startFOVFog_fin=0.2f,
        startScaleFog_begin = 0.06f,startScaleFog_fin=0.05f;

    public void StartFOVNow()
    {
        mainCam.orthographicSize = startFOV_fin;
        uiCam.orthographicSize = startFOV_fin;
        fog.profile.noiseColorBlend = startFOVFog_fin;
        fog.profile.scale = startScaleFog_fin;
        fog.UpdateMaterialProperties();
    }
    public void StartFOV()
    {
        if(c_fov!=null) StopCoroutine(c_fov);
        c_fov = StartCoroutine(C_StartFOV());
    }
    IEnumerator C_StartFOV()
    {
        float startTime = Time.unscaledTime;
        float duration = 2.5f;
        while (true)
        {
            float ratio = (Time.unscaledTime - startTime) / duration;
            if (ratio > 0.99f) break;
            float evaluatedRatio = startFOVCurve.Evaluate(ratio);
            float fov = Mathf.Lerp(startFOV_begin, startFOV_fin, evaluatedRatio);
            float density = startFOVFog_begin*(1-evaluatedRatio) + startFOVFog_fin*evaluatedRatio;
            float scale = startScaleFog_begin*(1-evaluatedRatio) + startScaleFog_fin*evaluatedRatio;
            
            mainCam.orthographicSize = fov;
            uiCam.orthographicSize = fov;
            fog.profile.noiseColorBlend = density;
            fog.profile.scale = scale;
            
            fog.UpdateMaterialProperties();
            yield return null;
        }
        
        mainCam.orthographicSize = startFOV_fin;
        uiCam.orthographicSize = startFOV_fin;
        fog.profile.noiseColorBlend = startFOVFog_fin;
        fog.profile.scale = startScaleFog_fin;
        fog.UpdateMaterialProperties();
    }
    //clearFOV
    public void ClearFOV(float duration = 2.5f,float _fov = 7,float _density = 1.0f)
    {
        if(c_fov!=null) StopCoroutine(c_fov);
        c_fov = StartCoroutine(C_ClearFOV(duration,_fov,_density));
    }
    IEnumerator C_ClearFOV(float duration = 2.5f,float _fov = 7,float _density = 1.0f)
    {
        float startTime = Time.unscaledTime;
        
        while (true)
        {
            float ratio = (Time.unscaledTime - startTime) / duration;
            if (ratio > 0.99f) break;
            float evaluatedRatio = startFOVCurve.Evaluate(ratio);
            float fov = startFOV_fin*(1-evaluatedRatio) + _fov*evaluatedRatio;
            float density = startFOVFog_fin*(1-evaluatedRatio) + _density*evaluatedRatio;
            float scale = startScaleFog_fin*(1-evaluatedRatio) + startScaleFog_begin*evaluatedRatio;
            
            mainCam.orthographicSize = fov;
            uiCam.orthographicSize = fov;
            fog.profile.noiseColorBlend = density;
            fog.profile.scale = scale;
            
            fog.UpdateMaterialProperties();
            yield return null;
        }
        
        mainCam.orthographicSize = _fov;
        uiCam.orthographicSize = _fov;
        fog.profile.noiseColorBlend = _density;
        fog.profile.scale = startScaleFog_begin;
        fog.UpdateMaterialProperties();
    }
    //deathFOV
    public AnimationCurve deathFOVCurve;
    public float deathFOVDuration;
    public float deathFOV_fin = 6.5f;
    private float deathFOV_begin = 4,// deathFOV_fin = 6,
        deathFOVFog_begin = 0.2f,deathFOVFog_fin=1.0f,
        deathScaleFog_begin = 0.05f,deathScaleFog_fin=0.06f;
    public void DeathFOV()
    {
        if(c_fov!=null) StopCoroutine(c_fov);
        c_fov = StartCoroutine(C_DeathFOV());
    }
    IEnumerator C_DeathFOV()
    {
        float startTime = Time.unscaledTime;
        float duration = deathFOVDuration;
        while (true)
        {
            float ratio = (Time.unscaledTime - startTime) / duration;
            if (ratio > 0.99f) break;
            float evaluatedRatio = deathFOVCurve.Evaluate(ratio);
            float fov = deathFOV_begin*(1-evaluatedRatio) + deathFOV_fin*evaluatedRatio;
            float density = deathFOVFog_begin*(1-evaluatedRatio) + deathFOVFog_fin*evaluatedRatio;
            float scale = deathScaleFog_begin*(1-evaluatedRatio) + deathScaleFog_fin*evaluatedRatio;
            
            mainCam.orthographicSize = fov;
            uiCam.orthographicSize = fov;
            fog.profile.noiseColorBlend = density;
            fog.profile.scale = scale;
            fog.UpdateMaterialProperties();
            BeautifySettings.settings.purkinjeLuminanceThreshold.value = Mathf.Clamp01(ratio*2);
            yield return null;
        }
        
        mainCam.orthographicSize = deathFOV_fin;
        uiCam.orthographicSize = deathFOV_fin;
        fog.profile.noiseColorBlend = deathFOVFog_fin;
        fog.profile.scale = deathScaleFog_fin;
        BeautifySettings.settings.purkinjeLuminanceThreshold.value = 1;
        fog.UpdateMaterialProperties();
    }
    #endregion
    [Button]
    public void MoveToTarget()
    {
        transform.position =target.transform.position;
    }
    public void Setting()
    {
        startFixedDeltaTime = Time.fixedDeltaTime;
        startMaxDeltaTime = Time.maximumDeltaTime;
        productionT = transform.GetComponentInChildren<Camera>().transform.parent;
        shakeT = productionT.transform.parent;
        shakeT_OriginalPos = shakeT.localPosition;
        shakeT_OriginalRot = shakeT.localRotation;
        mainCam = transform.GetComponentInChildren<Camera>();
        uiCam = mainCam.transform.GetChild(0).GetComponent<Camera>();
        mainCam.orthographicSize = startFOV_begin;
        uiCam.orthographicSize = startFOV_begin;
        fog.profile.noiseColorBlend = startFOVFog_begin;
        fog.profile.scale = startScaleFog_begin;
        fog.UpdateMaterialProperties();
        instance = this;
        MoveToTarget();
        myPos = transform.position;
        BeautifySettings.settings.depthOfFieldFocalLength.Override(0.03f);
        BeautifySettings.settings.depthOfFieldAperture.Override(0);
        Canvas_Player.instance.OnLateUpdate.AddListener(Cam_LateUpdate);
    }

    private Vector3 myPos,moveCurrentSmooth;
    private float target_cutscene_ratio = 0;
    private void Cam_LateUpdate()
    {
        //시간 설정
        //Time.timeScale = Time.unscaledTime>stopEndTime?1:stopScale;
        //Time.fixedDeltaTime = Time.timeScale*startFixedDeltaTime;
        //이동
        if (target == null) return;
        cameraEffects?.Invoke();
        
        
        Vector3 targetPos = Player.instance.transform.position;
        if (Player.instance.target != null || Player.instance.executedTarget!=null)
        {
            Vector3 originalPos = Player.instance.transform.position;
            Vector3 enemyPos = Vector3.zero;
            if(Player.instance.target!=null) enemyPos = Player.instance.target.transform.position;
            else if (Player.instance.executedTarget != null)
                enemyPos = Player.instance.executedTarget.transform.position;
            
            Vector3 lookVec = enemyPos - originalPos;
            float magnitude = lookVec.magnitude;
            float camMaxDist = maxDistance;
            if (Player.instance.target != null) camMaxDist *= Player.instance.target.camDistanceRatio;
            if (magnitude > maxDistance) lookVec = lookVec.normalized * camMaxDist;

            targetPos += lookVec*0.5f;
        }
        //TargetPos_Limit();
        myPos = Vector3.SmoothDamp(myPos,targetPos,ref moveCurrentSmooth,moveDamp);
        if(cutsceneT!=null)transform.position = myPos * (1 - cutsceneRatio) + cutsceneT.position * cutsceneRatio;
        else transform.position = myPos;
        
        
    }

    public static float Degree()
    {
        return instance.transform.rotation.eulerAngles.y;
        
    }

    private float Ratio(float currentTime,float BeginTime, float FinTime)
    {
        return 1-((FinTime - currentTime) / (FinTime - BeginTime));
    }
    
}
