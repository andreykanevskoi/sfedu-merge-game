using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    private static string _resourcesPath = "UI/";
    public static UIManager current;

    [SerializeField] private Canvas _canvas;

    public GameObject CreateElement(string name) {
        GameObject go = Instantiate(Resources.Load(_resourcesPath + name) as GameObject);
        SetToCanvas(go);
        return go;
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
