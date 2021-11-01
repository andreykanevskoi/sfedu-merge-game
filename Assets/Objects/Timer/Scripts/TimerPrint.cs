using UnityEngine;

public class TimerPrint : MonoBehaviour
{
    void FixedUpdate()
    {
        PrintTimers();
    }

    void PrintTimers()
    {
        for (var i = 0; i < TimerManager.GetNumberTimers; i++)
        {
            string output = "";
            output += "Timer" + TimerManager.GetTimer(i).GetName;
            output += " , Time:" + TimerManager.GetTimer(i).GetRemainingTimerTime;
            Debug.Log(output);
        }
    }
}
