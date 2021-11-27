using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class VictoryRequirement {
    public Placeable placeable;
    public int amount;

    private int _currentAmount;
    private RequirementElement _requirementElement;

    private LevelManager _levelManager;

    public void Init(LevelManager levelManager, RequirementElement requirementElement) {
        _levelManager = levelManager;
        _requirementElement = requirementElement;
        _requirementElement.Init(placeable.GetComponent<SpriteRenderer>().sprite, 0, amount);

        //_requirementElement.gameObject.SetActive(false);
    }

    public void Start() {
        //_requirementElement.gameObject.SetActive(true);
        _requirementElement.StartAnimation();
    }

    private void Update() {
        _requirementElement.UpdateElement(_currentAmount);
    }

    public void Mark() {
        _currentAmount++;
        Update();
        if (_currentAmount == amount) {
            _levelManager.OnTaskComplete();
        }
    }

    public void Dismark() {
        _currentAmount--;
        Update();
        if (_currentAmount == amount - 1) {
            _levelManager.OnTaskFallBack();
        }
    }
}

