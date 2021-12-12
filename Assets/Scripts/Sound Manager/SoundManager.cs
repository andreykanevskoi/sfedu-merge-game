using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    // Звуки
    public enum Sound
    {
        merge,
        nonMerge,
    }

    public enum Music
    {
        BGSampleLevel,
        BGCamp,
    }

    // Объект для звуков
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;
    // Объект для музыки
    private static GameObject musicGameObject;
    private static AudioSource musicAudioSource;

    //состояние настроек звука
    private static int _musicOn;
    private static int _audioOn;

    //загружаем настройки
    static SoundManager()
    {
        _musicOn = PlayerPrefs.GetInt("Music", 1);
        _audioOn = PlayerPrefs.GetInt("Audio", 1);

        oneShotGameObject = GameObject.Find("OneShotSound");
        oneShotAudioSource = oneShotGameObject.GetComponent<AudioSource>();

        musicGameObject = GameObject.Find("Music");
        musicAudioSource = musicGameObject.GetComponent<AudioSource>();
    }

    // Функция воспроизведения звука
    public static void PlaySound(Sound sound)
    {
        if(_audioOn == 0) return;   
        oneShotAudioSource.PlayOneShot(GetSound(sound));
    }

    public static void PlaySound(AudioClip clip)
    {
        if(_audioOn == 0) return;
        oneShotAudioSource.PlayOneShot(clip);
    }

    public static void PlayMusic(Music music)
    {
        if(_musicOn == 0) return;
        musicAudioSource.PlayOneShot(GetMusic(music));
    }

    private static AudioClip GetSound(Sound sound)
    {
        foreach (SoundResources.SoundAudioClip soundAudioClip in SoundResources.instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }

    private static AudioClip GetMusic(Music music)
    {
        foreach (SoundResources.MusicAudioClip musicAudioClip in SoundResources.instance.musicAudioClipArray)
        {
            if (musicAudioClip.music == music)
            {
                return musicAudioClip.audioClip;
            }
        }
        return null;
    }

    /// <summary>
    /// Смена состояния настройки звука
    /// </summary>
    public static int ChangeAudioSettings()
    {
        _audioOn = _audioOn == 0 ? 1 : 0;
        return _audioOn;
    }
    
    /// <summary>
    /// Смена состояния настройки vepsrb
    /// </summary>
    public static int ChangeMusicSettings()
    {
        if (_musicOn == 0)
        {
            _musicOn = 1;
            musicAudioSource.mute = false;
        }
        else
        {
            _musicOn = 0;
            musicAudioSource.mute = true;
        }
        return _musicOn;
    }

    /// <summary>
    /// Сохранение настроек звука
    /// </summary>
    public static void SaveSoundSettings()
    {
        PlayerPrefs.SetInt("Music", _musicOn);
        PlayerPrefs.SetInt("Audio", _audioOn);
    }
    
    public static int GetAudioOn() => _audioOn;
    public static int GetMusicOn() => _musicOn;
}
