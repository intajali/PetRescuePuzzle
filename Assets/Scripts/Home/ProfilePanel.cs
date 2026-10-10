using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField] Button backBtn;
    [SerializeField] Button profileAvatar1;
    [SerializeField] Button profileAvatar2;

    [SerializeField] Button btnUpdateUsername;

    [SerializeField] TMP_InputField inputUsername;
    [SerializeField] GameObject ProfilePicPanel; // The panel containing avatar selection

    [Header("DOTween Animation")]
    [SerializeField] private float fadeTimer = 1f;
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private Image rankBadge;
    [SerializeField] private Sprite[] rankBadges;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] List<GameObject> items = new List<GameObject>();

    [SerializeField] private Image mainProfileImage; // Assign this in Inspector: The main profile image displayed on the ProfilePanel itself.
    [SerializeField] private PetCollectionView collectionView;

    public void RenderView(string username)
    {
        // #region agent log
        PlayerDataManager.DbgLog("A", "ProfilePanel.RenderView", "render", "{\"argLen\":" + PlayerDataManager.DbgLen(username) + ",\"pdmLen\":" + PlayerDataManager.DbgLen(PlayerDataManager.Instance.GetUsername()) + "}");
        // #endregion
      
        gameObject.SetActive(true);
        AddListenersToButtons();
        Debug.Log("Panel Status " + gameObject.activeSelf);
        /*        if (PlayerDataManager.Instance.usernameExists())
                {
                    UpdateProfileData();
                }*/
        collectionView.RenderView();
        UpdateProfileData();
        PanelFadeIn();

        inputUsername.interactable = false;
        inputUsername.text = CapitalizeFirst(username.Trim());
    }

    void AddListenersToButtons()
    {
        profileAvatar1.onClick.AddListener(() => OnAvatarButtonPressed(1));
        profileAvatar2.onClick.AddListener(() => OnAvatarButtonPressed(2));
        btnUpdateUsername.onClick.AddListener(() => OnUpdateUsernameButtonPressed());
        inputUsername.onEndEdit.AddListener((value) => OnUsernameInputEnded(value));
    }

    void OnUpdateUsernameButtonPressed()
    {
        Debug.Log("Update Username Button Pressed");
        inputUsername.interactable = true;
         inputUsername.Select();
        inputUsername.ActivateInputField();
    }

    void OnUsernameInputEnded(string value)
    {
        Debug.Log("Username Input Ended: " + value);
        string capitalizedUsername = CapitalizeFirst(value.Trim());
        inputUsername.text = capitalizedUsername;
        PlayerDataManager.Instance.SetUsername(capitalizedUsername);
        FirebaseHandler.Instance.SaveUpdatedUsernameToFirebase(capitalizedUsername); 
        inputUsername.interactable = false;
        MainMenu.UsernameUpdateAction?.Invoke();
    }

    void OnAvatarButtonPressed(int avatarIndex)
    {
        ShowAvatarPanelAnimation();
        Debug.Log("Avatar " + avatarIndex + " pressed.");
    }

    public void ShowAvatarPanelAnimation()
    {
        ProfilePicPanel.SetActive(true);
        CanvasGroup avatarCanvasGroup = ProfilePicPanel.GetComponent<CanvasGroup>();

        if (avatarCanvasGroup != null)
        {
            avatarCanvasGroup.alpha = 0f; // Start with transparent panel
            ProfilePicPanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, fadeTimer).SetEase(Ease.OutBounce);
            avatarCanvasGroup.DOFade(1f, fadeTimer).SetEase(Ease.InOutSine);
        }
        else
        {
            Debug.LogError("AvatarPanel does not have a CanvasGroup component.");
        }
    }

    public void UpdateProfileData()
    {
        Debug.Log("UpdateProfileData");
        usernameText.text = PlayerDataManager.Instance.GetUsername();
        // #region agent log
        PlayerDataManager.DbgLog("C", "ProfilePanel.UpdateProfileData", "usernameText set", "{\"textLen\":" + PlayerDataManager.DbgLen(usernameText.text) + "}");
        // #endregion
        // Rank logic needs to come from the leaderboard system if you have one, or is hardcoded for now
        rankText.text = "1<sup>st</sup>";
        rankBadge.sprite = rankBadges[0];

        // Update the main profile image displayed on this panel
        int selectedAvatarID = PlayerDataManager.Instance.GetSelectedAvatarID();


        if (PlayerDataManager.Instance.avatarData != null && selectedAvatarID >= 0 && selectedAvatarID < PlayerDataManager.Instance.avatarData.avatarInfoList.Count)
        {
            AvatarInfo selectedAvatar = PlayerDataManager.Instance.avatarData.avatarInfoList[selectedAvatarID];
            if (mainProfileImage != null) // Check if the Image component is assigned
            {
                mainProfileImage.sprite = selectedAvatar.avatarIcon;
            }
        }
        else
        {
            Debug.LogWarning("Invalid avatar ID or AvatarData not assigned in PlayerDataManager. Cannot update main profile image on ProfilePanel.");
            // Optionally set a default sprite here if avatar is not found or data is invalid.
        }
    }
    public void PanelFadeIn()
    {
        float panelMoveDuration = fadeTimer * 2f;
        canvasGroup.alpha = 1f;
        rectTransform.transform.localPosition = new Vector3(0f, -1000f, 0f);
        rectTransform.DOAnchorPos(Vector2.zero, panelMoveDuration, false).SetEase(Ease.OutExpo);
        canvasGroup.DOFade(1, panelMoveDuration).SetEase(Ease.InOutSine);
        StartCoroutine(ItemsAnimations());
    }

    public void PanelFadeOut()
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
            item.transform.DOScale(1f, fadeTimer).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(delayBetweenItems);
        }
    }

    private void OnEnable()
    {
        backBtn.onClick.AddListener(OnBackButtonClick);
    }

    void OnBackButtonClick()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
       // UIManager.Instance.playerProfile.RenderView();
        Disable();
    }
    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        PanelFadeOut();
    }

    public string CapitalizeFirst(string value)
    {
        Debug.Log("CapitalizeFirst: " + value);
        if (string.IsNullOrEmpty(value))
        return value;

        string capitalized = char.ToUpperInvariant(value[0]) + value[1..];
        Debug.Log("Capitalized: " + capitalized);
        return capitalized;
    }
}
