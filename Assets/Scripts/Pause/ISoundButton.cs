using System;
using UnityEngine;
using UnityEngine.UI;

public class ISoundButton : MonoBehaviour
{
    [SerializeField] private Sprite _spriteEnabled;
    [SerializeField] private Sprite _spriteDisabled;
    [SerializeField] private Image _image;

    protected void ChangeIcon(int state)
    {
        if (state == 1)
        {
            _image.sprite = _spriteEnabled;
            return;
        }

        _image.sprite = _spriteDisabled;
    }
}
