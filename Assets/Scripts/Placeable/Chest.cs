using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Placeable
{
    [SerializeField] private int _opTimeH = 0;
    [SerializeField] private int _opTimeM = 0;
    [SerializeField] private int _opTimeS = 0;
    [SerializeField] private Sprite _openChest;
    [SerializeField] private TimerView _timerView;
    [SerializeField] private List<Placeable> _itemsInChest;
    private Timer _timer = null;

    void OnEnable()
    {
        if (_opTimeS != 0 || _opTimeM != 0 || _opTimeH != 0)
        {
            AddTimer(_opTimeS, _opTimeM, _opTimeH);
            ActivateChest();
        }
    }

    public void AddTimer(int seconds, int minutes = 0, int hours = 0)
    {
        _timer = new Timer(seconds, minutes, hours);
    }
    
    public void InitChest(Placeable[] items, int seconds, int minutes = 0, int hours = 0)
    {
        AddItems(items);
        AddTimer(seconds,minutes,hours);
        ActivateChest();
    }

    private void ActivateChest()
    {
        if (_timer != null)
        {
            _timerView = Instantiate(_timerView, transform).GetComponent<TimerView>();
            _timerView.InitTimerView(_timer);
        }
    }

    public void OpenChest()
    {
        if (!_timer.TimerPassed()) return;
        Debug.Log("Открыть сундук");
        Destroy(_timerView.gameObject);
        GetComponent<SpriteRenderer>().sprite = _openChest;
        //GiveItem();
    }

    public void AddItems(Placeable[] items)
    {
        foreach (var i in items)
        {
            Debug.Log(items);
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
        if(_timer == null) return;
        if(!_timer.TimerPassed()) return;
        GiveItemInGame();
        if (_itemsInChest.Count == 0)
        {
            DestroyChest();
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

            Debug.Log("PlaceItem " + newPlaceable.BaseName);
        }
    }
    

    public override void Click()
    {
        GiveItem();
    }

    private void DestroyChest()
    {
        fieldManager.RemovePlaceableFromField(this);
        Destroy(gameObject);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        if (_itemsInChest != null) {
            writer.Write(_itemsInChest.Count);
            foreach (var prefab in _itemsInChest) {
                writer.Write(prefab.prefabId);
            }
        }
        else {
            writer.Write(0);
        }

        if (_timer != null)
        {
            writer.Write(1);
            writer.Write(_timer.GetCreationTime.ToString());
            writer.Write(_timer.GetTimerTime.ToString());
        }
        else
        {
            writer.Write(0);
        }
        
    }

    public override void Load(GameDataReader reader, PlaceableFactory factory)
    {
        base.Load(reader, factory);
        int count = reader.ReadInt();
        if (count > 0) {
            _itemsInChest = new List<Placeable>();
            for (int i = 0; i < count; i++) {
                int id = reader.ReadInt();
                _itemsInChest.Add(factory.GetPrefab(id));
            }
        }

        int timerExists = reader.ReadInt();
        if (timerExists == 1)
        {
            _timer = new Timer(DateTime.Parse(reader.ReadString()), TimeSpan.Parse(reader.ReadString()));
        }
        ActivateChest();
    }

    public override bool BeginDrag()
    {
        if (_timerView != null)
        {
            _timerView.Hide();
        }
        return base.BeginDrag();
    }

    public override void Drop(Vector3 currentMousePosition)
    {
        if (_timerView != null)
        {
            _timerView.Show();
        }
        base.Drop(currentMousePosition);
    }
}
