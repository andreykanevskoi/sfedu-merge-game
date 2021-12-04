using System;
using System.Text;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    private TextMesh _timerTime;
    private Timer _timer;
    private const string separator = ":";
    private void OnEnable()
    {
        if (_timerTime == null)
        {
            _timerTime = GetComponent<TextMesh>();
        }
    }
    
    public void InitTimerView(Timer t)
    {
        _timer = t;
        _timerTime = GetComponent<TextMesh>();
        UpdateTimerView();
        InvokeRepeating(nameof(UpdateTimerView), 0f, 0.5f);
    }

    private string GetRemainingTimerTimeToString()
    {
        TimeSpan ts = _timer.GetRemainingTimerTime();
        StringBuilder sb = new StringBuilder();
        sb.Append(GetDaysToStrings(ts));
        sb.Append(GetHoursToStrings(ts));
        sb.Append(GetMinutesToStrings(ts));
        sb.Append(GetSecondsToStrings(ts));
        return sb.ToString();
    }

    private void UpdateTimerView()
    {
        if (_timer.TimerPassed())
        {
            Debug.Log("Активация сундука через TimerView");
            transform.parent.GetComponent<Chest>().OpenChest();
            return;
        }
        _timerTime.text = GetRemainingTimerTimeToString();
    }

    private string GetDaysToStrings(TimeSpan ts)
    {
        if (ts.Days >= 1)
            return ts.Days + separator;
        return null;
    }
    
    private string GetHoursToStrings(TimeSpan ts)
    {
        if (ts.Hours >= 1)
            return ts.Hours + separator;
        return null;
    }
    
    private string GetMinutesToStrings(TimeSpan ts)
    {
        if (ts.Minutes >= 1)
            return ts.Minutes + separator;
        return null;
    }
    
    private string GetSecondsToStrings(TimeSpan ts)
    {
        return ts.Seconds.ToString();
    }
}
