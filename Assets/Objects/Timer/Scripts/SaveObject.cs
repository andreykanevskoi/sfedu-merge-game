using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class SaveObject
{
    public List<TimerData> AllTimers { get; private set; }= new List<TimerData>();
    public DateTime RetentionTime { get; private set; }
    public void SaveData()
    {
        for (var i = 0; i < TimerManager.GetNumberTimers(); i++)
        {
            var timer = TimerManager.GetTimer(i);
            TimerData td = new TimerData(timer);
            AllTimers.Add(td);
        }
        RetentionTime = DateTime.UtcNow;
        Debug.Log("Файл сохранён в " + Application.persistentDataPath);
    }
    
    
    
    [System.Serializable] 
    public struct TimerData
    {
        public float TimerTime { get; private set; }
        public string Action { get; private set; }
        public bool IsActived { get; private set; }
        public string Name { get; private set; }
        public TimerData(float t, string a, bool isA, string n)
        {
            TimerTime = t;
            Action = a;
            IsActived = isA;
            Name = n;
        }

        public TimerData(Timer timer)
        {
            TimerTime = timer.GetTimeTimer;
            Action = timer.GetAction;
            IsActived = timer.GetActive;
            Name = timer.GetName;
        }

    }
}
