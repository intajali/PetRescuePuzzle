
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using EasyUI.Toast;
using TMPro;

public class OutOfMovesPopUp : MonoBehaviour
{
    [SerializeField] private Button buttonBuyMoves;
    [SerializeField] private Button buttonIgnore;

    [SerializeField] private TMP_Text playerCoins;

    private int playerAvailableCoins = 0;

    [SerializeField] private NotificationPopup popup;

    public void Render()
    {
        AddListeners();
        playerAvailableCoins = PlayerDataManager.Instance.GetCoins();
        playerCoins.text = playerAvailableCoins.ToString();
        gameObject.SetActive(true);
    }

    private void AddListeners()
    {
        buttonBuyMoves.onClick.RemoveAllListeners();
        buttonBuyMoves.onClick.AddListener(OnBuyMoveButtonClicked);
        buttonIgnore.onClick.RemoveAllListeners();
        buttonIgnore.onClick.AddListener(OnIgnoreButtonClicked);
    }

   

    private void OnBuyMoveButtonClicked()
    {
        // Open Store from Here
        //var playercoins = PlayerDataManager.Instance.GetCoins();
        if(playerAvailableCoins >= PlayerDataManager.Instance.movesCost)
        {
            //GamePlayHUD.Instance.moveRemaining.text = "" + movesToAdd;
            
            PlayerDataManager.Instance.SpendCoins(PlayerDataManager.Instance.movesCost);
            AddMoves();
            // gameObject.SetActive(false);
            string context = "\"{0} Moves Added !!!\";";
            popup.RenderView(true, PlayerDataManager.Instance.giveMoves , context, () =>
            {
                Invoke("DisableView",0.5f);
            });

        }
        else
        {
            Toast.Show("Need More Coins to Buy Lives", 2f, ToastColor.Red, ToastPosition.BottomCenter);
            GameController.action.storePanelGamePlay.RenderView();
        }

    }

    void AddMoves()
    {
        int movesReward = PlayerDataManager.Instance.giveMoves;
        GamePlayHUD.Instance.moveRemaining.text = movesReward.ToString();
        //movesCounts -= 1;
        GameController.action.addMoves(movesReward);
        GamePlayHUD.Instance.moveCount = movesReward;
    }

    private void OnIgnoreButtonClicked()
    {
        Timer.timer.Lost();
       DisableView();
    }

    public void DisableView()
    {
        gameObject.SetActive(false);
    }
}
