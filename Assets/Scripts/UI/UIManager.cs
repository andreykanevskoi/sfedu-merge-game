using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager current;

    [SerializeField] private Canvas _canvas;

    public Popup CreatePopupWindows() {
        GameObject go = Instantiate(Resources.Load("UI/Popup") as GameObject);
        SetToCanvas(go);
        return go.GetComponent<Popup>();
    }

    public BlackScreen CreateBlackScreen() {
        GameObject go = Instantiate(Resources.Load("UI/BlackScreen") as GameObject);
        SetToCanvas(go);
        return go.GetComponent<BlackScreen>();
    }

    public LevelCompleteWindow CreateLevelCompleteWindow() {
        GameObject go = Instantiate(Resources.Load("UI/LevelCompleteWindows") as GameObject);
        SetToCanvas(go);
        return go.GetComponent<LevelCompleteWindow>();
    }

    public void SetToCanvas(GameObject go) {
        go.transform.SetParent(_canvas.transform);
        go.transform.localScale = Vector3.one;
        var rectTransform = go.GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }

    private void Awake() {
        if (current != null) {
            Destroy(gameObject);
            return;
        }

        current = this;
    }
}
