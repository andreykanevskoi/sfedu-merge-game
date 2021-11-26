using System;
using System.Collections.Generic;

[System.Serializable] 
public class SaveObject
{
    public List<TimerData> AllTimers { get; private set; } = new List<TimerData>();
    public void SaveData()
    {
        for (var i = 0; i < TimerManager.GetNumberTimers; i++)
        {
            var timer = TimerManager.GetTimer(i);
            var td = new TimerData(timer);
            AllTimers.Add(td);
        }
    }
    
    [System.Serializable] 
    public struct TimerData
    {
        public DateTime CreationDate { get; private set; }
        public TimeSpan TimerTime { get; private set; }
        public string Action { get; private set; }
        public string Name { get; private set; }
        public TimerData(DateTime cd, TimeSpan t, string a, string n)
        {
            CreationDate = cd;
            TimerTime = t;
            Action = a;
            Name = n;
        }

        public TimerData(Timer timer)
        {
            CreationDate = timer.GetCreationTime;
            TimerTime = timer.GetTimerTime;
            Action = timer.GetAction;
            Name = timer.GetName;
        }

    }
}
