using System;
using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using Febucci.UI;
using Febucci.UI.Core;
using PrimeTween;
using Sirenix.OdinInspector;
using SSCS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class SelectUI : MonoBehaviour
{
    [FoldoutGroup("Main")] public List<Camera> cams = new List<Camera>();
    [FoldoutGroup("Main")] public CloudShadows cloud;
    [FoldoutGroup("Main")] public UIElement_Tip tip;
    [FoldoutGroup("Main")] public UIElement_Frame frame;
    
    [FoldoutGroup("StageStart")] public CanvasGroup ssVignetteCanvasGroup,ssMainCanvasGroup,ssStartBtnCanvasGroup,
        ssMission1CanvasGroup,ssMission2CanvasGroup,ssMission3CanvasGroup;
    [FoldoutGroup("StageStart")] public TypewriterCore ssTypewriter;
    [FoldoutGroup("StageStart")] public TMP_Text ssTmp;
    [FoldoutGroup("StageStart")] public Transform ssStar1Transform, ssStar2Transform, ssStar3Transform;
    [FoldoutGroup("StageStart")] public List<Image> ssRaycastTarget = new List<Image>();
    private Tween _tBlur,_tPurkinje;
    private Sequence _seqStageStart,_seqStarBlink;
    private string _strStart = "플레이!";
    private bool _stageStart = false;
    

    public void Start()
    {
        Setting_MapControl();
        frame.Setting();
        
        _stageStart = true;
        cloud.profile.coverage = 1;
        cloud.profile.cloudsOpacity = 1;
        frame.gameObject.SetActive(false);
        foreach (var cam in cams) cam.orthographicSize = 6.5f;
        moveVec = Vector2.zero;
        MoveMap(true);
    }
    [Button]
    public void Select_Start()
    {
        _seqStageStart.Stop();
        _stageStart = true;
        cloud.profile.coverage = 1;
        cloud.profile.cloudsOpacity = 1;
        frame.gameObject.SetActive(false);
        foreach (var cam in cams) cam.orthographicSize = 6.5f;
        
        _seqStageStart = Sequence.Create();
        _seqStageStart.Chain(Tween.Custom(0, 1, 1.25f, onValueChange: ratio =>
        {
            cloud.profile.coverage = Mathf.Lerp(1.0f, 0.3f, ratio);
            cloud.profile.cloudsOpacity = Mathf.Lerp(0.75f, 0.0f, ratio);
            BeautifySettings.settings.bloomIntensity.value = Mathf.Lerp(1.5f, 0.3f, ratio);
            float orthoSize = Mathf.Lerp(6.5f, 5.0f, ratio);
            foreach (var cam in cams) cam.orthographicSize = orthoSize;
            MoveMap(true);
        }, Ease.InOutQuart));
        _seqStageStart.Group(Tween.Delay(0.5f, () => frame.Reveal(1)));
        _seqStageStart.OnComplete(() => _stageStart = false);
    }
    public void StageStart_Enter(StageBanner banner)
    {
        if (_seqStageStart.isAlive) return;
        moveVec = Vector2.zero;
        _stageStart = true;
        _seqStageStart.Complete();
        _seqStarBlink.Complete();
        Transform tSsMainCanvasGroup = ssMainCanvasGroup.transform,
            tSsStartBtnCanvasGroup = ssStartBtnCanvasGroup.transform;
        tSsMainCanvasGroup.localScale = Vector3.one*1.1f;
        tSsStartBtnCanvasGroup.localScale = Vector3.one*0.8f;
        
        ssVignetteCanvasGroup.alpha = 0;
        ssMainCanvasGroup.alpha = 0;
        ssStartBtnCanvasGroup.alpha = 0;
        ssMission1CanvasGroup.alpha = 0;
        ssMission2CanvasGroup.alpha = 0;
        ssMission3CanvasGroup.alpha = 0;
        ssMission1CanvasGroup.transform.localScale = Vector3.one*0.4f;
        ssMission2CanvasGroup.transform.localScale = Vector3.one*0.4f;
        ssMission3CanvasGroup.transform.localScale = Vector3.one*0.4f;
        ssTmp.text = String.Empty;
        ssStar1Transform.localScale = Vector3.one;
        ssStar2Transform.localScale = Vector3.one;
        ssStar3Transform.localScale = Vector3.one;
        foreach (var image in ssRaycastTarget)
        {
            image.raycastTarget = true;
        }
        ssMainCanvasGroup.transform.parent.gameObject.SetActive(true);
        
        _seqStageStart = Sequence.Create();
        _seqStarBlink = Sequence.Create();
        _seqStageStart.timeScale = 1.1f;
        //카메라 이동
        Vector3 beginPos = camT.position, endPos = ClampVec(banner.transform.position);
        float duration = Mathf.Clamp((beginPos - endPos).magnitude/7.5f, 0.5f, 0.75f);
        endPos.y = beginPos.y;
        _seqStageStart.Chain(Tween.Custom(0,1,duration,onValueChange: ratio =>
        {
            Vector3 pos = Vector3.Lerp(beginPos,endPos,ratio);
            MoveMap(pos);
        },Ease.InOutCubic));
        //foreach (var cam in cams) _seqStageStart.Group(Tween.CameraOrthographicSize(cam, 4.5f, 0.75f, Ease.InOutQuart));
        //카메라,색감
        for (int i = 0; i < cams.Count; i++)
        {
            if(i==0) _seqStageStart.Chain(Tween.CameraOrthographicSize(cams[i], 3.5f, 0.75f, Ease.InOutQuart));
            else  _seqStageStart.Group(Tween.CameraOrthographicSize(cams[i], 3.5f, 0.75f, Ease.InOutQuart));
        }
        Tween_Purkinje(0.75f,0.75f,duration);
        Tween_Blur(0.5f,0.75f,duration);
        //패널 생성
        _seqStageStart.Group(Tween.Alpha(ssVignetteCanvasGroup, 1, 0.75f,Ease.OutExpo));
        _seqStageStart.Group(Tween.Alpha(ssMainCanvasGroup, 1, 0.75f,Ease.OutExpo));
        _seqStageStart.Group(Tween.Scale(ssMainCanvasGroup.transform, 1.0f, 0.5f, Ease.InBack));
        //생성 후 임팩트
        float starBlinkDelay = duration;
        _seqStarBlink.Group(Tween.Scale(ssStartBtnCanvasGroup.transform, 1.0f, 0.375f, Ease.OutBack,startDelay:0.4f+starBlinkDelay));
        _seqStarBlink.Group(Tween.Alpha(ssStartBtnCanvasGroup, 1, 0.75f,Ease.OutExpo,startDelay:0.4f+starBlinkDelay));
        _seqStarBlink.Group(Tween.Delay(0.425f+starBlinkDelay, onComplete: () => ssTypewriter.ShowText(_strStart)));
        //별 Punch
        _seqStarBlink.Group(Tween.PunchScale(ssStar1Transform, Vector3.one * 0.5f, 0.4f, 1, startDelay: 0.5f+starBlinkDelay));
        _seqStarBlink.Group(Tween.PunchScale(ssStar2Transform, Vector3.one * 0.5f, 0.4f, 1, startDelay: 0.6f+starBlinkDelay));
        _seqStarBlink.Group(Tween.PunchScale(ssStar3Transform, Vector3.one * 0.5f, 0.4f, 1, startDelay: 0.7f+starBlinkDelay));
        //도전과제
        _seqStarBlink.Group(Tween.Scale(ssMission1CanvasGroup.transform,0.5f, 0.375f, Ease.OutBack,startDelay: 0.375f+starBlinkDelay));
        _seqStarBlink.Group(Tween.Scale(ssMission2CanvasGroup.transform, 0.5f, 0.375f, Ease.OutBack,startDelay: 0.45f+starBlinkDelay));
        _seqStarBlink.Group(Tween.Scale(ssMission3CanvasGroup.transform, 0.5f, 0.375f, Ease.OutBack,startDelay: 0.525f+starBlinkDelay));
        _seqStarBlink.Group(Tween.Alpha(ssMission1CanvasGroup, 1.0f, 0.75f,Ease.OutExpo,startDelay: 0.375f+starBlinkDelay));
        _seqStarBlink.Group(Tween.Alpha(ssMission2CanvasGroup, 1.0f, 0.75f,Ease.OutExpo,startDelay: 0.45f+starBlinkDelay));
        _seqStarBlink.Group(Tween.Alpha(ssMission3CanvasGroup, 1.0f, 0.75f,Ease.OutExpo,startDelay: 0.525f+starBlinkDelay));
    }
    public void StageStart_Exit()
    {
        if (_seqStageStart.isAlive) return;
        _stageStart = false;
        _seqStageStart.Complete();
        ssMainCanvasGroup.transform.localScale = Vector3.one;
        ssVignetteCanvasGroup.alpha = 1;
        ssMainCanvasGroup.alpha = 1;
        ssStartBtnCanvasGroup.alpha = 1;
        ssMission1CanvasGroup.alpha = 1;
        ssMission2CanvasGroup.alpha = 1;
        ssMission3CanvasGroup.alpha = 1;
        ssMission1CanvasGroup.transform.localScale = Vector3.one*0.5f;
        ssMission2CanvasGroup.transform.localScale = Vector3.one*0.5f;
        ssMission3CanvasGroup.transform.localScale = Vector3.one*0.5f;
        ssTmp.text = _strStart;
        foreach (var image in ssRaycastTarget)
        {
            image.raycastTarget = false;
        }
        ssMainCanvasGroup.transform.parent.gameObject.SetActive(true);
        ssTypewriter.StartDisappearingText();
        _seqStageStart = Sequence.Create();
        _seqStageStart.timeScale = 1.1f;
        //카메라
        foreach (var cam in cams) _seqStageStart.Group(Tween.CameraOrthographicSize(cam, 5.0f, 1.0f, Ease.InOutQuart));
        Tween_Purkinje(0.75f,0.0f,0);
        //도전과제
        float missionDelay = 0.25f;
        _seqStageStart.Group(Tween.Scale(ssMission1CanvasGroup.transform,0.4f, 0.375f, Ease.InBack));
        _seqStageStart.Group(Tween.Scale(ssMission2CanvasGroup.transform, 0.4f, 0.375f, Ease.InBack,startDelay: 0.075f));
        _seqStageStart.Group(Tween.Scale(ssMission3CanvasGroup.transform, 0.4f, 0.375f, Ease.InBack,startDelay: 0.15f));
        _seqStageStart.Group(Tween.Alpha(ssMission1CanvasGroup, 0.0f, 0.375f,Ease.OutExpo,startDelay: missionDelay));
        _seqStageStart.Group(Tween.Alpha(ssMission2CanvasGroup, 0.0f, 0.375f,Ease.OutExpo,startDelay: 0.075f + missionDelay));
        _seqStageStart.Group(Tween.Alpha(ssMission3CanvasGroup, 0.0f, 0.375f,Ease.OutExpo,startDelay: 0.15f + missionDelay));
        //패널 생성
        _seqStageStart.Group(Tween.Alpha(ssMainCanvasGroup, 0, 0.375f,Ease.OutExpo,startDelay:0.375f));
        _seqStageStart.Group(Tween.Scale(ssMainCanvasGroup.transform, 0.8f, 0.5f, Ease.InBack));
        //생성 후 임팩트
        _seqStageStart.Group(Tween.Alpha(ssVignetteCanvasGroup, 0, 0.75f,Ease.OutExpo,startDelay:0.5f));
        _seqStageStart.OnComplete(() => ssMainCanvasGroup.transform.parent.gameObject.SetActive(false));
        Tween_Blur(0.25f, 0.0f, 0.25f);
    }
    public void StageStart_JustMove(StageBanner banner)
    {
        if (_seqStageStart.isAlive) return;
        moveVec = Vector2.zero;
        _stageStart = true;
        _seqStageStart.Complete();
        _seqStageStart = Sequence.Create();
        _seqStageStart.timeScale = 1.1f;
        //카메라 이동
        Vector3 beginPos = camT.position, endPos = ClampVec(banner.transform.position);
        float duration = Mathf.Clamp((beginPos - endPos).magnitude/7.5f, 0.5f, 0.75f);
        endPos.y = beginPos.y;
        _seqStageStart.Chain(Tween.Custom(0,1,duration,onValueChange: ratio =>
        {
            Vector3 pos = Vector3.Lerp(beginPos,endPos,ratio);
            MoveMap(pos);
        },Ease.InOutCubic));
        _seqStageStart.ChainCallback(()=> _stageStart = false);
    }
    public void Tween_Blur(float duration,float strength,float delay)
    {
        _tBlur.Complete();
        _tBlur = Tween.Custom(BeautifySettings.settings.blurIntensity.value, strength, duration, 
            onValueChange:blurIntensity => BeautifySettings.settings.blurIntensity.value = blurIntensity,startDelay:delay);
    }
    public void Tween_Purkinje(float duration,float strength,float delay)
    {
        _tPurkinje.Complete();
        _tPurkinje = Tween.Custom(BeautifySettings.settings.purkinjeLuminanceThreshold.value, strength, duration, 
            onValueChange:intensity => BeautifySettings.settings.purkinjeLuminanceThreshold.value = intensity,startDelay:delay);
    }
}
