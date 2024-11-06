using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public float matchTime = 60f; // Total match time in seconds
    public float turnTime = 10f; // Time per turn in seconds
    private float currentMatchTime;
    private float currentTurnTime;

    private Action timeUpCallback; // Callback for when match time ends

    void Start()
    {
        currentMatchTime = matchTime;
        currentTurnTime = turnTime;
    }

    void Update()
    {
        // Countdown match time, triggering a draw if it reaches zero
        if (currentMatchTime > 0)
        {
            currentMatchTime -= Time.deltaTime;
        }
        else if (currentMatchTime <= 0)
        {
            timeUpCallback?.Invoke(); // Trigger draw
            StopTimers();
        }

        // Countdown turn time, resetting each turn
        if (currentTurnTime > 0)
        {
            currentTurnTime -= Time.deltaTime;
        }
        else
        {
            currentTurnTime = turnTime; // Reset turn time if it runs out
        }
    }

    public bool MatchTimeOver()
    {
        // Returns true if match time is zero or below
        return currentMatchTime <= 0;
    }

    public void StartTurnTimer()
    {
        currentTurnTime = turnTime; // Reset the turn timer at the start of each turn
    }

    public void StartMatchTimer(Action callback)
    {
        timeUpCallback = callback;
        currentMatchTime = matchTime;
    }

    public void ResetTurnTimer()
    {
        currentTurnTime = turnTime; // Resets the turn timer
    }

    public void StopTimers()
    {
        currentTurnTime = 0;
        currentMatchTime = 0;
    }
}   