using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro; // Make sure you have this using directive for TextMeshProUGUI

public class DailyRewardManager : MonoBehaviour
{
    public RewardDetails[] dailyRewards;
    public int currentRewardCycleDayIndex; // 0-indexed: Represents the day in the 7-day cycle that is available to claim
    // For example: 0 for Day 1, 1 for Day 2, ..., 6 for Day 7.

    public Button[] rewardButtons;
    public Sprite claimNowSprite;
    public Sprite claimedSprite;
    public Sprite toBeClaimedSprite;

    private const string LastLoginDateKey = "LastLoginDate";
    private const string RewardCycleDayIndexKey = "RewardCycleDayIndex"; // Stores currentRewardCycleDayIndex
    // We will continue to use "RewardClaimedDay{dayIndex}" for individual day claims.

    void Start()
    {
        InitializeDailyRewardState();
        SetupButtons();
        //PopulateDailyRewards(); // Set up text and images once
        UpdateAllButtonStates(); // Refresh visual states after setup
    }

    private void InitializeDailyRewardState()
    {
        string lastLoginDateStr = PlayerPrefs.GetString(LastLoginDateKey, "");
        DateTime currentDate = DateTime.Now;

        // Retrieve the current day index from PlayerPrefs, default to 0 (Day 1)
        currentRewardCycleDayIndex = PlayerPrefs.GetInt(RewardCycleDayIndexKey, 0);

        if (string.IsNullOrEmpty(lastLoginDateStr))
        {
            // First time login or no previous login data.
            Debug.Log("First time login or no previous login data. Initializing Day 1.");
            currentRewardCycleDayIndex = 0; // Start at Day 1 (index 0)
            ResetAllRewardClaimedStates(); // Ensure all day rewards are unclaimed initially
            SetNewLoginDate(currentDate); // Save current date
        }
        else
        {
            DateTime lastLoginDate = DateTime.Parse(lastLoginDateStr);

            if (IsNewDay(lastLoginDate, currentDate))
            {
                Debug.Log("New calendar day detected!");

                // Check if player missed a day (i.e., last login was NOT yesterday)
                if (lastLoginDate.Date.AddDays(1) != currentDate.Date)
                {
                    Debug.Log("Player missed a day. Resetting streak and cycle.");
                    currentRewardCycleDayIndex = 0; // Reset to Day 1
                    ResetAllRewardClaimedStates(); // Reset all claims for a fresh cycle
                }
                else
                {
                    // Player logged in consecutively, move to the next day's reward
                    // This increments the day index and loops back to 0 after the last day
                    currentRewardCycleDayIndex = (currentRewardCycleDayIndex + 1) % dailyRewards.Length;

                    // If we just cycled back to Day 1 (index 0), it means the full 7-day cycle completed.
                    // In this case, we need to mark all days as unclaimed for the new cycle.
                    if (currentRewardCycleDayIndex == 0)
                    {
                        Debug.Log("7-day cycle completed. Resetting for a new cycle.");
                        ResetAllRewardClaimedStates();
                    }
                }

                // Update the last login date for the next session
                SetNewLoginDate(currentDate);
            }
            else
            {
                // Same day, no changes to currentRewardCycleDayIndex or claims status
                Debug.Log($"Same day ({currentDate.ToShortDateString()}). Reward status remains as is.");
            }
        }

        // Save the updated index (important for persistence)
        PlayerPrefs.SetInt(RewardCycleDayIndexKey, currentRewardCycleDayIndex);
        PlayerPrefs.Save();
        Debug.Log($"Current available reward is for Day {currentRewardCycleDayIndex + 1}");
    }

    private void SetupButtons()
    {
        for (int i = 0; i < rewardButtons.Length; i++)
        {
            int dayIndex = i; // Closure for the lambda
            rewardButtons[i].onClick.RemoveAllListeners(); // Prevent duplicate listeners
            rewardButtons[i].onClick.AddListener(() => ClaimReward(dayIndex));
        }
    }

    private void PopulateDailyRewards()
    {
        for (int i = 0; i < dailyRewards.Length; i++)
        {
            if (i < rewardButtons.Length) // Ensure we don't go out of bounds
            {
                // Use dayNumber from ScriptableObject for UI display (usually 1-indexed)
                rewardButtons[i].transform.Find("DayText").GetComponent<TMP_Text>().text = $"Day {dailyRewards[i].dayNumber}";

                // Assuming one primary reward to display on the button
                if (dailyRewards[i].rewards.Length > 0)
                {
                    rewardButtons[i].transform.Find("RewardAmount").GetComponent<TextMeshProUGUI>().text = $"{dailyRewards[i].rewards[0].rewardAmount}";
                    rewardButtons[i].transform.Find("RewardIcon").GetComponent<Image>().sprite = dailyRewards[i].rewards[0].rewardImage;
                }
            }
        }
    }

    private void UpdateAllButtonStates()
    {
        for (int i = 0; i < rewardButtons.Length; i++)
        {
            UpdateButtonState(i);
        }
    }

    private void UpdateButtonState(int dayIndex)
    {
        // Check if the reward for this specific day (dayIndex) is claimed
        bool isClaimedForThisDay = PlayerPrefs.GetInt($"RewardClaimedDay{dayIndex}", 0) == 1;

        // This is the CRUCIAL part: only the currentRewardCycleDayIndex is available to claim
        bool isAvailableToClaim = (dayIndex == currentRewardCycleDayIndex) && !isClaimedForThisDay;

        // It's locked if its index is greater than the current active day's index.
        bool isLocked = dayIndex > currentRewardCycleDayIndex;

        Image buttonImage = rewardButtons[dayIndex].GetComponent<Image>();

        if (isClaimedForThisDay)
        {
            buttonImage.sprite = claimedSprite;
            rewardButtons[dayIndex].interactable = false;
        }
        else if (isAvailableToClaim)
        {
            buttonImage.sprite = claimNowSprite;
            rewardButtons[dayIndex].interactable = true;
        }
        else if (isLocked)
        {
            buttonImage.sprite = toBeClaimedSprite;
            rewardButtons[dayIndex].interactable = false;
        }
        else // This covers previous days (dayIndex < currentRewardCycleDayIndex) that haven't been claimed yet.
             // You might want to make them "Claimed" automatically if they were missed, or "Locked" if it's a strict linear path.
             // For a "calendar" style, they should probably be locked if not the current day.
        {
            buttonImage.sprite = toBeClaimedSprite; // Show as locked if it's a past day not claimed (missed)
            rewardButtons[dayIndex].interactable = false;
        }
    }

    public void ClaimReward(int dayIndex)
    {
        // Strictly only allow claiming the *current* day's reward cycle index
        if (dayIndex != currentRewardCycleDayIndex)
        {
            Debug.Log($"Error: Can only claim reward for Day {currentRewardCycleDayIndex + 1}. You clicked Day {dayIndex + 1}.");
            return;
        }

        // Also check if it's already claimed for THIS specific day
        if (PlayerPrefs.GetInt($"RewardClaimedDay{dayIndex}", 0) == 1)
        {
            Debug.Log($"Reward for Day {dayIndex + 1} already claimed for the current cycle.");
            return;
        }

        // Mark the reward for this specific day as claimed for the current cycle
        PlayerPrefs.SetInt($"RewardClaimedDay{dayIndex}", 1);
        PlayerPrefs.Save();

        // Give the actual rewards
        if (dayIndex < dailyRewards.Length)
        {
            RewardDetails rewardDetails = dailyRewards[dayIndex];
            foreach (var reward in rewardDetails.rewards)
            {
                PlayerDataManager.Instance.AddReward(reward.rewardAmount, reward.rewardType);
                Debug.Log($"You received {reward.rewardAmount} {reward.rewardType} for Day {dayIndex + 1}!");
            }
        }

        UpdateButtonState(dayIndex); // Update the state of the claimed button to "Claimed"

        // No need to increment currentRewardCycleDayIndex here, that happens on new day detection in InitializeDailyRewardState.

        // Notify UI for coin update (e.g., in UIManager)
        MainMenu.CoinUpdateAction?.Invoke();
    }

    private void SetNewLoginDate(DateTime currentDate)
    {
        PlayerPrefs.SetString(LastLoginDateKey, currentDate.ToString());
        PlayerPrefs.Save();
    }

    private bool IsNewDay(DateTime lastLogin, DateTime currentDate)
    {
        // Compares only the date part, ignoring time
        return lastLogin.Date != currentDate.Date;
    }

    private void ResetAllRewardClaimedStates()
    {
        // This is crucial when a new cycle starts or a day is missed.
        for (int i = 0; i < dailyRewards.Length; i++)
        {
            PlayerPrefs.SetInt($"RewardClaimedDay{i}", 0);
        }
        PlayerPrefs.Save();
        Debug.Log("All daily rewards marked as unclaimed for the new cycle.");
    }

    // You can keep these for now, assuming they are just for panel management
    private void OnEnable() { /* backBtn.onClick.AddListener(OnBackButtonClick); */ } // UIManager should probably open this panel
    void OnBackButtonClick()
    {
        // No UIManager.Instance.dailyRewards.RenderView() here. Just close.
        if (UIManager.Instance.dailyRewards != null)
        {
            UIManager.Instance.dailyRewards.Disable();
        }
    }
    private void OnDisable() { /* backBtn.onClick.RemoveListener(OnBackButtonClick); */ }
}
