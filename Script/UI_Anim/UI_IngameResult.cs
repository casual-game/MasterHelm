using System;
using System.Collections;
using System.Collections.Generic;
using Beautify.Universal;
using LeTai.Asset.TranslucentImage;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_IngameResult : MonoBehaviour
{
    [TitleGroup("Main")] 
    public StageData stageData;
    [FoldoutGroup("Main/data")] public TranslucentImage imgBGL, imgBGR;
    [FoldoutGroup("Main/data")] public Image imgGradientL,imgGradientR;
    [FoldoutGroup("Main/data")] public CanvasGroup cgL, cgR;
    [FoldoutGroup("Main/data")] public UI_IngameItemGroup itemGroup;
    [FoldoutGroup("Main/data")] public UI_IngameEarnableItem earnableItem;
    [FoldoutGroup("Main/data")] public GameObject gFenceL, gFenceR;
    [FoldoutGroup("Main/data")] public Image imgBG;
    [FoldoutGroup("Main/button")] public UI_PunchButton gBtnBack, gBtnContinue,gBtnRevive;
    [FoldoutGroup("Main/button")] public CanvasGroup cgBtnParent; 
    [FoldoutGroup("Main/deco")] public CanvasGroup cgDeco;
    [FoldoutGroup("Main/deco")] public Transform decoShadow, decoRoot;
    [FoldoutGroup("Main/score")] public Color cNorm, cSuccess, cFailed;
    [FoldoutGroup("Main/score")] public List<ContentSizeFitter> fittersScore = new List<ContentSizeFitter>();
    [FoldoutGroup("Main/score")] public Image imgStar1, imgStar2, imgStar3;
    [FoldoutGroup("Main/score")] public Color cStarActivated,cStarDeactivated;
    [FoldoutGroup("Main/score")] public TMP_Text 
        tmpTimeCurrent,
        tmpTimeTarget,
        tmpScoreCurrent,
        tmpScoreTarget,
        tmpComboCurrent,
        tmpComboTarget;
    
    private enum ResultState
    {
        Pause =0,Success=1,Failed =2
    }
    private ResultState _resultState;
    private Sequence _seqFrame,_seqButton,_seqEarnable,_seqDeco,_seqFailed,_seqFailedCount;
    private RectTransform _rtFrameL, _rtFrameR,_rtEarnableItem,_rtBtnParent;
    private Vector2 _anchoredPosFrameL, _anchoredPosFrameR,_anchoredPosBtnParent,_anchoredPosEarnableItem;

    private void Awake()
    {
        _rtFrameL = cgL.GetComponent<RectTransform>();
        _rtFrameR = cgR.GetComponent<RectTransform>();
        _rtEarnableItem = earnableItem.GetComponent<RectTransform>();
        _rtBtnParent = cgBtnParent.GetComponent<RectTransform>();
        _anchoredPosFrameL = _rtFrameL.anchoredPosition;
        _anchoredPosFrameR = _rtFrameR.anchoredPosition;
        _anchoredPosBtnParent = _rtBtnParent.anchoredPosition;
        _anchoredPosEarnableItem = _rtEarnableItem.anchoredPosition;
        v2MoneyReveal = new Vector2(0, -59);
        v2MoneyHide = new Vector2(0, 59);
        earnableItem.Setting(stageData);
        gBtnBack.Setting();
        gBtnContinue.Setting();
        gBtnRevive.Setting();
        v2Create = imgCreate.rectTransform.sizeDelta;
        createBG.color = Color.clear;
        createBG.gameObject.SetActive(false);
        tmpCoin.text = SaveManager.instance.coin.ToString();
        tmpGem.text = SaveManager.instance.gem.ToString();
    }

    //개별 상태
    
    public void Back()
    {
        switch (_resultState)
        {
            case ResultState.Pause:
                Pause_Fin();
                break;
            case ResultState.Success:
                Success_Fin();
                break;
            case ResultState.Failed:
                Failed_Fin();
                break;
        }
    }
    #region Pause
    [TitleGroup("Pause")]
    [FoldoutGroup("Pause/data")] public Sprite spritePause;
    [FoldoutGroup("Pause/data")] public Color colorPause;
    [FoldoutGroup("Pause/data")] public CanvasGroup cgPauseTitle,cgEarnableItem;
    [FoldoutGroup("Pause/data")][Button]
    public void Pause_Begin()
    {
        _resultState = ResultState.Pause;
        Pause_Score();
        imgBGL.spriteBlending = 0.1f;
        imgBGR.spriteBlending = 0.1f;
        imgBGL.sprite = spritePause;
        imgBGR.sprite = spritePause;
        imgGradientL.color = colorPause;
        imgGradientR.color = colorPause;
        itemGroup.Deactivate();
        earnableItem.Activate();
        gFenceL.SetActive(false);
        gFenceR.SetActive(false);
        //타이틀
        cgPauseTitle.gameObject.SetActive(true);
        cgSuccessTitle.gameObject.SetActive(false);
        cgFailedTitle.gameObject.SetActive(false);
        //기타 효과들
        Frame_Activate(Ease.InOutCubic,1,2);
        Button_Activate(true,true,false);
        Earnable_Activate();
        Deco_Activate();
        CamArm.instance.Tween_UIPurkinje(1.0f,0.25f);
    }
    [FoldoutGroup("Pause/data")][Button]
    public void Pause_Fin()
    {
        //타이틀
        cgPauseTitle.gameObject.SetActive(true);
        cgSuccessTitle.gameObject.SetActive(false);
        cgFailedTitle.gameObject.SetActive(false);
        //기타 효과들
        Frame_Deactivate();
        Button_Deactivate(true,true);
        Earnable_Deactivate();
        Deco_Deactivate();
        CamArm.instance.Tween_UIPurkinje(0.0f,0.25f,0.25f);
    }
    public void Pause_Score()
    {
        int currentTime = 133;
        int currentScore = 450;
        int currentCombo = 25;

        if (currentTime <= stageData.targetTime)
        {
            imgStar1.color = cStarActivated;
            tmpTimeCurrent.color = cSuccess;
        }
        else
        {
            imgStar1.color = cStarDeactivated;
            tmpTimeCurrent.color = cFailed;
        }
        tmpTimeCurrent.text = (currentTime/60)+":"+((currentTime%60) < 10? "0": String.Empty)+(currentTime%60);
        tmpTimeTarget.text = (stageData.targetTime/60)+":"+(stageData.targetTime%60);

        if (currentScore >= stageData.targetScore)
        {
            imgStar2.color = cStarActivated;
            tmpScoreCurrent.color = cSuccess;
        }
        else
        {
            imgStar2.color = cStarDeactivated;
            tmpScoreCurrent.color = cFailed;
        }
        tmpScoreCurrent.text = currentScore.ToString();
        tmpScoreTarget.text = stageData.targetScore.ToString();

        if (currentCombo >= stageData.targetCombo)
        {
            imgStar3.color = cStarActivated;
            tmpComboCurrent.color = cSuccess;
        }
        else
        {
            imgStar3.color = cStarDeactivated;
            tmpComboCurrent.color = cFailed;
        }
        tmpComboCurrent.text = currentCombo.ToString();
        tmpComboTarget.text = stageData.targetCombo.ToString();

        foreach (var fitter in fittersScore)
        {
            fitter.enabled = false;
            fitter.enabled = true;
        }
    }
    #endregion
    #region Success
    [TitleGroup("Success")]
    [FoldoutGroup("Success/data")] public Sprite spriteSuccess;
    [FoldoutGroup("Success/data")] public Color colorSuccess;
    [FoldoutGroup("Success/data")] public CanvasGroup cgSuccessTitle;
    [FoldoutGroup("Success/data")][Button]
    public void Success_Begin()
    {
        _resultState = ResultState.Success;
        imgBGL.spriteBlending = 0.1f;
        imgBGR.spriteBlending = 0.1f;
        imgBGL.sprite = spriteSuccess;
        imgBGR.sprite = spriteSuccess;
        imgGradientL.color = colorSuccess;
        imgGradientR.color = colorSuccess;
        itemGroup.Activate();
        earnableItem.Deactivate();
        gFenceL.SetActive(false);
        gFenceR.SetActive(false);
        //타이틀
        cgPauseTitle.gameObject.SetActive(false);
        cgSuccessTitle.gameObject.SetActive(true);
        cgFailedTitle.gameObject.SetActive(false);
        //기타 효과들
        float playSpeed = 0.75f;
        Frame_Activate(Ease.InOutBack,playSpeed,5);
        Button_Activate(true,false,false,playSpeed);
        Deco_Activate(playSpeed);
        CamArm.instance.Tween_UIPurkinje(0.0f,0.5f/playSpeed);
    }
    [FoldoutGroup("Success/data")][Button]
    public void Success_Fin()
    {
        //타이틀
        cgPauseTitle.gameObject.SetActive(false);
        cgSuccessTitle.gameObject.SetActive(true);
        cgFailedTitle.gameObject.SetActive(false);
        //기타 효과들
        Frame_Deactivate();
        Button_Deactivate(true,false);
        Deco_Deactivate();
        CamArm.instance.Tween_UIPurkinje(0.0f,0.25f);
    }
    public void Success_Score()
    {
        int currentTime = 123;
        int currentScore = 450;
        int currentCombo = 25;
    }

    
    #endregion
    #region Failed
    [TitleGroup("Failed")]
    [FoldoutGroup("Failed/data")] public Sprite spriteFailed;
    [FoldoutGroup("Failed/data")] public Color colorFailed;
    [FoldoutGroup("Failed/data")] public CanvasGroup cgFailedTitle,cgFailedBG;
    [FoldoutGroup("Failed/data")] public TMP_Text tmpFailedCount;
    [FoldoutGroup("Failed/data")][Button] 
    public void Failed_Begin()
    {
        _resultState = ResultState.Failed;
        imgBGL.spriteBlending = 0.75f;
        imgBGR.spriteBlending = 0.75f;
        imgBGL.sprite = spriteFailed;
        imgBGR.sprite = spriteFailed;
        imgGradientL.color = colorFailed;
        imgGradientR.color = colorFailed;
        itemGroup.Deactivate();
        earnableItem.Deactivate();
        gFenceL.SetActive(true);
        gFenceR.SetActive(true);
        //타이틀
        cgPauseTitle.gameObject.SetActive(false);
        cgSuccessTitle.gameObject.SetActive(false);
        cgFailedTitle.gameObject.SetActive(true);
        //기타 효과들
        float playSpeed = 0.75f;
        //Revive_Activate();
        CamArm.instance.Tween_ResetTimescale();
        Frame_Activate(Ease.InOutCubic,playSpeed,2,false);
        Button_Activate(true,false,true,playSpeed);
        Deco_Activate(playSpeed);
        Failed_Activate(playSpeed);
        Earnable_Activate(playSpeed);
        CamArm.instance.Tween_UIPurkinje(0.75f,0.5f/playSpeed);
    }
    [FoldoutGroup("Failed/data")][Button] 
    public void Failed_Fin()
    {
        //타이틀
        cgPauseTitle.gameObject.SetActive(false);
        cgSuccessTitle.gameObject.SetActive(false);
        cgFailedTitle.gameObject.SetActive(true);
        //기타 효과들
        Frame_Deactivate(Ease.InOutCubic,1,false);
        Button_Deactivate(true,false);
        Deco_Deactivate();
        Failed_Deactivate();
        Earnable_Deactivate();
        CamArm.instance.Tween_UIPurkinje(0.0f,0.5f);
        print("failed fin");
    }
    #endregion

    #region Popup
    [TitleGroup("Popup")]
    [FoldoutGroup("Popup/Revive")] public Image imgCreate,createBG;
    [FoldoutGroup("Popup/Revive")] public CanvasGroup cgCreate;
    [FoldoutGroup("Popup/Revive")] public TMP_Text tCreateText;
    [FoldoutGroup("Popup/Revive")] public RectTransform rtCurrentMoney;
    [FoldoutGroup("Popup/Revive")] public TMP_Text tmpCoin, tmpGem;
    private Sequence _seqCreate;
    private Vector2 v2Create, v2MoneyReveal, v2MoneyHide;
    private bool popupCreate = false;
    public void Revive_Begin()
    {
        //SoundManager.Play(SoundContainer_StageSelect.instance.sound_popup_create,0.375f);
        //BgmManager.instance.BgmLowpass(true);
        _seqCreate.Stop();
        cgCreate.gameObject.SetActive(true);
        createBG.gameObject.SetActive(true);
        rtCurrentMoney.gameObject.SetActive(true);
        Vector2 sizeDelta = v2Create;
        sizeDelta.y *= 0.5f;
        imgCreate.rectTransform.sizeDelta = sizeDelta;
        tCreateText.transform.localScale = Vector3.one*1.625f;
        cgCreate.alpha = 0;
        rtCurrentMoney.anchoredPosition = v2MoneyHide;
        _seqCreate = Sequence.Create(useUnscaledTime: true);
        _seqCreate.timeScale = 1.25f;
        _seqCreate.Chain(Tween.Alpha(cgCreate, 1, 0.5f));
        _seqCreate.Group(Tween.UISizeDelta(imgCreate.rectTransform, v2Create, 1.0f,Ease.InOutCirc));
        _seqCreate.Group(Tween.Scale(tCreateText.transform, 1.0f, 1.0f, Ease.InOutExpo));
        _seqCreate.Group(Tween.Color(createBG, new Color(0,0,0,0.9215f), 0.375f));
        _seqCreate.Group(Tween.UIAnchoredPosition(rtCurrentMoney, v2MoneyReveal, 1.0f, Ease.InOutCirc));
        popupCreate = true;
        _seqFailedCount.timeScale = 0;
    }
    public void Revive_Fin()
    {
        if (_seqCreate.isAlive) return;
        //BgmManager.instance.BgmLowpass(false);
        _seqCreate.Stop();
        cgCreate.gameObject.SetActive(true);
        rtCurrentMoney.gameObject.SetActive(true);
        Vector2 sizeDelta = v2Create;
        sizeDelta.y *= 0.5f;
        imgCreate.rectTransform.sizeDelta = v2Create;
        tCreateText.transform.localScale = Vector3.one;
        cgCreate.alpha = 1;
        rtCurrentMoney.anchoredPosition = v2MoneyReveal;
        _seqCreate = Sequence.Create(useUnscaledTime: true);
        _seqCreate.timeScale = 2.5f;
        _seqCreate.Chain(Tween.Alpha(cgCreate, 0, 0.2f,startDelay:0.2f));
        _seqCreate.Group(Tween.UISizeDelta(imgCreate.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqCreate.Group(Tween.Color(createBG, Color.clear, 0.4f));
        _seqCreate.Group(Tween.UIAnchoredPosition(rtCurrentMoney, v2MoneyHide, 1.0f, Ease.InOutCirc));
        _seqCreate.OnComplete(() =>
        {
            createBG.gameObject.SetActive(false);
            cgCreate.gameObject.SetActive(false);
            rtCurrentMoney.gameObject.SetActive(false);
        });
        
        popupCreate = false;
        _seqFailedCount.timeScale = 1;
    }
    public void Popup_Fin()
    {
        if(popupCreate) Revive_Fin();
    }

    public void Revive()
    {
        Hero.instance.Spawn();
        Failed_Fin();
        Revive_Fin();
        print("revive");
    }
    #endregion
    
    
    #region 서브 엘리먼트
    private void Frame_Activate(Ease ease = Ease.InOutCubic,float playSpeed=1.0f,int zoomStrength=1,bool useStop = true)
    {
        _rtFrameL.gameObject.SetActive(false);
        _rtFrameL.gameObject.SetActive(true);
        _rtFrameR.gameObject.SetActive(true);
        imgBG.gameObject.SetActive(true);
        if(useStop) CamArm.instance.Tween_UIStopped(true,0.5f,0);
        CamArm.instance.Tween_UIZoom(true,0.5f/playSpeed,0,ease,zoomStrength);
        cgL.alpha = 0;
        cgR.alpha = 0;
        imgBG.color = Color.clear;
        float amount = Screen.width * 0.069375f;
        _seqFrame.Stop();
        _rtFrameL.anchoredPosition = _anchoredPosFrameL + Vector2.left * amount;
        _rtFrameR.anchoredPosition = _anchoredPosFrameR + Vector2.right * amount;
        _seqFrame = Sequence.Create(useUnscaledTime:true);
        _seqFrame.timeScale = playSpeed;
        _seqFrame.Group(Tween.UIAnchoredPosition(_rtFrameL, _anchoredPosFrameL, 0.375f, Ease.OutBack));
        _seqFrame.Group(Tween.UIAnchoredPosition(_rtFrameR, _anchoredPosFrameR, 0.375f, Ease.OutBack));
        _seqFrame.Group(Tween.Alpha(cgL, 1, 0.2f));
        _seqFrame.Group(Tween.Alpha(cgR, 1, 0.2f));
        _seqFrame.Group(Tween.Color(imgBG, Color.black, 0.5f,startDelay:0.0f,ease:Ease.OutCubic));
    }
    private void Frame_Deactivate(Ease ease = Ease.InOutCubic,float playSpeed=1.0f,bool useStop = true)
    {
        if(useStop) CamArm.instance.Tween_UIStopped(false,0.5f,0);
        CamArm.instance.Tween_UIZoom(false,0.5f,0.25f,ease);
        cgL.alpha = 1;
        cgR.alpha = 1;
        imgBG.color = Color.black;
        float amount = Screen.width * 0.04625f;
        _seqFrame.Stop();
        _rtFrameL.anchoredPosition = _anchoredPosFrameL;
        _rtFrameR.anchoredPosition = _anchoredPosFrameR;
        _seqFrame = Sequence.Create(useUnscaledTime:true);
        _seqFrame.timeScale = playSpeed;
        _seqFrame.Group(Tween.UIAnchoredPosition(_rtFrameL, _anchoredPosFrameL + Vector2.left * amount, 0.5f, Ease.InBack));
        _seqFrame.Group(Tween.UIAnchoredPosition(_rtFrameR, _anchoredPosFrameR + Vector2.right * amount, 0.5f, Ease.InBack));
        _seqFrame.Group(Tween.Alpha(cgL, 0, 0.2f,startDelay: 0.3f));
        _seqFrame.Group(Tween.Alpha(cgR, 0, 0.2f,startDelay: 0.3f));
        _seqFrame.Group(Tween.Color(imgBG, Color.clear, 0.25f,startDelay:0.25f));
        _seqFrame.OnComplete(() =>
        {
            _rtFrameL.gameObject.SetActive(false);
            _rtFrameR.gameObject.SetActive(false);
            imgBG.gameObject.SetActive(false);
        });
    }
    private void Button_Activate(bool useBack,bool useContinue,bool useRevive,float delay = 0.2f,float playSpeed=1.0f)
    {
        _seqButton.Stop();
        cgBtnParent.gameObject.SetActive(true);
        _seqButton = Sequence.Create(useUnscaledTime:true);
        _seqButton.timeScale = playSpeed;
        //버튼 Parent 이동
        cgBtnParent.alpha = 0;
        _rtBtnParent.anchoredPosition = _anchoredPosBtnParent + Vector2.left * Screen.width * 0.069375f;
        _seqButton.Group(Tween.UIAnchoredPosition(_rtBtnParent, _anchoredPosBtnParent, 0.375f, Ease.OutBack));
        _seqButton.Group(Tween.Alpha(cgBtnParent, 1, 0.2f));
        //개별 버튼
        gBtnBack.gameObject.SetActive(useBack);
        gBtnContinue.gameObject.SetActive(useContinue);
        gBtnRevive.gameObject.SetActive(useRevive);
    }
    private void Button_Deactivate(bool useBack,bool useContinue,float delay = 0.2f,float playSpeed=1.0f)
    {
        _seqButton.Stop();
        _seqButton = Sequence.Create(useUnscaledTime:true);
        _seqButton.timeScale = playSpeed;
        //버튼 Parent 이동
        cgBtnParent.alpha = 1;
        _rtBtnParent.anchoredPosition = _anchoredPosBtnParent;
        _seqButton.Group(Tween.UIAnchoredPosition(_rtBtnParent, _anchoredPosBtnParent 
                                                  + Vector2.left * Screen.width * 0.04625f, 0.5f, Ease.InBack));
        _seqButton.Group(Tween.Alpha(cgBtnParent, 0, 0.2f,startDelay:0.3f));
        _seqButton.OnComplete(() => cgBtnParent.gameObject.SetActive(false));
    }
    private void Earnable_Activate(float playSpeed=1.0f)
    {
        _seqEarnable.Stop();
        earnableItem.gameObject.SetActive(true);
        _seqEarnable = Sequence.Create(useUnscaledTime:true);
        _seqEarnable.timeScale = playSpeed;
        cgEarnableItem.alpha = 0;
        _rtEarnableItem.anchoredPosition = _anchoredPosEarnableItem + Vector2.right * Screen.width * 0.069375f;
        _seqEarnable.Group(Tween.UIAnchoredPosition(_rtEarnableItem, _anchoredPosEarnableItem, 0.375f, Ease.OutBack));
        _seqEarnable.Group(Tween.Alpha(cgEarnableItem, 1, 0.2f));
    }
    private void Earnable_Deactivate(float playSpeed=1.0f)
    {
        _seqEarnable.Stop();
        _seqEarnable = Sequence.Create(useUnscaledTime:true);
        _seqEarnable.timeScale = playSpeed;
        cgEarnableItem.alpha = 1;
        _rtEarnableItem.anchoredPosition = _anchoredPosEarnableItem;
        _seqEarnable.Group(Tween.UIAnchoredPosition(_rtEarnableItem, _anchoredPosEarnableItem 
                           + Vector2.right * Screen.width * 0.069375f, 0.5f, Ease.InBack));
        _seqEarnable.Group(Tween.Alpha(cgEarnableItem, 0, 0.2f,startDelay: 0.3f));
        _seqEarnable.OnComplete(() => earnableItem.gameObject.SetActive(false));
    }
    private void Deco_Activate(float playSpeed=1.25f)
    {
        _seqDeco.Stop();
        cgDeco.gameObject.SetActive(true);
        cgDeco.alpha = 0;
        decoShadow.localScale = Vector3.one*1.1f;
        decoRoot.localScale = Vector3.one*1.2f;
        
        _seqDeco = Sequence.Create(useUnscaledTime:true);
        _seqDeco.timeScale = playSpeed;
        _seqDeco.Chain(Tween.Alpha(cgDeco, 1,0.375f));
        _seqDeco.Group(Tween.Scale(decoRoot, 1.0f, 0.5f, startDelay: 0.0f, ease: Ease.OutBack));
        _seqDeco.Group(Tween.Scale(decoShadow, 1.0f, 0.625f, startDelay: 0.0f, ease: Ease.OutBack));
    }
    private void Deco_Deactivate(float playSpeed=1.25f)
    {
        _seqDeco.Stop();
        cgDeco.gameObject.SetActive(true);
        cgDeco.alpha = 1;
        decoShadow.localScale = Vector3.one;
        decoRoot.localScale = Vector3.one;
        _seqDeco = Sequence.Create(useUnscaledTime:true);
        _seqDeco.timeScale = playSpeed;
        _seqDeco.Chain(Tween.Alpha(cgDeco, 0,0.25f,startDelay:0.5f));
        _seqDeco.Group(Tween.Scale(decoRoot, 1.4f, 0.5f, startDelay: 0.125f, ease: Ease.InBack));
        _seqDeco.Group(Tween.Scale(decoShadow, 1.2f, 0.625f, startDelay: 0.0f, ease: Ease.InBack));
        _seqDeco.OnComplete(() => cgDeco.gameObject.SetActive(false));
    }
    private void Failed_Activate(float playSpeed=1.0f)
    {
        gBtnRevive.Usable(true);
        cgFailedBG.gameObject.SetActive(true);
        _seqFailed.Stop();
        cgFailedBG.alpha = 0;
        _seqFailed = Sequence.Create(useUnscaledTime:true);
        _seqFailed.timeScale = playSpeed;
        _seqFailed.Group(Tween.Alpha(cgFailedBG, 1, 0.2f));
        
        //카운트
        _seqFailedCount.Stop();
        int num = 5;
        Transform tfc = tmpFailedCount.transform;
        _seqFailedCount = Sequence.Create(cycles:6,useUnscaledTime:true);
        _seqFailedCount.ChainCallback(Restart);
        _seqFailedCount.Group(Tween.Color(tmpFailedCount, Color.white, 0.35f));
        _seqFailedCount.Group(Tween.Scale(tfc, 1, 1.0f, Ease.OutExpo));
        void Restart()
        {
            if (num > 0)
            {
                tmpFailedCount.text = num.ToString();
                CamArm.instance.Tween_Chromatic(0.05f,1.0f,Ease.OutCirc);
                CamArm.instance.Tween_Radial(0.1f,0.0f,0.5f,0.05f);
                CamArm.instance.Tween_Shake(0.35f,40,Vector3.one*0.05f,Ease.OutSine);
                tmpFailedCount.color = Color.clear;
                tfc.localScale = Vector3.one*3;
                num--;
            }
            else
            {
                tmpFailedCount.text = String.Empty;
                CamArm.instance.Tween_Chromatic(0.05f, 2.5f, Ease.OutCirc);
                CamArm.instance.Tween_Radial(0.25f, 0.25f, 2.0f, 0.2f);
                CamArm.instance.Tween_Bloom(0.25f,0.25f,2.0f,50);
                CamArm.instance.Tween_Shake(0.35f, 40, Vector3.one * 0.1f, Ease.OutSine);
                CamArm.instance.Tween_UIStopped(true,1.0f,0);
                CamArm.instance.Tween_UIPurkinje(1.0f,1.0f);
                gBtnRevive.Usable(false);
            }
        }
    }
    private void Failed_Deactivate(float playSpeed=1.0f)
    {
        _seqFailed.Stop();
        _seqFailedCount.Stop();
        cgFailedBG.alpha = 1;
        _seqFailed = Sequence.Create(useUnscaledTime:true);
        _seqFailed.timeScale = playSpeed;
        _seqFailed.Group(Tween.Alpha(cgFailedBG, 0, 0.25f));
        _seqFailed.OnComplete(() => cgFailedBG.gameObject.SetActive(false));
        _seqFailedCount = Sequence.Create(useUnscaledTime:true);
        _seqFailedCount.Group(Tween.Color(tmpFailedCount, Color.clear, 0.375f));
        CamArm.instance.Tween_UIStopped(false,1.0f,0);
    }
    #endregion
    //디버깅
    [Button]
    public void DebugActivate()
    {
        cgL.gameObject.SetActive(false);
        cgL.gameObject.SetActive(true);
        cgR.gameObject.SetActive(true);
        imgBG.gameObject.SetActive(true);
        cgBtnParent.gameObject.SetActive(true);
        earnableItem.gameObject.SetActive(true);
        cgDeco.gameObject.SetActive(true);
        cgFailedBG.gameObject.SetActive(true);
    }
    [Button]
    public void DebugDeactivate()
    {
        cgL.gameObject.SetActive(false);
        cgR.gameObject.SetActive(false);
        imgBG.gameObject.SetActive(false);
        cgBtnParent.gameObject.SetActive(false);
        earnableItem.gameObject.SetActive(false);
        cgDeco.gameObject.SetActive(false);
        cgFailedBG.gameObject.SetActive(false);
    }
    
}
