using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicAudio : ISoundButton
{
    private void OnEnable()
    {
        ChangeIcon(SoundManager.GetMusicOn());
    }
    
    public void ChangeState()
    {
        ChangeIcon(SoundManager.ChangeMusicSettings());
    }
}
