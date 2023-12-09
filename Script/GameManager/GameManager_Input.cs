using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public partial class GameManager : MonoBehaviour
{
    public static Vector2 JS_Attack = Vector2.zero,JS_Move = Vector2.zero,JS_Action = Vector2.zero;
    public static bool BTN_Attack = false,BTN_Action,Bool_Move,Bool_Attack;
    public static float AttackReleasedTime = -100;
    
    [HideInInspector] public UnityEvent E_LateUpdate;
    [HideInInspector] public UnityEvent E_BTN_Action_Begin,E_BTN_Action_Fin,E_BTN_Attack_Begin,E_BTN_Attack_Fin;
    
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
    public static void Reset_AttackRealeasedTime()
    {
        AttackReleasedTime = -100;
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
    [HideInInspector] public UnityEvent E_Debug1_Begin, E_Debug1_Fin,E_Debug2_Begin, E_Debug2_Fin,E_Debug3_Begin, E_Debug3_Fin;
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
}
