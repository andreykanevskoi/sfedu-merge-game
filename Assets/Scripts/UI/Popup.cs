using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.InputSystem.UI;
using System;

public class Popup : MonoBehaviour {
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private Button _admitButton;
    [SerializeField] private Button _denyButton;

    private Action _admitAction;
    private Action _denyAction;

    private void OnDeny() {
        _denyAction?.Invoke();
        Destroy(gameObject);
    }

    public void Init(string message, Action admitAction, Action denyAction) {
        _message.SetText(message);

        _admitAction = admitAction;
        _denyAction = denyAction;

        _admitButton.onClick.AddListener(() => {
            _admitAction?.Invoke();
        });

        _denyButton.onClick.AddListener(() => {
            OnDeny();
        });
    }
}
