using System;

public class Timer
{
    private readonly DateTime _creationDate;
    private TimeSpan _timerTime;
    private readonly string _action;
    private readonly string _name;

    public Timer(DateTime creationDate, TimeSpan timerTime, string action, string name = "")
    {
        _creationDate = creationDate;
        _timerTime = timerTime;
        _action = action;
        _name = name;
    }
    
    public Timer(TimeSpan timerTime, string action, string name = "")
    {
        _creationDate = DateTime.UtcNow;
        _timerTime = timerTime;
        _action = action;
        _name = name;
    }
    
    private void TriggeringAction()
    {
        //здесь выполняем действие
        TimerManager.DeleteTimer(this);
    }

    public void CheckTimer()
    {
        if ((DateTime.UtcNow - _creationDate) >= _timerTime)
        {
            TriggeringAction();
        }
    }

    public TimeSpan GetRemainingTimerTime => _timerTime - GetTimerAge;
    private TimeSpan GetTimerAge => DateTime.UtcNow - _creationDate;
    
    public DateTime GetCreationTime => _creationDate;
    public TimeSpan GetTimerTime => _timerTime;
    public string GetAction => _action;
    public string GetName => _name;
}