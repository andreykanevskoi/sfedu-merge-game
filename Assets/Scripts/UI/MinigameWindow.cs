using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MinigameWindow : MonoBehaviour
{
    private MinigameObject _minigameObject;
    private Action _OnDone;

    private void OnMinigameComplete() {
        _minigameObject.dirtObject.Reset();
        _OnDone?.Invoke();
    }

    public void Init(MinigameObject minigameObject, Action OnDone) {
        _minigameObject = Instantiate(minigameObject, transform);
        _OnDone = OnDone;

        _minigameObject.dirtObject.OnMinigameEnd += OnMinigameComplete;

        _minigameObject.dirtObject.Begin();
    }

    public void SetObject(MinigameObject minigameObject) {
        _minigameObject = Instantiate(minigameObject, transform);
    }
}
