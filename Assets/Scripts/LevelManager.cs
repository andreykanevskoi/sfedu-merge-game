using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VictoryRequirement
{
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
        Debug.Log(_currentAmount);
        Debug.Log(amount);
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

public class LevelManager : MonoBehaviour
{
    [SerializeField] private VictoryRequirement[] _victoryRequirements;
    private Dictionary<string, VictoryRequirement> _requirements;

    [SerializeField] private GameObject _requirementsUIPanel;
    [SerializeField] private RequirementElement _requirementElementPrefab;

    private void Awake() {
        _requirements = new Dictionary<string, VictoryRequirement>();

        foreach(var req in _victoryRequirements) {
            var uiElem = Instantiate(_requirementElementPrefab, _requirementsUIPanel.transform);
            req.Init(uiElem);

            _requirements.Add(req.placeable.BaseName, req);
        }

        _victoryRequirements = null;
    }

    public void ObjectAppearance(Placeable placeable) {
        if (_requirements.ContainsKey(placeable.BaseName)) {
            _requirements[placeable.BaseName].Mark();
            Check();
        }
    }

    public void ObjectDisappearance(Placeable placeable) {
        if (_requirements.ContainsKey(placeable.BaseName)) {
            _requirements[placeable.BaseName].Dismark();
            Check();
        }
    }

    private void Check() {
        foreach (var requirement in _requirements.Values) { 
            if (!requirement.Check()) {
                return;
            }
        }

        Debug.Log("Level complete");
    }
}
