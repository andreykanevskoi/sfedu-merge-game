using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour {
    private Image _image;

    [SerializeField, Range(0.1f, 10f)] private float _fadeSpeed = 10f;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void SetA(float a) {
        Color color = _image.color;
        color = new Color(color.r, color.g, color.b, a);
        _image.color = color;
    }

    public void SetRaycastTarget(bool raycastTarget) {
        _image.raycastTarget = raycastTarget;
    }

    public IEnumerator BlackScreenFade(float fade) {
        yield return null;
        Debug.Log("Start fading");

        Color color = _image.color;
        float fadeLimit = fade;

        if (fade < color.a) {
            while (color.a > fadeLimit) {
                fade = color.a - (_fadeSpeed * Time.deltaTime);
                color = new Color(color.r, color.g, color.b, fade);
                _image.color = color;
                yield return null;
            }
        }
        else {
            while (color.a <= fadeLimit) {
                fade = color.a + (_fadeSpeed * Time.deltaTime);
                color = new Color(color.r, color.g, color.b, fade);
                _image.color = color;
                yield return null;
            }
        }
    }
}
