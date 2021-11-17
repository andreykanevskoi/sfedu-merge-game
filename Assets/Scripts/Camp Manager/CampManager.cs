using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

[System.Serializable]
public class SmogedArea {
    public Placeable requiredObject;
    public Tile smogTile;
}

public class CampManager : MonoBehaviour {
    /// <summary>
    /// Массив туманных областей.
    /// </summary>
    [SerializeField] private SmogedArea[] _areas;

    /// <summary>
    /// Словаль туманных областей по их требованиям.
    /// </summary>
    private Dictionary<string, SmogedArea> _requirements;

    /// <summary>
    /// Список открытых областей.
    /// </summary>
    private List<SmogedArea> _completed;

    [SerializeField] private FieldManager _fieldManager;

    /// <summary>
    /// Обработка события появления нового объекта.
    /// </summary>
    /// <param name="placeable">Появившийся объект</param>
    public void ObjectAppearance(Placeable placeable) {
        string baseName = placeable.BaseName;
        if (_requirements.ContainsKey(baseName)) {
            // Тип тайла области
            Tile tile = _requirements[baseName].smogTile;

            // Отметить область как завершенную
            _completed.Add(_requirements[baseName]);
            _requirements.Remove(baseName);

            // Начать исчезновение тумана
            Debug.Log("Before start");
            _fieldManager.RemoveSmogedArea(tile);
        }
    }

    private void Awake() {
        _completed = new List<SmogedArea>();

        // Преобразование массива в словарь
        _requirements = new Dictionary<string, SmogedArea>();
        foreach (var area in _areas) {
            _requirements.Add(area.requiredObject.BaseName, area);
        }
        _areas = null;
    }

    #region - OnEnable / OnDisable -

    private void OnEnable() {
        GameEvents.current.OnObjectAppearance += ObjectAppearance;
    }

    private void OnDisable() {
        GameEvents.current.OnObjectAppearance -= ObjectAppearance;
    }

    #endregion
}
