using UnityEngine;

public class ExitSaving: MonoBehaviour
{
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void Save()
    {
        SoundManager.SaveSoundSettings();
    }
}
