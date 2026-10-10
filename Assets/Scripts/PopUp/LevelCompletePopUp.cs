
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LevelCompletePopUp : MonoBehaviour
{
    [SerializeField] private Button buttonNextLevel;
    [SerializeField] private TextMeshProUGUI textCoinEarned;
    [SerializeField] private TextMeshProUGUI textCurrentLevel;
    [SerializeField] private TextMeshProUGUI textCurrentScore;
    [SerializeField] private Animator starAnimator;

    [Header("PowerUps rewards")]
    [SerializeField] private TextMeshProUGUI powerPistolRewardText;
    [SerializeField] private TextMeshProUGUI powerRifleRewardText;
    [SerializeField] private TextMeshProUGUI powerGranedRewardText;
    [SerializeField] private TextMeshProUGUI powerBubbleGumRewardText;

    int starCount;
    int currectPetId;

    private int pistolCount = 0;
    private int rifleCount = 0;
    private int granedCount = 0;
    private int bubbleCount = 0;



    public void Display(int currentLevel)
    {
        buttonNextLevel.onClick.RemoveAllListeners();
        buttonNextLevel.onClick.AddListener(OnNextButtonClicked);

        textCurrentLevel.text = "LEVEL " + currentLevel.ToString();

        LevelInfo lvlInfo = GameController.action.gamePlayHUD.GetLevelInfo();
        PetInfo petInfoData = PlayerDataManager.Instance.GetPetInfo();




        float r = ((GameController.action.moveCount * 1.0f) / lvlInfo.moveCount) * 100f;
        Debug.Log("Level Points Earned::" + r);
        int totalcoins = (int)r;
        if (r >= 40)
        {
            starCount = 3;
            totalcoins = lvlInfo.coinRewards;
        }
        else if (r >= 30)
        {
            starCount = 2;
            totalcoins = 20;
        }
        else
        {
            starCount = 1;
            totalcoins = 10;
        }

        Rewardspowerups(starCount);

        PlayerPrefs.SetInt("PlayerCoins", totalcoins);
        textCurrentScore.text = PlayerInfo.Info.currentScore.ToString();
        PlayerInfo.Info.UpdatePlayerHighScore();
        textCoinEarned.text = totalcoins.ToString();
        PlayerDataManager.Instance.AddReward(totalcoins, RewardDetails.RewardType.Coins);
        // lvlInfo.starsObtained = starCount;
        //petInfoData.collectedPoints = starCount;

        // Trigger the appropriate star animation
        Invoke("TriggerStarAnimationWithDelay", 0.1f);

        Debug.Log("Stars Obtained" + starCount);
        PlayerPrefs.SetInt("PetCoins", starCount);
        PlayerDataManager.Instance.SavePetCoins(starCount);

        gameObject.SetActive(true);

        Invoke("CheckForPetRelease", 0.5f);
    }

    private void Rewardspowerups(int starCount)
    {
        switch (starCount)
        {
            case 1:
                {
                    pistolCount = Random.Range(0, 3);
                    rifleCount = Random.Range(0, 2);
                    granedCount = 0;
                    bubbleCount = 0;
                }
                break;
            case 2:
                {
                    pistolCount = Random.Range(0, 3);
                    rifleCount = Random.Range(0, 3);
                    granedCount = 1;
                    bubbleCount = 0;
                }
                break;

            case 3:
                {
                    pistolCount = Random.Range(0, 5);
                    rifleCount = Random.Range(0, 3);
                    granedCount = 1;
                    bubbleCount = 1;
                }
                break;
        }

        powerPistolRewardText.text = pistolCount.ToString();
        powerRifleRewardText.text = rifleCount.ToString();
        powerGranedRewardText.text = granedCount.ToString();
        powerBubbleGumRewardText.text = bubbleCount.ToString();

    }

    private void UpdatePowerUpsData()
    {
        pistolCount += PlayerDataManager.Instance.LaserPostol;
        rifleCount += PlayerDataManager.Instance.LaserRifle;
        granedCount += PlayerDataManager.Instance.Grenade;
        bubbleCount += PlayerDataManager.Instance.BubbleGum;

        PlayerDataManager.Instance.UpdatePowerUps(pistolCount, rifleCount, granedCount, bubbleCount);
    }
    private void TriggerStarAnimationWithDelay()
    {
        TriggerStarAnimation(starCount);
    }

    private void TriggerStarAnimation(int starCount)
    {
        if (starAnimator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return;
        }

        // Reset all triggers to avoid interference
        starAnimator.ResetTrigger("One_Star");
        starAnimator.ResetTrigger("Two_Star");
        starAnimator.ResetTrigger("Three_Star");

        switch (starCount)
        {
            case 1:
                starAnimator.SetTrigger("One_Star");
                Debug.Log("Animator triggered: One_Star");
                break;
            case 2:
                starAnimator.SetTrigger("Two_Star");
                Debug.Log("Animator triggered: Two_Star");
                break;
            case 3:
                starAnimator.SetTrigger("Three_Star");
                Debug.Log("Animator triggered: Three_Star");
                break;
        }
    }

    /// <summary>
    /// Check for Pet Can released after level completed
    /// </summary>
    private void CheckForPetRelease()
    {
        int petCoins = PlayerDataManager.Instance.GetPetCoins();

        currectPetId = PlayerDataManager.Instance.GetActivePetToUnlock();
        var petData = PlayerDataManager.Instance.petRescueDataContainer.GetPetInfo(currectPetId);

      

        if (petData == null)
        {
            Debug.LogError("No - Pet Data found to upadte.");
            return;
        }

        if (petCoins >= petData.pointsToUnlock)
        {
            // Pet Rescued

            PlayerDataManager.Instance.UpdatePetProgress(currectPetId, petData.pointsToUnlock, true);
            currectPetId += 1;
            PlayerDataManager.Instance.SetActivePetToUnlock(currectPetId);
            PlayerDataManager.Instance.SavePetCoins(-petData.pointsToUnlock);
            petData.isUnlocked = true;
            Timer.timer.petRescuedPopup.updatePetUI(petData.happyPet, petData.petName);
            Timer.timer.petRescuedPopup.Render();
        }
    }

    private void OnNextButtonClicked()
    {
        UpdatePowerUpsData();
        Timer.timer.ClassicLvUp();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
