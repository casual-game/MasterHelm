using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Canvas_Player_World : MonoBehaviour
{
    public static Canvas_Player_World instance;
    private string s_skull = "Skull";
    public void Setting()
    {
        instance = this;
        anim = GetComponent<Animator>();
        Canvas_Player.instance.OnLateUpdate.AddListener(Update_UnitFrame);
        Data_Default();
        ManaStone_Append(3);
    }
    #region 변수_UI
    //public
    [TitleGroup("고정 위치")] public Vector3 rotate,addVec;
    [TitleGroup("고정 위치")] public float height = 1.5f, dist = 0.5f;
    [TitleGroup("Image")] public Image image_skill_L, image_skill_R, 
        image_skill_M, image_stone_0, image_stone_1, image_stone_2;
    [TitleGroup("Image_기본")] public Color defaultSkillColor;
    [TitleGroup("Image_기본")] public float blinkDelay, blinkDuration;
    //private
    private float delay_SkillL, delay_SkillR, delay_SkillM;
    private Color currentColor_SkillL,currentColor_SkillR,currentColor_SkillM;
    private Animator anim;
    #endregion
    #region 함수_UI
    public void Data_Default()
    {
        Color rightColor = Manager_Main.instance.mainData.
            ElementalColor_Uncharged(Player.instance.data_Weapon_SkillR.elementalAttributes);
        Color rightColorH = Manager_Main.instance.mainData.
            ElementalColor_Highlight(Player.instance.data_Weapon_SkillR.elementalAttributes);
        image_skill_R.color = rightColorH;
        currentColor_SkillR = rightColor;
        currentColor_SkillL = defaultSkillColor;
        delay_SkillR = blinkDelay;
        delay_SkillL = 0;
        if (Canvas_Player.instance.skillGauge_R.fullCharged)
        {
            image_skill_M.color = rightColorH;
            currentColor_SkillM = rightColor;
            delay_SkillM = blinkDelay;
        }
        else
        {
            currentColor_SkillM = defaultSkillColor;
            delay_SkillM = 0;
        }
        if (DOTween.IsTweening(image_skill_L)) image_skill_L.DOKill();
        if (DOTween.IsTweening(image_skill_R)) image_skill_R.DOKill();
        if (DOTween.IsTweening(image_skill_M)) image_skill_M.DOKill();
        image_skill_L.DOColor(currentColor_SkillL, blinkDuration).SetDelay(delay_SkillL);
        image_skill_R.DOColor(currentColor_SkillR, blinkDuration).SetDelay(delay_SkillR);
        image_skill_M.DOColor(currentColor_SkillM, blinkDuration).SetDelay(delay_SkillM);
    }
    public void Update_Data()
    {
        int endMotionType=0;
        //스킬 아닐때
        if (!Player.instance.isSkill) endMotionType = (int) Player.instance.motionData.endMotionType;
        //스킬일때
        else if(Player.instance.isLeftSkill.HasValue)
        {
            if (Player.instance.isLeftSkill.Value)
                endMotionType = (int) Player.instance.data_Weapon_SkillL.SkillL.endMotionType;
            else endMotionType = (int) Player.instance.data_Weapon_SkillR.SkillR.endMotionType;
        }
        //Left
        if (endMotionType == 0)
        {
            Color leftColor= Manager_Main.instance.mainData.
                ElementalColor_Uncharged(Player.instance.data_Weapon_SkillL.elementalAttributes);
            Color leftColorH= Manager_Main.instance.mainData.
                ElementalColor_Highlight(Player.instance.data_Weapon_SkillL.elementalAttributes);
            image_skill_L.color = leftColorH;
            currentColor_SkillL = leftColor;
            currentColor_SkillR = defaultSkillColor;
            delay_SkillL = blinkDelay;
            delay_SkillR = 0;
            if (Canvas_Player.instance.skillGauge_L.fullCharged)
            {
                image_skill_M.color = leftColorH;
                currentColor_SkillM = leftColor;
                delay_SkillM = blinkDelay;
            }
            else
            {
                currentColor_SkillM = defaultSkillColor;
                delay_SkillM = 0;
            }
        }
        //Right
        else
        {
            Color rightColor = Manager_Main.instance.mainData.
                ElementalColor_Uncharged(Player.instance.data_Weapon_SkillR.elementalAttributes);
            Color rightColorH = Manager_Main.instance.mainData.
                ElementalColor_Highlight(Player.instance.data_Weapon_SkillR.elementalAttributes);
            image_skill_R.color = rightColorH;
            currentColor_SkillR = rightColor;
            currentColor_SkillL = defaultSkillColor;
            delay_SkillR = blinkDelay;
            delay_SkillL = 0;
            if (Canvas_Player.instance.skillGauge_R.fullCharged)
            {
                image_skill_M.color = rightColorH;
                currentColor_SkillM = rightColor;
                delay_SkillM = blinkDelay;
            }
            else
            {
                currentColor_SkillM = defaultSkillColor;
                delay_SkillM = 0;
            }
        }
        //색 설정
        if (DOTween.IsTweening(image_skill_L)) image_skill_L.DOKill();
        if (DOTween.IsTweening(image_skill_R)) image_skill_R.DOKill();
        if (DOTween.IsTweening(image_skill_M)) image_skill_M.DOKill();
        image_skill_L.DOColor(currentColor_SkillL, blinkDuration).SetDelay(delay_SkillL);
        image_skill_R.DOColor(currentColor_SkillR, blinkDuration).SetDelay(delay_SkillR);
        image_skill_M.DOColor(currentColor_SkillM, blinkDuration).SetDelay(delay_SkillM);
    }
    public void Update_UnitFrame()
    {
        Transform t = Player.instance.T_Canvas;
        Vector3 framePos = t.position + Vector3.up * height + addVec +
                           Quaternion.Euler(rotate)*(Player.instance.transform.rotation*Vector3.forward*dist);
        transform.position = framePos;
        
        if(Time.timeScale>0.01f && Time.unscaledTime>canFillTime) ManaStone_Charge();
    }

    
    #endregion

    #region 집중 슬롯
    //public
    public int manaStone = 0;
    public float fillAmount = 0.0f;
    public float canFillTime = -100, fillDelay = 2.0f;
    [TitleGroup("Image_마법석")] public Image manaStone_0, manaStone_1, manaStone_2;
    //private
    
    private Image currentManaStoneImage;
    public void ManaStone_Append(float append)
    {
        FillAmount(Mathf.Clamp(fillAmount + append,0,3.01f));
        while (fillAmount >= manaStone+1) ManaStone_Add();
        while(fillAmount < manaStone) ManaStone_Remove();
        
        void ManaStone_Remove()
        {
            if (manaStone <= 0) return;
            string stateName = "Bullet_" + manaStone + "_" + (manaStone - 1);
            anim.CrossFade(stateName,0.2f,0);
            manaStone--;
        }
        void ManaStone_Add()
        {
            string stateName = "Bullet_" + manaStone + "_" + (manaStone + 1);
            anim.CrossFade(stateName,0.2f,0);
            manaStone++;
        }
        void FillAmount(float amount)
        {
            fillAmount = amount;
        
            manaStone_0.fillAmount = fillAmount>=1? 0 : Mathf.Clamp01(fillAmount);
            manaStone_1.fillAmount = fillAmount>=2? 0 : Mathf.Clamp01(fillAmount-1);
            manaStone_2.fillAmount = fillAmount>=3? 0 : Mathf.Clamp01(fillAmount-2);
        } 
    }
    private void ManaStone_Charge()
    {
        ManaStone_Append(Player.instance.data_Weapon_Main.manaChargeSpeed*Time.unscaledDeltaTime);
    }
    public void ManaStone_DelayCharge()
    {
        canFillTime = Time.unscaledTime + fillDelay;
    }

    #endregion
    
    public void Skull()
    {
        anim.SetTrigger(s_skull);
    }
    
}
