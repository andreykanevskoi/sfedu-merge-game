using UnityEngine;
using UnityEngine.InputSystem;

public class DestroyInputState : IInputManagerState {
    private static int _layerMask = 1 << LayerMask.NameToLayer("Tiles");

    private Vector3 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    private Placeable GetTarget() {
        Collider2D targetObject = Physics2D.OverlapPoint(GetMousePosition(), _layerMask);
        if (targetObject) {
            return targetObject.GetComponentInParent<Placeable>();
        }
        return null;
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
