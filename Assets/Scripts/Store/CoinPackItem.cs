
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CoinPackItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI coinValueText;
    [SerializeField] private Image coinImage;
    [SerializeField] private Button buyButton;
    [SerializeField] private int coinValue;

    public void RenderView()
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        gameObject.SetActive(true);
    }

    private void OnBuyButtonClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        PlayerDataManager.Instance.UpdatePlayeCoins(coinValue, () =>
        {
            //StorePenal.OnCoinPurchedAction?.Invoke(coinValue);
        });


    }
}
