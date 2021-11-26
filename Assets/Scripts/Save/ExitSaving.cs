using UnityEngine;

public class ExitSaving: MonoBehaviour
{
    private void OnApplicationFocus(bool hasFocus)
    {
        
    }

    private void OnApplicationQuit()
    {
        SoundManager.SaveSoundSettings();
    }
}
