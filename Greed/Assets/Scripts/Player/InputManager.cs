using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public Vector2 Dir {get; private set;}
    public bool Move {get; private set;}
    public bool X {get; private set;}
    public bool Y {get; private set;}
    public bool A {get; private set;}
    public bool B {get; private set;}
    public bool B_Released {get; private set;}
    public bool LeftShoulder {get; private set;}
    public bool RightShoulder {get; private set;}
    public bool LeftShoulder_Held {get; private set;}

    public void NextInputFrame() {
        X = false;
        Y = false;
        A = false;
        B = false;
        Move = false;
        RightShoulder = false;
        LeftShoulder = false;
    }

    public void PressA(InputAction.CallbackContext context) {if (!context.canceled) A = true;}
    public void PressB(InputAction.CallbackContext context) {
        if (!context.canceled) {
            B = true; B_Released = false;
        } 
        else B_Released = true;
    }
    public void PressX(InputAction.CallbackContext context) {if (!context.canceled) X = true;}
    public void PressY(InputAction.CallbackContext context) {if (!context.canceled) Y = true;}
    public void PressRightShoulder(InputAction.CallbackContext context) {if (!context.canceled) RightShoulder = true;}
    public void PressLeftShoulder(InputAction.CallbackContext context) {
        if (!context.canceled) {
            LeftShoulder = true; LeftShoulder_Held = true;
        }
        else
            LeftShoulder_Held = false;
    }


    public void PressMove(InputAction.CallbackContext context) {
        if (context.canceled)
            Dir = Vector2.zero;
        else
        {
            Dir = context.ReadValue<Vector2>();
            Move = true;
        }
    }

}
