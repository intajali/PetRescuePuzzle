using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryUI : MonoBehaviour
{
    public Image profileImage; // Assign 'Profile_Image' here
    public TextMeshProUGUI profileNameText; // Assign 'Profile_Name' here
    public TextMeshProUGUI scoreText; // Assign 'Text (TMP)' under 'Score' here
    public TextMeshProUGUI rankText; // Assign 'Text (TMP)' under 'Rank' here (if it's text)
    public Image rankImage; // Assign the Image component for the rank badge if you use it

    void Awake()
    {
        // Basic checks for component assignment
        if (profileImage == null) Debug.LogWarning("Profile Image not assigned in LeaderboardEntryUI on prefab.");
        if (profileNameText == null) Debug.LogWarning("Profile Name Text not assigned in LeaderboardEntryUI on prefab.");
        if (scoreText == null) Debug.LogWarning("Score Text not assigned in LeaderboardEntryUI on prefab.");
        if (rankText == null && rankImage == null) Debug.LogWarning("Neither Rank Text nor Rank Image assigned in LeaderboardEntryUI on prefab. One should be.");
    }
}