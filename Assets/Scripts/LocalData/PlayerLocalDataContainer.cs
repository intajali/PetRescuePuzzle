using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserLocalData", menuName = "ScriptableObjects/Create UserLocalData", order = 1)]
public class PlayerLocalDataContainer : ScriptableObject
{
    public string userID;
    public string username;
    public int playerLevel;
    public int petCoins; // Total collected pet coins for current pet unlock

    public int playerCoins;
    public int playerDiamonds;
    public int playerGems;
    public int playerBoosts;

    // Player Progress & Lives
    public int playerLives;
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
    public PetRescueDataContainer petRescueDataContainer;

    // Daily Reward
    public string lastDailyRewardClaimed; // ISO 8601 string

    // Profile Settings
    public int selectedAvatarID;

    [Header("PowerUps")]
    public PowerUpsData powerUpsData;

    public void SetUserName(string username)
    {
        this.username = username;
    }
    public string GetUserName() { return this.username; }

    public void SetPetCoin(int petCoins)
    {
        this.petCoins = petCoins;
    }
    public int GetPepetCoins() { return this.petCoins; }

    public void SetUserLevel(int playerLevel)
    {
        this.playerLevel = playerLevel;
    }
    public int GetPlayerLevel() { return this.playerLevel; }


    public void SetPlayerLives(int playerLives)
    {
        this.playerLives = playerLives;
    }
    public int GetPlayerLives() { return this.playerLives; }
   

}
[Serializable]
public class PowerUpsData
{
    public int grenade;
    public int laserPostol;
    public int laserRifle;
    public int bubbleGum;
    public void SetPowerUpsData(int grenade, int laserPostol, int laserRifle, int bubbleGum)
    {
        this.grenade = grenade;
        this.laserPostol = laserPostol;
        this.laserRifle = laserRifle;
        this.bubbleGum = bubbleGum;
    }

    public PowerUpsData GetPowerUps()
    {
        return new PowerUpsData()
        {
            grenade = this.grenade,
            laserPostol = this.laserPostol,
            laserRifle = this.laserRifle,
            bubbleGum = this.bubbleGum,
        };
    }
}
