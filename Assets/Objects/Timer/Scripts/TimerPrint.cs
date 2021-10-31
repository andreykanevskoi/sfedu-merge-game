using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPrint : MonoBehaviour
{
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
        for (var i = 0; i < TimerManager.GetNumberTimers(); i++)
        {
            string output = "";
            output += "Timer" + TimerManager.GetTimer(i).GetName;
            output += " , Time:" + TimerManager.GetTimer(i).GetTimeInSeconds();
            Debug.Log(output);
        }
    }
}
