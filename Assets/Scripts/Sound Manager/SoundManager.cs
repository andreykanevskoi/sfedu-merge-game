using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //состояние настроек звука
    private static int _musicOn;
    private static int _audioOn;

    // Звуки
    public enum Sound
    {
        merge,
        nonMerge,
        questComplete,
        startCamp,
        clean,
    }

    public enum Music
    {
        BGSampleLevel,
        BGCamp,
    }

    // Объект для звуков
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;
    // Объект для зацикленного звука (очистка)
    private static GameObject loopSoundGameObject;
    private static AudioSource loopSoundAudioSource;
    // Объект для музыки
    private static GameObject musicGameObject;
    private static AudioSource musicAudioSource;

    //состояние настроек звука
    private static int _musicOn;
    private static int _audioOn;

    public static SoundManager instance;

    //загружаем настройки
    void Awake()
    {
        if (!instance)
        {
            instance = this;
            oneShotGameObject = GameObject.Find("OneShotSound");
            loopSoundGameObject = GameObject.Find("LoopSound");
            musicGameObject = GameObject.Find("Music");

            oneShotAudioSource = oneShotGameObject.GetComponent<AudioSource>();
            loopSoundAudioSource = loopSoundGameObject.GetComponent<AudioSource>();
            musicAudioSource = musicGameObject.GetComponent<AudioSource>();
        }

        _musicOn = PlayerPrefs.GetInt("Music", 1);
        _audioOn = PlayerPrefs.GetInt("Audio", 1);
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

    public static void PlayLoopSound(Sound sound)
    {
        if (_audioOn == 0) return;
        if (oneShotAudioSource.clip == null || oneShotAudioSource.clip != GetSound(sound))
        {
            oneShotAudioSource.clip = GetSound(sound);
        }
        if (!oneShotAudioSource.isPlaying)
        {
            oneShotAudioSource.Play();
        }
    }

    public static void StopLoopSound()
    {
        oneShotAudioSource.Stop();
    }

    public static void PlayMusic(Music music)
    {
        if(_musicOn == 0) return;
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }

        musicAudioSource.clip = GetMusic(music);
        musicAudioSource.Play();
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
        if (_audioOn == 0)
        {
            _audioOn = 1;
            oneShotAudioSource.mute = false;
            loopSoundAudioSource.mute = false;
        }
        else
        {
            _audioOn = 0;
            oneShotAudioSource.mute = true;
            loopSoundAudioSource.mute = true;
        }
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
    public static void SaveSoundSettings() {
        PlayerPrefs.SetInt("Music", _musicOn);
        PlayerPrefs.SetInt("Audio", _audioOn);
    }

    public static int GetAudioOn() => _audioOn;
    public static int GetMusicOn() => _musicOn;
}
