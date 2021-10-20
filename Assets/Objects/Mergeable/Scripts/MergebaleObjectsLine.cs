using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New mergeable object", menuName = "Mergaeble")]
public class MergebaleObjectsLine : ScriptableObject {

    [SerializeField] private Mergeable[] _objectLevels;

    public int MaxLevel => _objectLevels.Length - 1;

    public Mergeable GetCurrentLevelObject(int level) {
        return _objectLevels[level];
    }

    public bool isMergeable(MergebaleObjectsLine line) {
        return Types.Equals(this, line);
    }
}
