using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRedirector : Placeable {
    [SerializeField] private GameObject _lable;

    [SerializeField] private string _sceneName;
    [SerializeField] private Placeable[] _levelReward;

    [SerializeField] private Chest _chestPrefab;

    private void HideLable() {
        _lable.gameObject.SetActive(false);
    }
    private void ShowLable() {
        _lable.gameObject.SetActive(true);
    }

    private void OnSmogAreaDisappearance() {
        if (!fieldManager.smogManager.IsSmoged(currentCell)) {
            ShowLable();
        }
    }

    public override void Click() {
        GameEvents.current.TriggerLevelRedirectionIntent(_sceneName);
    }

    public override bool BeginDrag() {
        return false;
    }

    private void OnLevelComplete(string sceneName) {
        if (_sceneName == sceneName && !fieldManager.smogManager.IsSmoged(currentCell)) {
            GameEvents.current.TriggerPlayerInputDisable();
            StartCoroutine(CreateReward());
        }
    }

    private IEnumerator CreateReward() {
        CameraHandler camera =  Camera.main.GetComponent<CameraHandler>();
        Debug.Log(transform.position);
        yield return StartCoroutine(camera.FocusOnPoint(transform.position));

        Chest chest = Instantiate(_chestPrefab, transform.parent);
        chest.AddItems(_levelReward);

        chest.currentCell = currentCell;
        chest.fieldManager = fieldManager;
        chest.Position = fieldManager.GetCellWorldPosition(currentCell);

        fieldManager.RemovePlaceableFromField(this);
        fieldManager.AddPlaceableToField(chest);
        
        chest.AddTimer(1000);
        
        GameEvents.current.TriggerPlayerInputEnable();
        PlayerPrefs.DeleteAll();

        Destroy(gameObject);
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);

        if (_levelReward != null) {
            writer.Write(_levelReward.Length);
            foreach (var prefab in _levelReward) {
                writer.Write(prefab.prefabId);
            }
        }
        else {
            writer.Write(0);
        }
        writer.Write(_chestPrefab.prefabId);
        writer.Write(_sceneName);
    }

    public override void Load(GameDataReader reader, PlaceableFactory factory) {
        base.Load(reader, factory);

        int count = reader.ReadInt();
        if (count > 0) {
            _levelReward = new Placeable[count];
            for (int i = 0; i < count; i++) {
                int Id = reader.ReadInt();
                _levelReward[i] = factory.GetPrefab(Id);
            }
        }
        _chestPrefab = (Chest) factory.GetPrefab(reader.ReadInt());
        _sceneName = reader.ReadString();
    }

    private void Start() {
        if(fieldManager.smogManager.IsSmoged(currentCell)) {
            HideLable();
        }
    }

    private void OnEnable() {
        GameEvents.current.OnSmogAreaDisappearance += OnSmogAreaDisappearance;
        GameEvents.current.OnLevelComplete += OnLevelComplete;
    }

    private void OnDisable() {
        GameEvents.current.OnSmogAreaDisappearance -= OnSmogAreaDisappearance;
        GameEvents.current.OnLevelComplete -= OnLevelComplete;
    }
}
