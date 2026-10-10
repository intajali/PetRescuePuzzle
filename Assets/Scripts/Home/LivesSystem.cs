using System;
using System.Collections;
using UnityEngine;
using GameUtils;
using UnityEngine.Events;

/// <summary>
/// This class handles the lives system in the game. It is used to add and remove lives and other classes
/// can subscribe to it in order to receive a notification when the number of lives changes.
/// </summary>
public class LivesSystem : MonoBehaviour
{
    private TimeSpan timeSpan;

    public Action<TimeSpan, int> onCountdownUpdated;
    public Action<int> onCountdownFinished;

    private DateTime lastLifeRefillTimeFromPDM; // Store the last refill time loaded from PDM
    public int currentLivesFromPDM; // Store current lives loaded from PDM
    public int maxLivesFromPDM; // Max lives from PDM
    private int timeToNextLifeFromPDM; // Time to next life from PDM


    public void InitializeLivesSystem(DateTime lastRefillTime, int initialLives, int maxL, int timeNext)
    {
        lastLifeRefillTimeFromPDM = lastRefillTime;
        currentLivesFromPDM = initialLives;
        maxLivesFromPDM = maxL;
        timeToNextLifeFromPDM = timeNext;
        Debug.LogFormat("InitializeLivesSystem :: CurrentLive = {0} , MaxLives = {1}", currentLivesFromPDM, maxLivesFromPDM);
        if (currentLivesFromPDM < maxLivesFromPDM)
        {
            DateTime curTime = DateTime.UtcNow;
            TimeSpan timeSinceLastRefill = curTime - lastLifeRefillTimeFromPDM;
            int livesToGive = (timeSinceLastRefill.TotalSeconds >= timeToNextLifeFromPDM) ? 1 : 0;
            Debug.Log("timeSinceLastRefill == "+ timeSinceLastRefill.Seconds);
            // Update current Lives count
            if(livesToGive > 0)
            {
               AddLive(livesToGive);

            }
            else
            {
                int totalReamingSec = timeToNextLifeFromPDM - (int)timeSinceLastRefill.TotalSeconds;
                Debug.Log("totalReamingSec == " + totalReamingSec);
                // Start Counter from this Point.
                StartTimer(totalReamingSec);

            }

        }
        else
        {
            StopCountdown();
        }
       
    }

    private void AddLive(int livesToGive)
    {
        GameTimer.StopTimer();
        currentLivesFromPDM += livesToGive;
        PlayerDataManager.Instance.playerLives = currentLivesFromPDM; // Update PDM's in-memory lives
        lastLifeRefillTimeFromPDM = DateTime.UtcNow;
        PlayerDataManager.Instance.SetLastLifeRefillTime(DateTime.UtcNow);
        PlayerDataManager.Instance.SavePlayerDataToFirebase();
        if (currentLivesFromPDM < maxLivesFromPDM)
        {
            InitializeLivesSystem(lastLifeRefillTimeFromPDM, currentLivesFromPDM, maxLivesFromPDM, timeToNextLifeFromPDM);

        }
        else
        {
            StopCountdown();
        }

    }
   

    /// <summary>
    /// Removes a life from the system.
    /// </summary>
    public void RemoveLife()
    {
        if(currentLivesFromPDM > 0)
            currentLivesFromPDM -= 1;

        PlayerDataManager.Instance.playerLives = currentLivesFromPDM; // Update PDM's in-memory lives
        lastLifeRefillTimeFromPDM = DateTime.UtcNow;
        PlayerDataManager.Instance.SetLastLifeRefillTime(DateTime.UtcNow);
        PlayerDataManager.Instance.SavePlayerDataToFirebase();

        InitializeLivesSystem(lastLifeRefillTimeFromPDM, currentLivesFromPDM, maxLivesFromPDM, timeToNextLifeFromPDM);


    }

    /// <summary>
    /// Sets the number of lives to the maximum number allowed by the game configuration.
    /// </summary>
    public void RefillLives()
    {
        var refillCost = PlayerDataManager.Instance.livesRefillCost;
        //PlayerPrefs.SetInt("PlayerLives", maxLives);
        PlayerDataManager.Instance.playerLives = maxLivesFromPDM; // Update PDM's in-memory lives
        PlayerDataManager.Instance.SpendCoins(refillCost);
        StopCountdown();
        PlayerDataManager.Instance.SetLastLifeRefillTime(DateTime.UtcNow); // Reset timer to now (lives are full)
    }

    

    /// <summary>
    /// Stops the countdown to give a free life to the player.
    /// </summary>
    public void StopCountdown()
    {
        if (onCountdownFinished != null)
        {
            onCountdownFinished(currentLivesFromPDM);
            //onCountdownFinished(numLives);
        }
    }

    /// <summary>
    /// Registers the specified callbacks to be called when the amount of lives changes.
    /// </summary>
    /// <param name="updateCallback">The callback to register for when the number of lives changes.</param>
    /// <param name="finishCallback">The callback to register for when the free life is given.</param>
    public void Subscribe(Action<TimeSpan, int> updateCallback, Action<int> finishCallback)
    {
        onCountdownUpdated += updateCallback;
        onCountdownFinished += finishCallback;
       
    }

    /// <summary>
    /// Unregisters the specified callbacks to be called when the amount of lives changes.
    /// </summary>
    /// <param name="updateCallback">The callback to unregister for when the number of lives changes.</param>
    /// <param name="finishCallback">The callback to unregister for when the free life is given.</param>
    public void Unsubscribe(Action<TimeSpan, int> updateCallback, Action<int> finishCallback)
    {
        if (onCountdownUpdated != null)
        {
            onCountdownUpdated -= updateCallback;
        }
        if (onCountdownFinished != null)
        {
            onCountdownFinished -= finishCallback;
        }
    }

    void StartTimer(float counter)
    {
        TimeSpan remainingTime = TimeSpan.FromSeconds(counter);
        GameTimer.CoutDownTimer(this, counter , (count) =>
        {
            if (count > 0)
            {
                remainingTime -= TimeSpan.FromSeconds(1);

                if (onCountdownUpdated != null)
                {
                    onCountdownUpdated(remainingTime, currentLivesFromPDM);
                }
            }
            else
            {
                AddLive(1);
            }

        });

       

    }

}