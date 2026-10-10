using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetRescuedPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text petName;
    [SerializeField] private TMP_Text descrition;
    [SerializeField] private Image petImage;
    [SerializeField] private Button btnContinue;

    [SerializeField] private Button btnEndScreenContinue;

    [SerializeField] private CanvasGroup panelCanvasGroup;

    [SerializeField] private AllPetReleasedPopup allPetReleasedPanel;

    List<PetFirebaseData> petDataList;

    private void OnEnable()
    {
        btnContinue.onClick.AddListener(OnContinueButtonClicked);
        btnEndScreenContinue.onClick.AddListener(OnEndScreenContinue);
    }

    private void OnEndScreenContinue()
    {

    }

    private void OnContinueButtonClicked()
    {
        bool isAllUnlocked = petDataList.All(t => t.isUnlocked);
        if (isAllUnlocked)
        {
            // Display All Pets Unlocked.
            allPetReleasedPanel.Render();
        }

        this.gameObject.SetActive(false);
    }

    public void Render()
    {
        petDataList = PlayerDataManager.Instance.GetPetServerDataList();
        panelCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad).OnStart(() => panelCanvasGroup.gameObject.SetActive(true));
    }

    public void updatePetUI(Sprite sp, string name)
    {
        if (petImage != null)
        {
            petImage.sprite = sp;
        }
        petName.text = name;
        descrition.text = "You’ve rescued " + name + "! Thank you for saving them!";
    }

    private void OnDisable()
    {
        btnContinue.onClick.RemoveAllListeners();
        btnEndScreenContinue.onClick.RemoveAllListeners();

        panelCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => panelCanvasGroup.gameObject.SetActive(false));
    }
}
