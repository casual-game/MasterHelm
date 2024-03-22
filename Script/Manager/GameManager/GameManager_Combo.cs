using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
public partial class GameManager : MonoBehaviour
{
    public static float recoveryDamage = 0.33f;
    [TitleGroup("콤보 시스템 인스펙터")]
    [TabGroup("콤보 시스템 인스펙터/ComboUI","인게임",SdfIconType.Controller)]
    public DamageNumber dmp_normal, dmp_strong, dmp_weak;
    [TabGroup("콤보 시스템 인스펙터/ComboUI","캔버스",SdfIconType.CardImage)]
    public Image image_Combo;
    [TabGroup("콤보 시스템 인스펙터/ComboUI","캔버스",SdfIconType.CardImage)]
    public RectTransform rectT_Combo_Main, rectT_Combo_Sub;
    [TabGroup("콤보 시스템 인스펙터/ComboUI","캔버스",SdfIconType.CardImage)]
    public DamageNumber dmp_Main, dmp_Sub;
    
    
    private float comboDelay = 2.5f;//-> 콤보 텍스트 초기화되는 딜레이 시간
    
    private Sequence s_combo;
    private DamageNumber dmp_created_main, dmp_created_sub;
    private float comboBeginTime = -100;
    private int comboAction = 0;
    private string subTest;
    private int id_fadeamount, id_chromaaberramount;
    private Vector2 comboAnchoredPos;
    private void Setting_UI()
    {
        image_Combo.material.SetFloat(s_fadeamount, 0.5f);
        image_Combo.material.SetFloat(s_chromaberramount, 0.05f);
        id_fadeamount = Shader.PropertyToID(s_fadeamount);
        id_chromaaberramount = Shader.PropertyToID(s_chromaberramount);
        comboAnchoredPos = image_Combo.rectTransform.anchoredPosition;
        

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
            
            
            s_combo.Stop();
            
            image_Combo.material.SetFloat(s_fadeamount, 0.5f);
            image_Combo.material.SetFloat(s_chromaberramount, 0.3f);
            image_Combo.rectTransform.anchoredPosition = comboAnchoredPos;
                
            if(dmp_created_main!=null) dmp_created_main.FadeOut();
            if(dmp_created_sub!=null) dmp_created_sub.FadeOut();
            ComboText();

            s_combo = Sequence.Create(useUnscaledTime:true)
                .Group(Tween.MaterialProperty(image_Combo.material, id_fadeamount, -0.1f, 0.2f))
                .Group(Tween.Custom(0,1,0.5f,onValueChange: newVal =>
                {
                    Vector2 RandomVec = Random.insideUnitCircle.normalized * Mathf.Clamp01(2-2*newVal) * 15;
                    image_Combo.rectTransform.anchoredPosition = comboAnchoredPos + RandomVec;
                } ))
                .Group(Tween.MaterialProperty(image_Combo.material, id_chromaaberramount, 0.05f, 0.25f))
                .ChainDelay(2.25f)
                .ChainCallback(() =>
                {
                    if (dmp_created_main != null) dmp_created_main.FadeOut();
                    if (dmp_created_sub != null) dmp_created_sub.FadeOut();
                })
                .Group(Tween.MaterialProperty(image_Combo.material, id_fadeamount, 0.5f, 0.5f));
        }
        else
        {
            comboAction++;
            comboBeginTime = Time.time;
            if (subComboText == s_normalattack) subComboText = s_continuousattack;
            subTest = subComboText;
            
            
            s_combo.Stop();
            
            image_Combo.material.SetFloat(s_chromaberramount, 0.3f);
            image_Combo.rectTransform.anchoredPosition = comboAnchoredPos;
                
            if(dmp_created_main!=null) dmp_created_main.FadeOut();
            if(dmp_created_sub!=null) dmp_created_sub.FadeOut();
            ComboText();
            
            s_combo = Sequence.Create(useUnscaledTime:true)
                .Group(Tween.Custom(0,1,0.5f,onValueChange: newVal =>
                {
                    Vector2 RandomVec = Random.insideUnitCircle.normalized * Mathf.Clamp01(2-2*newVal) * 15;
                    image_Combo.rectTransform.anchoredPosition = comboAnchoredPos + RandomVec;
                } ))
                .Group(Tween.MaterialProperty(image_Combo.material, id_chromaaberramount, 0.05f, 0.25f))
                .ChainDelay(2.25f)
                .ChainCallback(() =>
                {
                    if (dmp_created_main != null) dmp_created_main.FadeOut();
                    if (dmp_created_sub != null) dmp_created_sub.FadeOut();
                })
                .Group(Tween.MaterialProperty(image_Combo.material, id_fadeamount, 0.5f, 0.5f));
        }
    }
}
