using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class rotateAnimation : MonoBehaviour
{

    [Header("Rotation Animation")]
    [SerializeField] private Transform spriteToRotate;
    // Start is called before the first frame update

    private void Awake()
    {
        spriteToRotate = gameObject.GetComponent<Transform>();
    }
    void Start()
    {
        if (spriteToRotate != null)
        {
            spriteToRotate.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
        else
        {
            Debug.Log("Sprite Not Found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
