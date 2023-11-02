using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class GameManager : MonoBehaviour
{
    public static float recoveryDamage = 0.3f;
    
    [FoldoutGroup("UI")] [TitleGroup("UI/Combo")]
    public Image image_Combo;

    [FoldoutGroup("UI")] [TitleGroup("UI/Combo")]
    public RectTransform rectT_Combo_Main, rectT_Combo_Sub;

    [FoldoutGroup("UI")] [TitleGroup("UI/Combo")]
    public DamageNumber dmp_Main, dmp_Sub;
    [FoldoutGroup("UI")] [TitleGroup("UI/Combo")]
    public float comboDelay = 2.5f;
    private Sequence s_combo_first,s_combo_continuous,s_combo;
    private DamageNumber dmp_created_main, dmp_created_sub;
    private float comboBeginTime = -100;
    private int comboAction = 0;
    private string subTest;
    
    private void Setting_UI()
    {
        image_Combo.material.SetFloat(s_fadeamount, 0.5f);
        image_Combo.material.SetFloat(s_chromaberramount, 0.05f);
        
        s_combo_first = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .Append(image_Combo.material.DOFloat(-0.1f, s_fadeamount, 0.2f))
            .Join(image_Combo.rectTransform.DOShakeAnchorPos(0.5f, 15f, 30))
            .Join(image_Combo.material.DOFloat(0.3f, s_chromaberramount, 0.1f))
            .Insert(0.25f, image_Combo.material.DOFloat(0.05f, s_chromaberramount, 0.15f))
            .Insert(comboDelay, image_Combo.material.DOFloat(0.5f, s_fadeamount, 0.5f))
            .PrependCallback(() =>
            {
                image_Combo.material.SetFloat(s_fadeamount, 0.5f);
                image_Combo.material.SetFloat(s_chromaberramount, 0.05f);
                image_Combo.rectTransform.anchoredPosition = new Vector2(15, 0);
                
                if(dmp_created_main!=null) dmp_created_main.FadeOut();
                if(dmp_created_sub!=null) dmp_created_sub.FadeOut();
                
                ComboText();
            })
            .InsertCallback(comboDelay,() =>
            {
                if(dmp_created_main!=null) dmp_created_main.FadeOut();
                if(dmp_created_sub!=null) dmp_created_sub.FadeOut();
            });

        s_combo_continuous = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .Append(image_Combo.rectTransform.DOShakeAnchorPos(0.5f, 15f, 30))
            .Join(image_Combo.material.DOFloat(0.3f, s_chromaberramount, 0.1f))
            .Insert(0.25f, image_Combo.material.DOFloat(0.05f, s_chromaberramount, 0.15f))
            .Insert(comboDelay, image_Combo.material.DOFloat(0.5f, s_fadeamount, 0.5f))
            .PrependCallback(() =>
            {
                image_Combo.material.SetFloat(s_chromaberramount, 0.05f);
                image_Combo.rectTransform.anchoredPosition = new Vector2(15, 0);
                
                if(dmp_created_main!=null) dmp_created_main.FadeOut();
                if(dmp_created_sub!=null) dmp_created_sub.FadeOut();
                
                ComboText();
            })
            .InsertCallback(comboDelay,() =>
            {
                if(dmp_created_main!=null) dmp_created_main.FadeOut();
                if(dmp_created_sub!=null) dmp_created_sub.FadeOut();
            });


    }
    private void ComboText()
    {
        dmp_created_main = dmp_Main.Spawn(Vector3.zero, comboAction+s_action);
        dmp_created_main.SetAnchoredPosition(rectT_Combo_Main, new Vector2(0, 0));
        dmp_created_sub = dmp_Sub.Spawn(Vector3.zero, subTest);
        dmp_created_sub.SetAnchoredPosition(rectT_Combo_Sub, new Vector2(0, 0));
    }
    public void Combo(string subComboText)

    {
        if (Time.time - comboBeginTime > comboDelay)
        {
            comboAction = 1;
            comboBeginTime = Time.time;
            subTest = subComboText;
            
            
            if (s_combo_continuous.IsPlaying()) s_combo_continuous.Pause();
            if (!s_combo_first.IsInitialized()) s_combo_first.Play();
            else s_combo_first.Restart();
        }
        else
        {
            comboAction++;
            comboBeginTime = Time.time;
            if (subComboText == s_normalattack) subComboText = s_continuousattack;
            subTest = subComboText;
            
            
            if (s_combo_first.IsPlaying()) s_combo_first.Pause();
            if (!s_combo_continuous.IsInitialized()) s_combo_continuous.Play();
            else s_combo_continuous.Restart();
        }
    }
}
