using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    private TextMesh _timerTime;
    private Timer _timer;
    void OnEnable()
    {
        _timerTime = GetComponent<TextMesh>();
        ChestOpeningEventSystem.SighUpForEvent(TimerActivation);
    }
    
    private void OnDisable()
    {
        ChestOpeningEventSystem.UnsubscribeFromEvent(TimerActivation);
    }

    public void InitTimerView(Timer t)
    {
        _timer = t;
        UpdateTimerView();
        InvokeRepeating(nameof(UpdateTimerView), 0f, 0.1f);
    }

    string GetRemainingTimerTimeToString()
    {
        return _timer.GetRemainingTimerTime().TotalSeconds.ToString("F0");
    }

    void UpdateTimerView()
    {
        _timerTime.text = GetRemainingTimerTimeToString();
    }

    void TimerActivation(Timer timer)
    {
        if (_timer == timer)
        {
            Destroy(this.gameObject);
        }
    }
}
