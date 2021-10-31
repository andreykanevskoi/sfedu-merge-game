using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningScene: MonoBehaviour
{
    [SerializeField] GameObject _timerPrefab;
    void Start()
    {
        SaveLoadTimer.Load();
        TimerManager.LoadTimerPrefab(_timerPrefab);
        TimerManager.LoadTimersInGame(SaveLoadTimer.So);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
