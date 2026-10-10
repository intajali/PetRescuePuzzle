using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour
{
    public AvatarData avatarData;   // Reference to the AvatarData ScriptableObject
    public GameObject AvatarPanel;
    public Image profilePic;        // Reference to an Image component to display the profile pic

    // We no longer need 'selectedAvatarID' here, PlayerDataManager will manage it.
    //private int selectedAvatarID = -1;  // Store the ID of the selected avatar

    void Start()
    {
        // Load the saved avatar when the game starts
        //LoadSelectedAvatar();
        // Loading of selected avatar will be handled by PlayerDataManager.LoadPlayerDataFromFirebase.
        // This script's role is to update the profilePic based on PDM data and to set the ID in PDM when a new avatar is selected.
        // Call UpdateProfilePictureDisplay at startup to reflect current PDM data.
        UpdateProfilePictureDisplay(PlayerDataManager.Instance.GetSelectedAvatarID());
    }
/*    private void LoadSelectedAvatar()
    {
        if (PlayerPrefs.HasKey("SelectedAvatarID"))
        {
            selectedAvatarID = PlayerPrefs.GetInt("SelectedAvatarID");
            avatarData.avatarInfoList[selectedAvatarID].isSelected = true;

            UpdateProfilePicture(selectedAvatarID);
            Debug.Log("Loaded saved Avatar ID: " + selectedAvatarID);
        }
    }*/
    // This function will be triggered when the user clicks on an avatar button
    public void OnAvatarButtonPressed(int avatarIndex)
    {
        // IMPORTANT: Use PlayerDataManager to set the selected avatar ID
        PlayerDataManager.Instance.SetSelectedAvatarID(avatarIndex); // This sends to Firebase!

        // Update the profile picture displayed by this AvatarManager instantly
        UpdateProfilePictureDisplay(avatarIndex);

        // Optional: log the selection
        Debug.Log("Selected Avatar ID: " + avatarIndex);

        // Optionally hide the avatar selection panel after selection
        HideAvatarPanelAnimation();
    }

    // This method updates the profile picture UI managed by this AvatarManager (e.g., in the AvatarPanel)
    public void UpdateProfilePictureDisplay(int avatarIndex)
    {
        AvatarInfo selectedAvatar = avatarData.GetAvatarInfo(avatarIndex);
        if (selectedAvatar != null)
        {
            profilePic.sprite = selectedAvatar.avatarIcon;  // Set the profile picture to the selected avatar's icon
        }
        else
        {
            Debug.LogWarning($"AvatarInfo for index {avatarIndex} not found in AvatarData ScriptableObject.");
            // Optionally set a default sprite if avatar not found
        }
        // UIManager.Instance.mainMenu.UpdateProfileInfo(); // This will be called by SetSelectedAvatarID in PDM, avoid double-call
    }

    // This method updates the profile picture UI
    private void UpdateProfilePicture(int avatarIndex)
    {
        AvatarInfo selectedAvatar = avatarData.GetAvatarInfo(avatarIndex);
        if (selectedAvatar != null)
        {
            profilePic.sprite = selectedAvatar.avatarIcon;  // Set the profile picture to the selected avatar's icon
        }
        UIManager.Instance.mainMenu.UpdateProfileInfo();
    }

    public void HideAvatarPanelAnimation()
    {
        float fadeTimer = 0.5f;
        AvatarPanel.SetActive(true);
        CanvasGroup avatarCanvasGroup = AvatarPanel.GetComponent<CanvasGroup>();

        if (avatarCanvasGroup != null)
        {
            avatarCanvasGroup.DOFade(0f, fadeTimer).SetEase(Ease.InOutSine);
            AvatarPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, -1000f), fadeTimer).SetEase(Ease.InOutSine);
            StartCoroutine(DeactivatePanelAfterAnimation(fadeTimer));
        }
        else
        {
            Debug.LogError("AvatarPanel does not have a CanvasGroup component.");
        }
    }


    private IEnumerator DeactivatePanelAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

        AvatarPanel.SetActive(false);
    }
}
