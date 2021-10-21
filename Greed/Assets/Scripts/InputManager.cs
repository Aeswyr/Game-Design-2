using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public Vector2 Dir {get; private set;}
    private InputAction.CallbackContext moveCTX = default(InputAction.CallbackContext);
    public bool X {get; private set;}
    public bool Y {get; private set;}
    public bool A {get; private set;}
    public bool B {get; private set;}

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
    }

    public void PressA() {A = true;Debug.Log("A");}
    public void PressB() {B = true;Debug.Log("B");}
    public void PressX() {X = true;Debug.Log("X");}
    public void PressY() {Y = true;Debug.Log("Y");}

    public void PressMove(InputAction.CallbackContext context) {
        Dir = context.ReadValue<Vector2>();
        moveCTX = context;
        Debug.Log("Move");
    }

}
