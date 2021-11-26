using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTransitionAnimator : MonoBehaviour {
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private CanvasGroup locationName;

    private void Awake() {
        if(locationName) {
            locationName.alpha = 0f;
        }
    }

    public Coroutine StartSceneStartAnimation() {
        return StartCoroutine(StartAnimation());
    }

    public Coroutine StartSceneEndAnimation() {
        return StartCoroutine(EndAnimation());
    }

    private IEnumerator StartAnimation() {
        yield return StartCoroutine(BlackScreenFade(0.5f, 1f));
        StartCoroutine(LocationNameAnimation());
        yield return StartCoroutine(BlackScreenFade(0f, 1f));
        if (blackScreen) {
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }
    }

    private IEnumerator EndAnimation() {
        if (blackScreen) {
            blackScreen.gameObject.SetActive(true);
        }
        yield return StartCoroutine(BlackScreenFade(1f, 1f));
    }

    private IEnumerator LocationNameAnimation() {
        if (!locationName) yield break;

        Debug.Log("LocationNameAnimation");

        while (locationName.alpha < 1f) {
            locationName.alpha = locationName.alpha + (2f * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        while (locationName.alpha > 0f) {
            locationName.alpha = locationName.alpha - (1f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator BlackScreenFade(float fadeLimit, float fadeSpeed) {
        if (!blackScreen) yield break;

        if (fadeLimit <= blackScreen.alpha) {
            Debug.Log("BlackScreenFade");
            while (blackScreen.alpha > fadeLimit) {
                blackScreen.alpha = blackScreen.alpha - (fadeSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else {
            while (blackScreen.alpha <= fadeLimit) {
                blackScreen.alpha = blackScreen.alpha + (fadeSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
