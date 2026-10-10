// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// This class is used to manage the bar to buy lives that is located on the level scene.
/// </summary>
public class BuyLivesBar : MonoBehaviour
{
    public static BuyLivesBar instance;

    [SerializeField]
    private Sprite enabledLifeSprite;

    [SerializeField]
    private Sprite disabledLifeSprite;

    [SerializeField]
    private Image lifeImage;

    [SerializeField]
    private Text numLivesText;

    [SerializeField]
    private Text timeToNextLifeText;

    [SerializeField]
    private Image buttonImage;

    [SerializeField]
    private Sprite enabledButtonSprite;

    [SerializeField]
    private Sprite disabledButtonSprite;

    [SerializeField]
    private GameObject BuyLivesPopup;

    /// <summary>
    /// Unity's Awake method.
    /// </summary>
    private void Awake()
    {
        Assert.IsNotNull(enabledLifeSprite);
        Assert.IsNotNull(disabledLifeSprite);
        Assert.IsNotNull(lifeImage);
        Assert.IsNotNull(numLivesText);
        Assert.IsNotNull(timeToNextLifeText);
        Assert.IsNotNull(buttonImage);
        Assert.IsNotNull(enabledButtonSprite);
        Assert.IsNotNull(disabledButtonSprite);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    /// <summary>
    /// Unity's Start method.
    /// </summary>
    private void Start()
    {
        int numLives = PlayerDataManager.Instance.GetLives();
        var maxLives = PlayerDataManager.Instance.maxLives;

        numLivesText.text = numLives.ToString();
        buttonImage.sprite = numLives == maxLives ? disabledButtonSprite : enabledButtonSprite;
        PlayerDataManager.Instance.livesSystem.Subscribe(OnLivesCountdownUpdated, OnLivesCountdownFinished);
        PlayerDataManager.Instance.livesSystem.InitializeLivesSystem(PlayerDataManager.Instance.GetLastLifeRefillTime(), numLives, maxLives, PlayerDataManager.Instance.timeToNextLife);

    }

    /// <summary>
    /// Unity's OnDestroy method.
    /// </summary>
    private void OnDestroy()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.livesSystem != null)
        {
            PlayerDataManager.Instance.livesSystem.Unsubscribe(OnLivesCountdownUpdated, OnLivesCountdownFinished);
        }
    }

    /// <summary>
    /// Called when the buy button is pressed.
    /// </summary>
    public void OnBuyButtonPressed()
    {
        var numLives = PlayerDataManager.Instance.GetLives();
        var maxLives = PlayerDataManager.Instance.maxLives;
        if (numLives < maxLives)
        {
            BuyLivesPopup.SetActive(true);
        }
        else
        {
            Debug.Log("Lives are FULL ALREADY");
        }
    }

    /// <summary>
    /// Called when the lives countdown is updated.
    /// </summary>
    /// <param name="timeSpan">The time left until the next free life.</param>
    /// <param name="lives">The current number of lives.</param>
    private void OnLivesCountdownUpdated(TimeSpan timeSpan, int lives)
    {
        timeToNextLifeText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        UpdateLivesUI(lives);
    }

    /// <summary>
    /// Called when the lives countdown finishes.
    /// </summary>
    /// <param name="lives">The current number of lives.</param>
    private void OnLivesCountdownFinished(int lives)
    {
        timeToNextLifeText.text = "FULL";
        UpdateLivesUI(lives);
    }

    public void updateLives(int lives)
    {
        numLivesText.text = lives.ToString();
        Debug.Log("updateLives" + lives);
    }
    // Centralized method to update lives-related UI
    private void UpdateLivesUI(int lives)
    {
        numLivesText.text = lives.ToString();
        lifeImage.sprite = lives == 0 ? disabledLifeSprite : enabledLifeSprite;
        var maxLives = PlayerDataManager.Instance.maxLives;
        buttonImage.sprite = lives == maxLives ? disabledButtonSprite : enabledButtonSprite;
    }
    public void UpdateUI() // Call this to refresh UI from current PDM state
    {
        int numLives = PlayerDataManager.Instance.GetLives();
        var maxLives = PlayerDataManager.Instance.maxLives;
        numLivesText.text = numLives.ToString();
        buttonImage.sprite = numLives == maxLives ? disabledButtonSprite : enabledButtonSprite;
        lifeImage.sprite = numLives == 0 ? disabledLifeSprite : enabledLifeSprite; // Ensure life image is correct on start
    }
}