using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    //состояние настроек звука
    private static int _musicOn;
    private static int _audioOn;

    // Звуки
    public enum Sound
    {
        merge,
        nonMerge,
    }

    // Музыкальные звуки
    public enum Music
    {
        motorsport,
    }

    // Объект для звуков
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;
    // Объект для музыки
    private static GameObject musicGameObject;
    private static AudioSource musicAudioSource;

    static SoundManager()
    {
        _musicOn = PlayerPrefs.GetInt("Music", 1);
        _audioOn = PlayerPrefs.GetInt("Audio", 1);

        if (oneShotGameObject == null)
        {
            oneShotGameObject = GameObject.Find("OneShotSound");
            oneShotAudioSource = oneShotGameObject.GetComponent<AudioSource>();
        }
    }


    // Функция воспроизведения звука
    public static void PlaySound(Sound sound)
    {
        if (oneShotGameObject == null)
        {
            oneShotGameObject = GameObject.Find("OneShotSound");
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }        
        oneShotAudioSource.PlayOneShot(GetSound(sound));
    }

    public static void PlaySound(AudioClip clip)
    {
        if (oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject("One Shot Sound");
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }
        oneShotAudioSource.PlayOneShot(clip);
    }

    public static void PlayMusic(Music music)
    {
        if (musicGameObject == null)
        {
            musicGameObject = new GameObject("Music");
            musicAudioSource = musicGameObject.AddComponent<AudioSource>();
        }
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
    public static int ChangeAudioSettings() {
        _audioOn = _audioOn == 0 ? 1 : 0;
        return _audioOn;
    }

    /// <summary>
    /// Смена состояния настройки vepsrb
    /// </summary>
    public static int ChangeMusicSettings() {
        _musicOn = _musicOn == 0 ? 1 : 0;
        return _musicOn;
    }

    /// <summary>
    /// Сохранение настроек звука
    /// </summary>
    public static void SaveSoundSettings() {
        PlayerPrefs.SetInt("Music", _musicOn);
        PlayerPrefs.SetInt("Audio", _audioOn);
    }

    public static int GetAudioOn() => _audioOn;
    public static int GetMusicOn() => _musicOn;
}
