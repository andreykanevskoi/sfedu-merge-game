using System;
using System.Collections.Generic;
using UnityEngine;


public static class TimerManager
{
    private static List<Timer> _allTimers = new List<Timer>();

    static TimerManager()
    {
        //TimerManager.InitTimerManager(); 
    }
    public static Timer CreateTimer(DateTime creationDate, TimeSpan timerTime)
    {
        var newTimer = new Timer(creationDate, timerTime);
        _allTimers.Add(newTimer);
        SaveLoadTimer.Save();
        return newTimer;
    }
    public static Timer CreateTimer(TimeSpan timerTime)
    {
        var newTimer = new Timer(timerTime);
        _allTimers.Add(newTimer);
        SaveLoadTimer.Save();
        return newTimer;
    }
    
    public static void DeleteTimer(Timer timer)
    {
        _allTimers.Remove(timer);
        
        SaveLoadTimer.Save();
    }
    
    public static int GetNumberTimers => _allTimers.Count;
    public static Timer GetTimer(int index) => _allTimers[index];

    public static void CheckAllTimers()
    {
        for(int i = _allTimers.Count - 1; i >= 0; i--)
        {
            _allTimers[i].TimerPassed();
        }
    }

    private static void InitTimerManager()
    {
        SaveLoadTimer.Load();
        Debug.Log(SaveLoadTimer.So.AllTimers.Count);
        foreach (var t in SaveLoadTimer.So.AllTimers)
        {
            CreateTimer(t.CreationDate,t.TimerTime);
        }
    }
}