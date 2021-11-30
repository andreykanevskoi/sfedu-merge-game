using System;
using UnityEngine;

public class Timer
{
    private readonly DateTime _creationDate;
    private TimeSpan _timerTime;

    public Timer(DateTime creationDate, TimeSpan timerTime)
    {
        _creationDate = creationDate;
        _timerTime = timerTime;
    }
    
    public Timer(TimeSpan timerTime)
    {
        _creationDate = DateTime.UtcNow;
        _timerTime = timerTime;
    }

    public bool TimerPassed()
    {
        if ((DateTime.UtcNow - _creationDate) >= _timerTime)
        {
            return true;
        }

        return false;
    }

    public TimeSpan GetRemainingTimerTime()
    {
        TimerPassed();
        return _timerTime - GetTimerAge;
    }
    private TimeSpan GetTimerAge => DateTime.UtcNow - _creationDate;
    
    public DateTime GetCreationTime => _creationDate;
    public TimeSpan GetTimerTime => _timerTime;
}