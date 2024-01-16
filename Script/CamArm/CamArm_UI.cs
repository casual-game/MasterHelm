using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class CamArm : MonoBehaviour
{
    private void Setting_UI()
    {
        tmp_round = g_Round_Norm_Num.GetComponent<TMP_Text>();
    }
    [TitleGroup("UI")] [FoldoutGroup("UI/round")]
    public Graphic
        g_Round_bg,
        g_Round_Norm_Text,
        g_Round_Norm_Num,
        g_Round_Final_Text,
        g_Round_Final_Num,
        g_Round_Clear_Text,
        g_Round_Clear_Num,
        g_Clear_bg;
    [TitleGroup("UI")] [FoldoutGroup("UI/round")]
    public CanvasGroup cg_Round,cg_Clear;
    [TitleGroup("UI")] [FoldoutGroup("UI/round")]
    public ParticleImage pi_Round,pi_Clear;
    private Sequence seqRound;
    public TMP_Text tmp_round;
    [Button]
    public void UI_Round(bool isFinal, int round=0)
    {
        seqRound.Stop();
        //글자
        float textRatio = 1.25f;
        //
        foreach (Transform t in cg_Round.transform)  t.localScale = Vector3.zero;
        if (isFinal)
        {
            cg_Round.gameObject.SetActive(true);
            g_Round_bg.gameObject.SetActive(true);
            g_Round_Final_Text.gameObject.SetActive(true);
            g_Round_Final_Num.gameObject.SetActive(true);
            g_Round_Norm_Text.gameObject.SetActive(false);
            g_Round_Norm_Num.gameObject.SetActive(false);
            pi_Round.gameObject.SetActive(true);
            pi_Round.Play();
            //
            seqRound = Sequence.Create(cycleMode: CycleMode.Yoyo, cycles: 2)
                .Group(Tween.Alpha(cg_Round,0,1,0.25f))
                //BG
                .Group(Tween.Scale(g_Round_bg.transform, 0.6f, 1.0f * textRatio,
                    0.375f, startDelay: 0.0f, ease: Ease.OutBack))
                .Group(Tween.Alpha(g_Round_bg, 0.0f, 1.0f,
                    0.25f * textRatio, startDelay: 0.0f))
                //Text
                .Group(Tween.Scale(g_Round_Final_Num.transform, 0.6f, 1.0f * textRatio,
                    0.375f, startDelay: 0.125f, ease: Ease.OutBack))
                .Group(Tween.Alpha(g_Round_Final_Num, 0.0f, 1.0f,
                    0.25f * textRatio, startDelay: 0.125f))
                //Num
                .Group(Tween.Scale(g_Round_Final_Text.transform, 0.6f, 1.0f * textRatio,
                    0.375f, startDelay: 0.25f, ease: Ease.OutBack))
                .Group(Tween.Alpha(g_Round_Final_Text, 0.0f, 1.0f,
                    0.2f * textRatio, startDelay: 0.25f))
                //Delay
                .ChainDelay(1.1f);
        }
        else
        {
            cg_Round.gameObject.SetActive(true);
            g_Round_bg.gameObject.SetActive(true);
            g_Round_Norm_Text.gameObject.SetActive(true);
            g_Round_Norm_Num.gameObject.SetActive(true);
            g_Round_Final_Text.gameObject.SetActive(false);
            g_Round_Final_Num.gameObject.SetActive(false);
            pi_Round.gameObject.SetActive(true);
            pi_Round.Play();
            tmp_round.text = round.ToString();
            //
            seqRound = Sequence.Create(cycleMode: CycleMode.Yoyo, cycles: 2)
                .Group(Tween.Alpha(cg_Round,0,1,0.25f))
                //BG
                .Group(Tween.Scale(g_Round_bg.transform, 0.6f, 1.0f * textRatio,
                    0.375f, startDelay: 0.0f, ease: Ease.OutBack))
                .Group(Tween.Alpha(g_Round_bg, 0.0f, 1.0f,
                    0.25f * textRatio, startDelay: 0.0f))
                //Text
                .Group(Tween.Scale(g_Round_Norm_Text.transform, 0.6f, 1.0f * textRatio,
                    0.375f, startDelay: 0.125f, ease: Ease.OutBack))
                .Group(Tween.Alpha(g_Round_Norm_Text, 0.0f, 1.0f,
                    0.25f * textRatio, startDelay: 0.125f))
                //Num
                .Group(Tween.Scale(g_Round_Norm_Num.transform, 0.6f, 1.0f * textRatio,
                    0.375f, startDelay: 0.25f, ease: Ease.OutBack))
                .Group(Tween.Alpha(g_Round_Norm_Num, 0.0f, 1.0f,
                    0.2f * textRatio, startDelay: 0.25f))
                //Delay
                .ChainDelay(1.0f);
        }
        seqRound.OnComplete(() => cg_Round.gameObject.SetActive(false));
    }

    [Button]
    public void UI_Clear()
    {
        seqRound.Stop();
        cg_Clear.gameObject.SetActive(true);
        g_Clear_bg.rectTransform.sizeDelta = new Vector2(g_Clear_bg.rectTransform.sizeDelta.x, 600);
        g_Round_Clear_Num.transform.localScale = Vector3.one*0.6f;
        g_Round_Clear_Text.transform.localScale = Vector3.one*0.6f;
        pi_Clear.Play();
        //
        seqRound = Sequence.Create(cycleMode: CycleMode.Yoyo, cycles: 2)
            .Group(Tween.Delay(0.25f,()=> pi_Round.Play()))
            .Group(Tween.Alpha(cg_Clear, 0, 1, 0.25f))
            //BG
            .Group(Tween.Scale(cg_Clear.transform, 0.6f, 1.0f,
                0.425f, startDelay: 0.0f, ease: Ease.OutBack))
            //Text,Num
            .Group(Tween.Scale(g_Round_Clear_Num.transform, 0.6f, 1.0f,
                0.375f, startDelay: 0.0f, ease: Ease.OutBack))
            .Group(Tween.Scale(g_Round_Clear_Text.transform, 0.6f, 1.0f,
                0.375f, startDelay: 0.125f, ease: Ease.OutBack))
            //BG 내리기
            .Group(Tween.UISizeDelta(g_Clear_bg.rectTransform, new Vector2(g_Clear_bg.rectTransform.sizeDelta.x, 840), 
                0.75f, Ease.InOutBack,startDelay: 0.0f))
            //Delay
            .ChainDelay(1.1f)
            .OnComplete(() => cg_Round.gameObject.SetActive(false));

    }
}
