using System;
using System.Collections.Generic;
using UnityEngine;


public static class TimerManager
{
    private static List<Timer> _allTimers = new List<Timer>();

    static TimerManager()
    {
        TimerManager.InitTimerManager();    
    }
    public static void CreateTimer(DateTime creationDate, TimeSpan timerTime, string action, string name = "")
    {
        var newTimer = new Timer(creationDate, timerTime, action, name);
        _allTimers.Add(newTimer);
        SaveLoadTimer.Save();
    }
    public static void CreateTimer(TimeSpan timerTime, string action, string name = "")
    {
        var newTimer = new Timer(timerTime, action, name);
        _allTimers.Add(newTimer);
        SaveLoadTimer.Save();
    }
    
    public static void DeleteTimer(Timer timer)
    {
        Debug.Log(timer.GetAction);
        _allTimers.Remove(timer);
        
        SaveLoadTimer.Save();
    }
    
    public static int GetNumberTimers => _allTimers.Count;
    public static Timer GetTimer(int index) => _allTimers[index];

    public static void CheckAllTimers()
    {
        for(int i = _allTimers.Count - 1; i >= 0; i--)
        {
            _allTimers[i].CheckTimer();
        }
    }

    private static void InitTimerManager()
    {
        SaveLoadTimer.Load();
        Debug.Log(SaveLoadTimer.So.AllTimers.Count);
        foreach (var t in SaveLoadTimer.So.AllTimers)
        {
            CreateTimer(t.CreationDate,t.TimerTime, t.Action, t.Name);
        }
    }
}