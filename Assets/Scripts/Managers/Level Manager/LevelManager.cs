using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private VictoryRequirement[] _victoryRequirements;
    private Dictionary<string, VictoryRequirement> _requirements;

    [SerializeField] private GameObject _requirementsUIPanel;
    [SerializeField] private RequirementElement _requirementElementPrefab;
    [SerializeField] private MinigameObject _minigameObject;

    [SerializeField] private FieldManager _fieldManager;

    [SerializeField] private LevelTransitionAnimator _sceneLoader;

    private int _taskCompleted = 0;

    private MergeStatisticCollector _mergeStatistic;
    private TileStatisticCollector _tileStatistic;

    public void ObjectAppearance(Placeable placeable) {
        if (_requirements.ContainsKey(placeable.BaseName)) {
            _requirements[placeable.BaseName].Mark();
        }
    }

    public void ObjectDisappearance(Placeable placeable) {
        if (_requirements.ContainsKey(placeable.BaseName)) {
            _requirements[placeable.BaseName].Dismark();
        }
    }

    private void Awake() {
        _requirements = new Dictionary<string, VictoryRequirement>();

        foreach (var req in _victoryRequirements) {
            var uiElem = Instantiate(_requirementElementPrefab, _requirementsUIPanel.transform);
            req.Init(this, uiElem);

            _requirements.Add(req.placeable.BaseName, req);
        }
        _victoryRequirements = null;

        _fieldManager.InitFreePositionStorage();
        _fieldManager.InitTileManager();
        _fieldManager.InitSmogManager();

        _mergeStatistic = new MergeStatisticCollector();
        _tileStatistic = new TileStatisticCollector();
    }

    private void Start() {
        _fieldManager.LateInitObjectManager();
        StartCoroutine(StartLevelAnimation());
    }

    private IEnumerator StartLevelAnimation() {
        // Ожидаем первого кадра
        yield return null;
        if (_sceneLoader) {
            // Ждём завершения анимации начала уровня
            yield return _sceneLoader.StartSceneStartAnimation();
        }
        foreach(var req in _requirements.Values) {
            req.Start();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnTaskComplete() {
        _taskCompleted++;
        if (_taskCompleted == _requirements.Count) {
            OnLevelCompleted();
        }
    }

    public void OnTaskFallBack() {
        _taskCompleted--;
    }

    private void OnLevelCompleted() {
        PlayerPrefs.SetString("complete", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        GameEvents.current.TriggerPlayerInputDisable();

        _requirementsUIPanel.gameObject.SetActive(false);

        UIManager.current.CreateElement("EndLevelMessage");

        if (_minigameObject) {
            StartMiniGame();
            return;
        }

        ShowEndWindow();
    }

    private void StartMiniGame() {
        MinigameWindow window = UIManager.current.CreateElement("MinigameWindow").GetComponent<MinigameWindow>();
        window.Init(_minigameObject,
            () => {
                Destroy(window.gameObject);
                ShowEndWindow();
            }
        );
    }

    private void ShowEndWindow() {
        var window = UIManager.current.CreateElement("LevelCompleteWindows").GetComponent<LevelCompleteWindow>(); ;
        window.Init(_mergeStatistic, _tileStatistic, () => StartCoroutine(StartRedirection()));
    }

    private IEnumerator StartRedirection() {
        if (_sceneLoader) {
            // Ждём завершения анимации начала уровня
            yield return _sceneLoader.StartSceneEndAnimation();
        }
        Debug.Log("LoadScene");
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    
    public void SceneExit(string sceneName)
    {
        StartCoroutine(StartRedirection());
    }

    private void OnEnable() {
        GameEvents.current.OnObjectAppearance += ObjectAppearance;
        GameEvents.current.OnObjectDisappearance += ObjectDisappearance;
        GameEvents.current.OnExitScene += SceneExit;
    }

    private void OnDisable() {
        GameEvents.current.OnObjectAppearance -= ObjectAppearance;
        GameEvents.current.OnObjectDisappearance -= ObjectDisappearance;
        GameEvents.current.OnExitScene -= SceneExit;
    }
}
