using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject timerObject;

    void Awake()
    {
        Time.timeScale = 1;
        SetPauseVisibility(false);
        SetWinVisibility(false);
        SetTimerVisibility(true);
    }

    public void SetPauseVisibility(bool isVisible)
    {
        pausePanel.SetActive(isVisible);
        SetTimerVisibility(!isVisible);
    }
    
    public void SetWinVisibility(bool isVisible)
    {
        winPanel.SetActive(isVisible);
        SetTimerVisibility(!isVisible);
    }

    public void SetTimerVisibility(bool isVisible)
    {
        timerObject.SetActive(isVisible);
    }

    public void UnPause()
    {
        SetPauseVisibility(false);
        Time.timeScale = 1;
    }

    public void OpenScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

}
