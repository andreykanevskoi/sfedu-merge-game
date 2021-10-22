using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mergeable : Placeable
{
    [SerializeField] private MergebaleObjectsLine _line;
    public MergebaleObjectsLine ObjectLine => _line;

    [SerializeField]
    private int _currentLevel;
    public int Level => _currentLevel;

    public bool isMergeable(Mergeable other) {
        if (_currentLevel == _line.MaxLevel || _currentLevel != other.Level) {
            return false;
        }
        return _line.isMergeable(other.ObjectLine);
    }

    public Mergeable GetNextLevelObject() {
        return _line.GetCurrentLevelObject(_currentLevel + 1);
    }

    public Mergeable Merge(Mergeable mergeable) {
        Mergeable newMergeable = Instantiate(GetNextLevelObject());
        newMergeable.transform.position = transform.position;
        newMergeable.currentCell = currentCell;

        Destroy(gameObject);
        Destroy(mergeable.gameObject);

        return newMergeable;
    }
}
