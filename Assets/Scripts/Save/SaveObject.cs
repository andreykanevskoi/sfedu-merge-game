using System;
using System.Collections.Generic;
using Unity.Mathematics;

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
        public TimerData(DateTime cd, TimeSpan t)
        {
            CreationDate = cd;
            TimerTime = t;
        }

        public TimerData(Timer timer)
        {
            CreationDate = timer.GetCreationTime;
            TimerTime = timer.GetTimerTime;
        }

    }
    
    // [Serializable]
    // public struct ChestData
    // {
    //     public int3 _ { get; private set; }
    //     public List<Placeable> _itemsInChest { get; private set; }
    //     public TimerData _timer { get; private set; }
    //     public bool _isOpen { get; private set; }
    //
    //     public ChestData(Chest c)
    //     {
    //         
    //     }
    // }
}
