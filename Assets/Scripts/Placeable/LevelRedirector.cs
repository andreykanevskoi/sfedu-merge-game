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

        chest.currentCell = currentCell;
        chest.fieldManager = fieldManager;
        chest.Position = fieldManager.GetCellWorldPosition(currentCell);

        fieldManager.RemovePlaceableFromField(this);
        fieldManager.AddPlaceableToField(chest);
        
        //инициализация сундука
        chest.InitChest(_levelReward, 10);
        
        GameEvents.current.TriggerPlayerInputEnable();

        Destroy(gameObject);
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
