using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Assertions;
using EasyUI.Toast;
using System;

public class StorePenal : MonoBehaviour
{
    [SerializeField] Button backBtn;
    [SerializeField] Button addLivesBtn;

    [SerializeField] TextMeshProUGUI availableCoinsText;

    [Header ("Store Life Reference")]
    [SerializeField] private Sprite enabledLifeSprite;
    [SerializeField] private Sprite disabledLifeSprite;
    [SerializeField] private Image lifeImage;
    [SerializeField] TMP_Text livesText;

    [SerializeField] private Image plusBtnImage;
    [SerializeField] private Sprite enabledplusSprite;
    [SerializeField] private Sprite disabledplusSprite;

    [Header("Ads Button")]
    [SerializeField] private Button adsBtn;

    [Header("DOTween Animation")]
    [SerializeField] private float fadeTimer = 1f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] List<GameObject> items = new List<GameObject>();
    [SerializeField] Button[] powerupBtns;

    [SerializeField] private List<CoinPackItem> coinPackItems;

    [SerializeField] private NotificationPopup notificationPopup;

    public static Action<int> OnCoinPurchedAction;

    private void Awake()
    {
        Assert.IsNotNull(enabledLifeSprite);
        Assert.IsNotNull(disabledLifeSprite);
        Assert.IsNotNull(lifeImage);
        Assert.IsNotNull(livesText);
       // Assert.IsNotNull(timeToNextLifeText);
        Assert.IsNotNull(plusBtnImage);
        Assert.IsNotNull(enabledplusSprite);
        Assert.IsNotNull(disabledplusSprite);
    }
    public void RenderView()
    {
        SetData();
        UpdateAvailableCoin(0);
        for (int i = 0; i < coinPackItems.Count; i++)
        {
            coinPackItems[i].RenderView();
        }
        // AddListeners();
        gameObject.SetActive(true);
        Debug.Log("Panel Status " + gameObject.activeSelf);
        PanelFadeIn();
    }
    private void SetData()
    {
        int numLives = PlayerDataManager.Instance.GetLives();
        var maxLives = PlayerDataManager.Instance.maxLives;
        livesText.text = numLives.ToString();
        plusBtnImage.sprite = numLives == maxLives ? disabledplusSprite : enabledplusSprite;

        if (numLives == 5)
        {
            adsBtn.interactable = false;
        }
        else
        {
            adsBtn.interactable = true;
        }

        for (int i = 0; i < powerupBtns.Length; i++)
        {
            int buttonIndex = i;
            powerupBtns[i].onClick.AddListener(() => OnPowerupButtonPressed(buttonIndex));
        }
        //PlayerDataManager.Instance.AddReward(500, RewardDetails.RewardType.Coins);
    }

    public void UpdateAvailableCoin(int coinsAdded)
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUY);
        availableCoinsText.text = PlayerDataManager.Instance.GetCoins().ToString();
        if (coinsAdded > 0)
        {
            notificationPopup.RenderView(true, coinsAdded , "\"{0} Coins Added !!!\";");
        }
    }

    void PlusSpriteChange(int lives)
    {
        lifeImage.sprite = lives == 0 ? disabledLifeSprite : enabledLifeSprite;
        var maxLives = PlayerDataManager.Instance.maxLives;
        plusBtnImage.sprite = lives == maxLives ? disabledplusSprite : enabledplusSprite;
    }

    void OnPowerupButtonPressed(int buttonIndex)
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        int availableCoins = PlayerDataManager.Instance.GetCoins();
        int puPrice = 100;
        Debug.Log("Power-up button pressed: " + buttonIndex);
        if (availableCoins < puPrice)
        {
            Toast.Show("Unsufficient Coins", ToastColor.Red, ToastPosition.BottomCenter);
            return;
        }
        if (buttonIndex == 0)
        {
            PlayerDataManager.Instance.SpendCoins(puPrice);
            PlayerDataManager.Instance.AddReward(1, RewardDetails.RewardType.LASER_PISTOL);
          
            notificationPopup.RenderView(true, 1, "\"{0} Laser Pistol Added !!!\";");
            
        }
        else if (buttonIndex == 1)
        {
            PlayerDataManager.Instance.SpendCoins(puPrice);
            PlayerDataManager.Instance.AddReward(1, RewardDetails.RewardType.LASER_RIFLE);
            notificationPopup.RenderView(true, 1, "\"{0} Laser Rifle Added !!!\";");
        }
        else if (buttonIndex == 2)
        {
            PlayerDataManager.Instance.SpendCoins(puPrice);
            PlayerDataManager.Instance.AddReward(1, RewardDetails.RewardType.GRENADE);
            notificationPopup.RenderView(true, 1, "\"{0} Graned Added !!!\";");

        }
        else if (buttonIndex == 3)
        {
            PlayerDataManager.Instance.SpendCoins(puPrice);
            PlayerDataManager.Instance.AddReward(1, RewardDetails.RewardType.BUBBLE_GUM);
            notificationPopup.RenderView(true, 1, "\"{0} Special Pistol Added !!!\";");
        }
        else
        {
            Debug.LogError("Power-up ERROR " + buttonIndex);
        }
        //UpdateAvailableCoin(0);
    }

    void LiveSpriteChange(int lives)
    {
        //timeToNextLifeText.text = "FULL";
        livesText.text = lives.ToString();
        lifeImage.sprite = lives == 0 ? disabledLifeSprite : enabledLifeSprite;
        plusBtnImage.sprite = disabledplusSprite;
    }

  

    public void PanelFadeIn(/*CanvasGroup canvasGroup, RectTransform rectTransform*/)
    {
        float panelMoveDuration = fadeTimer;
        canvasGroup.alpha = 1f;
        rectTransform.transform.localPosition = new Vector3(0f, -1000f, 0f);
        rectTransform.DOAnchorPos(Vector2.zero, panelMoveDuration, false).SetEase(Ease.OutExpo);
        canvasGroup.DOFade(1, panelMoveDuration).SetEase(Ease.InOutSine);
        StartCoroutine(ItemsAnimations());
    }

    public void PanelFadeOut(/*CanvasGroup canvasGroup, RectTransform rectTransform*/)
    {
        canvasGroup.alpha = 0f;
        rectTransform.transform.localPosition = new Vector3(0f, 0f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, -1000f), fadeTimer, false).SetEase(Ease.InOutQuint);
        canvasGroup.DOFade(0, fadeTimer);
    }

    IEnumerator ItemsAnimations()
    {
        foreach (var item in items)
        {
            item.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        float delayBetweenItems = .05f;
        foreach (var item in items)
        {
            item.transform.DOScale(1f, fadeTimer).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(delayBetweenItems);
        }
    }

    private void OnEnable()
    {
        OnCoinPurchedAction += UpdateAvailableCoin;
        backBtn.onClick.AddListener(OnBackButtonClick);
    }

    void OnBackButtonClick()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
        PlayerDataManager.Instance.SavePlayerDataToFirebase();
        //if (UIManager.Instance != null)
        //    UIManager.Instance.storePanel.RenderView();
        Disable();
    }

    private void OnDisable()
    {
        //if (UIManager.Instance != null)
        //{
        //    UIManager.Instance.mainMenu.updateCoinsUI();
        //}
        backBtn.onClick.RemoveAllListeners();
        OnCoinPurchedAction -= UpdateAvailableCoin;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        PanelFadeOut();
    }
}
