using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Текущее состояние
    private IInputManagerState _currentState;
    private InputControls _controls;

    void Start() {
        _currentState = new DragInputState();
        _controls = new InputControls();

        _controls.Player.Enable();
        // Привязка обработчиков к событиям ввода
        _controls.Player.Touch.performed += OnClick;
        _controls.Player.Release.performed += OnRelease;
        _controls.Player.Hold.performed += OnHold;
    }

    public void OnClick(InputAction.CallbackContext context) {
        _currentState.OnClick();
    }

    public void OnRelease(InputAction.CallbackContext context) {
        _currentState.OnRelease();
    }

    public void OnHold(InputAction.CallbackContext context) {
        _currentState.OnHold();
    }

    void Update() {
        _currentState.Update();
    }
}
