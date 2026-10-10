using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SettingPenal : MonoBehaviour
{
    [SerializeField] Button backBtn;
   // [SerializeField] Button saveBtn;
    [SerializeField] Button aboutUsBtn;

    [Header("Sliders")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle muteToggle;
    [SerializeField] private Image muteIcon;



    public Sprite togggleOnSprite;
    public Sprite togggleOffSprite;

    public Sprite muteOnSprite;
    public Sprite muteOffSprite;


    [Header("DOTween Animation")]
    [SerializeField] private float fadeTimer = 1f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] List<GameObject> items = new List<GameObject>();

    int previousGameState;

    public void RenderView(int preGameState)
    {
        previousGameState = preGameState;
        if (GameController.action)
        {
            GameController.action.GameState = (int)Timer.GameState.SETTING;
        }
        SaveAudioSetting();
        AddListeners();
        // AddListeners();
        gameObject.SetActive(true);
        Debug.Log("Panel Status" + gameObject.activeSelf);
       PanelFadeIn();
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

    private void AddListeners()
    {
        backBtn.onClick.RemoveAllListeners();
        backBtn.onClick.AddListener(OnBackButtonClick);

        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.AddListener(SetSFX);

        musicSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.AddListener(SetMusic);

        vibrationToggle.onValueChanged.RemoveAllListeners();
        vibrationToggle.onValueChanged.AddListener(OnVibrationValueChanged);

        muteToggle.onValueChanged.RemoveAllListeners();
        muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);

        aboutUsBtn.onClick.RemoveAllListeners();
        aboutUsBtn.onClick.AddListener(OnAboutUsBtnClicked);
    }

   

    public void SetSFX(float level)
    {
        PlayerDataManager.Instance.SetSFXVolume(level); // Save to Firebase via PDM

        AudioManager.Instance.SetSFXVolume(level);

    }

    public void SetMusic(float level)
    {
        PlayerDataManager.Instance.SetMusicVolume(level); // Save to Firebase via PDM
        AudioManager.Instance.SetMusicVolume(level);
    }


    public void OnMuteToggleChanged(bool value)
    {
        if (muteToggle != null)
        {
            muteToggle.GetComponent<Image>().sprite = value == true ? togggleOnSprite : togggleOffSprite;
            muteIcon.sprite = value ? muteOnSprite : muteOffSprite;
        }
        AudioManager.Instance.SetMute(value);
        //PlayerPrefs.SetInt("Notification", value == true ? 1 : 0);
        PlayerDataManager.Instance.SetNotificationEnabled(value); // Save to Firebase via PDM

    }

    private void OnVibrationValueChanged(bool value)
    {
        if (vibrationToggle != null)
        {
            vibrationToggle.GetComponent<Image>().sprite = value == true ? togggleOnSprite : togggleOffSprite;
        }
        //PlayerPrefs.SetInt("Vibration", value == true ? 1 : 0);
        PlayerDataManager.Instance.SetVibrationEnabled(value); // Save to Firebase via PDM

    }

    public void SaveAudioSetting()
    {
        float master = PlayerDataManager.Instance.GetMasterVolume();
        float sfx = PlayerDataManager.Instance.GetSFXVolume();
        float music = PlayerDataManager.Instance.GetMusicVolume();

        if (sfxSlider != null)
            sfxSlider.value = sfx;

        if (musicSlider != null)
            musicSlider.value = music;

        bool vibration = PlayerDataManager.Instance.GetVibrationEnabled();
        bool notification = PlayerDataManager.Instance.GetNotificationEnabled();

        if (vibrationToggle != null)
        {
            vibrationToggle.isOn = vibration; // Set toggle state directly
            vibrationToggle.GetComponent<Image>().sprite = vibration ? togggleOnSprite : togggleOffSprite;
        }

        if (muteToggle != null)
        {
            muteToggle.isOn = notification; // Set toggle state directly
            muteToggle.GetComponent<Image>().sprite = notification ? togggleOnSprite : togggleOffSprite;
            muteIcon.sprite = notification ? muteOnSprite : muteOffSprite;

        }
        AudioManager.Instance.ApplySavedSettings(music, sfx,notification);
        AudioManager.Instance.SetMute(notification);

        // PlayerDataManager.Instance.SavePlayerDataToFirebase();
    }

    void OnAboutUsBtnClicked()
    {
        UIManager.Instance.aboutUsPanel.RenderView();
    }

    void OnBackButtonClick()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);

        this.Disable();
    }

    private void Disable()
    {
        if (GameController.action)
        {
            GameController.action.GameState = previousGameState;
        }
        SaveAudioSetting();
        PlayerDataManager.Instance.SavePlayerDataToFirebase();
        gameObject.SetActive(false);
        PanelFadeOut();
    }
}

   



