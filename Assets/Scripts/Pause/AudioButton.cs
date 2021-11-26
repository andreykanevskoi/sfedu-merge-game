using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioButton : ISoundButton
{
    private void OnEnable()
    {
        ChangeIcon(SoundManager.GetAudioOn());
    }

    public void ChangeState()
    {
        ChangeIcon(SoundManager.ChangeAudioSettings());
    }
}
