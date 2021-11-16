using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

[System.Serializable]
public class SmogedArea {
    public Placeable requiredObject;
    public Tile smogTile;
}

public class CampManager : MonoBehaviour, ISaveable {
    /// Массив туманных областей.
    [SerializeField] private SmogedArea[] _areas;
    /// Словаль туманных областей по их требованиям.
    private Dictionary<string, SmogedArea> _requirements;
    /// Список открытых областей.
    private List<SmogedArea> _completed;

    private Storage _storage;

    // [SerializeField] private Storage _storage;
    [SerializeField] private PlaceableFactory _factory;
    [SerializeField] private FieldManager _fieldManager;
    [SerializeField] private GameObject _objectsParent;

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
            _fieldManager.RemoveSmogedArea(tile);
        }
    }

    private void Awake() {
        _storage = new Storage();
        _completed = new List<SmogedArea>();

        _factory.Init();
        _fieldManager.InitSmogManager();

        // Преобразование массива в словарь
        _requirements = new Dictionary<string, SmogedArea>();
        foreach (var area in _areas) {
            _requirements.Add(area.requiredObject.BaseName, area);
        }
        _areas = null;

        if (_storage.CheckFile()) {
            LoadSave();
        }
    }

    private void LoadSave() {
        Placeable[] placeables = FindObjectsOfType<Placeable>();
        foreach (var placeable in placeables) {
            Destroy(placeable.gameObject);
        }

        _storage.Load(this, _factory);
    }

    public void Save(GameDataWriter writer) {
        writer.Write(_completed.Count);
        foreach (var compeled in _completed) {
            writer.Write(compeled.requiredObject.BaseName);
        }

        Placeable[] placeables = FindObjectsOfType<Placeable>();
        writer.Write(placeables.Length);
        foreach (var placeable in placeables) {
            writer.Write(placeable.prefabId);
            placeable.Save(writer);
        }
    }

    public void Load(GameDataReader reader, PlaceableFactory factory) {
        Debug.Log("Load");
        
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++) {
            string baseName = reader.ReadString();
            if (_requirements.ContainsKey(baseName)) {
                _fieldManager.smogManager.DeleteInstantly(_requirements[baseName].smogTile);
                _requirements.Remove(baseName);
            }
        }


        count = reader.ReadInt();
        Debug.Log(count);
        for (int i = 0; i < count; i++) {
            int prefabId = reader.ReadInt();
            Debug.Log(prefabId);
            Placeable placeable = Instantiate(factory.GetPrefab(prefabId), _objectsParent.transform);
            placeable.Load(reader, factory);
        }
    }

    private void OnApplicationQuit() {
        Storage storage = new Storage();
        storage.Save(this);
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
