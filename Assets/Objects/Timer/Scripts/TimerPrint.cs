using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPrint : MonoBehaviour
{
    [SerializeField] private TimerManager _timerManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PrintTimers();
    }

    void PrintTimers()
    {
        for (var i = 0; i < _timerManager.GetNumberTimers(); i++)
        {
            string output = "";
            output += "Timer" + _timerManager.GetTimer(i).GetName;
            output += " , Time:" + _timerManager.GetTimer(i).GetTime();
            Debug.Log(output);
        }
    }
}
