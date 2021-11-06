using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Placeable
{
    [SerializeField] private Sprite _openChest;
    //[SerializeField] private Sprite _closeChest;
    [SerializeField] private GameObject _timerView;
    private Timer _timer;
    void OnEnable()
    {
        ChestOpeningEventSystem.SighUpForEvent(OpenChest);
    }

    private void OnDisable()
    {
        ChestOpeningEventSystem.UnsubscribeFromEvent(OpenChest);
    }

    public void SetTimer(Timer timer)
    {
        _timer = timer;
        TimerView tv = Instantiate(_timerView, transform).GetComponent<TimerView>();
        tv.SetTimer(_timer);
    }

    void OpenChest(Timer timer)
    {
        if (_timer == timer)
        {
            Debug.Log("Открыть сундук");
            GetComponent<SpriteRenderer>().sprite = _openChest;
        }
    }
}
