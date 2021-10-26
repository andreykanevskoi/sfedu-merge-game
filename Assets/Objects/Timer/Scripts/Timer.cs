using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _timerTime = 0;
    [SerializeField] private string _action = null;
    private bool _isActived = false;
    private string _name;

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
        Destroy(this.gameObject);
    }

    public void SetTimer(float time, string action, string name)
    {
        _timerTime = time;
        _action = action;
        _name = name;
        StartTimer();
    }

    void StartTimer()
    {
        _isActived = true;
    }

    public DateTime GetTime()
    {
        return DateTime.MinValue.AddSeconds(_timerTime);
    }

    public string GetTimerName()
    {
        return _name;
    }
}