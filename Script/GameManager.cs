using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static Transform Folder_Hero;
    public static Vector2 JS_Attack = Vector2.zero,JS_Move = Vector2.zero,JS_Action = Vector2.zero;
    public static bool BTN_Attack = false,BTN_Action,Bool_Move,Bool_Attack;
    [ShowInInspector] public static float AttackReleasedTime = -100;
    public UnityEvent E_LateUpdate;
    public UnityEvent E_BTN_Action_Begin,E_BTN_Action_Fin,E_BTN_Attack_Begin,E_BTN_Attack_Fin;
    
    //string들을 미리 캐시로 저장
    #region strings
    public static string s_speed = "Speed",s_rot = "Rot",s_crouch = "Crouch",s_state_change = "State_Change",s_state_type = "State_Type"
        ,s_footstep = "Footstep",s_charge_normal = "Charge_Normal",s_player = "Player"
        ,s_ladder = "Ladder",s_ladder_speed = "Ladder_Speed",s_transition = "Transition"
        ,s_hit = "Hit",s_hit_additive = "Hit_Additive",s_hit_rot = "Hit_Rot",s_hit_type = "Hit_Type"
        ,s_publiccolor = "_PublicColor",s_leftstate = "LeftState",s_chargeenterindex = "ChargeEnterIndex";
    #endregion

    #region Curves
    public AnimationCurve curve_inout,curve_in,curve_out;
    
    #endregion
    public void Awake()
    {
        Instance = this;
        Transform mainFolder = CreateFolder("Folder", null);
        Folder_Hero = CreateFolder("Hero", mainFolder);
        Transform CreateFolder(string folderName,Transform parent)
        {
            GameObject f = new GameObject(folderName);
            f.transform.SetParent(parent);
            f.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);
            f.transform.localScale = Vector3.one;
            return f.transform;
        }
    }

    public void LateUpdate()
    {
        E_LateUpdate?.Invoke();
    }

    #region InputSystem
    //기본
    public void Input_JS_Move(InputAction.CallbackContext inputValue)
    {
        if (inputValue.started)
        {
            JS_Move = Vector2.zero;
            Bool_Move = false;
        }
        else if (inputValue.performed)
        {
            JS_Move = inputValue.ReadValue<Vector2>();
            Bool_Move = true;
        }
        else if (inputValue.canceled)
        {
            JS_Move = Vector2.zero;
            Bool_Move = false;
        }
    }
    public void Input_JS_Attack(InputAction.CallbackContext inputValue)
    {
        if (inputValue.started)
        {
            Bool_Attack = false;
        }
        else if (inputValue.performed && BTN_Attack)
        {
            JS_Attack = inputValue.ReadValue<Vector2>();
            Bool_Attack = true;
        }
        else if (inputValue.canceled)
        {
            Bool_Attack = false;
        }
    }
    public static float DelayCheck_Attack()
    {
        return Time.time - AttackReleasedTime;
    }
    public void Input_JS_Action(InputAction.CallbackContext inputValue)
    {
        if (inputValue.performed && BTN_Action) JS_Action = inputValue.ReadValue<Vector2>();
    }
    public void Input_BTN_Attack(InputAction.CallbackContext inputValue)
    {
        if (inputValue.started)
        {
            BTN_Attack = true;
            JS_Attack = Vector2.zero;
            E_BTN_Attack_Begin?.Invoke();
        }
        else if (inputValue.canceled)
        {
            BTN_Attack = false;
            JS_Attack = Vector2.zero;
            AttackReleasedTime = Time.time;
            E_BTN_Attack_Fin?.Invoke();
        }
    }
    public void Input_BTN_Action(InputAction.CallbackContext inputValue)
    {
        if (inputValue.started)
        {
            BTN_Action = true;
            JS_Action = Vector2.zero;
            E_BTN_Action_Begin?.Invoke();
        }
        else if (inputValue.canceled)
        {
            BTN_Action = false;
            JS_Action = Vector2.zero;
            E_BTN_Action_Fin?.Invoke();
        }
    }
    //디버그
    public UnityEvent E_Debug1_Begin, E_Debug1_Fin,E_Debug2_Begin, E_Debug2_Fin,E_Debug3_Begin, E_Debug3_Fin;
    public void Input_BTN_Debug1(InputAction.CallbackContext inputValue)
    {
        if (inputValue.started)
        {
            E_Debug1_Begin?.Invoke();
        }
        else if (inputValue.canceled)
        {
            E_Debug1_Fin?.Invoke();
        }
    }
    public void Input_BTN_Debug2(InputAction.CallbackContext inputValue)
    {
        if (inputValue.started)
        {
            E_Debug2_Begin?.Invoke();
        }
        else if (inputValue.canceled)
        {
            E_Debug2_Fin?.Invoke();
        }
    }
    public void Input_BTN_Debug3(InputAction.CallbackContext inputValue)
    {
        if (inputValue.started)
        {
            E_Debug3_Begin?.Invoke();
        }
        else if (inputValue.canceled)
        {
            E_Debug3_Fin?.Invoke();
        }
    }
    #endregion
}
public enum AttackMotionType {Center=0,LeftSlash=60,RightSlash=-60}
public enum PlayerSmashedType {None =-1,Bound=2,Screw=3,Flip=4,Center=5,Stun=6}
