using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelCompleteWindow : MonoBehaviour {
    [SerializeField] private GameObject _mergeStatisticPanel;
    [SerializeField] private GameObject _digStatisticPanel;
    [SerializeField] private Button _toCampButton;

    public void Init(Action action) {
        _toCampButton.onClick.AddListener(() => {
            action?.Invoke();
        });
    }
}
