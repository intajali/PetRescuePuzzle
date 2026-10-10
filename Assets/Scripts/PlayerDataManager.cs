using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions; // Required for ContinueWithOnMainThread
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;
    private string userID;
    private string _username;
    private int playerCoins;
    private int playerDiamonds;
    private int playerGems;
    private int playerBoosts;
    public int playerLives;
    private int grenade;
    private int laserPostol;
    private int laserRifle;
    private int bubbleGum;
    public int maxLives = 5;
    public int timeToNextLife;
    public int livesRefillCost = 100;
    public int movesCost = 50;
    public int giveMoves = 5;

    //newAdded Attributes for Firebase
    private int _playerLevel;
    private int _petToUnlock;
    private int _petCoins;
    private Dictionary<string, PetFirebaseData> _pets = new Dictionary<string, PetFirebaseData>();
    private DateTime _lastLifeRefillTime;
    private DateTime _lastDailyRewardClaimed;
    private int _selectedAvatarID;
    private int _highScore;
    private int _storyDone;

    // Game Settings
    private float _masterVolume;
    private float _sfxVolume;
    private float _musicVolume;
    private int _vibrationEnabled;
    private int _notificationEnabled;

    public LivesSystem livesSystem;
    public PetRescueDataContainer petRescueDataContainer;
    public PowersUpDataContainer powerupDataContainer;

    public PlayerLocalDataContainer playerLocalDataContainer;

    public AvatarData avatarData;

    private bool _dataLoadedFromFirebase = false;

    private PetInfo petInfo;

    public bool IsDataLoaded = false;
    private const string LocalPlayerFileName = "playerdata.json";
    private const string PrefsPlayerLevelKey = "PRP_PlayerLevel";
    private const string PrefsPlayerCoinsKey = "PRP_PlayerCoins";
    private const string PrefsPlayerLivesKey = "PRP_PlayerLives";
    private const string PrefsUserIdKey = "PRP_UserID";
    private bool _isSavingProgress;

    public int Grenade { get => grenade; set => grenade = value; }
    public int LaserPostol { get => laserPostol; set => laserPostol = value; }
    public int LaserRifle { get => laserRifle; set => laserRifle = value; }
    public int BubbleGum { get => bubbleGum; set => bubbleGum = value; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            IsDataLoaded = false;
            // Load disk data immediately so Android/iOS keep progress even if Firebase is slow or creates a new guest UID.
            LoadLocalData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(FirebaseHandler.Instance.ClockTimer());
    }

    /// <summary>
    /// Set Player Current Data.
    /// </summary>
    /// <param name="playerData"></param>
    private void UpdateCurrentData(PlayerFirebaseData playerData)
    {
        // #region agent log
        DbgLog("B", "PlayerDataManager.UpdateCurrentData", "applying data", "{\"incomingLen\":" + DbgLen(playerData.username) + ",\"prevLen\":" + DbgLen(_username) + ",\"caller\":\"" + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + "\"}");
        // #endregion
        this.userID = playerData.userID;
        this._username = playerData.username ?? CreateDefaultuserName();
        this.playerCoins = playerData.playerCoins;
        this.playerLives = playerData.playerLives;
        this._playerLevel = playerData.playerLevel;
        this.playerDiamonds = playerData.playerDiamonds;
        this.playerGems = playerData.playerGems;
        this.playerBoosts = playerData.playerBoosts;
        this._highScore = playerData.highScore;

        this._petCoins = playerData.petCoins;
        this._petToUnlock = playerData.petToUnlock;
        this._selectedAvatarID = playerData.selectedAvatarID;
        this._pets = playerData.pets ?? CreateDefaultPetData();

        // Music Setting Data
        this._masterVolume = playerData.masterVolume;
        this._musicVolume = playerData.musicVolume;
        this._sfxVolume = playerData.sfxVolume;
        this._storyDone = playerData.storyDone;
        this._vibrationEnabled = playerData.vibrationEnabled;
        this._notificationEnabled = playerData.notificationEnabled;

        // PowerUps Data
        this.Grenade = playerData.PU_Bomb;
        this.LaserPostol = playerData.PU_Slash;
        this.LaserRifle = playerData.PU_Row;
        this.BubbleGum = playerData.PU_Column;

        this._lastLifeRefillTime = ParseStoredDate(playerData.lastLifeRefillTime, DateTime.Now);
        this._lastDailyRewardClaimed = ParseStoredDate(playerData.lastDailyRewardClaimed, DateTime.UtcNow.AddDays(-1));

        livesSystem.InitializeLivesSystem(_lastLifeRefillTime, playerLives, maxLives, timeToNextLife);
        if (UIManager.Instance != null && UIManager.Instance.mainMenu != null)
        {
            MainMenu mainMenu = UIManager.Instance.mainMenu;
            MainMenu.CoinUpdateAction?.Invoke();
            mainMenu.UpdatedPetIcon();
            mainMenu.UpdateProfileInfo();
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(_musicVolume);
            AudioManager.Instance.SetSFXVolume(_sfxVolume);
        }
        ApplyLoadedSettings();
        SavePlayerDataToScriptableObject();
        SavePlayerDataToLocal(playerData);
        SaveCriticalProgressToPlayerPrefs(playerData);

        IsDataLoaded = true;
    }
    /// <summary>                                                                                    
    /// Load Player Data.                                                                            
    /// </summary>
    /// <param name="uid"></param>                                                                   
    public async void LoadPlayerData(string uid)
    {
        if (InternetChecker.Instance != null && InternetChecker.Instance.IsInternetConnected)
        {
            if (FirebaseHandler.Instance == null || FirebaseHandler.Instance.DbRef == null)
            {
                Debug.LogError("Firebase Database reference is null. Cannot load player data.");
                LoadLocalData();
                return;
            }

            await FirebaseHandler.Instance.DbRef.Child("users").Child(uid).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"LoadPlayerDataFromFirebase: Failed to load user data: {task.Exception}");
                    _dataLoadedFromFirebase = false;
                    LoadLocalData();
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists && snapshot.Value != null)
                    {
                        try
                        {
                            string json = snapshot.GetRawJsonValue();
                            PlayerFirebaseData loadedData = JsonConvert.DeserializeObject<PlayerFirebaseData>(json);
                            loadedData.userID = uid;
                            CheckIfFirebaseDataIsOld(loadedData);
                            Debug.Log($"Player data loaded successfully for UID: {uid}");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to deserialize player data for UID {uid}: {ex.Message}");
                            _dataLoadedFromFirebase = false;
                            if (!TryRestoreExistingLocalData(uid))
                            {
                                CreateDefaultPlayerData(uid);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"No player data found for UID: {uid}. Keeping local progress if it exists.");
                        if (!TryRestoreExistingLocalData(uid))
                        {
                            CreateDefaultPlayerData(uid);
                        }
                        else
                        {
                            SavePlayerDataToFirebase();
                        }
                    }
                }
            });
        }
        else
        {
            Debug.Log("<color=blue> LOAD LOCAL DATA :: PlayerDataManager :: LoadPlayerDataFromFirebase()</color>");
            LoadLocalData();
        }


    }

    /// <summary>
    /// Check If player played Offline and the data should be updated to firebase.
    /// </summary>
    private void CheckIfFirebaseDataIsOld(PlayerFirebaseData firebaseData)
    {
        PlayerFirebaseData localPlayerData = LoadPlayerLocalData();

        if (localPlayerData != null && firebaseData != null)
        {
            if (localPlayerData.playerLevel >= firebaseData.playerLevel)
            {
                UpdateCurrentData(localPlayerData);
            }
            else
            {
                UpdateCurrentData(firebaseData);
            }
        }
        else if (localPlayerData == null && firebaseData == null)
        {
            CreateDefaultPlayerData("");
        }
        else if(localPlayerData == null)
        {
            UpdateCurrentData(firebaseData);
        }
        else 
        {
            UpdateCurrentData(localPlayerData);
        }
    }



    /// <summary>
    /// Load Data from Local from file.
    /// </summary>
    public void LoadLocalData()
    {
        Debug.Log("Loading Local Data");
        PlayerFirebaseData localPlayerData = LoadPlayerLocalData();
        if (localPlayerData != null)
        {
            UpdateCurrentData(localPlayerData);
        }
        else
        {
            // Create Default Data if local Data not Found.
            CreateDefaultPlayerData(string.Empty);
        }
    }

    public string CreateDefaultuserName()
    {
        return string.Format("User_{0}", UnityEngine.Random.Range(1000, 9999));
    }

    public Dictionary<string, PetFirebaseData> CreateDefaultPetData()
    {
        var pets = new Dictionary<string, PetFirebaseData>();
        if (petRescueDataContainer != null && petRescueDataContainer.PetInfoList != null)
        {
            foreach (var petInfo in petRescueDataContainer.PetInfoList)
            {
                // For a new user, pets are not unlocked and collectedPoints are 0
                pets.Add($"petId_{petInfo.petIndexID}", new PetFirebaseData(
                    false, // isUnlocked starts false for new user
                    petInfo.pointsToUnlock,
                    0, // collectedPoints starts at 0 for new user
                    petInfo.petName
                ));
            }
        }
        return pets;
    }

    private void CreateDefaultPlayerData(string uid)
    {
        if (TryRestoreExistingLocalData(uid))
        {
            Debug.Log("Skipped default player data because local progress already exists.");
            return;
        }

        PlayerFirebaseData defaultData = new PlayerFirebaseData(uid,
           new PlayerDefaultDataModel()
        );

        UpdateCurrentData(defaultData);
    }

    private bool TryRestoreExistingLocalData(string uid)
    {
        PlayerFirebaseData localPlayerData = LoadPlayerLocalData();
        if (localPlayerData == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(uid))
        {
            localPlayerData.userID = uid;
        }

        UpdateCurrentData(localPlayerData);
        return true;
    }

    public void SavePlayerDataToFirebase()
    {
        PersistProgress(false);
    }

    public void PersistProgress(bool forceCloudSave)
    {
        if (_isSavingProgress || !IsDataLoaded)
        {
            return;
        }

        _isSavingProgress = true;
        try
        {
            Debug.Log("PersistProgress forceCloudSave=" + forceCloudSave);
            PlayerFirebaseData dataToSave = BuildCurrentPlayerData();

            SavePlayerDataToLocal(dataToSave);
            SavePlayerDataToScriptableObject();
            SaveCriticalProgressToPlayerPrefs(dataToSave);

            if (InternetChecker.Instance != null && InternetChecker.Instance.IsInternetConnected && FirebaseHandler.Instance != null)
            {
                if (forceCloudSave)
                {
                    FirebaseHandler.Instance.SaveUserGameplayDataToFirebase(dataToSave);
                }
                else
                {
                    StartCoroutine(FirebaseHandler.Instance.CheckForDataSaveToFireBase(dataToSave));
                }
            }
        }
        finally
        {
            _isSavingProgress = false;
        }
    }

    private PlayerFirebaseData BuildCurrentPlayerData()
    {
        PlayerFirebaseData dataToSave = new PlayerFirebaseData();
        dataToSave.userID = userID;
        dataToSave.username = _username;
        dataToSave.playerCoins = playerCoins;
        dataToSave.playerDiamonds = playerDiamonds;
        dataToSave.playerGems = playerGems;
        dataToSave.playerBoosts = playerBoosts;
        dataToSave.PU_Bomb = Grenade;
        dataToSave.PU_Slash = LaserPostol;
        dataToSave.PU_Row = LaserRifle;
        dataToSave.PU_Column = BubbleGum;
        dataToSave.playerLives = playerLives;
        dataToSave.playerLevel = Mathf.Max(1, _playerLevel);
        dataToSave.lastLifeRefillTime = _lastLifeRefillTime.ToString("o");
        dataToSave.highScore = _highScore;
        dataToSave.storyDone = _storyDone;
        dataToSave.masterVolume = _masterVolume;
        dataToSave.sfxVolume = _sfxVolume;
        dataToSave.musicVolume = _musicVolume;
        dataToSave.vibrationEnabled = _vibrationEnabled;
        dataToSave.notificationEnabled = _notificationEnabled;
        dataToSave.petToUnlock = _petToUnlock;
        dataToSave.petCoins = _petCoins;
        dataToSave.pets = _pets;
        dataToSave.lastDailyRewardClaimed = _lastDailyRewardClaimed.ToString("o");
        dataToSave.selectedAvatarID = _selectedAvatarID;
        return dataToSave;
    }

    /// <summary>
    /// Save Playre Data to Local ScriptableObject.
    /// </summary>
    public void SavePlayerDataToScriptableObject()
    {
        try
        {
            playerLocalDataContainer.userID = userID;
            playerLocalDataContainer.username = _username;
            playerLocalDataContainer.playerCoins = playerCoins;
            playerLocalDataContainer.playerDiamonds = playerDiamonds;
            playerLocalDataContainer.playerGems = playerGems;
            playerLocalDataContainer.playerBoosts = playerBoosts;
            playerLocalDataContainer.powerUpsData.grenade = Grenade;
            playerLocalDataContainer.powerUpsData.laserPostol = LaserPostol;
            playerLocalDataContainer.powerUpsData.laserRifle = LaserRifle;
            playerLocalDataContainer.powerUpsData.bubbleGum = BubbleGum;
            playerLocalDataContainer.playerLives = playerLives;
            playerLocalDataContainer.playerLevel = _playerLevel;
            playerLocalDataContainer.lastLifeRefillTime = _lastLifeRefillTime.ToString("o");
            playerLocalDataContainer.highScore = _highScore;
            playerLocalDataContainer.storyDone = _storyDone;
            playerLocalDataContainer.masterVolume = _masterVolume;
            playerLocalDataContainer.sfxVolume = _sfxVolume;
            playerLocalDataContainer.musicVolume = _musicVolume;
            playerLocalDataContainer.vibrationEnabled = _vibrationEnabled;
            playerLocalDataContainer.notificationEnabled = _notificationEnabled;
            playerLocalDataContainer.petToUnlock = _petToUnlock;
            playerLocalDataContainer.petCoins = _petCoins;
            playerLocalDataContainer.lastDailyRewardClaimed = _lastDailyRewardClaimed.ToString("o");
            playerLocalDataContainer.selectedAvatarID = _selectedAvatarID;

            for (int i = 0; i < _pets.Count; i++)
            {
                string key = string.Concat("petId_",(i + 1));
                if (_pets[key].isUnlocked)
                {
                    playerLocalDataContainer.petRescueDataContainer.SavePetRescuedData(i + 1);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Save Player Local Data to scriptv :: "+e.StackTrace);
        }

    }

    /// <summary>
    /// Save Player Data Locally
    /// </summary>
    /// <param name="playerData"></param>
public void SavePlayerDataToLocal(PlayerFirebaseData playerData)
    {
        try
        {
            string json = JsonConvert.SerializeObject(playerData);
            string path = Path.Combine(Application.persistentDataPath, LocalPlayerFileName);
            Debug.Log(path);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogError("SavePlayerDataToLocal failed: " + e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>    
    public PlayerFirebaseData LoadPlayerLocalData()
    {
        string path = Path.Combine(Application.persistentDataPath, LocalPlayerFileName);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                PlayerFirebaseData playerData = JsonConvert.DeserializeObject<PlayerFirebaseData>(json);
                if (playerData != null && playerData.playerLevel <= 0)
                {
                    playerData.playerLevel = Mathf.Max(1, PlayerPrefs.GetInt(PrefsPlayerLevelKey, 1));
                }
                return playerData;
            }
            catch (Exception e)
            {
                Debug.LogError("LoadPlayerLocalData failed: " + e.Message);
            }
        }

        return LoadCriticalProgressFromPlayerPrefs();
    }

    private void SaveCriticalProgressToPlayerPrefs(PlayerFirebaseData playerData)
    {
        if (playerData == null)
        {
            return;
        }

        PlayerPrefs.SetString(PrefsUserIdKey, playerData.userID ?? string.Empty);
        PlayerPrefs.SetInt(PrefsPlayerLevelKey, Mathf.Max(1, playerData.playerLevel));
        PlayerPrefs.SetInt(PrefsPlayerCoinsKey, playerData.playerCoins);
        PlayerPrefs.SetInt(PrefsPlayerLivesKey, playerData.playerLives);
        PlayerPrefs.Save();
    }

    private PlayerFirebaseData LoadCriticalProgressFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PrefsPlayerLevelKey))
        {
            return null;
        }

        PlayerFirebaseData fallback = new PlayerFirebaseData(PlayerPrefs.GetString(PrefsUserIdKey, string.Empty), new PlayerDefaultDataModel());
        fallback.playerLevel = Mathf.Max(1, PlayerPrefs.GetInt(PrefsPlayerLevelKey, 1));
        fallback.playerCoins = PlayerPrefs.GetInt(PrefsPlayerCoinsKey, fallback.playerCoins);
        fallback.playerLives = PlayerPrefs.GetInt(PrefsPlayerLivesKey, fallback.playerLives);
        return fallback;
    }

    private DateTime ParseStoredDate(string value, DateTime fallback)
    {
        if (!string.IsNullOrEmpty(value) &&
            DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parsed))
        {
            return parsed;
        }
        return fallback;
    }

    public void ApplyLoadedSettings()
    {
        // Apply audio settings via SettingPenal or a dedicated SoundManager
        if (UIManager.Instance != null && UIManager.Instance.setiingPenal != null)
        {
            UIManager.Instance.setiingPenal.SaveAudioSetting();
        }
    }


    // --- Public Getters (replace PlayerPrefs.GetInt/GetString/GetFloat) ---
    public int GetCoins() { return playerCoins; }
    public int GetDiamonds() { return playerDiamonds; }
    public int GetGems() { return playerGems; }
    public int GetBoosts() { return playerBoosts; }
    public int GetLives() { return playerLives; } // Public field, directly used
    public int GetLevel() { return _playerLevel; }
    public string GetUsername() { return _username; }
    public int GetActivePetToUnlock() { return _petToUnlock; }
    public int GetPetCoins() { return _petCoins; }

    public DateTime GetLastLifeRefillTime() { return _lastLifeRefillTime; }
    public DateTime GetLastDailyRewardClaimed() { return _lastDailyRewardClaimed; }
    public int GetSelectedAvatarID() { return _selectedAvatarID; }
    public int GetHighScore() { return _highScore; }
    public int GetStoryDone() { return _storyDone; }
    public float GetMasterVolume() { return _masterVolume; }
    public float GetSFXVolume() { return _sfxVolume; }
    public float GetMusicVolume() { return _musicVolume; }
    public bool GetVibrationEnabled() { return _vibrationEnabled == 1; }
    public bool GetNotificationEnabled() { return _notificationEnabled == 1; }


    // --- Public Getters for Power-ups ---
    public int GetPowerUpCount_Thunder() { return LaserPostol; }
    public int GetPowerUpCount_Bomb() { return Grenade; }
    public int GetPowerUpCount_Row() { return LaserRifle; }
    public int GetPowerUpCount_Column() { return BubbleGum; }

    public void AddReward(int amount, RewardDetails.RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardDetails.RewardType.Coins:
                {
                    playerCoins += amount;
                    MainMenu.CoinUpdateAction?.Invoke();
                }
                break;
            case RewardDetails.RewardType.Diamonds: playerDiamonds += amount; break;
            case RewardDetails.RewardType.Gems: playerGems += amount; break;
            case RewardDetails.RewardType.Boosts: playerBoosts += amount; break;
            case RewardDetails.RewardType.Lives: playerLives += amount; break;
            case RewardDetails.RewardType.GRENADE: grenade += amount; break;
            case RewardDetails.RewardType.LASER_RIFLE: laserRifle += amount; break;
            case RewardDetails.RewardType.LASER_PISTOL: laserPostol += amount; break;
            case RewardDetails.RewardType.BUBBLE_GUM: bubbleGum += amount; break;
            default: Debug.LogError("Unknown reward type"); break;
        }
        SavePlayerDataToFirebase();
        Debug.Log($"Player now has {playerCoins} coins, {LaserPostol} LaserPistol, {LaserRifle} Rifle, {Grenade} Graned, and {BubbleGum} BubbleGums.");
        //onCoinsUpdated?.Invoke(playerCoins);
        //UIManager.Instance?.mainMenu?.updateCoinsUI();
       
    }

    public void UpdatePlayeCoins(int amount , Action callback = null)
    {
        playerCoins += amount;
        MainMenu.CoinUpdateAction?.Invoke();
        StorePenal.OnCoinPurchedAction?.Invoke(playerCoins);
        callback?.Invoke();
    }

    public void SpendCoins(int amount)
    {
        //NEW Implementation
        playerCoins -= amount;
        if (playerCoins < 0) playerCoins = 0;
        // SavePlayerDataToFirebase();
        //onCoinsUpdated?.Invoke(playerCoins);
        MainMenu.CoinUpdateAction?.Invoke();
        StorePenal.OnCoinPurchedAction?.Invoke(playerCoins);
    }



    public void UpdatePlayerLevel(int level)
    {
        _playerLevel = level;
        SavePlayerDataToFirebase();
    }
    // #region agent log
    public static void DbgLog(string hyp, string loc, string msg, string dataJson)
    {
        try
        {
            Directory.CreateDirectory("/Users/pruthvi/Repositeries/Pet-Rescue-Puzzle-2D/.cursor");
            File.AppendAllText("/Users/pruthvi/Repositeries/Pet-Rescue-Puzzle-2D/.cursor/debug-6f9d2c.log", "{\"sessionId\":\"6f9d2c\",\"runId\":\"run1\",\"hypothesisId\":\"" + hyp + "\",\"location\":\"" + loc + "\",\"message\":\"" + msg + "\",\"data\":" + dataJson + ",\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n");
        }
        catch (Exception) { }
    }
    public static string DbgLen(string s) { return s == null ? "-1" : s.Length.ToString(); }
    // #endregion

    public void SetUsername(string un)
    {
        // #region agent log
        DbgLog("B", "PlayerDataManager.SetUsername", "SetUsername", "{\"newLen\":" + DbgLen(un) + ",\"prevLen\":" + DbgLen(_username) + ",\"isDataLoaded\":" + (IsDataLoaded ? "true" : "false") + "}");
        // #endregion
        _username = un;
        SavePlayerDataToFirebase();
    }
    public void SetActivePetToUnlock(int index)
    {
        PlayerPrefs.SetInt("PetToUnlock", index);
        _petToUnlock = index;
        SavePlayerDataToFirebase();
    }

    public void SavePetCoins(int petCoin)
    {
        _petCoins += petCoin; // Total pet coins
        SavePlayerDataToFirebase();
    }

    public void UpdatePetProgress(int petId, int pointsCollected, bool unlockedStatus)
    {
        string petKey = $"petId_{petId}";
        if (_pets.ContainsKey(petKey))
        {
            _pets[petKey].collectedPoints = pointsCollected;
            _pets[petKey].isUnlocked = unlockedStatus;
            FirebaseHandler.Instance.SavePetUnlockedData(_pets);
        }
        else
        {
            Debug.LogWarning($"Attempted to update pet progress for non-existent pet ID: {petId}");
        }
    }
    public PetFirebaseData GetPetData(int petIndexID)
    {
        string petKey = $"petId_{petIndexID}";
        if (_pets.ContainsKey(petKey))
        {
            return _pets[petKey];
        }
        return null;
    }

    public List<PetFirebaseData> GetPetServerDataList()
    {
        List<PetFirebaseData> dataList = new List<PetFirebaseData>();
        foreach (var pet in _pets)
        {
            dataList.Add(pet.Value);
        }
        return dataList;
    }
    public void SetLastLifeRefillTime(DateTime time)
    {
        _lastLifeRefillTime = time;
        SavePlayerDataToFirebase();
    }
    public void SetLastDailyRewardClaimed(DateTime time)
    {
        _lastDailyRewardClaimed = time;
        SavePlayerDataToFirebase();
    }

    public void SetSelectedAvatarID(int id)
    {
        _selectedAvatarID = id;
        SavePlayerDataToFirebase();
        UIManager.Instance?.mainMenu?.UpdateProfileInfo();
    }
    public void SetHighScore(int score)
    {
        if (score > _highScore)
        {
            _highScore = score;
            FirebaseHandler.Instance.SaveUserHighScoreToFirebase(_highScore);
            FirebaseHandler.Instance.UpdateLeaderboardToFirebase(_username, _highScore, _selectedAvatarID);
        }
    }

    public void SetStoryDone(int status)
    {
        _storyDone = status;
    }
    // --- Settings Setters ---
    public void SetMasterVolume(float level)
    {
        _masterVolume = level;
        // SavePlayerDataToFirebase();
    }
    public void SetSFXVolume(float level)
    {
        _sfxVolume = level;
        // SavePlayerDataToFirebase();
    }
    public void SetMusicVolume(float level)
    {
        _musicVolume = level;
        // SavePlayerDataToFirebase();
    }
    public void SetVibrationEnabled(bool enabled)
    {
        _vibrationEnabled = enabled ? 1 : 0;
        //SavePlayerDataToFirebase();
    }
    public void SetNotificationEnabled(bool enabled)
    {
        _notificationEnabled = enabled ? 1 : 0;
        // SavePlayerDataToFirebase();
    }


    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        PersistProgress(true);
    }
    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("OnApplicationPause :: "+pauseStatus);
        if (pauseStatus)
        {
            PersistProgress(true);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PersistProgress(true);
        }
    }

    /// <summary>
    /// Attempts to spend a power-up of a given type.
    /// </summary>
    /// <param name="powerType">The type of power-up to spend.</param>
    /// <returns>True if the power-up was successfully spent, false if the player has none left.</returns>
    public bool SpendPowerUp(PowerTypes powerType)
    {
        bool spentSuccessfully = false;
        //int powerUpCount = 0;
        switch (powerType)
        {
            case PowerTypes.LASER_PISTOL:
                
                if (laserPostol > 0)
                {
                    //var pData = powerupDataContainer.GetPowerUpData(PowerTypes.LASER_PISTOL);
                    //if (pData != null)
                    //{
                    //    SpendCoins(pData.cost);
                    //}
                    laserPostol -= 1;
                    //SaveLaserPistol(powerUpCount);
                    spentSuccessfully = true;
                }
                break;

            case PowerTypes.LASER_RIFLE:
                
                if (laserRifle > 0)
                {
                    //var pData = powerupDataContainer.GetPowerUpData(PowerTypes.LASER_RIFLE);
                    //if (pData != null)
                    //{
                    //    SpendCoins(pData.cost);
                    //}
                    laserRifle -= 1;
                    //SaveLaserRifle(powerUpCount);
                    spentSuccessfully = true;
                }
                break;
            case PowerTypes.GRENADE:
                
                if (grenade > 0)
                {
                    //var pData = powerupDataContainer.GetPowerUpData(PowerTypes.GRENADE);
                    //if (pData != null)
                    //{
                    //    SpendCoins(pData.cost);
                    //}
                    grenade -= 1;
                   // SaveGrenade(powerUpCount);
                    spentSuccessfully = true;
                }
                break;
            case PowerTypes.BUBBLE_GUM:
                
                if (bubbleGum > 0)
                {
                    //var pData = powerupDataContainer.GetPowerUpData(PowerTypes.BUBBLE_GUM);
                    //if (pData != null)
                    //{
                    //    SpendCoins(pData.cost);
                    //}
                    bubbleGum -= 1;
                    //SaveBubbleGum(powerUpCount);
                    spentSuccessfully = true;
                }
                break;

        }

        if (spentSuccessfully)
        {
            SavePlayerDataToFirebase(); // Save the new count to the database
        }

        return spentSuccessfully;
    }
    //public int GetPlayerLives()
    //{
    //    if (PlayerPrefs.HasKey("PlayerLives"))
    //    {
    //        return PlayerPrefs.GetInt("PlayerLives");
    //    }
    //    else
    //    {
    //        Debug.LogError("No Value in PlayerLives");
    //        PlayerPrefs.SetInt("PlayerLives", 5);

    //        return PlayerPrefs.GetInt("PlayerLives");
    //    }
    //}

    public void UpdatePowerUps(int laserPistol , int laserRifle , int graned , int bubbleGum)
    {
        this.LaserPostol = laserPistol;
        this.LaserRifle = laserRifle;
        this.Grenade = graned;
        this.BubbleGum = bubbleGum;

        //SaveLaserPistol(laserPistol);
        //SaveLaserRifle(laserRifle);
        //SaveGrenade(graned);
        //SaveBubbleGum(bubbleGum);

        powerupDataContainer.SaveData(PowerTypes.LASER_PISTOL, new PowerUpDatModel()
        {
            PowerAvailable = laserPistol,
        });

        powerupDataContainer.SaveData(PowerTypes.LASER_RIFLE, new PowerUpDatModel()
        {
            PowerAvailable = laserRifle,
        });

        powerupDataContainer.SaveData(PowerTypes.GRENADE, new PowerUpDatModel()
        {
            PowerAvailable = graned,
        });

        powerupDataContainer.SaveData(PowerTypes.BUBBLE_GUM, new PowerUpDatModel()
        {
            PowerAvailable = bubbleGum,
        });

        GameController.action.powerUps.UpdatePowerUpTexts();

    }

    public void SaveLaserPistol(int laserPistol)
    {
        PlayerPrefs.SetInt("LASER_PISTOL", laserPistol);
    }

    public int GetLaserPistol()
    {
        return PlayerPrefs.GetInt("LASER_PISTOL", 3);
    }

    public void SaveLaserRifle(int laserPistol)
    {
        PlayerPrefs.SetInt("LASER_RIFLE", laserPistol);
    }

    public int GetLaserRifle()
    {
        return PlayerPrefs.GetInt("LASER_RIFLE", 3);
    }

    public void SaveBubbleGum(int bubbleGum)
    {
        PlayerPrefs.SetInt("BUBBLE_GUM", bubbleGum);
    }

    public int GetBubbleGum()
    {
        return PlayerPrefs.GetInt("BUBBLE_GUM", 1);
    }

    public void SaveGrenade(int granade)
    {
        PlayerPrefs.SetInt("GRENADE", granade);
    }

    public int GetGranade()
    {
        return PlayerPrefs.GetInt("GRENADE", 1);
    }


    public PetInfo GetPetInfo()
    {
        return petInfo;
    }
}
[Serializable]
public class Goal
{
    public int target;
    public int score;
    public int type;

    public Goal(int target, int type)
    {
        score = 0;
        this.target = target;
        this.type = type;
    }

    public void UpdateScore(int score)
    {
        this.score += score;
    }

    public bool IsComplete()
    {
        return score >= target;
    }


}

/// <summary>
/// Default Player Data
/// </summary>

[Serializable]
public class PlayerDefaultDataModel
{
    public string userID;
    public string _username = "";
    public int playerCoins = 150;
    public int playerDiamonds = 0;
    public int playerGems = 0;
    public int playerBoosts = 0;
    public int playerLives = 5;
    public int grenade = 1;
    public int laserPostol = 3;
    public int laserRifle = 3;
    public int bubbleGum = 1;
    public int timeToNextLife = 0;
    public int giveMoves = 5;

    //newAdded Attributes for Firebase
    public int _playerLevel = 1;
    public int _petToUnlock = 1;
    public int _petCoins = 0;
    public Dictionary<string, PetFirebaseData> _pets = PlayerDataManager.Instance.CreateDefaultPetData();
    public DateTime _lastLifeRefillTime = DateTime.Now;
    public DateTime _lastDailyRewardClaimed = DateTime.UtcNow.AddDays(-1);
    public int _selectedAvatarID = 1;
    public int _highScore = 0;
    public int _storyDone = 0;

    // Game Settings
    public float _masterVolume = 1f;
    public float _sfxVolume = 1f;
    public float _musicVolume = 1f;
    public int _vibrationEnabled = 1;
    public int _notificationEnabled = 1;

}
