using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using EasyUI.Toast;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button dailyRewardBtn;
    [SerializeField] private Button anotherSettingBtn;
    [SerializeField] private Button aboutUsBtn;
    [SerializeField] private Button lifeBtn;
    [SerializeField] private Button coinBtn;
    [SerializeField] private Button petReleasedBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button storeBtn;
    [SerializeField] private Button leaderboardBtn;
    [SerializeField] private Button playbtn;
    [SerializeField] private Button profileBtn;


    [Header("All Text Reference")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text usernameText;

    [Header("AvatarProfile Reference")]
    public Image avatarImage;
    [SerializeField] private AvatarData avatarData;

    [Header("All Images Reference")]
    [SerializeField] private Image petImageIcon;
    [SerializeField] private Image petProgressFill;

    public static UnityAction CoinUpdateAction;

    public static UnityAction UsernameUpdateAction;


    private int petId;
    private void OnEnable()
    {
        AddListener();
    }

    private void Start()
    {
        UIManager.Instance.registerPanel.RenderView();
        updateCoinsUI();
        UpdatedPetIcon();
        UpdateProfileInfo();
        //if (PlayerDataManager.Instance != null)
        //{
        //    PlayerDataManager.Instance.SetStoryDone(1);
        //}
        AdmobAdsManager.instance.RequestBanner();
        AudioManager.Instance.PlayBGMusic(ClipTypes.MENU);
    }
    void AddListener()
    {
        settingBtn.onClick.AddListener(OnSettingBtnClicked);
        leaderboardBtn.onClick.AddListener(OnLeaderBoardBtnClicked);
        petReleasedBtn.onClick.AddListener(OnPetReleasedBtnClicked);
        storeBtn.onClick.AddListener(OnStoreBtnClicked);

        dailyRewardBtn.onClick.AddListener(OnDailyRewardBtnClicked);
        playbtn.onClick.AddListener(OnPlayBtnClicked);
        coinBtn.onClick.AddListener(OnCoinBtnPressed);
        dailyRewardBtn.onClick.AddListener(OnDailyRewardBtnClicked);
        profileBtn.onClick.AddListener(OnProfileBtnClicked);

        CoinUpdateAction += updateCoinsUI;
        UsernameUpdateAction += UpdateProfileInfo;
    }
    void OnSettingBtnClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        UIManager.Instance.setiingPenal.RenderView((int)Timer.GameState.MAIN_MENU);
        AddBtnClickAnimation();
    }

    void OnLeaderBoardBtnClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        UIManager.Instance.leaerboardPanel.RenderView();
    }

    void OnPetReleasedBtnClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        UIManager.Instance.petReleasedPanel.RenderView();
    }

    public void updateCoinsUI()
    {
        coinText.text = PlayerDataManager.Instance.GetCoins().ToString();
    }
    void OnStoreBtnClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        UIManager.Instance.storePanel.RenderView();
    }

    void OnDailyRewardBtnClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        UIManager.Instance.dailyRewards.RenderView();
    }

    void OnCoinBtnPressed()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        UIManager.Instance.storePanel.RenderView();
    }

    void OnProfileBtnClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        UIManager.Instance.playerProfile.RenderView(usernameText.text);
    }

    void OnPlayBtnClicked()
    {
        if (PlayerDataManager.Instance.GetLives() >= 1)
        {
            AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
            Time.timeScale = 1;
            PlayerInfo.MODE = 0;
            PlayerInfo.MapPlayer = new Player();
            int currentLevel = PlayerDataManager.Instance.GetLevel();
            if (currentLevel <= 0)
            {
                currentLevel = 1;
            }
            PlayerInfo.MapPlayer.Level = currentLevel;
            PlayerInfo.MapPlayer.Name = currentLevel.ToString();
            PlayerInfo.MapPlayer.HightScore = PlayerPrefs.GetInt(PlayerInfo.KEY_CLASSIC_HISCORE, 0);

            if (PlayerDataManager.Instance.GetLevel() <= 0)
            {
                PlayerDataManager.Instance.UpdatePlayerLevel(currentLevel);
            }

            PlayerDataManager.Instance.SavePlayerDataToFirebase();
            SceneManager.LoadScene("PlayScene");
            AdmobAdsManager.instance.DestroyBannerAd();
        }
        else
        {
            Debug.Log("No Lives to Play! Refill Lives to continue");
            Toast.Show("No lives left! Refill to play.", 2f, ToastColor.Orange, ToastPosition.BottomCenter);
        }

    }
    private void OnDisable()
    {
        settingBtn.onClick.RemoveAllListeners();
        leaderboardBtn.onClick.RemoveAllListeners();
        petReleasedBtn.onClick.RemoveAllListeners();
        storeBtn.onClick.RemoveAllListeners();
        aboutUsBtn.onClick.RemoveAllListeners();
        dailyRewardBtn.onClick.RemoveAllListeners();
        playbtn.onClick.RemoveAllListeners();
        coinBtn.onClick.RemoveAllListeners();
        dailyRewardBtn.onClick.RemoveAllListeners();
        profileBtn.onClick.RemoveAllListeners();

        CoinUpdateAction -= updateCoinsUI;
    }

    void AddBtnClickAnimation()
    {
        transform.DOKill();

        transform.DOPunchScale(Vector3.one * 0.2f, .3f, 10, 1f);

    }


    public void UpdatedPetIcon()
    {
        petId = PlayerDataManager.Instance.GetActivePetToUnlock();

        // Get pet data from PlayerDataManager's loaded Firebase data
        PetFirebaseData petData = PlayerDataManager.Instance.GetPetData(petId);
        int petCoins = PlayerDataManager.Instance.GetPetCoins(); // This is total, not for specific pet

        if (petData != null)
        {
            Debug.Log($"Pet ID: {petId}, Pet Name: {petData.petName}, Pet Unlocked: {petData.isUnlocked}, Collected Points: {petData.collectedPoints}");

            // The progress bar for "current pet to unlock" should probably use petData.collectedPoints
            // relative to petData.pointsToUnlock for that specific pet.
            float progress = (petCoins * 1.0f) / petData.pointsToUnlock;
            petProgressFill.fillAmount = progress;


            // Display happy pet if unlocked, otherwise normal/sad.
            // You'll need to pass the actual Sprite references from your PetRescueDataContainer
            // to the petImageIcon, as Firebase only stores strings/numbers.
            // Example (assuming you retrieve the sprite based on petId and isUnlocked state):
            PetInfo currentPetInfoSO = PlayerDataManager.Instance.petRescueDataContainer.GetPetInfo(petId);
            if (currentPetInfoSO != null)
            {
                petImageIcon.sprite = petData.isUnlocked ? currentPetInfoSO.happyPet : currentPetInfoSO.NormalPet;
            }
            else
            {
                Debug.LogWarning($"PetInfo for ID {petId} not found in ScriptableObject.");
            }

        }
        else
        {
            Debug.LogWarning($"Pet data not found in PlayerDataManager for pet ID: {petId}");
        }
    }

    public void UpdateProfileInfo()
    {
        string username = PlayerDataManager.Instance.GetUsername();
        // #region agent log
        PlayerDataManager.DbgLog("A", "MainMenu.UpdateProfileInfo", "main menu username refresh", "{\"len\":" + PlayerDataManager.DbgLen(username) + "}");
        // #endregion
        username = CapitalizeFirst(username);
        usernameText.text = username;

        int selectedAvatarID = PlayerDataManager.Instance.GetSelectedAvatarID();
        if (avatarData != null && selectedAvatarID >= 0 && selectedAvatarID < avatarData.avatarInfoList.Count)
        {
            AvatarInfo selectedAvatar = avatarData.avatarInfoList[selectedAvatarID];
            avatarImage.sprite = selectedAvatar.avatarIcon;
        }
        else
        {
            Debug.LogWarning("Invalid avatar ID or AvatarData not assigned.");
            // Optionally set a default avatar image
        }
    }

    public static string CapitalizeFirst(string value)
{
    if (string.IsNullOrEmpty(value))
        return value;

    return char.ToUpperInvariant(value[0]) + value[1..];
}
}
