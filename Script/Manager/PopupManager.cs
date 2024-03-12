using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    
    [FoldoutGroup("Create")] public Image imgCreate,createBG;
    [FoldoutGroup("Create")] public CanvasGroup cgCreate;
    [FoldoutGroup("Create")] public TMP_Text tCreateText,tCreateInfoText;
    [FoldoutGroup("Create")] public Transform btnCoin, btnGem;

    [FoldoutGroup("Success")] public Image imgSuccess;
    [FoldoutGroup("Success")] public CanvasGroup cgSuccess;
    [FoldoutGroup("Success")] public TMP_Text tSuccessText;
    [FoldoutGroup("Success")] public CanvasGroup cgSuccessBg;

    [FoldoutGroup("Negative")] public GameObject negativeBG;
    [FoldoutGroup("Negative")] public Image imgNegative;
    [FoldoutGroup("Negative")] public CanvasGroup cgNegative;
    [FoldoutGroup("Negative")] public TMP_Text tNegativeText;
    
    [FoldoutGroup("Positive")] public GameObject positiveBG;
    [FoldoutGroup("Positive")] public Image imgPositive;
    [FoldoutGroup("Positive")] public CanvasGroup cgPositive;
    [FoldoutGroup("Positive")] public TMP_Text tPositiveText;
    
    private Sequence _seqCreate, _seqSuccess, _seqNegative,_seqPositive;
    private Vector2 v2Create, v2Success, v2Negative,v2Positive;
    private Item_Weapon _createdWeapon = null;

    public void Awake()
    {
        v2Create = imgCreate.rectTransform.sizeDelta;
        v2Success = imgSuccess.rectTransform.sizeDelta;
        v2Negative = imgNegative.rectTransform.sizeDelta;
        v2Positive = imgPositive.rectTransform.sizeDelta;
        instance = this;
        createBG.color = Color.clear;
        createBG.gameObject.SetActive(false);
    }

    [Button]
    public void Create_Begin(bool isGem,Item_Weapon weapon)
    {
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_popup_create,0.375f);
        BgmManager.instance.BgmLowpass(true);
        _seqCreate.Stop();
        _createdWeapon = weapon;
        cgCreate.gameObject.SetActive(true);
        createBG.gameObject.SetActive(true);
        btnCoin.gameObject.SetActive(!isGem);
        btnGem.gameObject.SetActive(isGem);
        Vector2 sizeDelta = v2Create;
        sizeDelta.y *= 0.5f;
        imgCreate.rectTransform.sizeDelta = sizeDelta;
        tCreateText.transform.localScale = Vector3.one*1.625f;
        cgCreate.alpha = 0;
        tCreateText.text = "제작: " + _createdWeapon.title;
        tCreateInfoText.text = "공격력: " + _createdWeapon.power + "\n체력: " + _createdWeapon.hp;
        _seqCreate = Sequence.Create();
        _seqCreate.timeScale = 1.25f;
        _seqCreate.Chain(Tween.Alpha(cgCreate, 1, 0.5f));
        _seqCreate.Group(Tween.UISizeDelta(imgCreate.rectTransform, v2Create, 1.0f,Ease.InOutCirc));
        _seqCreate.Group(Tween.Scale(tCreateText.transform, 1.0f, 1.0f, Ease.InOutExpo));
        _seqCreate.Group(Tween.Color(createBG, new Color(0,0,0,0.9215f), 0.375f));
    }

    [Button]
    public void Create_Fin(bool success)
    {
        if (_seqCreate.isAlive) return;
        BgmManager.instance.BgmLowpass(false);
        _seqCreate.Stop();
        cgCreate.gameObject.SetActive(true);
        Vector2 sizeDelta = v2Create;
        sizeDelta.y *= 0.5f;
        imgCreate.rectTransform.sizeDelta = v2Create;
        tCreateText.transform.localScale = Vector3.one;
        cgCreate.alpha = 1;
        
        _seqCreate = Sequence.Create();
        _seqCreate.timeScale = 2.5f;
        _seqCreate.Chain(Tween.Alpha(cgCreate, 0, 0.2f,startDelay:0.2f));
        _seqCreate.Group(Tween.UISizeDelta(imgCreate.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqCreate.Group(Tween.Color(createBG, Color.clear, 0.4f));
        _seqCreate.OnComplete(() =>
        {
            createBG.gameObject.SetActive(false);
            cgCreate.gameObject.SetActive(false);
        });
        if (success)
        {
            Success("'"+_createdWeapon.title+"' 1개 획득!");
        }
        else
        {
            SoundManager.Play(SoundContainer_StageSelect.instance.sound_clickfailed);
        }
    }

    [Button]
    public void Success(string str)
    {
        if (_seqNegative.isAlive || _seqSuccess.isAlive) return;
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_popup_success);
        tSuccessText.text = str;
        _seqSuccess.Stop();
        cgSuccess.gameObject.SetActive(true);
        cgSuccessBg.gameObject.SetActive(true);
        Vector2 sizeDelta = v2Success;
        sizeDelta.y *= 0.5f;
        imgSuccess.rectTransform.sizeDelta = sizeDelta;
        tSuccessText.transform.localScale = Vector3.one*3.5f;
        cgSuccess.alpha = 0;
        cgSuccessBg.alpha = 0;
        
        _seqSuccess = Sequence.Create();
        _seqSuccess.timeScale = 1.5f;
        _seqSuccess.Chain(Tween.Alpha(cgSuccess, 1, 0.25f));
        _seqSuccess.Group(Tween.UISizeDelta(imgSuccess.rectTransform, v2Success, 0.5f,Ease.OutCirc));
        _seqSuccess.Group(Tween.Scale(tSuccessText.transform, 1.0f, 0.5f, Ease.OutCirc));
        _seqSuccess.Group(Tween.Alpha(cgSuccessBg, 1, 0.25f));
        _seqSuccess.ChainDelay(1.5f);
        _seqSuccess.Chain(Tween.Alpha(cgSuccess, 0, 0.25f,startDelay:0.25f));
        _seqSuccess.Group(Tween.Alpha(cgSuccessBg, 0, 0.25f,startDelay:0.25f));
        _seqSuccess.Group(Tween.UISizeDelta(imgSuccess.rectTransform, sizeDelta, 0.5f,Ease.InOutCirc));
        _seqSuccess.OnComplete(() =>
        {
            cgSuccess.gameObject.SetActive(false);
            cgSuccessBg.gameObject.SetActive(false);
        });
    }
    [Button]
    public void Negative(string str,float duration = 1.5f)
    {
        if (_seqNegative.isAlive || _seqSuccess.isAlive) return;
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_popup_negative,0.125f);
        tNegativeText.text = str;
        _seqNegative.Stop();
        cgNegative.gameObject.SetActive(true);
        negativeBG.SetActive(true);
        Vector2 sizeDelta = v2Negative;
        sizeDelta.y *= 0.5f;
        imgNegative.rectTransform.sizeDelta = sizeDelta;
        tNegativeText.transform.localScale = Vector3.one*1.5f;
        cgNegative.alpha = 0;
        
        _seqNegative = Sequence.Create();
        _seqNegative.timeScale = 1.5f;
        _seqNegative.Chain(Tween.Alpha(cgNegative, 1, 0.25f));
        _seqNegative.Group(Tween.UISizeDelta(imgNegative.rectTransform, v2Negative, 0.5f,Ease.OutCirc));
        _seqNegative.Group(Tween.Scale(tNegativeText.transform, 1.0f, 0.5f, Ease.OutCirc));
        _seqNegative.ChainDelay(duration);
        _seqNegative.Chain(Tween.Alpha(cgNegative, 0, 0.2f,startDelay:0.2f));
        _seqNegative.Group(Tween.UISizeDelta(imgNegative.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqNegative.OnComplete(() =>
        {
            cgNegative.gameObject.SetActive(false);
            negativeBG.SetActive(false);
        });
    }
    [Button]
    public void Positive(string str,float duration = 1.5f)
    {
        if (_seqSuccess.isAlive || _seqPositive.isAlive) return;
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_popup_positive,0.1875f);
        tPositiveText.text = str;
        _seqPositive.Stop();
        cgPositive.gameObject.SetActive(true);
        positiveBG.SetActive(true);
        Vector2 sizeDelta = v2Positive;
        sizeDelta.y *= 0.5f;
        imgPositive.rectTransform.sizeDelta = sizeDelta;
        tPositiveText.transform.localScale = Vector3.one*1.5f;
        cgPositive.alpha = 0;
        
        _seqPositive = Sequence.Create();
        _seqPositive.timeScale = 1.5f;
        _seqPositive.Chain(Tween.Alpha(cgPositive, 1, 0.25f));
        _seqPositive.Group(Tween.UISizeDelta(imgPositive.rectTransform, v2Positive, 0.5f,Ease.OutCirc));
        _seqPositive.Group(Tween.Scale(tPositiveText.transform, 1.0f, 0.5f, Ease.OutCirc));
        _seqPositive.ChainDelay(duration);
        _seqPositive.Chain(Tween.Alpha(cgPositive, 0, 0.2f,startDelay:0.2f));
        _seqPositive.Group(Tween.UISizeDelta(imgPositive.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqPositive.OnComplete(() =>
        {
            cgPositive.gameObject.SetActive(false);
            positiveBG.SetActive(false);
        });
    }

    public void Popup_Dev()
    {
        Negative("아직 개발중인 컨텐츠입니다.");
    }
}
