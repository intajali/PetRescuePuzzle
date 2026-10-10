// PlayerFirebaseData.cs
using System;
using System.Collections.Generic;

// --- Nested Pet Data Class ---
[Serializable]
public class PetFirebaseData
{
    public bool isUnlocked;
    public int pointsToUnlock; // From ScriptableObject, stored for clarity/flexibility
    public int collectedPoints;
    public string petName;

    public PetFirebaseData() { } // Default constructor for Firebase deserialization

    public PetFirebaseData(bool unlocked, int pointsTarget, int collected, string name)
    {
        isUnlocked = unlocked;
        pointsToUnlock = pointsTarget;
        collectedPoints = collected;
        petName = name;
    }
}

// --- Main Player Data Class ---
[Serializable]
public class PlayerFirebaseData
{
    public UserGamePlayData userData;
    // Player Currency & Resources
    public string userID;
    public string username;
    public int playerCoins;
    public int playerDiamonds;
    public int playerGems;
    public int playerBoosts;
    public int PU_Bomb;
    public int PU_Slash;
    public int PU_Row;
    public int PU_Column;

    // Player Progress & Lives
    public int playerLives;
    public int playerLevel;
    public string lastLifeRefillTime; // ISO 8601 string
    public int highScore;
    public int storyDone;

    // Game Settings
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
    public int vibrationEnabled;    // 0 for false, 1 for true
    public int notificationEnabled; // 0 for false, 1 for true

    // Pet Data
    public int petToUnlock;
    public int petCoins; // Total collected pet coins for current pet unlock
    public Dictionary<string, PetFirebaseData> pets; // Using Dictionary for string keys (e.g., "petId_1")

    // Daily Reward
    public string lastDailyRewardClaimed; // ISO 8601 string

    // Profile Settings
    public int selectedAvatarID;

    public PlayerFirebaseData() { } // Default constructor for Firebase deserialization

    public PlayerFirebaseData(UserGamePlayData userData)
    {
        this.userData = userData;
    }
    // Constructor to create default data for a new user
    public PlayerFirebaseData(string userID, PlayerDefaultDataModel defaultDataModel)
    {
        
        this.userID = userID;
        username = defaultDataModel._username;
        playerCoins = defaultDataModel.playerCoins;
        playerDiamonds = defaultDataModel.playerDiamonds;
        playerGems = defaultDataModel.playerGems;
        playerBoosts = defaultDataModel.playerBoosts;
        PU_Bomb = defaultDataModel.grenade;
        PU_Slash = defaultDataModel.laserPostol;
        PU_Row = defaultDataModel.laserRifle;
        PU_Column = defaultDataModel.bubbleGum;
        playerLives = defaultDataModel.playerLives;
        playerLevel = defaultDataModel._playerLevel;
        lastLifeRefillTime = defaultDataModel._lastLifeRefillTime.ToString("o");
        highScore  = defaultDataModel._highScore;
        storyDone = defaultDataModel._storyDone;
        masterVolume = defaultDataModel._masterVolume;
        sfxVolume = defaultDataModel._sfxVolume;
        musicVolume = defaultDataModel._musicVolume;
        vibrationEnabled = defaultDataModel._vibrationEnabled;
        notificationEnabled = defaultDataModel._notificationEnabled;
        petToUnlock = defaultDataModel._petToUnlock;
        petCoins = defaultDataModel._petCoins;
        lastDailyRewardClaimed =defaultDataModel._lastDailyRewardClaimed.ToString("o");
        selectedAvatarID = defaultDataModel._selectedAvatarID;

        pets = defaultDataModel._pets;
       
    }
}


[Serializable]
public class UserGamePlayData
{
    public UserInfo userInfo;
    public UserPowerUps powerUps;
    public UserSettingsData settingsData;

    public UserGamePlayData(UserInfo userInfo, UserPowerUps powerUps, UserSettingsData settingsData)
    {
        this.userInfo = userInfo;
        this.powerUps = powerUps;
        this.settingsData = settingsData;
    }
}

[Serializable]
public class UserInfo
{
    public string username;
    public int playerCoins;
    public int playerDiamonds;
    public int playerGems;
    public int playerBoosts;
    public int playerLives;
    public int highScore;

    public int playerLevel;

    public UserPetInfo userPetInfo;

    public string lastLifeRefillTime;
    public string lastDailyRewardClaimed;

    public UserInfo(string username, int playerCoins, int playerDiamonds, int playerGems, int playerBoosts, int playerLives, int highScore, int playerLevel, UserPetInfo userPetInfo, string lastLifeRefillTime, string lastDailyRewardClaimed)
    {
        this.username = username;
        this.playerCoins = playerCoins;
        this.playerDiamonds = playerDiamonds;
        this.playerGems = playerGems;
        this.playerBoosts = playerBoosts;
        this.playerLives = playerLives;
        this.highScore = highScore;
        this.playerLevel = playerLevel;
        this.userPetInfo = userPetInfo;
        this.lastLifeRefillTime = lastLifeRefillTime;
        this.lastDailyRewardClaimed = lastDailyRewardClaimed;
    }
}

[Serializable]
public class UserPetInfo
{
    public int petToUnlock;
    public int petCoins;

    public UserPetInfo(int petToUnlock, int petCoins)
    {
        this.petToUnlock = petToUnlock;
        this.petCoins = petCoins;
    }
}

[Serializable]
public class UserPowerUps
{
    public int grenade;
    public int laserPostol;
    public int laserRifle;
    public int bubbleGum;

    public UserPowerUps(int grenade, int laserPostol, int laserRifle, int bubbleGum)
    {
        this.grenade = grenade;
        this.laserPostol = laserPostol;
        this.laserRifle = laserRifle;
        this.bubbleGum = bubbleGum;

    }
}


[Serializable]
public class UserSettingsData
{
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
    public int vibrationEnabled;
    public int notificationEnabled;
    public int selectedAvatarID;
    public int storyDone;

    public UserSettingsData(float masterVolume, float sfxVolume, float musicVolume, int vibrationEnabled, int notificationEnabled, int selectedAvatarID, int storyDone)
    {
        this.masterVolume = masterVolume;
        this.sfxVolume = sfxVolume;
        this.musicVolume = musicVolume;
        this.vibrationEnabled = vibrationEnabled;
        this.notificationEnabled = notificationEnabled;
        this.selectedAvatarID = selectedAvatarID;
        this.storyDone = storyDone;
    }
}
