using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class VictoryRequirement {
    public Placeable placeable;
    public int amount;

    private int _currentAmount;
    private RequirementElement _requirementElement;

    public void Init(RequirementElement requirementElement) {
        _requirementElement = requirementElement;

        _requirementElement.Image.sprite = placeable.GetComponent<SpriteRenderer>().sprite;
        Update();
    }

    private void Update() {
        _requirementElement.UpdateElement(_currentAmount, amount);
    }

    public void Mark() {
        _currentAmount++;
        Update();
    }

    public void Dismark() {
        _currentAmount--;
        Update();
    }

    public bool Check() {
        return _currentAmount >= amount;
    }
}

