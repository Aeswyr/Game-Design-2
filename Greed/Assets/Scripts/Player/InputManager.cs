using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public Vector2 Dir {get; private set;}
    private InputAction.CallbackContext moveCTX = default(InputAction.CallbackContext);
    private InputAction.CallbackContext BCTX = default(InputAction.CallbackContext);
    public bool X {get; private set;}
    public bool Y {get; private set;}
    public bool A {get; private set;}
    public bool B {get; private set;}
    public bool B_Held {get; private set;}

    void FixedUpdate() {
        if (!moveCTX.Equals(default(InputAction.CallbackContext)) && moveCTX.canceled)
        {
        
            Dir = Vector2.zero;
            moveCTX = default(InputAction.CallbackContext);
        }
        X = false;
        Y = false;
        A = false;
        B = false;

        if (!BCTX.Equals(default(InputAction.CallbackContext))) {
            if (BCTX.canceled) {
                B_Held = false;
                BCTX = moveCTX = default(InputAction.CallbackContext);
            } else
                B_Held = true;
        }
    }

    public void PressA(InputAction.CallbackContext context) {if (context.started) A = true;}
    public void PressB(InputAction.CallbackContext context) {if (context.started) B = true; BCTX = context;}
    public void PressX(InputAction.CallbackContext context) {if (context.started) X = true;}
    public void PressY(InputAction.CallbackContext context) {if (context.started) Y = true;}

    public void PressMove(InputAction.CallbackContext context) {
        Dir = context.ReadValue<Vector2>();
        moveCTX = context;
    }

}
