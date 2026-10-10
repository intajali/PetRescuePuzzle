using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonSelector : MonoBehaviour
{
    [SerializeField] RectTransform contentPanel;
    [SerializeField] GameObject buttonPrefab;
    public AvatarData avatarData; 
    public AvatarManager avatarManager;

    void Start()
    {
        PopulateScrollView();
    }


    void PopulateScrollView()
    {
        // Iterate through the avatar info list to create buttons dynamically
        for (int i = 0; i < avatarData.avatarInfoList.Count; i++) 
        {
            // Instantiate a new button from the prefab
            GameObject buttonObject = Instantiate(buttonPrefab, contentPanel);
            Button button = buttonObject.GetComponent<Button>();
            AvatarInfo avatar = avatarData.avatarInfoList[i];

            // Set the button's sprite and ID from AvatarData
            button.GetComponentInChildren<Image>().sprite = avatar.avatarIcon;
            //button.GetComponentInChildren<Text>().text = "Avatar " + avatar.avatarIndexID;

            // Add an onClick listener to the button to select it when clicked
            int avatarIndex = i; // This will be the button's index in the list
            button.onClick.AddListener(() => avatarManager.OnAvatarButtonPressed(avatarIndex));
        }
    }
}
