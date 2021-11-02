using UnityEngine;

public class Mergeable : Placeable {
    [SerializeField] private MergebaleObjectsLine _line;
    public MergebaleObjectsLine ObjectLine => _line;

    [SerializeField]
    private int _currentLevel;
    public int Level => _currentLevel;

    public bool isMergeable(Mergeable other) {
        if (other == this)
            return false;

        if (_currentLevel == _line.MaxLevel || _currentLevel != other.Level) {
            return false;
        }
        return _line.Equals(other.ObjectLine);
    }

    public Mergeable GetNextLevelObject() {
        return _line.GetCurrentLevelObject(_currentLevel + 1);
    }

    public Mergeable Merge(Mergeable mergeable) {
        if (!isMergeable(mergeable)) {
            return null;
        }

        Mergeable newMergeable = Instantiate(GetNextLevelObject());

        Destroy(gameObject);
        Destroy(mergeable.gameObject);

        return newMergeable;
    }
}
