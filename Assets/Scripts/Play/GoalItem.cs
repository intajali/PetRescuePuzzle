
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalItem : MonoBehaviour
{
    public Image petToCollect;
    public TextMeshProUGUI petCount;
    public TextMeshProUGUI petCollectedCount;

    private int petCollected = 0;
    public void Render(Sprite petToCollect , int petCount)
    {
        petCollected = 0;
        this.petToCollect.sprite = petToCollect;
        this.petCount.text = "/" + petCount.ToString();
        petCollectedCount.text = "0";

        gameObject.SetActive(true);
    }

    public void UpdatePetCollectedCount(int petCollected)
    {
        this.petCollected += petCollected;
        petCollectedCount.text = this.petCollected.ToString();
    }
}
