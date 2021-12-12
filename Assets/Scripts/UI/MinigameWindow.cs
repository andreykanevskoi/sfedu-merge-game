using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MinigameWindow : MonoBehaviour
{
    [SerializeField] private GameObject _helperHand;
    [SerializeField] private GameObject _objectWindow;

    private MinigameObject _minigameObject;
    private Action _OnDone;

    public void Init(MinigameObject minigameObject, Action OnDone) {
        _minigameObject = Instantiate(minigameObject, _objectWindow.transform);
        _OnDone = OnDone;

        _minigameObject.dirtObject.OnMinigameEnd += OnMinigameComplete;
        _minigameObject.dirtObject.OnFirstTouch += () => _helperHand.SetActive(false);

        StartCoroutine(Prepare());
    }

    private IEnumerator Prepare() {
        yield return new WaitForSeconds(1f);

        GetComponent<Animator>().SetTrigger("ObjectAppearanceTrigger");
        yield return new WaitForSeconds(0.2f);

        _helperHand.SetActive(true);
        _minigameObject.dirtObject.Begin();
    }

    private void OnMinigameComplete() {
        StartCoroutine(EndAnimation());
    }

    private IEnumerator EndAnimation() {
        GetComponent<Animator>().SetTrigger("EndTrigger");
        yield return new WaitForSeconds(1f);
        _minigameObject.dirtObject.Reset();
        _OnDone?.Invoke();
    }
}
