using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image buttonImage; // Assign the button's Image component
    public Sprite normalSprite; // Default button sprite
    public Sprite pressedSprite; // Button sprite without shadow

    private Vector3 originalScale;
    private Vector3 pressedScale;
    private Vector3 pressedPosition;
    private Vector3 originalPosition;
    public float pressAmount = 0.05f; // How much the button shrinks and moves down
    public float shrinkFactor = 0.9f; // Shrink height factor


    void Awake()
    {
        buttonImage = GetComponent<Image>(); // Auto-assign Image component
        if (buttonImage != null)
        {
            normalSprite = buttonImage.sprite; // Set default sprite automatically
        }
        else 
        {
            Debug.LogError("buttonImage is NULL");
        }
    }
    void Start()
    {
        originalScale = transform.localScale;
        pressedScale = new Vector3(originalScale.x, originalScale.y * shrinkFactor, originalScale.z); // Shrink only height (Y-axis)
        originalPosition = transform.position;
        pressedPosition = originalPosition + new Vector3(0, -pressAmount, 0); // Move down slightly
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = pressedScale;
        transform.position = pressedPosition;
        if (buttonImage && pressedSprite)
        {
            buttonImage.sprite = pressedSprite; // Change sprite to pressed version
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(ResetButton());
    }

    private IEnumerator ResetButton()
    {
        yield return new WaitForSeconds(0.1f); // Small delay before resetting
        transform.localScale = originalScale;
        transform.position = originalPosition;
        if (buttonImage && normalSprite)
        {
            buttonImage.sprite = normalSprite; // Revert back to normal sprite
        }
    }
}
