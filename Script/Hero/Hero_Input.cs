using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Hero : MonoBehaviour
{
    private Vector2 moveStick;

    public void Input_moveStick(InputAction.CallbackContext ctx)
    {
        moveStick = ctx.ReadValue<Vector2>();
    }
}
