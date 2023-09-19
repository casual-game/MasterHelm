using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Vector2 JS_Attack = Vector2.zero,JS_Move = Vector2.zero,JS_Action = Vector2.zero;
    public static bool BTN_Attack = false,BTN_Action,Bool_Move;
    public UnityEvent E_BTN_Action_Begin;
    public UnityEvent E_BTN_Action_Fin;
    //string들을 미리 캐시로 저장
    #region strings
    public static string s_speed = "Speed",s_rot = "Rot",s_turn = "Turn",s_crouch = "Crouch",s_roll = "Roll"
        ,s_footstep = "Footstep";
    #endregion
    public void Awake()
    {
        instance = this;
    }

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
        if (inputValue.performed && BTN_Attack) JS_Attack = inputValue.ReadValue<Vector2>();
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
        }
        else if (inputValue.canceled)
        {
            BTN_Attack = false;
            JS_Attack = Vector2.zero;
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
}
