using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Placeable
{
    [SerializeField] private int _openingTimeInMinutes = 0;
    [SerializeField] private Sprite _openChest;
    [SerializeField] private GameObject _timerView;
    [SerializeField] private List<Placeable> _itemsInChest;
    private Timer _timer;
    private bool _isOpen = false;
    
    void OnEnable()
    {
        ChestOpeningEventSystem.SighUpForEvent(OpenChest);
        if (_openingTimeInMinutes != 0)
        {
            InitChest(new Timer(new TimeSpan(0, 0, 3), "TestOpen", "TestChest"));
        }
         
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

    public void AddItem(Placeable[] items)
    {
        foreach (var i in items)
        {
            _itemsInChest.Add(i);
        }
    }

    public void DeleteItem(Placeable[] items)
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
                DestroyChest();
                break;
            case 1:
                GiveItemInGame();
                DestroyChest();
                break;
            default:
                GiveItemInGame();
                break;
        }
    }

    private void GiveItemInGame()
    {
        Debug.Log("GiveItemInGame");
        Vector3Int cellPositon = new Vector3Int();
        if (fieldManager.GetNearestPosition(currentCell, ref cellPositon))
        {
            Placeable newPlaceable = Instantiate(_itemsInChest[_itemsInChest.Count - 1], transform.parent);

            newPlaceable.Position = fieldManager.GetCellWorldPosition(cellPositon);
            newPlaceable.fieldManager = fieldManager;
            newPlaceable.currentCell = cellPositon;
            
            _itemsInChest.RemoveAt(_itemsInChest.Count - 1);
            fieldManager.AddPlaceableToField(newPlaceable);

            Debug.Log("PlaceItem");
        }
    }
    

    public override void Click()
    {
        GiveItem();
    }

    private void DestroyChest()
    {
        fieldManager.RemovePlaceableToField(this);
        Destroy(gameObject);
    }
}
