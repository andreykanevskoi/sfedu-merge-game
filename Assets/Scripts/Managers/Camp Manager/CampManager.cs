using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private string _completedSceneName = "";

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
        _completed = new List<SmogedArea>();

        _factory.Init();

        _fieldManager.InitFreePositionStorage();
        _fieldManager.InitTileManager();
        _fieldManager.InitSmogManager();

        _storage = new Storage();

        // Преобразование массива в словарь
        _requirements = new Dictionary<string, SmogedArea>();
        foreach (var area in _areas) {
            _requirements.Add(area.requiredObject.BaseName, area);
        }
        _areas = null;

        if (_storage.CheckFile()) {
            LoadSave();
        }
        else {
            _fieldManager.LateInitObjectManager();
        }
    }

    private void Start() {
        StartCoroutine(StartLevelAnimation());
    }
     
    private IEnumerator StartLevelAnimation() {
        BlackScreen blackScreen = UIManager.current.CreateBlackScreen();
        yield return StartCoroutine(blackScreen.BlackScreenFade(0f));
        Destroy(blackScreen.gameObject);
        if (_completedSceneName != "") {
            GameEvents.current.TriggerLevelComplete(_completedSceneName);
        }
    }

    private void AskRedirection(string sceneName) {
        GameEvents.current.TriggerPlayerInputDisable();
        Popup popup = UIManager.current.CreatePopupWindows();
        popup.Init("Вы уверены, что хотите перейти на уровень?",
                   () => StartCoroutine(StartRedirection(sceneName)),
                   () => GameEvents.current.TriggerPlayerInputEnable()
        );
    }

    private IEnumerator StartRedirection(string sceneName) {
        BlackScreen blackScreen = UIManager.current.CreateBlackScreen();
        blackScreen.SetA(0f);
        yield return StartCoroutine(blackScreen.BlackScreenFade(1f));
        Debug.Log("LoadScene");
        _storage.Save(this);
        LoadScene(sceneName);
    }

    private void LoadScene(string sceneName) {
        if (sceneName != "") {

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            return;
        }
        Debug.LogError("No scene name");
    }

    private void LoadSave() {
        // Уничтожаем все объекты на игровом поле
        Placeable[] placeables = FindObjectsOfType<Placeable>();
        foreach (var placeable in placeables) {
            Destroy(placeable.gameObject);
        }

        _fieldManager.InitObjectManager();

        _storage.Load(this, _factory);
    }

    /// <summary>
    /// Сохранение лагеря в файл.
    /// </summary>
    /// <param name="writer"></param>
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

    /// <summary>
    /// Загрузка лагеря из файла сохранения.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="factory"></param>
    public void Load(GameDataReader reader, PlaceableFactory factory) {
        Debug.Log("Load");
        
        int count = reader.ReadInt(); // Количество выполненых условий
        for (int i = 0; i < count; i++) {
            string baseName = reader.ReadString(); // Опеределитель условия
            if (_requirements.ContainsKey(baseName)) {
                _fieldManager.smogManager.DeleteInstantly(_requirements[baseName].smogTile); // Удаляем связанную область тумана
                _completed.Add(_requirements[baseName]);   // Отмечаем условие как выполненое
                _requirements.Remove(baseName);  // Удаляем условие из невыполненых
            }
        }

        // Количество сохраннёных объектов
        count = reader.ReadInt();
        Debug.Log(count);
        for (int i = 0; i < count; i++) {
            // Идентификатор объекта
            int prefabId = reader.ReadInt();
            Debug.Log(prefabId);

            // Создание объекта по его индентификатору
            Placeable placeable = Instantiate(factory.GetPrefab(prefabId), _objectsParent.transform);
            placeable.fieldManager = _fieldManager;
            // Загрузка внутреней информации объекта
            placeable.Load(reader, factory);
            // Добавление объекта на поле
            _fieldManager.AddPlaceableWithNoEventTriggered(placeable);
        }

        if (PlayerPrefs.HasKey("complete")) {
            _completedSceneName = PlayerPrefs.GetString("complete");
        }
    }

    private void OnApplicationPause(bool pause) {
        Debug.Log("OnApplicationPause");
        _storage.Save(this);
    }

    private void OnApplicationQuit() {
        Debug.Log("OnApplicationQuit");
        _storage.Save(this);
    }

    #region - OnEnable / OnDisable -

    private void OnEnable() {
        GameEvents.current.OnObjectAppearance += ObjectAppearance;
        GameEvents.current.OnLevelRedirectionIntent += AskRedirection;
    }

    private void OnDisable() {
        GameEvents.current.OnObjectAppearance -= ObjectAppearance;
        GameEvents.current.OnLevelRedirectionIntent -= AskRedirection;
    }

    #endregion
}
