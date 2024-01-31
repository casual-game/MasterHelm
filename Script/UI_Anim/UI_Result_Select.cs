using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_Result_Select : MonoBehaviour
{
    [TitleGroup("Part")] public CanvasGroup upperPart, lowerPart,resultPart;
    [TitleGroup("Part")] public Image[] cracks;
    [TitleGroup("Part")] public Image select1, select2;
    [TitleGroup("Part")] public TMP_Text tmpCount,tmpResult;
    [TitleGroup("Data")] public float lowerWidth_Collapsed, lowerWidth_Extended;
    [TitleGroup("Data")] public Material targetMat;
    [TitleGroup("Data")] public Color failedColor, successColor,selectColor;
    [TitleGroup("Data")] public ParticleImage piSuccess,piSelect1,piSelect2;
    private Sequence _sequence,_seqSelect,_seqReroll,_seqChromatic,_seqOutline;
    private RectTransform _rtLowerPart,_rt;
    private Vector2 _pos;
    private Material _copyMat;
    private int _idChromatic,_idOutline;
    private float _selectWidth;
    private int _itemCount;
    private void Start()
    {
        Setting(4);
    }

    private void Setting(int count)
    {
        foreach (var crack in cracks) crack.color = new Color(1, 1, 1, 0);
        _rt = GetComponent<RectTransform>();
        _pos = _rt.anchoredPosition;
        _rtLowerPart = lowerPart.GetComponent<RectTransform>();
        _copyMat = Instantiate(targetMat);
        _idChromatic = Shader.PropertyToID(GameManager.s_chromaberramount);
        _idOutline = Shader.PropertyToID(GameManager.s_alphaoutlinecolor);
        _selectWidth = -select1.rectTransform.rect.width;
        select1.rectTransform.offsetMax = new Vector2(_selectWidth,select1.rectTransform.offsetMax.y);
        select2.rectTransform.offsetMax = new Vector2(_selectWidth,select2.rectTransform.offsetMax.y);
        select1.gameObject.SetActive(false);
        select2.gameObject.SetActive(false);
        foreach (var img in GetComponentsInChildren<Image>())
        {
            if (img.material == targetMat) img.material = _copyMat;
        }
        upperPart.gameObject.SetActive(false);
        lowerPart.gameObject.SetActive(false);
        resultPart.gameObject.SetActive(false);
        _copyMat.SetFloat(GameManager.s_chromaberramount,0.0f);
        _copyMat.SetColor(GameManager.s_alphaoutlinecolor,Color.clear);
        _itemCount = count;
        tmpCount.text = count.ToString();
    }

    [Button]
    public void Reset()
    {
        upperPart.transform.gameObject.SetActive(false);
        lowerPart.transform.gameObject.SetActive(false);
        resultPart.transform.gameObject.SetActive(false);
    }
    [Button]
    public void Spawn_Upper()
    {
        Transform ut = upperPart.transform;
        _sequence.Complete();
        upperPart.alpha = 0;
        ut.localScale = Vector3.one*0.75f;
        ut.gameObject.SetActive(true);
        
        _sequence = Sequence.Create();
        _sequence.Chain(Tween.Scale(ut, Vector3.one, 0.375f, Ease.OutBack));
        _sequence.Group(Tween.Alpha(upperPart, 1.0f, 0.2f));
    }

    [Button]
    public void Spawn_Lower()
    {
        Transform lt = lowerPart.transform;
        _sequence.Complete();
        lowerPart.alpha = 0;
        _rtLowerPart.sizeDelta = new Vector2(lowerWidth_Collapsed, _rtLowerPart.sizeDelta.y);
        lt.localScale = Vector3.one*0.75f;
        lt.gameObject.SetActive(true);
        
        _sequence = Sequence.Create();
        _sequence.Chain(Tween.Scale(lt, Vector3.one, 0.375f, Ease.OutBack));
        _sequence.Group(Tween.Alpha(lowerPart, 1.0f, 0.2f));
        _sequence.Group(Tween.UISizeDelta(_rtLowerPart, new Vector2(lowerWidth_Extended,
            _rtLowerPart.sizeDelta.y), 0.65f, Ease.InOutBack));
    }

    [Button]
    public void Reroll()
    {
        _seqReroll.Complete();
        _seqReroll = Sequence.Create();
        _seqReroll.Chain(Tween.UISizeDelta(_rtLowerPart, new Vector2(lowerWidth_Collapsed,
            _rtLowerPart.sizeDelta.y), 0.65f, Ease.InOutBack));
        _seqReroll.ChainDelay(0.25f);
        _seqReroll.Chain(Tween.UISizeDelta(_rtLowerPart, new Vector2(lowerWidth_Extended,
            _rtLowerPart.sizeDelta.y), 0.65f, Ease.InOutBack));
    }

    [Button]
    public void Success()
    {
        Chromatic(0.125f,0.0f,0.5f,0.5f);
        Outline(0.125f,0.5f,0.5f,successColor);
        piSuccess.Play();
        _sequence.Complete();
        
        _sequence = Sequence.Create();
        _sequence.Chain(Tween.Custom(1, 0, 0.5f, onValueChange: ratio =>
        {
            Vector2 pos = _pos + Random.insideUnitCircle * 10 * ratio;
            _rt.anchoredPosition = pos;
        }));
        _sequence.ChainDelay(0.375f);
        _sequence.OnComplete(Reroll);
    }

    [Button]
    public void Failed()
    {
        Chromatic(0.125f,0.0f,0.5f,0.4f);
        Outline(0.125f,0.25f,0.5f,failedColor);
        _sequence.Complete();
        foreach (var crack in cracks) crack.color = Color.white;
        
        _sequence = Sequence.Create();
        _sequence.Chain(Tween.Custom(1, 0, 0.5f, onValueChange: ratio =>
        {
            Vector2 pos = _pos + Random.insideUnitCircle * 15 * ratio;
            _rt.anchoredPosition = pos;
        }));
        _sequence.ChainDelay(0.5f);
        foreach (var crack in cracks) 
            _sequence.Group(Tween.Color(crack, Color.clear, 0.75f));
        _sequence.OnComplete(Reroll);
    }

    [Button]
    public void Select1()
    {
        Outline(3.5f,1.0f,1.0f,selectColor);
        Chromatic(3.5f,1.0f,1.0f,0.125f);
        //
        _seqSelect.Complete();
        select1.gameObject.SetActive(true);
        select1.rectTransform.offsetMax = new Vector2(_selectWidth,select1.rectTransform.offsetMax.y);
        
        _seqSelect = Sequence.Create();
        _seqSelect.Chain(Tween.UIOffsetMaxX(select1.rectTransform,0, 1.0f, Ease.Linear));
        _seqSelect.Group(Tween.Custom(0, 1, 1.0f, onValueChange: ratio =>
        {
            Vector2 pos = _pos + Random.insideUnitCircle * 3 * ratio;
            _rt.anchoredPosition = pos;
        }));
        _seqSelect.OnComplete(() =>
        {
            piSelect1.Stop(false);
            Failed();
        });
    }

    [Button]
    public void Select2()
    {
        
    }

    [Button]
    public void Result()
    {
        Transform rt = resultPart.transform;
        _sequence.Complete();
        resultPart.alpha = 0;
        rt.localScale = Vector3.one*0.75f;
        rt.gameObject.SetActive(true);
        
        _sequence = Sequence.Create();
        _sequence.Chain(Tween.Scale(rt, Vector3.one, 0.375f, Ease.OutBack));
        _sequence.Group(Tween.Alpha(resultPart, 1.0f, 0.2f));
    }

    private void Chromatic(float begin,float delay,float fin,float strength)
    {
        _seqChromatic.Stop();
        _copyMat.SetFloat(GameManager.s_chromaberramount,0.0f);
        _seqChromatic = Sequence.Create();
        _seqChromatic.Chain(Tween.MaterialProperty(_copyMat, _idChromatic, strength, begin));
        _seqChromatic.ChainDelay(delay);
        _seqChromatic.Chain(Tween.MaterialProperty(_copyMat, _idChromatic, 0, fin));
    }
    private void Outline(float begin,float delay,float fin,Color color)
    {
        _seqOutline.Stop();
        _copyMat.SetColor(GameManager.s_alphaoutlinecolor,Color.clear);
        _seqOutline = Sequence.Create();
        _seqOutline.Chain(Tween.MaterialProperty(_copyMat, _idOutline, color, begin));
        _seqOutline.ChainDelay(delay);
        _seqOutline.Chain(Tween.MaterialProperty(_copyMat, _idOutline, Color.clear, fin));
    }
}
