using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelAnimatios : MonoBehaviour
{
    public static PanelAnimatios Instance;

    [Header("DOTween Animation")]
    [SerializeField] private float fadeTimer = 2f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
     
    }

    public void PanelFadeIn(/*CanvasGroup canvasGroup, RectTransform rectTransform*/)
    {
        float panelMoveDuration = fadeTimer * 1.5f;
        canvasGroup.alpha = 0f;
        rectTransform.transform.localPosition = new Vector3(0f, -1000f, 0f);
        rectTransform.DOAnchorPos(Vector2.zero, panelMoveDuration, false).SetEase(Ease.OutExpo);
        canvasGroup.DOFade(1, panelMoveDuration).SetEase(Ease.InOutSine);
        //StartCoroutine(ItemsAnimations());
    }

    public void PanelFadeOut(/*CanvasGroup canvasGroup, RectTransform rectTransform*/)
    {
        canvasGroup.alpha = 1f;
        rectTransform.transform.localPosition = new Vector3(0f, 0f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, -1000f), fadeTimer, false).SetEase(Ease.InOutQuint);
        canvasGroup.DOFade(0, fadeTimer);
    }

    IEnumerator ItemsAnimations()
    {
        foreach (var item in items)
        {
            item.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        }

        float delayBetweenItems = .05f;
        foreach (var item in items)
        {
            item.transform.DOScale(1f, fadeTimer).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(delayBetweenItems);
        }
    }
}
