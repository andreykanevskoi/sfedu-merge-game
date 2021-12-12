using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonInMainMenu : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene("Camp");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
