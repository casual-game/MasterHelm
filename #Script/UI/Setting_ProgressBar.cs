using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Setting_ProgressBar : MonoBehaviour
{
    //비주얼 세팅
    public Vector2 minmmax = new Vector2(89,610);
    public TMP_Text tmp_percentage;
    public Color defaultColor;
    public Color glowColor;
    public float glowDuration = 0.5f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0,0,1,1);

    public float moveDuration = 0.25f;
    //메인 세팅
    public float max = 100;
    public float current = 100;
    public void SetValue(float value)
    {
        current = Mathf.Clamp(value,0,max);
        StopCoroutine("C_MoveRatio");
        StartCoroutine("C_MoveRatio");
    }

    
    IEnumerator C_MoveRatio()
    {
        Blink();
        float targetRatio = current/max;
        float beginRatio = _ratio;
        float beginTime = Time.unscaledTime;

        while (Time.unscaledTime - beginTime < moveDuration)
        {
            float timeRatio = moveCurve.Evaluate((Time.unscaledTime - beginTime) / moveDuration);
            ratio = Mathf.Lerp(beginRatio,targetRatio,timeRatio);
            tmp_percentage.text = Mathf.RoundToInt(ratio * 100) + "%";
            yield return null;
        }
        ratio = targetRatio;
        tmp_percentage.text = Mathf.RoundToInt(ratio * 100) + "%";
    }
    //PRIVATE
    private float _ratio=1;
    private Image image;
    public float ratio
    {
        get
        {
            return _ratio;
        }
        set
        {
            _ratio = Mathf.Clamp01(value);
            #if UNITY_EDITOR
            image = GetComponent<Image>();
            #endif
            _rectTransform.sizeDelta = 
                new Vector2(_rectTransform.sizeDelta.x,Mathf.Lerp(minmmax.x, minmmax.y, _ratio));
        }
    }
    private RectTransform _rectTransform;
    private void Start()
    {
        image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }
    [Button]
    public void Blink()
    {
        StopCoroutine("C_GLOW");
        StartCoroutine("C_GLOW");
    }
    private IEnumerator C_GLOW()
    {
        float beginTime = Time.unscaledTime;
        while (Time.unscaledTime-beginTime<glowDuration)
        {
            float ratio = (Time.unscaledTime - beginTime) / glowDuration;
            image.color = Color.Lerp(glowColor, defaultColor, ratio);
            yield return null;
        }
        
    }

}
