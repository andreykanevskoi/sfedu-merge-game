using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    public void AutoQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            //выход в лагерь
            SceneManager.LoadScene(1);
            return;
        }
        //выход в главное меню
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
