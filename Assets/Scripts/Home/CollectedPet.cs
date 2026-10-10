
using UnityEngine;
using UnityEngine.UI;

public class CollectedPet : MonoBehaviour
{
    [SerializeField] private Image petImage;

    public void RenderView(Sprite petSprite)
    {
        petImage.sprite = petSprite;
    }
}
