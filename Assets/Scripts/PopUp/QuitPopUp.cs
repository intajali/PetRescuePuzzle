
using System;
using UnityEngine;
using UnityEngine.UI;
using static Timer;

public class QuitPopUp : MonoBehaviour
{
    [SerializeField] private Button buttonQuit;
    [SerializeField] private Button buttonPlayOn;
    public void Render()
    {
        buttonQuit.onClick.RemoveAllListeners();
        buttonQuit.onClick.AddListener(OnQuitButtonClicked);

        buttonPlayOn.onClick.RemoveAllListeners();
        buttonPlayOn.onClick.AddListener(OnPlayOnButtonClicked);

        gameObject.SetActive(true);
    }

    private void OnPlayOnButtonClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
        GameController.action.GameState = (int)GameState.PLAYING;
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    private void OnQuitButtonClicked()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
        //PlayerDataManager.Instance.livesSystem.RemoveLife();
        gameObject.SetActive(false);
        ButtonActionController.Click.HomeScene();
    }
}
