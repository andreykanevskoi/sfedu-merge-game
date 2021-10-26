using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
     [SerializeField] GameObject timer;
     private ArrayList _fieldTimers = new ArrayList();
     private int _numberTimer = 0;
    void Start()
    {
        CreateTimer(201, "ez", _numberTimer++.ToString());
        CreateTimer(204, "www", _numberTimer++.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateTimer(float time, string action, string name = "")
    {
        GameObject newTimer = Instantiate(timer);
        newTimer.GetComponent<Timer>().SetTimer(time, action, name + '_' + _fieldTimers.Count);
        _fieldTimers.Add(newTimer);
    }

    public int GetNumberTimers()
    {
        return _fieldTimers.Count;
    }

    public GameObject GetTimer(int index)
    {
        return (GameObject)_fieldTimers[index];
    }
}
