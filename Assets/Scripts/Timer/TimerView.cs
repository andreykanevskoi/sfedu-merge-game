using System;
using System.Text;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    private TextMesh _timerTime;
    private Timer _timer;
    private Transform _textBackground;
    private const string Separator = ":";
    private int _characterNumber;
    
    //массив методов получения дней, часов, минут, секунд
    delegate string GetTimeGelegate(TimeSpan ts);
    private GetTimeGelegate[] _getTimeFunction;

    private void Awake()
    {
        _timerTime = GetComponent<TextMesh>();
        _textBackground = transform.GetChild(0);
        _getTimeFunction = new[] {(GetTimeGelegate) GetDaysToStrings, GetHoursToStrings,
            GetMinutesToStrings, GetSecondsToStrings};
    }

    public void InitTimerView(Timer t)
    {
        _timer = t;
        StartUpdateView();
    }

    private string GetRemainingTimerTimeToString()
    {
        TimeSpan ts = _timer.GetRemainingTimerTime();
        StringBuilder sb = new StringBuilder();
        bool prevValues = false;
        foreach (var d in _getTimeFunction)
        {
            string value = d(ts);
            //Debug.Log(value);
            //если предыдущие значение есть, добавляем текущее, даже если 0
            if (value != "0" || (prevValues && value == "0"))
            {
                sb.Append(value + Separator);
                prevValues = true;
            }
            else
            {
                prevValues = false;
            }
            
        }
        return sb.Length > 1 ? sb.ToString(0, sb.Length - 1) : "0";
    }

    private void UpdateTimerView()
    {
        if (_timer.TimerPassed())
        {
            CancelInvoke(nameof(UpdateTimerView));
            Debug.Log("Активация сундука через TimerView");
            transform.parent.GetComponent<Chest>().OpenChest();
            return;
        }
        ResizeBackGround(GetRemainingTimerTimeToString().Replace(":", "").Length);
        _timerTime.text = GetRemainingTimerTimeToString();
    }

    private string GetDaysToStrings(TimeSpan ts)
    {
        return ts.Days.ToString();
    }
    
    private string GetHoursToStrings(TimeSpan ts)
    {
        return ts.Hours.ToString();
    }
    
    private string GetMinutesToStrings(TimeSpan ts)
    {
        return ts.Minutes.ToString();
    }
    
    private string GetSecondsToStrings(TimeSpan ts)
    {
        return ts.Seconds.ToString();
    }

    private void ResizeBackGround(int characterNumber)
    {
        _textBackground.transform.localScale = new Vector3(6.5f * characterNumber, 8 , 1);
    }

    public void Hide()
    {
        CancelInvoke(nameof(UpdateTimerView));
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartUpdateView();
    }

    private void StartUpdateView()
    {
        UpdateTimerView();
        InvokeRepeating(nameof(UpdateTimerView), 0f, 0.5f);
    }
}
