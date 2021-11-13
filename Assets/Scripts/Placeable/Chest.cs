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
    private bool _isOpen = false;
    
    void OnEnable()
    {
        ChestOpeningEventSystem.SighUpForEvent(OpenChest);
         InitChest(new Timer(new TimeSpan(0, 0, 10), "TestOpen", "TestChest"));
    }

    private void OnDisable()
    {
        ChestOpeningEventSystem.UnsubscribeFromEvent(OpenChest);
    }

    public void InitChest(Timer timer)
    {
        _timer = timer;
        TimerView tv = Instantiate(_timerView, transform).GetComponent<TimerView>();
        tv.InitTimerView(_timer);
    }

    void OpenChest(Timer timer)
    {
        if (_timer != timer) return;
        Debug.Log("Открыть сундук");
        _isOpen = true;
        GetComponent<SpriteRenderer>().sprite = _openChest;
        //GiveItem();
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

    private void GiveItem()
    {
        if(!_isOpen) return;
        switch(_itemsInChest.Count)
        {
            case 0:
                Destroy(gameObject);
                break;
            case 1:
                GiveItemInGame();
                Destroy(gameObject);
                break;
            default:
                GiveItemInGame();
                break;
        }
    }

    private void GiveItemInGame()
    {
        Mergeable newMergeable = Instantiate(_itemsInChest[_itemsInChest.Count - 1], transform.parent);
        _itemsInChest.RemoveAt(_itemsInChest.Count - 1);
    }
    

    public override void Click()
    {
        GiveItem();
    }
}
