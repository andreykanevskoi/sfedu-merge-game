using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public static class SaveObject
{
    private static List<TimerData> allTimers = new List<TimerData>();
    public static List<TimerData> SaveTimers()
    {
        TimerManager tm = GameObject.FindObjectOfType<TimerManager>();
        for (var i = 0; i < tm.GetNumberTimers(); i++)
        {
            var timer = tm.GetTimer(i);
            TimerData td = new TimerData(timer);
            allTimers.Add(td);
        }
        Debug.Log("Файл сохранён в " + Application.persistentDataPath);
        return allTimers;
    }
    
    [System.Serializable] 
    public struct TimerData
    {
        float timerTime;
        string action;
        bool isActived;
        public string name;

        public TimerData(float t, string a, bool isA, string n)
        {
            timerTime = t;
            action = a;
            isActived = isA;
            name = n;
        }

        public TimerData(Timer timer)
        {
            timerTime = timer.GetTimeTimer;
            action = timer.GetAction;
            isActived = timer.GetActive;
            name = timer.GetName;
        }
        
    }
}
