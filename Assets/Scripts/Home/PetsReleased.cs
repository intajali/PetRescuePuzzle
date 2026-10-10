using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;


public class PetsReleased : MonoBehaviour
{
    [SerializeField] Button backBtn;
    //[SerializeField] Button petReleasedBtn;
    //[SerializeField] GameObject petReleasedPenal;

    [Header("DOTween Animation")]
    [SerializeField] private float fadeTimer = 1f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] List<GameObject> items = new List<GameObject>();



    [Header("PetsReference")]
    [SerializeField] private List<PetPanelUI> petUIList;


    private void OnEnable()
    {
        //petReleasedBtn.onClick.AddListener(OnPetsReleasedButtonClick);
        backBtn.onClick.AddListener(OnBackButtonClick);
    }

    public void RenderView()
    {
        // AddListeners();
        gameObject.SetActive(true);
        Debug.Log("Panel Status" + gameObject.activeSelf);
        RenderPetStatus();
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

    void OnBackButtonClick()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
        UIManager.Instance.petReleasedPanel.RenderView();
        Disable();
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();
    }

    private void Disable()
    {
        gameObject.SetActive(false);
       PanelFadeOut();
    }

    /*    public void RenderPetStatus()
        {
            var petData = PlayerDataManager.Instance.petRescueDataContainer;

            for (int i = 0; i < petUIList.Count && i < petData.PetInfoList.Count; i++)
            {
                var petUI = petUIList[i];
                var petInfo = petData.PetInfoList[i];

                petUI.petImage.sprite = petInfo.isUnlocked ? petInfo.happyPet : petInfo.sadPet;
                // petUI.petNameText.text = petInfo.petName + petInfo.isUnlocked ? "Unlocked" : "Locked";
                petUI.petNameText.text = petInfo.petName + " " + (petInfo.isUnlocked ? "Unlocked" : "Locked");
                //petUI.statusText.text = petInfo.isUnlocked ? "Unlocked" : "Locked";
                petUI.chainIcon.gameObject.SetActive(!petInfo.isUnlocked);
                petUI.lockImage1.gameObject.SetActive(!petInfo.isUnlocked);
                petUI.lockImage2.gameObject.SetActive(!petInfo.isUnlocked);
            }
        }*/

    public void RenderPetStatus()
    {
        // Get all pet data from the PlayerDataManager's loaded Firebase data
        var allPetsData = PlayerDataManager.Instance.petRescueDataContainer.PetInfoList; // Use the SO for names and sprites

        for (int i = 0; i < petUIList.Count && i < allPetsData.Count; i++)
        {
            var petUI = petUIList[i];
            var petInfoSO = allPetsData[i]; // Info from ScriptableObject (fixed data)

            // Get THIS user's specific data for this pet from PlayerDataManager
            PetFirebaseData userPetData = PlayerDataManager.Instance.GetPetData(petInfoSO.petIndexID);

            if (userPetData != null)
            {
                // Use isUnlocked from user's Firebase data
                petUI.petImage.sprite = userPetData.isUnlocked ? petInfoSO.happyPet : petInfoSO.sadPet;
                petUI.petNameText.text = $"{petInfoSO.petName} {(userPetData.isUnlocked ? "Unlocked" : "Locked")}";
                petUI.chainIcon.gameObject.SetActive(!userPetData.isUnlocked);
                petUI.lockImage1.gameObject.SetActive(!userPetData.isUnlocked);
                petUI.lockImage2.gameObject.SetActive(!userPetData.isUnlocked);

                // You might want to display collected points here too
                // petUI.collectedPointsText.text = $"{userPetData.collectedPoints}/{userPetData.pointsToUnlock}";
            }
            else
            {
                Debug.LogWarning($"User data not found for pet ID: {petInfoSO.petIndexID}");
                // Default to locked state if no user data for this pet
                petUI.petImage.sprite = petInfoSO.sadPet;
                petUI.petNameText.text = $"{petInfoSO.petName} Locked";
                petUI.chainIcon.gameObject.SetActive(true);
                petUI.lockImage1.gameObject.SetActive(true);
                petUI.lockImage2.gameObject.SetActive(true);
            }
        }
    }


    /* void OnPetsReleasedButtonClick()
     {
         petReleasedPenal.SetActive(true);
     }



     private void OnDisable()
     {
         petReleasedBtn.onClick.RemoveListener(OnPetsReleasedButtonClick);
         backBtn.onClick.RemoveListener(OnBackButtonClick);
     }
    */
}

[System.Serializable]
public class PetPanelUI
{
    public Image petImage;
    public Image lockImage1;
    public Image lockImage2;
    public TextMeshProUGUI petNameText;
    //public TextMeshProUGUI statusText;
    public Image chainIcon;
    // public TextMeshProUGUI collectedPointsText; // If you add this to your prefab

}