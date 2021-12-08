using UnityEngine;

public class SoundResources : MonoBehaviour
{
    public static SoundResources instance;

    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    // Массив звуков
    public SoundAudioClip[] soundAudioClipArray;

    // Класс для обработки звуковых файлов и самих звуков
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }

    // Массив музыки
    public MusicAudioClip[] musicAudioClipArray;

    // Класс для обработки музыкальных файлов
    [System.Serializable]
    public class MusicAudioClip
    {
        public SoundManager.Music music;
        public AudioClip audioClip;
    }
}