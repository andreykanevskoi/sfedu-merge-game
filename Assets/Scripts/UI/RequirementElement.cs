using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RequirementElement : MonoBehaviour {
    [SerializeField] private Image _image;
    public Image Image { get => _image; }

    [SerializeField] private TextMeshProUGUI _amount;
    private static string _textPattern = "{0} / {1}";

    public void UpdateElement(int current, int amount) {
        _amount.SetText(_textPattern, current, amount);
    }
}
