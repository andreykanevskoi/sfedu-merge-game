using System;
using System.Text;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    private TextMesh _timerTime;
    private Timer _timer;
    private const char Separator = ':';
    private int _characterNumber;
    private SpriteRenderer _spriteRendererBackground;

    private float _paddingWight = 0.4f;
    private float _paddingHeight = 0.2f;
    private float _expCoefForSeparator = 0.4f;
    
    //массив методов получения дней, часов, минут, секунд
    delegate string GetTimeDelegate(TimeSpan ts);
    private GetTimeDelegate[] _getTimeFunction;

    private void Awake()
    {
        _timerTime = GetComponent<TextMesh>();
        _getTimeFunction = new[] {(GetTimeDelegate) GetDaysToStrings, GetHoursToStrings,
            GetMinutesToStrings, GetSecondsToStrings};
        _spriteRendererBackground = GetComponentInChildren<SpriteRenderer>();
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
                sb.Append(Format(value, prevValues) + Separator);
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

        string s = GetRemainingTimerTimeToString();
        int separatorNumber = s.Split(Separator).Length - 1;
        int remainingTimerLenght = s.Length - separatorNumber;
        ResizeBackGround(remainingTimerLenght, separatorNumber);
        _timerTime.text = s;
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

    private void ResizeBackGround(int characterNumber, int separatorNumber)
    {
        _spriteRendererBackground.size = new Vector2(characterNumber + (separatorNumber * _expCoefForSeparator) 
                                                                     + _paddingWight, 1 + _paddingHeight);
        // var transform1 = _textBackground.transform;
        // transform1.localScale = new Vector3(2.7f * characterNumber, 
        //     transform1.localScale.y);
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

    private string Format(string s, bool prevV)
    {
        if (s.Length == 1 && prevV)
            return "0" + s;
        return s;
    }
}
