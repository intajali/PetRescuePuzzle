using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllPetReleasedPopup : MonoBehaviour
{
    public Button btnContinue;
    // Start is called before the first frame update
    public void Render()
    {
        btnContinue.onClick.RemoveAllListeners();
        btnContinue.onClick.AddListener(OnContinueClicked);

        gameObject.SetActive(true);
            
    }

    private void OnContinueClicked()
    {
        Application.Quit();
    }
}
