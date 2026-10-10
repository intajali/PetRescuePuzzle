/*
 * Created on 2023
 *
 * Copyright (c) 2023 Faiz Creations
 * Support : www.faizcreations.com
 */

//using Doozy.Engine.Themes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelsManager : MonoBehaviour
{

    public static LabelsManager instance;
    public GameObject labelPrefab;
    public string[] labelText;
    //public Image[] labelImage;
    public Sprite[] labelImage;

    Animator animator;

    public Text selectedText;
    public Image selectedImage;
    public float destroyTime;

    void Awake()
    {
        instance = this;
        animator = labelPrefab.GetComponent<Animator>();

    }

    private void Start()
    {
    }



    public void showLabelonProgress()
    {
       
        selectedText.text = labelText[Random.Range(0, 4)];
        selectedImage.sprite = labelImage[Random.Range(0, 3)];

       // SoundManager.instance.PlaySound_textLabels();
        animator.SetBool("showLabel", true);

        labelPrefab.SetActive(true);
        StartCoroutine(DeactivateLabelPrefab());
        //GameObject instance = Instantiate(labelPrefab, transform);
    }

    private IEnumerator DeactivateLabelPrefab() 
    {
        
        yield return new WaitForSeconds(destroyTime); // Wait for 2 seconds

        animator.SetBool("showLabel", false);

        StartCoroutine(closeAnimation());
    }

    private IEnumerator closeAnimation()
    {
        yield return new WaitForSeconds(1f); // Wait for 2 seconds

        labelPrefab.SetActive(false);

    }

    public void hideLabel()
    {

        labelPrefab.SetActive(false);

    }
}
