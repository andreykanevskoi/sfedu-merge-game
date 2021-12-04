using UnityEngine;

public class TimerView : MonoBehaviour
{
    private TextMesh _timerTime;
    private Timer _timer;
    private void OnEnable()
    {
        if (_timerTime == null)
        {
            _timerTime = GetComponent<TextMesh>();
        }
    }
    
    public void InitTimerView(Timer t)
    {
        _timer = t;
        _timerTime = GetComponent<TextMesh>();
        UpdateTimerView();
        InvokeRepeating(nameof(UpdateTimerView), 0f, 0.5f);
    }

    private string GetRemainingTimerTimeToString()
    {
        return _timer.GetRemainingTimerTime().TotalSeconds.ToString("F0");
    }

    private void UpdateTimerView()
    {
        if (_timer.TimerPassed())
        {
            Debug.Log("Активация сундука через TimerView");
            transform.parent.GetComponent<Chest>().OpenChest();
            return;
        }
        _timerTime.text = GetRemainingTimerTimeToString();
    }
}
