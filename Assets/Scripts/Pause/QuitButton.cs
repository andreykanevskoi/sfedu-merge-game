using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    private static string _campSceneName;
    private static string _mainMenuSceneName;

    private void Awake()
    {
        _campSceneName = GetSceneNameByBuildIndex(1);
        _mainMenuSceneName = GetSceneNameByBuildIndex(0);
    }

    public void AutoQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            //выход в лагерь
            //SceneManager.LoadScene(1);
            GameEvents.current.TriggerExitScene(_campSceneName);
            return;
        }
        //выход в главное меню
        //SceneManager.LoadScene(0);
        GameEvents.current.TriggerExitScene(_mainMenuSceneName);
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
    
    private static string GetSceneNameByBuildIndex(int buildIndex)
    {
        return GetSceneNameFromScenePath(SceneUtility.GetScenePathByBuildIndex(buildIndex));
    }

    private static string GetSceneNameFromScenePath(string scenePath)
    {
        // Unity's asset paths always use '/' as a path separator
        var sceneNameStart = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
        var sceneNameEnd = scenePath.LastIndexOf(".", StringComparison.Ordinal);
        var sceneNameLength = sceneNameEnd - sceneNameStart;
        return scenePath.Substring(sceneNameStart, sceneNameLength);
    }
}
