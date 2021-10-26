using System;
using UnityEngine;

public class GameEvents : MonoBehaviour {
    public static GameEvents current;

    private void Awake() {
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

    public event Action<Vector3Int> OnTileDestroy;
    public void TriggerTileDestroy(Vector3Int position) {
        OnTileDestroy?.Invoke(position);
    }

    public event Action<Vector3, Placeable> OnDrop;
    public void TriggerDrop(Vector3 position, Placeable placeable) {
        OnDrop?.Invoke(position, placeable);
    }

    public event Action<Vector3, Placeable> OnDrag;
    public void TriggerDrag(Vector3 position, Placeable placeable) {
        OnDrag?.Invoke(position, placeable);
    }
}
