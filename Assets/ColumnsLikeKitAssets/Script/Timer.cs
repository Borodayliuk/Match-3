using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Timer : MonoBehaviour
{
    public float timeRemaining = 10;
    public bool timerIsRunning = false;
    public GameObject _TimerTextValue;//Level reached text value


    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        DisplayTime(timeRemaining);
    }

    void Update()
    {
        if (timerIsRunning && !GameStateManager.isCountingDown)
        {
            if (timeRemaining > 0 && !GameStateManager.IsGameOver )
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                GameStateManager.IsTimesUp = true;
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        (_TimerTextValue.GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}