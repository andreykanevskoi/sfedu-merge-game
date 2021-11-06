using System;
using UnityEngine;

public class RunningScene: MonoBehaviour
{
    public Chest _chest;
    void Start()
    {
        //TimerManager.LoadTimersInGame();
        TimerManager.CreateTimer(new TimeSpan(0,0,10), "First");
        _chest.SetTimer(TimerManager.GetTimer(0));
        //TimerManager.CreateTimer(new TimeSpan(0,0,1), "Second");
        //SaveLoadTimer.Save();
    }
    
    void Update()
    {
        TimerManager.CheckAllTimers();
    }
}
