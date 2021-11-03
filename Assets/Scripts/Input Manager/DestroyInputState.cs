using UnityEngine;
using UnityEngine.InputSystem;

public class DestroyInputState : IInputManagerState {
    private Vector3 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    void IInputManagerState.OnClick() {
    }

    void IInputManagerState.OnHold() {
    }

    void IInputManagerState.OnRelease() {
        GameEvents.current.TriggerFieldClick(GetMousePosition());
    }

    void IInputManagerState.Update() {
        GameEvents.current.TriggerSelectTile(GetMousePosition());
    }
}
