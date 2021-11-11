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
    public bool B_Held {get; private set;}

    public void NextInputFrame() {
        X = false;
        Y = false;
        A = false;
        B = false;
        Move = false;
    }

    public void PressA(InputAction.CallbackContext context) {if (!context.canceled) A = true;}
    public void PressB(InputAction.CallbackContext context) {
        if (!context.canceled) {
            B = true; B_Held = true;
        } 
        else B_Held = false;
    }
    public void PressX(InputAction.CallbackContext context) {if (!context.canceled) X = true;}
    public void PressY(InputAction.CallbackContext context) {if (!context.canceled) Y = true;}

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
