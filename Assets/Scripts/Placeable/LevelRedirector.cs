using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRedirector : Placeable {

    [SerializeField] string _sceneName;
    [SerializeField] Placeable[] _levelReward;

    [SerializeField] Chest _chestPrefab;

    public void HideLable() { }
    public void ShowLable() { }

    public override void Click() {
        Debug.Log("Click");
        if (_sceneName != "") {

            SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
            return;
        }
        Debug.LogError("No scene name");
    }

    public override bool BeginDrag() {
        return false;
    }
}
