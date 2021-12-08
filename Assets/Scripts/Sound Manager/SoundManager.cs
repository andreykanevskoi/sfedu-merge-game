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
}
