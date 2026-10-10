using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Timer;

public class PowerUp : MonoBehaviour
{
    public Button powerUpButton;
    public Button infoButton;
    public PowerTypes PowerTypes = PowerTypes.NONE;
    public TextMeshProUGUI powerUpCountText;
    public GameObject glowPanel;
    public void Render()
    {
        powerUpButton.onClick.RemoveAllListeners();
        powerUpButton.onClick.AddListener(OnPowerUpClicked);

        infoButton.onClick.RemoveAllListeners();
        infoButton.onClick.AddListener(OnInfoButtonClicked);

        gameObject.SetActive(true);
    }

    public void OnInfoButtonClicked()
    {
        GameController.action.GameState = (int)GameState.POWER_UP_INFO;
        HighlightGlow();
        GameController.action.powerUps.OnInfoClicked(PowerTypes, this);
    }

    private void OnPowerUpClicked()
    {
        //PowerUps.OnPowerSelect?.Invoke(PowerTypes);
        GameController.action.powerUps.OnPowerSelected(PowerTypes);
    }

    public void UpdatePowerUpCounts(int count)
    {
        powerUpCountText.text = count.ToString();

    }

    public void HighlightGlow()
    {
        glowPanel.SetActive(true);
    }

    public void DisableGlow()
    {
        GameController.action.GameState = (int)GameState.PLAYING;
        glowPanel.SetActive(false);
    }
}
