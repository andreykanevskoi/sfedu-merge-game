using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchModeButton : MonoBehaviour {

    [SerializeField] private Sprite _handSprite;
    [SerializeField] private Sprite _shovelSprite;
    [SerializeField] private Button _button;

    public void ChangeSprite() {
        if (_button.image.sprite == _handSprite) {
            _button.image.sprite = _shovelSprite;
            return;
        }
        _button.image.sprite = _handSprite;
    }
}
