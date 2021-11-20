using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    public void AutoQuit()
    {
        if (SceneManager.GetActiveScene().handle == 0)
        {
            Application.Quit();
        }

        SceneManager.LoadScene(0);
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
