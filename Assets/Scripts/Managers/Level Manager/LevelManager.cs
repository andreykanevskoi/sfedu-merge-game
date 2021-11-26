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

    [SerializeField] private FieldManager _fieldManager;

    [SerializeField] private LevelTransitionAnimator _sceneLoader;

    public void ObjectAppearance(Placeable placeable) {
        if (_requirements.ContainsKey(placeable.BaseName)) {
            _requirements[placeable.BaseName].Mark();
            Check();
        }
    }

    public void ObjectDisappearance(Placeable placeable) {
        if (_requirements.ContainsKey(placeable.BaseName)) {
            _requirements[placeable.BaseName].Dismark();
            Check();
        }
    }

    private void Check() {
        foreach (var requirement in _requirements.Values) { 
            if (!requirement.Check()) {
                return;
            }
        }

        OnLevelCompleted();
    }

    public void OnLevelCompleted() {
        PlayerPrefs.SetString("complete", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        GameEvents.current.TriggerPlayerInputDisable();
        var window = UIManager.current.CreateLevelCompleteWindow();
        window.Init(() => StartCoroutine(StartRedirection()));
    }

    private IEnumerator StartRedirection() {
        BlackScreen blackScreen = UIManager.current.CreateBlackScreen();
        blackScreen.SetA(0f);

        yield return StartCoroutine(blackScreen.BlackScreenFade(1f));
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void Awake() {
        _requirements = new Dictionary<string, VictoryRequirement>();

        foreach (var req in _victoryRequirements) {
            var uiElem = Instantiate(_requirementElementPrefab, _requirementsUIPanel.transform);
            req.Init(uiElem);

            _requirements.Add(req.placeable.BaseName, req);
        }
        _victoryRequirements = null;

        _fieldManager.InitFreePositionStorage();
        _fieldManager.InitTileManager();
        _fieldManager.InitSmogManager();
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
    }

    private void OnEnable() {
        GameEvents.current.OnObjectAppearance += ObjectAppearance;
        GameEvents.current.OnObjectDisappearance += ObjectDisappearance;
    }

    private void OnDisable() {
        GameEvents.current.OnObjectAppearance -= ObjectAppearance;
        GameEvents.current.OnObjectDisappearance -= ObjectDisappearance;
    }
}
