using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public static class ChestOpeningEventSystem
{
    private static UnityEvent<Timer> _timerActivation = new TimerActivationEvent();

    public static void SighUpForEvent(UnityAction<Timer> ua)
    {
        _timerActivation.AddListener(ua);
    }

    public static void UnsubscribeFromEvent(UnityAction<Timer> ua)
    {
        _timerActivation.RemoveListener(ua);
    }

    public static void TriggeringEvent(Timer timer)
    {
        _timerActivation?.Invoke(timer);
    }
}

[System.Serializable]
public class TimerActivationEvent : UnityEvent<Timer>
{
}