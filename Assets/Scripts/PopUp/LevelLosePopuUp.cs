using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelLosePopuUp : MonoBehaviour
{
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text textScore;

    [SerializeField] private Button buttonHome;


    public void Render()
    {
        //currentLevelText.text = currentLevel.ToString();
        currentLevelText.text = "LEVEL " + PlayerInfo.MapPlayer.Level;
        textScore.text = PlayerInfo.Info.currentScore.ToString();

        PlayerInfo.Info.UpdatePlayerHighScore();

        buttonHome.onClick.RemoveAllListeners();
        buttonHome.onClick.AddListener(OnHomeButtonClicked);

        gameObject.SetActive(true);
    }

    private void OnHomeButtonClicked()
    {
        ButtonActionController.Click.HomeScene();
    }
}
