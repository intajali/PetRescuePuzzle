using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BreathingAnimation : MonoBehaviour
{
    [Header ("Breathing Animation")]
    [SerializeField] private float scale = 1.1f;
    [SerializeField] private float duration = 1f;


    void Start()
    {

    }

    private void Awake()
    {
        BreathingAnim();
    }

    void BreathingAnim()
    {
        transform.DOScale(scale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
