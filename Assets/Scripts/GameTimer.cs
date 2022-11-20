using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public int secondsDuration;
    public bool showMinutes;
    public bool showSeconds;
    public bool showMilliseconds;

    [Space]
    public TextMeshProUGUI timerText;
    public UnityEvent onTimerEnd;

    float secondsLeft;


    //set up timer
    void Start()
    {
        if (!timerText)
            timerText = GetComponentInChildren<TextMeshProUGUI>();

        secondsLeft = secondsDuration;
        timerText.text = GetTimeString();
    }


    //count timer down then disable script
    void Update()
    {
        secondsLeft -= Time.deltaTime;
        if (secondsLeft > 0)
        {
            timerText.text = GetTimeString();
            return;
        }

        secondsLeft = 0.0001f;
        timerText.text = GetTimeString();

        onTimerEnd.Invoke();
        enabled = false;
    }


    //get text that shows current time remaining
    string GetTimeString()
    {
        string timeText = "";

        if (showMinutes)
            timeText = MinutesString();

        if (showSeconds)
        {
            if (showMinutes)
                timeText += ":";

            timeText += SecondsString();
        }

        if (showMilliseconds)
        {
            if (showSeconds)
                timeText += ".";

            timeText += MillisecondsString();
        }

        return timeText;
    }


    //convert seconds float into minutes string
    string MinutesString()
    {
        return ((int)secondsLeft / 60).ToString();
    }


    //convert seconds float into seconds string
    string SecondsString()
    {
        int seconds = (int)secondsLeft % 60;
        
        if (showMinutes && seconds < 10)
            return "0" + seconds.ToString();
        else
            return seconds.ToString();
    }


    //convert seconds float into milliseconds string
    string MillisecondsString()
    {
        return (secondsLeft.ToString() + "000").Substring(2,3);
    }
}
