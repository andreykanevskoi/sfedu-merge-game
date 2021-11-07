using System;
using UnityEngine;

public class GameEvents : MonoBehaviour {
    public static GameEvents current;

    private void OnEnable() {
        if (!current) {
            current = this;
        }
    }

    public event Action OnModeSwitch;
    public void TriggerModeSwitch() {
        OnModeSwitch?.Invoke();
    }

    public event Action<Vector3> OnFieldClick;
    public void TriggerFieldClick(Vector3 position) {
        OnFieldClick?.Invoke(position);
    }

    public event Action<Vector3> OnTileSelect;
    public void TriggerSelectTile(Vector3 position) {
        OnTileSelect?.Invoke(position);
    }

    public event Action<Vector3, Placeable> OnObjectDrop;
    public void TriggerDrop(Vector3 position, Placeable placeable) {
        OnObjectDrop?.Invoke(position, placeable);
    }

    public event Action<Vector3, Placeable> OnObjectDrag;
    public void TriggerDrag(Vector3 position, Placeable placeable) {
        OnObjectDrag?.Invoke(position, placeable);
    }

    public event Action<Placeable> OnObjectAppearance;
    public void TriggerObjectAppearance(Placeable placeable) {
        OnObjectAppearance?.Invoke(placeable);
    }

    public event Action<Placeable> OnObjectDisappearance;
    public void TriggerObjectDisappearance(Placeable placeable) {
        OnObjectDisappearance?.Invoke(placeable);
    }
}
