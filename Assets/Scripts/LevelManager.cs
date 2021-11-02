using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VictoryRequirement
{
    public Placeable placeable;
    public int amount;

    private int _currentAmount;

    public void Mark() {
        _currentAmount++;
    }

    public void Dismark() {
        _currentAmount--;
    }

    public bool Check() {
        return _currentAmount >= amount;
    }
}

public class LevelManager : MonoBehaviour
{
    [SerializeField] private VictoryRequirement[] _victoryRequirements;
    private Dictionary<string, VictoryRequirement> _requirements;

    private void Awake() {
        _requirements = new Dictionary<string, VictoryRequirement>();

        foreach(var req in _victoryRequirements) {
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
