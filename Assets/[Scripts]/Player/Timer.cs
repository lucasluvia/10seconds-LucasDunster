using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TimerState
{
    COUNTDOWN,
    COOLDOWN
}


public class Timer : MonoBehaviour
{
    public TimerState timerState;

    [SerializeField] float CountdownTarget = 10f;
    [SerializeField] float CooldownTarget = 5f;

    [SerializeField] Color CountdownColour;
    [SerializeField] Color CooldownColour;

    TextMeshProUGUI timerText;
    float currentTime;


    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
        currentTime = CountdownTarget;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        timerText.text = ((int)currentTime+1).ToString();

        switch (timerState)
        {
            case TimerState.COUNTDOWN:
                CountdownUpdate();
                break;
            case TimerState.COOLDOWN:
                CooldownUpdate();
                break;
        }
        
    }

    private void CountdownUpdate()
    {
        timerText.color = CountdownColour;

        if (currentTime < 0f)
        {
            currentTime = CooldownTarget;
            timerState = TimerState.COOLDOWN;
        }
    }

    private void CooldownUpdate()
    {
        timerText.color = CooldownColour;

        if (currentTime < 0f)
        {
            currentTime = CountdownTarget;
            timerState = TimerState.COUNTDOWN;
        }
    }

}
