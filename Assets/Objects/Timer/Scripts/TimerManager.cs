using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimerManager : MonoBehaviour
{
     [SerializeField] GameObject _timerPrefab;
     
     private List<Timer> _AllTimers = new List<Timer>();
     private int _numberTimer = 0;
    void Start()
    {
        SaveLoadTimer.Load();
        LoadTimers(SaveLoadTimer.savedTimers);
        CreateTimer(10, "ez", _numberTimer++.ToString());
        CreateTimer(24, "www", _numberTimer++.ToString());
        SaveLoadTimer.Save();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateTimer(float time, string action, string name = "")
    {
        GameObject newTimer = Instantiate(_timerPrefab);
        newTimer.GetComponent<Timer>().SetTimer(time, action, name + '_' + _AllTimers.Count);
        _AllTimers.Add(newTimer.GetComponent<Timer>());
    }

    public int GetNumberTimers()
    {
        return _AllTimers.Count;
    }

    public Timer GetTimer(int index)
    {
        return _AllTimers[index];
    }

    public void DeleteTimer(Timer timer)
    {
        Destroy(_AllTimers[_AllTimers.BinarySearch(timer)]);
        _AllTimers.Remove(timer);
    }

    void LoadTimers(List<SaveObject.TimerData> td)
    {
        Debug.Log(td.Count);
        for (int i = 0; i < td.Count; i++)
        {
            
            Debug.Log(td[i].name);
        }
    }
}
