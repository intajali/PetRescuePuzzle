// LeaderBoard.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using TMPro; // Required for TextMeshProUGUI

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] Button backBtn;

    [Header("DOTween Animation")]
    [SerializeField] private float fadeTimer = 1f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] List<GameObject> items = new List<GameObject>();

    [Header("Leaderboard UI")]
    [SerializeField] private Transform leaderboardContentParent; // Parent transform for leaderboard entries
    [SerializeField] private GameObject leaderboardEntryPrefab; // Your 'UserData' prefab

    [Header("Rank Badges")]
    [SerializeField] private Sprite[] rankBadgeSprites; // Assign your rank badge sprites here (e.g., 0 for 1st, 1 for 2nd, etc.)

    public void RenderView()
    {
        gameObject.SetActive(true);
        Debug.Log("Panel Status " + gameObject.activeSelf);
        PanelFadeIn();
        FetchAndDisplayLeaderboard();
    }

    private void FetchAndDisplayLeaderboard()
    {
        if (FirebaseHandler.Instance.DbRef == null)
        {
            Debug.LogError("Firebase Database reference is null. Cannot fetch leaderboard.");
            return;
        }

        // Clear existing entries
        foreach (Transform child in leaderboardContentParent)
        {
            Destroy(child.gameObject);
        }

        FirebaseHandler.Instance.DbRef.Child("leaderboards").OrderByChild("highScore").LimitToLast(500).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Failed to fetch leaderboard: {task.Exception}");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

                if (snapshot.Exists && snapshot.Value != null)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        try
                        {
                            LeaderboardEntry entry = JsonConvert.DeserializeObject<LeaderboardEntry>(childSnapshot.GetRawJsonValue());
                            entries.Add(entry);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError($"Error deserializing leaderboard entry: {ex.Message}");
                        }
                    }
                }

                // Sort the entries by high score in descending order
                entries.Sort((a, b) => b.highScore.CompareTo(a.highScore));

                // Display only the top 15 entries
                for (int i = 0; i < Mathf.Min(entries.Count, 15); i++)
                {
                    GameObject entryGO = Instantiate(leaderboardEntryPrefab, leaderboardContentParent);
                    // --- THE ONLY CHANGE IS HERE ---
                    LeaderboardEntryUI uiElements = entryGO.GetComponentInChildren<LeaderboardEntryUI>(true); // Get component from children (include inactive)

                    if (uiElements != null)
                    {
                        // Set Rank
                        uiElements.rankText.text = (i + 1).ToString();
                        if (rankBadgeSprites != null && i < rankBadgeSprites.Length && uiElements.rankImage != null)
                        {
                            uiElements.rankImage.sprite = rankBadgeSprites[i];
                            uiElements.rankImage.gameObject.SetActive(true);
                            if (uiElements.rankText != null) uiElements.rankText.gameObject.SetActive(false);
                        }
                        else
                        {
                            if (uiElements.rankText != null) uiElements.rankText.gameObject.SetActive(true);
                            if (uiElements.rankImage != null) uiElements.rankImage.gameObject.SetActive(false);
                        }

                        // Set Username
                        uiElements.profileNameText.text = entries[i].username;

                        // Set Score
                        uiElements.scoreText.text = entries[i].highScore.ToString();

                        // Set Profile Image (Avatar)
                        if (uiElements.profileImage != null && PlayerDataManager.Instance.avatarData != null)
                        {
                            // Ensure selectedAvatarID is stored in LeaderboardEntry when pushed to Firebase!
                            // (as per previous recommendation in PlayerDataManager.UpdateLeaderboard)
                            // And ensure LeaderboardEntry class has 'public int selectedAvatarID;'
                            AvatarInfo avatar = PlayerDataManager.Instance.avatarData.GetAvatarInfo(entries[i].selectedAvatarID);
                            if (avatar != null)
                            {
                                uiElements.profileImage.sprite = avatar.avatarIcon;
                            }
                            else
                            {
                                Debug.LogWarning($"Avatar sprite not found for ID {entries[i].selectedAvatarID} for user {entries[i].username}.");
                                // Consider setting a default placeholder sprite here if needed
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("LeaderboardEntryUI component not found on prefab instance or its children. UI won't update for this entry.");
                    }
                }
                Debug.Log($"Leaderboard loaded with {Mathf.Min(entries.Count, 15)} entries displayed.");
            }
        });
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string uid;
        public string username;
        public int highScore;
        public string timestamp;
        public int selectedAvatarID; // Make sure this is added to PlayerDataManager.UpdateLeaderboard
    }

    // LeaderboardEntryUI class remains in its own file (LeaderboardEntryUI.cs)
    // Make sure it's public.
    // public class LeaderboardEntryUI : MonoBehaviour { ... }


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
        Disable();
        UIManager.Instance.mainMenu.gameObject.SetActive(true); // Return to main menu
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
}