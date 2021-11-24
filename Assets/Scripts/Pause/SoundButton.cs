using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    private int _soundOn;
    [SerializeField] private Sprite _spriteEnabled;
    [SerializeField] private Sprite _spriteDisabled;
    [SerializeField] private Image _image;
    [SerializeField] private String _playerPrefsKey;
    private void OnEnable()
    {
        _soundOn = PlayerPrefs.GetInt(_playerPrefsKey, 1);
        ChangeIcon();
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt(_playerPrefsKey, _soundOn);
    }

    public void SwitchOn()
    {
        _soundOn = _soundOn == 0 ? 1 : 0;
        ChangeIcon();
    }

    private void ChangeIcon()
    {
        if (_soundOn == 1)
        {
            _image.sprite = _spriteEnabled;
            return;
        }

        _image.sprite = _spriteDisabled;
    }
}
