using UnityEngine;

public class SoundResources : MonoBehaviour
{
    private static SoundResources _instance;

    public static SoundResources instance
    {
        get
        {
            if (_instance == null) _instance = (Instantiate(Resources.Load("SoundResources")) as GameObject).GetComponent<SoundResources>();
            return _instance;
        }
    }

    public SoundAudioClip[] soundAudioClipArray;

    // Класс для обработки звуковых файлов и самих звуков
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}