using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StageBanner : MonoBehaviour
{
    [TitleGroup("SaveData")] public bool activated,selected;
    [TitleGroup("SaveData")][Range(0,3)] public int starCount = 0;
    [FoldoutGroup("Setting")] public Color activatedColor,deactivatedColor;
    [FoldoutGroup("Setting")] [Range(1, 3)] public int difficulty = 1;
    [FoldoutGroup("Setting")] public TMP_Text tmpDifficulty;
    [FoldoutGroup("Setting")] public Image imgBanner;
    [FoldoutGroup("Setting")] public List<Image> matImages = new List<Image>();
    [FoldoutGroup("Setting")] public List<Image> stars = new List<Image>();
    [FoldoutGroup("Setting")] public Material matNorm;
    [FoldoutGroup("Setting")] public Material matSelected;
    private static string strDeactivated = "잠김", strEasy = "쉬움", strNorm = "보통", strHard = "어려움";
    private static string strAlphaOutlineBlend = "_AlphaOutlineBlend",strAlphaOutlineColor = "_AlphaOutlineColor";
    private SelectUI _selectUI;
    private Sequence _seqMpb;
    private Tween _tPunch;
    private Vector3 _scale;
    public void Setting()
    {
        _scale = transform.localScale;
    }

    public void UpdateBanner()
    {
        if (activated)
        {
            if (difficulty == 1) tmpDifficulty.text = strEasy;
            else if (difficulty == 2) tmpDifficulty.text = strNorm;
            else tmpDifficulty.text = strHard;
            imgBanner.color = activatedColor;

            Color cDeadStar = new Color(0.25f, 0.25f, 0.25f, 1.0f);
            stars[0].color = starCount>=1?Color.white:cDeadStar;
            stars[1].color = starCount>=2?Color.white:cDeadStar;
            stars[2].color = starCount>=3?Color.white:cDeadStar;
        }
        else
        {
            tmpDifficulty.text = strDeactivated;
            imgBanner.color = deactivatedColor;
            foreach (var star in stars) star.color = new Color(0.25f, 0.25f, 0.25f, 1.0f);
        }

        if (selected)
        {
            foreach (var image in matImages)
            {
                image.material = matSelected;
            }
            if(activated) Click_Activate();
            else Click_Deactivate();
        }
        else
        {
            foreach (var image in matImages)
            {
                image.material = matNorm;
            }
            Click_Reset();
        }
    }

    public void Click_Activate()
    {
        _seqMpb.Stop();
        _tPunch.Stop();
        Transform t = transform;
        matSelected.SetColor(strAlphaOutlineColor,Color.white);
        matSelected.SetFloat(strAlphaOutlineBlend, 0);
        t.localScale = _scale;
        _tPunch = Tween.PunchScale(t, Vector3.one * 0.5f, 0.5f, 2);
        _seqMpb = Sequence.Create();
        _seqMpb.Group(Tween.Custom(0, 1, 0.125f, 
            onValueChange: ratio =>matSelected.SetFloat(strAlphaOutlineBlend,ratio)));
        _seqMpb.ChainDelay(0.5f);
        _seqMpb.Chain(Tween.Custom(1, 0, 0.75f, 
            onValueChange: ratio =>matSelected.SetFloat(strAlphaOutlineBlend,ratio)));
    }
    public void Click_Deactivate()
    {
        _seqMpb.Stop();
        _tPunch.Stop();
        Transform t = transform;
        matSelected.SetColor(strAlphaOutlineColor,new Color(1.0f,0,0,1));
        matSelected.SetFloat(strAlphaOutlineBlend, 0);
        t.localScale = _scale;
        _tPunch = Tween.PunchScale(t, Vector3.one * 0.5f, 0.5f, 2);
        _seqMpb = Sequence.Create();
        _seqMpb.Group(Tween.Custom(0, 1, 0.125f, 
            onValueChange: ratio =>matSelected.SetFloat(strAlphaOutlineBlend,ratio)));
        _seqMpb.ChainDelay(0.5f);
        _seqMpb.Chain(Tween.Custom(1, 0, 0.75f, 
            onValueChange: ratio =>matSelected.SetFloat(strAlphaOutlineBlend,ratio)));
    }
    public void Click_Reset()
    {
        _seqMpb.Stop();
    }
}
