using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private IInputManagerState _currentState;
    private InputControls _controls;

    // Start is called before the first frame update
    void Start() {
        _currentState = new DragInputState();
        _controls = new InputControls();
        _controls.Player.Enable();

        _controls.Player.Touch.performed += OnClick;
        _controls.Player.Drag.performed += OnDrag;
    }

    public void OnClick(InputAction.CallbackContext context) {
        _currentState.OnClick();
    }

    public void OnDrag(InputAction.CallbackContext context) {
        _currentState.OnDrag();
    }

    // Update is called once per frame
    void Update() {
        _currentState.Update();
    }
}
