using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject timerObject;

    void Awake()
    {
        UnPause();
    }

    public void SetPauseVisibility(bool isVisible)
    {
        pausePanel.SetActive(isVisible);
    }

    public void SetTimerVisibility(bool isVisible)
    {
        timerObject.SetActive(isVisible);
    }

    public void UnPause()
    {
        SetPauseVisibility(false);
        SetTimerVisibility(true);
        Time.timeScale = 1;
    }

}
