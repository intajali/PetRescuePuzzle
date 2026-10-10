using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    public bool isSwitched = false;
    public Image backgroundImage;
    public Image backgroundImage2;
    public Image backgroundImage3;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchImage(Sprite sprite)
    {
        if (!isSwitched)
        {
            backgroundImage2.sprite = sprite;
            animator.SetTrigger("Switch1");
        }
        else
        {
            backgroundImage3.sprite = sprite;
            animator.SetTrigger("Switch2");
        }
        isSwitched = !isSwitched;
    }


    public void setImage(Sprite sprite)
    {
    if (!isSwitched)
        {
            backgroundImage3.sprite = sprite;
        }
    else 
        { 
            backgroundImage2.sprite = sprite;
        }
    }
}
