using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AboutUs : MonoBehaviour
{
    [SerializeField] Button backBtn;

    [Header("DOTween Animation")]
    [SerializeField] private float fadeTimer = 1f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] List<GameObject> items = new List<GameObject>();

    private void OnEnable()
    { 
        backBtn.onClick.AddListener(OnBackButtonClick);
    }

    public void RenderView()
    {
        gameObject.SetActive(true);
        Debug.Log("Panel Status" + gameObject.activeSelf);
        PanelFadeIn();
    }

    public void PanelFadeIn(/*CanvasGroup canvasGroup, RectTransform rectTransform*/)
    {
        float panelMoveDuration = fadeTimer * 2f;
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
        UIManager.Instance.aboutUsPanel.RenderView();
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
}
