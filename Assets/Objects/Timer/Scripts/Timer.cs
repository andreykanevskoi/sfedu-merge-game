using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _timerTime = 0;
    [SerializeField] private string _action = null;
    private string _name = null;
    private bool _isActived = false;

    //private const string EmptyAction = "Default";
    void Start()
    {
        if (_timerTime != 0)
        {
            StartTimer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActived)
        {
            _timerTime -= (Time.deltaTime);
            if (_timerTime < 0)
            {
                TriggeringAction();
            }
        }
    }

    void TriggeringAction()
    {
        Debug.Log(_action);
        TimerManager.DeleteTimer(this);
    }

    public void SetTimer(float time, string action, string name = "")
    {
        _timerTime = time;
        _action = action;
        _name = name;
    }

    public void StartTimer()
    {
        _isActived = true;
    }

    public DateTime GetTimeInSeconds()
    {
        if (_timerTime > 0)
        {
            return DateTime.MinValue.AddSeconds(_timerTime);
        }

        return DateTime.MinValue;
    }

    public float GetTimeTimer => _timerTime;
    public string GetAction => _action;
    public bool GetActive => _isActived;
    public string GetName => _name;
}