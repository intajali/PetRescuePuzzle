using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StoryScene_2 : MonoBehaviour
{
    [SerializeField] Button nextBtn;

    [Header("DOTween Animation")]
    [SerializeField] private float fadeTimer = 1f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] List<GameObject> items = new List<GameObject>();

    public void RenderView()
    {
        gameObject.SetActive(true);
        Debug.Log("Panel Status " + gameObject.activeSelf);
        PanelFadeIn();
    }

    public void PanelFadeIn()
    {
        float panelMoveDuration = fadeTimer;
        canvasGroup.alpha = 1f;
        rectTransform.transform.localPosition = new Vector3(-1000f, 0f);
        rectTransform.DOAnchorPos(Vector2.zero, panelMoveDuration, false).SetEase(Ease.OutExpo);
        canvasGroup.DOFade(1, panelMoveDuration).SetEase(Ease.InOutSine);
        StartCoroutine(ItemsAnimations());
    }

    public void PanelFadeOut()
    {
        canvasGroup.DOFade(0, fadeTimer).SetEase(Ease.InOutSine);

        rectTransform.DOAnchorPos(new Vector2(0f, -1000f), fadeTimer, false).SetEase(Ease.InOutQuint);
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
            item.transform.DOScale(1f, fadeTimer).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(delayBetweenItems);
        }
    }

    private void OnEnable()
    {
        nextBtn.onClick.AddListener(OnNextBtnClick);
    }

    void OnNextBtnClick()
    {
        StoryMenu.Instance.story_3.RenderView();
        PanelFadeOut();
    }
    private void OnDisable()
    {
        nextBtn.onClick.RemoveAllListeners();
    }
 
}
