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
    [FoldoutGroup("Main")] public GameObject block;
    
    [FoldoutGroup("Create")] public Image imgCreate,createBG;
    [FoldoutGroup("Create")] public CanvasGroup cgCreate;
    [FoldoutGroup("Create")] public TMP_Text tCreateText,tCreateInfoText;
    [FoldoutGroup("Create")] public Transform btnCoin, btnGem;

    [FoldoutGroup("Positive")] public Image imgPositive;
    [FoldoutGroup("Positive")] public CanvasGroup cgPositive;
    [FoldoutGroup("Positive")] public TMP_Text tPositiveText;

    [FoldoutGroup("Negative")] public Image imgNegative;
    [FoldoutGroup("Negative")] public CanvasGroup cgNegative;
    [FoldoutGroup("Negative")] public TMP_Text tNegativeText;
    
    private Sequence _seqCreate, _seqPositive, _seqNegative;
    private Vector2 v2Create, v2Positive, v2Negative;
    private Item_Weapon _createdWeapon = null;

    public void Awake()
    {
        v2Create = imgCreate.rectTransform.sizeDelta;
        v2Positive = imgPositive.rectTransform.sizeDelta;
        v2Negative = imgNegative.rectTransform.sizeDelta;
        instance = this;
        createBG.color = Color.clear;
        createBG.gameObject.SetActive(false);
    }

    [Button]
    public void Create_Begin(bool isGem,Item_Weapon weapon)
    {
        _seqCreate.Stop();
        _createdWeapon = weapon;
        block.SetActive(true);
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
            block.SetActive(false);
            createBG.gameObject.SetActive(false);
            cgCreate.gameObject.SetActive(false);
        });
        if (success)
        {
            Success("'"+_createdWeapon.title+"' 1개 획득!");
        }
    }

    [Button]
    public void Success(string str)
    {
        if (_seqNegative.isAlive || _seqPositive.isAlive) return;
        tPositiveText.text = str;
        _seqPositive.Stop();
        cgPositive.gameObject.SetActive(true);
        block.SetActive(true);
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
        _seqPositive.ChainDelay(1.5f);
        _seqPositive.Chain(Tween.Alpha(cgPositive, 0, 0.2f,startDelay:0.2f));
        _seqPositive.Group(Tween.UISizeDelta(imgPositive.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqPositive.OnComplete(() =>
        {
            block.SetActive(false);
            cgPositive.gameObject.SetActive(false);
        });
    }
    [Button]
    public void Negative(string str)
    {
        if (_seqNegative.isAlive || _seqPositive.isAlive) return;
        tNegativeText.text = str;
        _seqNegative.Stop();
        cgNegative.gameObject.SetActive(true);
        block.SetActive(true);
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
        _seqNegative.ChainDelay(1.0f);
        _seqNegative.Chain(Tween.Alpha(cgNegative, 0, 0.2f,startDelay:0.2f));
        _seqNegative.Group(Tween.UISizeDelta(imgNegative.rectTransform, sizeDelta, 0.4f,Ease.InCirc));
        _seqNegative.OnComplete(() =>
        {
            block.SetActive(false);
            cgNegative.gameObject.SetActive(false);
        });
    }

    public void Popup_Dev()
    {
        Negative("아직 개발중인 컨텐츠입니다.");
    }
}
