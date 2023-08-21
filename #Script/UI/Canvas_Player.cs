using System;
using System.Collections;
using System.Collections.Generic;
//using Beautify.Universal;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public partial class Canvas_Player : MonoBehaviour
{
    public static Canvas_Player instance;
    public void Setting()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();
        anim.speed = 0;
        //ingame 각종 인풋 세팅
        RectTransform ingame = transform.Find("Ingame").GetComponent<RectTransform>();
        //ingame.pivot = Vector2.one*0.5f;
        //ingame.anchoredPosition = new Vector2(Screen.width * 0.5f, -Screen.height * 0.5f);
        //ingame.sizeDelta = new Vector2(Screen.width, Screen.height);
        foreach (var stick in ingame.GetComponentsInChildren<UI_Stick>())
        {
            stick.Setting();
        }
        
        
        skillGauge_L = ingame.Find("Skill").Find("Fill").Find("Left").GetComponentInChildren<Setting_SkillGauge>();
        skillGauge_R = ingame.Find("Skill").Find("Fill").Find("Right").GetComponentInChildren<Setting_SkillGauge>();
        unitFrame = ingame.Find("UnitFrame").GetComponent<CanvasGroup>();
        health = ingame.Find("UnitFrame").Find("Bar_Health").Find("Fill").GetComponent<Setting_ProgressBar>();
        transition = transform.Find("Transition").GetComponentInChildren<Image>();
        
        
        Setting_Inventory();
        Setting_Death();
        Setting_Sound();
        anim.SetBool(s_enter,false);
        menuType = MenuType.None;
    }
    public void Setting_After()
    {
        Color colorL_Highlight = Manager_Main.instance.mainData.ElementalColor_Highlight
            (Player.instance.data_Weapon_SkillL.elementalAttributes);
        Color colorL_Charged = Manager_Main.instance.mainData.ElementalColor_Charged
            (Player.instance.data_Weapon_SkillL.elementalAttributes);
        Color colorL_Uncharged = Manager_Main.instance.mainData.ElementalColor_Uncharged
            (Player.instance.data_Weapon_SkillL.elementalAttributes);
        Color colorR_Highlight = Manager_Main.instance.mainData.ElementalColor_Highlight
            (Player.instance.data_Weapon_SkillR.elementalAttributes);
        Color colorR_Charged = Manager_Main.instance.mainData.ElementalColor_Charged
            (Player.instance.data_Weapon_SkillR.elementalAttributes);
        Color colorR_Uncharged = Manager_Main.instance.mainData.ElementalColor_Uncharged
            (Player.instance.data_Weapon_SkillR.elementalAttributes);
        skillGauge_L.Setting(colorL_Uncharged,colorL_Charged,colorL_Highlight,Player.instance.data_Weapon_SkillL.elementalAttributes);
        skillGauge_R.Setting(colorR_Uncharged,colorR_Charged,colorR_Highlight,Player.instance.data_Weapon_SkillR.elementalAttributes);
    }
    #region InputSystem, Event
    
    [HideInInspector] public UnityEvent OnLateUpdate,OnABPressed,OnABReleased,OnRBPressed,OnRBReleased,OnSBPressed;
    public static float LS_Scale,
        RS_Scale,
        AS_Scale,AS_LastScale,
        RB_PressedTime = -100,
        AB_PressedTime = -100,AB_ReleasedTime=0,
        SB_PressedTime = -100;
    public static Vector2 LS,RS,AS;
    public static bool RB_Pressed,AB_Pressed,SB_Pressed,AS_Dragged;
    

    public void OnLeftStick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //print("Update");
            LS = context.ReadValue<Vector2>();
            LS_Scale = LS.magnitude;
        }
        else if (context.canceled)
        {
            //print("Released");
            LS = Vector2.zero;
            LS_Scale = 0;
        }
        else if(context.started)
        {
            //print("Pressed");
            LS = Vector2.zero;
            LS_Scale = 0;
        }
    }
    public void OnRightStick(InputAction.CallbackContext context)
    {
        if(context.performed && RB_Pressed)
        {
            //print("Update");
            RS = context.ReadValue<Vector2>();
            RS_Scale = RS.magnitude;
        }
        else if (context.canceled)
        {
            //print("Released");
            RS = Vector2.zero;
            RS_Scale = 0;
        }
        else if (context.started && RB_Pressed)
        {
            //print("Pressed");
            RS = Vector2.zero;
            RS_Scale = 0;
        }
    }

    public void OnActionStick(InputAction.CallbackContext context)
    {
        if(context.performed && AB_Pressed)
        {
            //print("AS_Dragged");
            AS = context.ReadValue<Vector2>();
            AS_Scale = AS.magnitude;
            AS_LastScale = AS_Scale;
            AS_Dragged = true;
        }
        else if (context.canceled)
        {
            //print("Released");
            AS = Vector2.zero;
            AS_Scale = 0;
            
        }
        else if (context.started  && AB_Pressed)
        {
            //print("Pressed");
            AS = Vector2.zero;
            AS_Scale = 0;
            AS_LastScale = 0;
        }
    }

    public void OnRightButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RB_PressedTime = Time.time;
            RB_Pressed = true;
            OnRBPressed?.Invoke();
        }
        else if (context.canceled)
        {
            RB_Pressed = false;
            OnRBReleased?.Invoke();
        }
    }
    public void OnActionButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AB_Pressed = true;
            AB_PressedTime = Time.unscaledTime;
            AS_LastScale = 0;
            AS_Dragged = false;
            OnABPressed?.Invoke();
        }
        else if (context.canceled)
        {
            AB_Pressed = false;
            AB_ReleasedTime = Time.unscaledTime;
            OnABReleased?.Invoke();
        }
    }
    public void OnSkillButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SB_PressedTime = Time.unscaledTime;
            SB_Pressed = true;
            OnSBPressed?.Invoke();
        }
        else if (context.canceled) SB_Pressed = false;
    }
    public static float Degree_Left()
    {
        float deg = 90-Mathf.Atan2(LS.y, LS.x) * Mathf.Rad2Deg;
        while (deg > 180) deg -= 360;
        while (deg < -180) deg += 360;
        return deg;
    }
    public static float Degree_Right()
    {
        float deg = 90-Mathf.Atan2(RS.y, RS.x) * Mathf.Rad2Deg;
        while (deg > 180) deg -= 360;
        while (deg < -180) deg += 360;
        return deg;
    }
    public static float Degree_Action()
    {
        float deg = 90-Mathf.Atan2(AS.y, AS.x) * Mathf.Rad2Deg;
        while (deg > 180) deg -= 360;
        while (deg < -180) deg += 360;
        return deg;
    }
    public void LateUpdate()
    {
        OnLateUpdate?.Invoke();
    }
    #endregion
    #region UI
    
    private RectTransform rectTransform;
    [HideInInspector] public CanvasGroup unitFrame;
    [HideInInspector] public Setting_SkillGauge skillGauge_L,skillGauge_R;
    [HideInInspector] public Setting_ProgressBar health;
    private Image transition;
    #endregion
    #region Animation
    
    [FoldoutGroup("TextMeshPro")] public TMP_Text tmp_Level,tmp_Tooltip_Header,tmp_Tooltip_info
        ,tmp_Coin,tmp_Crystal,tmp_InfoText;
    [HideInInspector]public Animator anim;
    //Enter-------------------------------------------------------------------------------------------------------------
    private string s_enter = "Enter";
    public void Enter()
    {
        anim.SetBool(s_enter,true);
    }
    public void PlayerStart()
    {
        Player.instance.animator.SetTrigger("Transition");
    }
    //하단의 Info--------------------------------------------------------------------------------------------------------
    public enum InfoType {No_ManaStone=0,No_SkillGauge=1}
    private string c_no_manastone = "마법석이 부족합니다!"
        , c_no_skillGauge = "스킬 게이지가 부족합니다!";
    public void InfoText(InfoType infoType)
    {
        string stateName = "Activated";
        switch (infoType)
        {
            case InfoType.No_ManaStone:
                tmp_InfoText.text = c_no_manastone;
                break;
            case InfoType.No_SkillGauge:
                tmp_InfoText.text = c_no_skillGauge;
                break;
        }
        if(!anim.GetCurrentAnimatorStateInfo(2).IsName(stateName))anim.Play(stateName,2);
    }
    //Coin, Crystal-----------------------------------------------------------------------------------------------------
    private string str_coin = "Anim_Coin",
        str_crystal = "Anim_Crystal",
        str_AddCoin = "AddCoin",
        str_AddCrystal = "AddCrystal";
    private Coroutine crt_coin = null, crt_crystal = null;
    public void Coin(int coin=1)
    {
        anim.SetInteger(str_AddCoin,anim.GetInteger(str_AddCoin)+coin);
        anim.CrossFadeInFixedTime(str_coin,0,2);
    }
    public void Crystal(int crystal=1)
    {
        anim.SetInteger(str_AddCrystal,anim.GetInteger(str_AddCrystal)+crystal);
        anim.CrossFadeInFixedTime(str_crystal,0,3);
    }
    
    //0:None, 1:Player, 2:Inventory, 3.Setting--------------------------------------------------------------------------
    public enum MenuType {None=0,Player=1,Inventory=2,Setting=3}
    private MenuType menuType = MenuType.None;
    public void Menu(MenuType menuType)
    {
        if (menuType == this.menuType)
        {
            anim.CrossFade("Menu_None",0.5f,0);
            this.menuType = MenuType.None;
            return;
        }
        switch (menuType)
        {
            case MenuType.None:
                anim.CrossFade("Menu_None",0.2f,0);
                break;
            case MenuType.Player:
                anim.CrossFade("Menu_Player",0.2f,0);
                break;
            case MenuType.Inventory:
                anim.CrossFade("Menu_Inventory",0.2f,0);
                break;
            case MenuType.Setting:
                anim.CrossFade("Menu_Setting",0.2f,0);
                break;
        }

        this.menuType = menuType;
    }
    public void Menu_Setting()
    {
        Menu(MenuType.Setting);
    }
    public void Menu_Player()
    {
        Menu(MenuType.Player);
    }
    public void Menu_Inventory()
    {
        Menu(MenuType.Inventory);
    }
    public void SetSetting()
    {
        foreach (var image in GetComponentsInChildren<Image>())
        {
            image.raycastTarget = false;
            print(image.gameObject.name);
        }
    }
    //Clear-------------------------------------------------------------------------------------------------------------
    private string s_animclearactivate1 = "Anim_Clear_Activate_1",
                   s_animclearactivate2 = "Anim_Clear_Activate_2",
                   Anim_Clear_Activate_3 = "Anim_Clear_Activate_3";
    public void Level_Clear()
    {
        anim.CrossFade(s_animclearactivate1,0,0,0);
    }
    #endregion

    #region Particle

    [FoldoutGroup("Particle")]
    public ParticleSystem particle_skill_fire, particle_skill_water, particle_skill_wind, particle_skill_ground;

    public void ElementalParticle(ElementalAttributes elementalAttributes)
    {
        switch (elementalAttributes)
        {
            case ElementalAttributes.Fire:
                particle_skill_fire.Play();
                break;
            case ElementalAttributes.Water:
                particle_skill_water.Play();
                break;
            case ElementalAttributes.Wind:
                particle_skill_wind.Play();
                break;
            case ElementalAttributes.Ground:
                particle_skill_ground.Play();
                break;
        }
    }

    #endregion

    public void Production_Begin()
    {
        CamArm.instance.Production_Begin();
    }
}
