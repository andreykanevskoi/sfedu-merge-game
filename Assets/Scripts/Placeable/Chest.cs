using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Placeable
{
    [SerializeField] private Sprite _openChest;
    [SerializeField] private GameObject _timerView;
    [SerializeField] private List<Mergeable> _itemsInChest;
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

    public void AddItem(Mergeable[] items)
    {
        foreach (var i in items)
        {
            _itemsInChest.Add(i);
        }
    }

    public void DeleteItem(Mergeable[] items)
    {
        if (_itemsInChest.Count == 0) return;
        foreach (var i in items)
        {
            _itemsInChest.Remove(i); 
        }
    }

    public void GiveItem()
    {
        //выдать предмет на поле
        if (_itemsInChest.Count == 0) return;
        Mergeable newMergeable = Instantiate(_itemsInChest[_itemsInChest.Count - 1], transform.parent);
        _itemsInChest.RemoveAt(_itemsInChest.Count - 1);
    }

   
}
