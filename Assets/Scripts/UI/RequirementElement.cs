using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RequirementElement : MonoBehaviour {
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _currentAmount;
    [SerializeField] private TextMeshProUGUI _requiredAmount;

    private bool _started = false;

    public void Init(Sprite sprite, int currentAmount, int requiredAmount) {
        _image.sprite = sprite;
        _currentAmount.SetText(currentAmount.ToString());
        _requiredAmount.SetText(requiredAmount.ToString());
    }

    public void StartAnimation() {
        GetComponent<Animator>().SetTrigger("AppearanceTrigger");
        _started = true;
    }

    public void UpdateElement(int amount) {
        _currentAmount.SetText(amount.ToString());
        if (_started) {
            GetComponent<Animator>().SetTrigger("ChangeAmountTrigger");
        }
    }
}
