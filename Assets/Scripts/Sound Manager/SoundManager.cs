using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //состояние настроек звука
    private static int _musicOn;
    private static int _audioOn;
    private static bool _initSoundSettings = false;

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
    [SerializeField] private AudioSource _oneShotAudioSource;
    // Объект для зацикленного звука (очистка)
    [SerializeField] private AudioSource _loopSoundAudioSource;
    // Объект для музыки
    [SerializeField] private AudioSource _musicAudioSource;

    public static SoundManager instance;

    //загружаем настройки
    void Awake()
    {
        if (!instance)
        {
            Debug.Log("Init SoundManager");
            InitSoundSettings();
            instance = this;
        }
    }

    // Функция воспроизведения звука
    public static void PlaySound(Sound sound)
    {
        if(_audioOn == 0) return;   
        instance._oneShotAudioSource.PlayOneShot(GetSound(sound));
    }

    public static void PlaySound(AudioClip clip)
    {
        if(_audioOn == 0) return;
        instance._oneShotAudioSource.PlayOneShot(clip);
    }

    public static void PlayLoopSound(Sound sound)
    {
        if (_audioOn == 0) return;
        if (instance._oneShotAudioSource.clip == null || instance._oneShotAudioSource.clip != GetSound(sound))
        {
            instance._oneShotAudioSource.clip = GetSound(sound);
        }
        if (!instance._oneShotAudioSource.isPlaying)
        {
            instance._oneShotAudioSource.Play();
        }
    }

    public static void StopLoopSound()
    {
        instance._oneShotAudioSource.Stop();
    }

    public static void PlayMusic(Music music)
    {
        if (instance._musicAudioSource.isPlaying)
        {
            instance._musicAudioSource.Stop();
        }
        
        instance._musicAudioSource.clip = GetMusic(music);
        instance._musicAudioSource.Play();
        UpdateMusicStatus();
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
        _musicOn = _musicOn == 0 ? 1 : 0;
        UpdateMusicStatus();
        return _musicOn;
    }

    private static void UpdateMusicStatus()
    {
        if (!instance)
        {
            return;
        }
        if (_musicOn == 0)
        {
            instance._musicAudioSource.mute = true;
            return;
        }
        instance._musicAudioSource.mute = false;
    }

    private static void InitSoundSettings()
    {
        if (_initSoundSettings) return;
        Debug.Log(_musicOn);
        _musicOn = PlayerPrefs.GetInt("Music", 1);
        _audioOn = PlayerPrefs.GetInt("Audio", 1);
        Debug.Log(_musicOn);
        _initSoundSettings = true;
    }

    /// <summary>
    /// Сохранение настроек звука
    /// </summary>
    public static void SaveSoundSettings() {
        PlayerPrefs.SetInt("Music", _musicOn);
        PlayerPrefs.SetInt("Audio", _audioOn);
    }

    public static int GetAudioOn()
    {
        InitSoundSettings();
        return _audioOn;
    }

    public static int GetMusicOn()
    {
        InitSoundSettings();
        return _musicOn;
    }
}
