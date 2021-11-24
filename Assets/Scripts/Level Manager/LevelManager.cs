﻿using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private VictoryRequirement[] _victoryRequirements;
    private Dictionary<string, VictoryRequirement> _requirements;

    [SerializeField] private GameObject _requirementsUIPanel;
    [SerializeField] private RequirementElement _requirementElementPrefab;

    [SerializeField] private FieldManager _fieldManager;

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

        Debug.Log("Level complete");
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
