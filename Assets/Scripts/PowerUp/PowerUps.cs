using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using EasyUI.Toast;
using TMPro;
using UnityEngine;

public enum PowerTypes
{
    NONE = 0,
    GRENADE = 1,
    BOMB_X = 2,
    POWERUP_ROW = 3,
    POWERUP_COLUMN = 4,
    LASER_RIFLE = 5,
    BUBBLE_GUM = 6,
    LASER_PISTOL = 7,
    SNOW_GLOBE = 8,

}
public class PowerUps : MonoBehaviour
{
    public PowerTypes currentSelectedPower;

    public List<PowerUp> powerUpList;
    [SerializeField] TMP_Text[] powerupTexts;
    [SerializeField] private GameObject infoContainer;
    [SerializeField] private TextMeshProUGUI infoContainerText;

    public string LASER_PISTOL = "This Power Ups will Destroy any one of the Jewel at a time.";
    public string LASER_RIFLE = "This Power Ups will Destroy any one of the Jewel Rows Or Columns at a time.";
    public string GRENADE = "This Power Ups makes a blast effect and removed all the Enemy Jewels from the Board.";
    public string BUBBLE_GUN = "This Power Ups Converts any non enemy jewels to Special Jewel, Which can destory all the matched tiles at a Time. ";

    private PowerUp selectedPowerUp;
    public void Render()
    {
        infoContainer.SetActive(false);
        infoContainerText.text = string.Empty;
        foreach (PowerUp power in powerUpList)
        {
            power.Render();
        }
        UpdatePowerUpTexts();

        ShowPowerUpsTutorial();
    }

    private void Start()
    {
        //UpdatePowerUpTexts();
    }

    public void UpdatePowerUpTexts()
    {
        for (int i = 0; i < powerUpList.Count; i++)
        {
            // The PowerTypes enum starts with usable powers at 1, so we add 1 to the loop index
            PowerTypes type = powerUpList[i].PowerTypes;
            int count = 0;

            switch (type)
            {
                case PowerTypes.GRENADE:
                    count = PlayerDataManager.Instance.Grenade;
                    powerUpList[2].UpdatePowerUpCounts(count);
                    break;
                case PowerTypes.LASER_PISTOL:
                    count = PlayerDataManager.Instance.LaserPostol;
                    powerUpList[0].UpdatePowerUpCounts(count);
                    break;
                case PowerTypes.LASER_RIFLE:
                    count = PlayerDataManager.Instance.LaserRifle;
                    powerUpList[1].UpdatePowerUpCounts(count);
                    break;
                case PowerTypes.BUBBLE_GUM:
                    count = PlayerDataManager.Instance.BubbleGum;
                    powerUpList[3].UpdatePowerUpCounts(count);
                    break;
            }

            // Update the corresponding text field
            // powerupTexts[i].text = count.ToString();
        }

    }
    /*    public void OnPowerSelected(PowerTypes types)
        {
            if(GameController.action.IsPowerInUse)
                return;
            currentSelectedPower = types;
            ExecutePower();
        }*/

    public void OnPowerSelected(PowerTypes types)
    {
        if (GameController.action.IsPowerInUse)
            return;

        // --- NEW LOGIC ---
        // 1. Try to spend the power-up first.
        bool successfullySpent = PlayerDataManager.Instance.SpendPowerUp(types);

        // 2. If spending fails (because the count is 0), do nothing.
        if (!successfullySpent)
        {
            Debug.Log("No " + types.ToString() + " power-ups left!");
            Toast.Show("No " + types.ToString() + " power-ups left!", 2f, ToastColor.Red, ToastPosition.BottomCenter);
            return;
        }

        // 3. If successful, update the UI and execute the power.
        UpdatePowerUpTexts();
        currentSelectedPower = types;
        ExecutePower();
    }

    /// <summary>
    /// Execute Power based on currect selected power up types
    /// </summary>
    private void ExecutePower()
    {
        GameController.action.IsPowerInUse = true;
        Debug.Log("PowerUps :: ExecutePower :: " + currentSelectedPower);
        // Code 
        // send power to game manager and execute 

        int x = UnityEngine.Random.Range(0, 7);
        int y = UnityEngine.Random.Range(0, 9);

        GameObject obj = JewelSpawner.spawn.JewelGrib[x, y];
        if (obj != null)
        {
            GameController.action.ExecutePowerUps(obj, obj, currentSelectedPower);
        }
    }


    public void OnInfoClicked(PowerTypes types, PowerUp selectedPowerUp)
    {
        this.selectedPowerUp = selectedPowerUp;
        string infoText = string.Empty;
        switch (types)
        {
            case PowerTypes.LASER_PISTOL:
                infoText = LASER_PISTOL;
                break;

            case PowerTypes.LASER_RIFLE:
                infoText = LASER_RIFLE;
                break;

            case PowerTypes.GRENADE:
                infoText = GRENADE;
                break;
            case PowerTypes.BUBBLE_GUM:
                infoText = BUBBLE_GUN;
                break;
        }
        infoContainer.SetActive(true);
        infoContainerText.text = infoText;

        StartCoroutine(DisableInfoContainer());
    }

    IEnumerator DisableInfoContainer()
    {
        yield return new WaitForSeconds(3f);
        infoContainer.SetActive(false);
        infoContainerText.text = string.Empty;
        selectedPowerUp.DisableGlow();
       
        ShowPowerUpsTutorial();
    }

    int c = 0;
    public void ShowPowerUpsTutorial()
    {
        int storyIndex = PlayerDataManager.Instance.GetStoryDone();
       
        if (storyIndex != 1)
        {
            if (c < powerUpList.Count)
            {
                powerUpList[c].OnInfoButtonClicked();
                c += 1;
            }
        }

        if (c >= powerUpList.Count)
        {
            if (PlayerDataManager.Instance != null)
            {
                c = 0;
                PlayerDataManager.Instance.SetStoryDone(1);
            }
        }

    }

}
