using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    public void AutoQuit()
    {
        if (SceneManager.GetActiveScene().handle == 1)
        {
            //выход в главное меню
            Application.Quit();
        }
        //выход в лагерь
        SceneManager.LoadScene(1);
    }

    public void QuitInIndex(int sceneIndex)
    {
        if (SceneManager.sceneCount <= sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    public void QuitInName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
