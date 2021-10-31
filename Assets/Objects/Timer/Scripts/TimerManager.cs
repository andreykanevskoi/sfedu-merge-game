using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


public static class TimerManager
{
     [SerializeField] static GameObject _timerPrefab;
     
     private static List<Timer> _AllTimers = new List<Timer>();

     static void CreateTimer(float time, string action, string name = "", bool activateTimer = true)
    {
        Timer newTimer = Object.Instantiate(_timerPrefab).GetComponent<Timer>();
        newTimer.SetTimer(time, action, name);
        _AllTimers.Add(newTimer);
        if (activateTimer)
        {
            newTimer.StartTimer();
        }
    }

    public static int GetNumberTimers()
    {
        return _AllTimers.Count;
    }

    public static Timer GetTimer(int index)
    {
        return _AllTimers[index];
    }

    public static void DeleteTimer(Timer timer)
    {
        Object.Destroy(_AllTimers[_AllTimers.BinarySearch(timer)]);
        _AllTimers.Remove(timer);
    }

    public static void LoadTimersInGame(SaveObject so)
    {
        Debug.Log(so.AllTimers.Count);
        float offset = (float) (DateTime.UtcNow - so.RetentionTime).TotalSeconds;
        foreach (var t in so.AllTimers)
        {
            CreateTimer(t.TimerTime - offset, t.Action, t.Name);
        }
    }

    public static void LoadTimerPrefab(GameObject t)
    {
        _timerPrefab = t;
    }
}
