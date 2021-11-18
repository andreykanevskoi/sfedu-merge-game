using UnityEngine;

public class SoundResourcesScript : MonoBehaviour
{
    private static SoundResourcesScript _instance;

    public static SoundResourcesScript instance
    {
        get
        {
            if (_instance == null) _instance = (Instantiate(Resources.Load("SoundResources")) as GameObject).GetComponent<SoundResourcesScript>();
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