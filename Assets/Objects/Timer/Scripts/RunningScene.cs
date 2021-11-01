using System;
using UnityEngine;

public class RunningScene: MonoBehaviour
{
    void Start()
    {
        TimerManager.LoadTimersInGame();
        //TimerManager.CreateTimer(new TimeSpan(0,0,30), "First");
        //TimerManager.CreateTimer(new TimeSpan(0,0,1), "Second");
        //SaveLoadTimer.Save();
    }
    
    void Update()
    {
        TimerManager.CheckAllTimers();
    }
}
