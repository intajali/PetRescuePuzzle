using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using EasyUI.Toast;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField username;
    [SerializeField] private Button submit;

    [SerializeField] private CanvasGroup panelCanvasGroup;
    // Start is called before the first frame update
    public void RenderView()
    {
        submit.onClick.RemoveAllListeners();
        submit.onClick.AddListener(OnSubmit);
        string userName = PlayerDataManager.Instance.GetUsername();
        if (!string.IsNullOrEmpty(userName))
        {
            Debug.Log("USERNAME EXISTS");
            hidePanel(0.1f);
        }
        else 
        {
            showPanel(0.1f);
            Debug.Log("USERNAME DOES NOT EXIST, SHOWING REGISTRATION PANEL");
        }
    }

    private void OnEnable()
    {
        panelCanvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutQuad).OnStart(() => panelCanvasGroup.gameObject.SetActive(true));
    }


    void OnSubmit()
    {
        string input = username.text;
        // #region agent log
        PlayerDataManager.DbgLog("D", "RegisterPanel.OnSubmit", "submit", "{\"inputLen\":" + PlayerDataManager.DbgLen(input) + ",\"valid\":" + (Regex.IsMatch(input, @"^[a-zA-Z0-9]{3,15}$") ? "true" : "false") + "}");
        // #endregion

        // Example regex: Only letters and numbers, 3 to 15 characters
        string pattern = @"^[a-zA-Z0-9]{3,15}$";

        if (Regex.IsMatch(input, pattern))
        {
            Debug.Log("Username is valid: " + input);
            Toast.Show("Register Success", ToastColor.Green, ToastPosition.BottomCenter);
            PlayerDataManager.Instance.SetUsername(input);
            MainMenu.UsernameUpdateAction?.Invoke();
            hidePanel(1f);

        }
        else
        {
            Debug.LogWarning("Invalid username! Must be 3-15 letters/numbers only.");
            Toast.Show("Invalid username! Must be 3-15 letters/numbers only.", ToastColor.Red, ToastPosition.BottomCenter);
        }
    }

    public void hidePanel(float dur)
    {
        panelCanvasGroup.DOFade(0f, dur).SetEase(Ease.InOutQuad).OnComplete(() => panelCanvasGroup.gameObject.SetActive(false));
    }
    public void showPanel(float dur)
    {
        panelCanvasGroup.gameObject.SetActive(true);
        panelCanvasGroup.DOFade(1f, dur).SetEase(Ease.InOutQuad);
    }

   
}
