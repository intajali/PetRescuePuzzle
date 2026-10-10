// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using EasyUI.Toast;
using DG.Tweening;
using System.Collections;

/// <summary>
/// This class contains the logic associated to the popup for buying lives.
/// </summary>
public class BuyLivesPopup : MonoBehaviour
{
    [SerializeField]
    private Sprite lifeSprite;

    [SerializeField]
    private List<Image> lifeImages;

    //FAIZ-[SerializeField] private Text refillCostText;

    [SerializeField]
    private TMP_Text timeToNextLifeText;

    [SerializeField]
    private ParticleSystem lifeParticles;

    [SerializeField]
    private Button RewardedAdLifelButton;

    [SerializeField]
    private Button refillButton;

    [SerializeField]
    private Image refillButtonImage;

    [SerializeField]
    private Sprite refillButtonDisabledSprite;

    [SerializeField]
    private Button closeBtn;

    [Header("DOTween Animation")]
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private float fadeTimer = .5f;


    private void OnEnable()
    {
        FadeIn();
        // Ensure UI is updated when enabled
        UpdateLifeSprites(PlayerDataManager.Instance.GetLives());
        var maxLives = PlayerDataManager.Instance.maxLives;
        var numLives = PlayerDataManager.Instance.GetLives();
        if (numLives >= maxLives)
        {
            DisableRefillButton();
        }
        else
        {
            EnableRefillButton(); // Ensure button is enabled if lives are not full
        }
    }
    /// <summary>
    /// Unity's Awake method.
    /// </summary>
    void Awake()
    {
        Assert.IsNotNull(lifeSprite);
        //FAIZ-Assert.IsNotNull(refillCostText);
        Assert.IsNotNull(timeToNextLifeText);
        Assert.IsNotNull(lifeParticles);
        Assert.IsNotNull(refillButton);
        Assert.IsNotNull(RewardedAdLifelButton);
        Assert.IsNotNull(refillButtonImage);
        Assert.IsNotNull(refillButtonDisabledSprite);
        Assert.IsNotNull(closeBtn);

        /*            if (Admobtest.instance.IsRewardedAdLoaded())
                    {
                        RewardedAdLifelButton.interactable = true;
                        Debug.Log(" RewardedAdLifelButton.interactable = true");
                    }
                    else
                    {
                        RewardedAdLifelButton.interactable = false;
                        Debug.Log(" RewardedAdLifelButton.interactable = false");
                    }*/
    }

    /// <summary>
    /// Unity's Start method.
    /// </summary>
    void Start()
    {
        PlayerDataManager.Instance.livesSystem.Subscribe(OnLivesCountdownUpdated, OnLivesCountdownFinished);
        /*            var maxLives = PlayerDataManager.Instance.maxLives;
                    var numLives = PlayerPrefs.GetInt("PlayerLives");
                    if (numLives >= maxLives)
                    {
                        DisableRefillButton();
                    }
                    UpdateLifeSprites(numLives);*/
        //FAIZ-refillCostText.text = PuzzleMatchManager.instance.gameConfig.livesRefillCost.ToString();


        closeBtn.onClick.AddListener(OnCloseButtonPressed);
        refillButton.onClick.AddListener(OnRefillButtonPressed);
        RewardedAdLifelButton.onClick.AddListener(RewardedAdLife);
    }

    /// <summary>
    /// Unity's OnDestroy method.
    /// </summary>
    private void OnDestroy()
    {
        //PlayerDataManager.Instance.livesSystem.Unsubscribe(OnLivesCountdownUpdated, OnLivesCountdownFinished);
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.livesSystem != null)
        {
            PlayerDataManager.Instance.livesSystem.Unsubscribe(OnLivesCountdownUpdated, OnLivesCountdownFinished);
        }
        refillButton.onClick.RemoveListener(OnRefillButtonPressed);
        RewardedAdLifelButton.onClick.RemoveListener(RewardedAdLife);
        closeBtn.onClick.RemoveListener(OnCloseButtonPressed);
    }

    /// <summary>
    /// Called when the refill button is pressed.
    /// </summary>
    public void OnRefillButtonPressed()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        //var numCoins = PlayerPrefs.GetInt("PlayerCoins");
        var numCoins = PlayerDataManager.Instance.GetCoins();
        Debug.Log("PlayerCoins" + numCoins);
        if (numCoins >= PlayerDataManager.Instance.livesRefillCost)
        {
            //PlayerDataManager.Instance.livesSystem.RefillLives();
            PlayerDataManager.Instance.livesSystem.RefillLives(); // This calls PDM.SpendCoins and PDM.playerLives update
            lifeParticles.Play();
            
            DisableRefillButton();
        }
        else
        {
            Toast.Show("Need More Coins to Buy Lives", 2f, ToastColor.Red, ToastPosition.BottomCenter);
            UIManager.Instance.storePanel.RenderView();

            /*            var scene = parentScene;
                        if (scene != null)
                        {
                            scene.CloseCurrentPopup();
                            SoundManager.instance.PlaySound("Button");
                            scene.OpenPopup<BuyCoinsPopup>("Popups/BuyCoinsPopup",
                                popup =>
                                {
                                    popup.onClose.AddListener(
                                        () =>
                                        {
                                            scene.OpenPopup<BuyLivesPopup>("Popups/BuyLivesPopup");
                                        });
                                });
                        }*/
        }
        MainMenu.CoinUpdateAction?.Invoke();
        // Update lives sprites immediately
        UpdateLifeSprites(PlayerDataManager.Instance.GetLives());
        /*        int numLives = PlayerPrefs.GetInt("PlayerLives");
                    if (numLives < 5)
                    {
                        PlayerDataManager.Instance.livesSystem.RefillLives();
                        lifeParticles.Play();
                        //SoundManager.instance.PlaySound("BuyPopButton");
                        DisableRefillButton();
                    }
                    else
                    {
        *//*                var scene = parentScene;
                        if (scene != null)
                        {
                            scene.CloseCurrentPopup();
                            SoundManager.instance.PlaySound("Button");
                        }*//*
                    }*/

    }

    /// <summary>
    /// Called when the close button is pressed.
    /// </summary>
    public void OnCloseButtonPressed()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        MainMenu.CoinUpdateAction?.Invoke();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the lives countdown is updated.
    /// </summary>
    /// <param name="timeSpan">The time left for a free life.</param>
    /// <param name="lives">The current number of lives.</param>
    private void OnLivesCountdownUpdated(TimeSpan timeSpan, int lives)
    {
        timeToNextLifeText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        UpdateLifeSprites(lives);
    }

    /// <summary>
    /// Called when the lives countdown finishes.
    /// </summary>
    /// <param name="lives">The current number of lives.</param>
    private void OnLivesCountdownFinished(int lives)
    {
       // timeToNextLifeText.text = "FULL";
        UpdateLifeSprites(lives);
    }

    /// <summary>
    /// Updates the life sprites.
    /// </summary>
    /// <param name="lives">The current number of lives.</param>
    private void UpdateLifeSprites(int lives)
    {
        if(lives == PlayerDataManager.Instance.maxLives)
        {
            timeToNextLifeText.text = "FULL";
        }
        if (lives > 5)
        {
            lives = 5;
        }
        for (var i = 0; i < lives; i++)
        {
            lifeImages[i].sprite = lifeSprite;
        }
    }

    private void UpdateLifeSpritesNew(int lives)
    {
        if (lives > 5)
        {
            lives = 5;
        }
        for (var i = 0; i < lives; i++)
        {
            lifeImages[i].sprite = lifeSprite;
        }

        //BuyLivesBar.instance.updateLives(lives);
    }

    /// <summary>
    /// Disables the refill button.
    /// </summary>
    private void DisableRefillButton()
    {
        refillButtonImage.sprite = refillButtonDisabledSprite;
        refillButton.interactable = false;
    }
    /// <summary>
    /// Enables the refill button.
    /// </summary>
    private void EnableRefillButton()
    {
        refillButtonImage.sprite = refillButtonDisabledSprite; // Assuming you have an enabled sprite
        refillButton.interactable = true;
        // Also ensure it's not full
        if (PlayerDataManager.Instance.GetLives() >= PlayerDataManager.Instance.maxLives)
        {
            DisableRefillButton();
        }
    }

    //Rewarded Ad For Life
    public void RewardedAdLife()
    {
        //Admobtest.instance.ShowRewardedAd(CompleteMethod);
        StartCoroutine(SimulateRewardedAd(true));
    }
    private IEnumerator SimulateRewardedAd(bool completed)
    {
        Debug.Log("Simulating rewarded ad...");
        yield return new WaitForSeconds(2f); // Simulate ad loading/playing time

        Debug.Log("Closed rewarded with:" + completed);
        if (completed)
        {
            PlayerDataManager.Instance.AddReward(1, RewardDetails.RewardType.Lives); // Add one life via PDM
            lifeParticles.Play();
            UpdateLifeSprites(PlayerDataManager.Instance.GetLives());
            Debug.Log("Rewarded Ad: Life added. Total lives: " + PlayerDataManager.Instance.GetLives());
            Toast.Show("Life Gained from Ad!", 2f, ToastColor.Green, ToastPosition.BottomCenter); // Feedback
        }
        else
        {
            Debug.Log("REWARDED AD NOT COMPLETED");
            Toast.Show("Ad not completed.", 2f, ToastColor.Red, ToastPosition.BottomCenter); // Feedback
        }
        EnableRefillButton(); // Re-enable button after ad attempt
    }

    private void CompleteMethod(bool completed)
    {
        var numLives = PlayerPrefs.GetInt("PlayerLives");
        Debug.Log("Closed rewarded with:" + completed);
        if (completed == true)
        {
            numLives++;
            PlayerPrefs.SetInt("PlayerLives", numLives);
            UpdateLifeSpritesNew(numLives);
            Debug.Log("numLives" + numLives);

        }
        else
        {
            Debug.Log("REWARDED AD NOOOOT");
        }
    }

    public void FadeIn()
    {
        canvas.DOFade(1, fadeTimer);
    }

    public void FadeOut()
    {
        canvas.DOFade(0, fadeTimer);
    }
    private void OnDisable()
    {
        FadeOut();
    }

}
