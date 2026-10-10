using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BottomBarController : MonoBehaviour
{
    public TextMeshProUGUI barText;
    public TextMeshProUGUI headingText;


    [SerializeField] private StoryScene currentScene;
    [SerializeField] private GameObject nextBtn;
    private int sentenceIndex = -1;
    private State state = State.COMPLETED;
    Coroutine routine = null;
    WaitForSeconds delay = new WaitForSeconds(0.06f);
    private int sceneCounter;

    private enum State
    {
        PLAYING, COMPLETED
    }
    // Start is called before the first frame update
    void Start()
    {

    }


    public void PlayScene(StoryScene scene)
    {
        currentScene = scene;
        sentenceIndex = 0;
        PlayNextSentance();
        nextBtn.SetActive(false);
    }

    public void PlayNextSentance()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        //StartCoroutine(TypeText(currentScene.sentences[++sentenceIndex].text));
      routine =  StartCoroutine(TypeText(currentScene.sentences[sentenceIndex].text));
        headingText.text = currentScene.sentences[sentenceIndex].heading.headingName;
        headingText.color = currentScene.sentences[sentenceIndex].heading.textColor;

        Debug.Log("Hello Sentence Index" + sentenceIndex);
        Debug.Log(currentScene.sentences[sentenceIndex].heading.textColor);
        Debug.Log(currentScene.sentences[sentenceIndex].heading.headingName);
    }

    public bool IsCompleted()
    {
        return 
            state == State.COMPLETED;
    }

    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;


    }

    private IEnumerator TypeText(string text)
    {
        barText.text = "";
        state = State.PLAYING;

        int wordIndex = 0;
        
        while (wordIndex < text.Length)
        {
            barText.text += text[wordIndex];

            wordIndex += 1;
            yield return delay;
        }
        Debug.Log("Loop END0000000000000");
        nextBtn.SetActive(true);
        Debug.Log("ShowNextButton", nextBtn);
        sceneCounter++;
        Debug.Log("SCENE COUNT"+ sceneCounter);
        state = State.COMPLETED;

        if (sceneCounter == 3)
        {
            nextBtn.SetActive(false);
            Debug.Log("HndeNextBun", nextBtn);
        }
    }

    public void nextbuttonHide()
    {
        nextBtn.SetActive(false);
    }

}
