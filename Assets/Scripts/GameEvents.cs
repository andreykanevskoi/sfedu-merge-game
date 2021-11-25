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

    public event Action OnSmogAreaDisappearance;
    public void TriggerSmogAreaDisappearance() {
        OnSmogAreaDisappearance?.Invoke();
    }

    public event Action OnPlayerInputDisable;
    public void TriggerPlayerInputDisable() {
        OnPlayerInputDisable?.Invoke();
    }

    public event Action OnPlayerInputEnable;
    public void TriggerPlayerInputEnable() {
        OnPlayerInputEnable?.Invoke();
    }

    public event Action<string> OnLevelRedirectionIntent;
    public void TriggerLevelRedirectionIntent(string sceneName) {
        OnLevelRedirectionIntent?.Invoke(sceneName);
    }

    public event Action<string> OnLevelComplete;
    public void TriggerLevelComplete(string sceneName) {
        OnLevelComplete?.Invoke(sceneName);
    }
}
