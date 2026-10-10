using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResolution : MonoBehaviour
{
    public Canvas canvas;
    public float referenceWidth = 1920f;
    public float referenceHeight = 1080f;

    void Start()
    {
        ScaleCanvas();
    }

    void ScaleCanvas()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }

        if (canvas != null)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(referenceWidth, referenceHeight);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f; // Adjust this value to prioritize width or height
            }
        }
    }
}
