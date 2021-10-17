using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mergeable : MonoBehaviour
{
    [SerializeField] private MergebaleObjectsLine _line;
    public MergebaleObjectsLine ObjectLine => _line;

    [SerializeField]
    private Draggable _sprite;

    [SerializeField]
    private int _currentLevel;
    public int Level => _currentLevel;


    // Текущее расположение на tilemap
    // [HideInInspector]
    public Vector3Int currentCell;

    public bool isMergeable(Mergeable other) {
        if (_currentLevel == _line.MaxLevel || _currentLevel != other.Level) {
            return false;
        }
        return _line.isMergeable(other.ObjectLine);
    }

    public void Merge(Mergeable other) {
        Vector3 position = _sprite.transform.position;
        Destroy(_sprite.gameObject);
        _sprite = null;

        _currentLevel++;

        _sprite = Instantiate(_line.GetCurrentLevelObject(_currentLevel), transform);
        _sprite.transform.position = position;
        _sprite.parent = this;
    }

    private void Awake() {
        _sprite.parent = this;
    }

    private void OnDestroy() {
        Destroy(_sprite.gameObject);
    }
}
