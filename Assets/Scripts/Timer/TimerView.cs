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
    }
    

    public void InitTimerView(Timer t)
    {
        _timer = t;
        UpdateTimerView();
        InvokeRepeating(nameof(UpdateTimerView), 0f, 0.5f);
    }

    string GetRemainingTimerTimeToString()
    {
        return _timer.GetRemainingTimerTime().TotalSeconds.ToString("F0");
    }

    void UpdateTimerView()
    {
        if (_timer.TimerPassed())
        {
            transform.parent.GetComponent<Chest>().OpenChest();
        }
        _timerTime.text = GetRemainingTimerTimeToString();
    }
}
