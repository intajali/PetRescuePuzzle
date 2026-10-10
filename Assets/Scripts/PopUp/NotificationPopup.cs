using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class NotificationPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;

    public string SuccessText = "{0} Moves Added !!!";
    public string FailureText = "Not Enough Coins :(";

    private float panelStartPosX = -1500f;

    public void RenderView(bool isSuccess , int moveAdded ,string context, Action callback = null)
    {
        notificationText.text = isSuccess ? string.Format(context,moveAdded) : context;
        gameObject.SetActive(true);
        transform.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.InOutExpo)
            .OnComplete(() =>
            {
                callback?.Invoke();
                Invoke("DisableView",1f);
            });
    }

    public void DisableView()
    {
        gameObject.SetActive(false);
        transform.position = new Vector3 (panelStartPosX,0,0);
    }
}
