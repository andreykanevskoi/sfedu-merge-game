using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RequirementElement : MonoBehaviour {
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _currentAmount;
    [SerializeField] private TextMeshProUGUI _requiredAmount;

    [SerializeField] private Animator _animator;

    public void Init(Sprite sprite, int currentAmount, int requiredAmount) {
        _image.sprite = sprite;
        _currentAmount.SetText(currentAmount.ToString());
        _requiredAmount.SetText(requiredAmount.ToString());
    }

    public void StartAnimation() {
        //_animator.gameObject.SetActive(true);
        _animator.SetTrigger("AppearanceTrigger");
    }

    public void UpdateElement(int amount) {
        _currentAmount.SetText(amount.ToString());
        _animator.SetTrigger("ChangeAmountTrigger");
    }
}
