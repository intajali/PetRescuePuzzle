using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryMenu : MonoBehaviour
{
    public static StoryMenu Instance;

    [SerializeField] public StoryScene_1 story_1;
    [SerializeField] public StoryScene_2 story_2;
    [SerializeField] public StoryScene_3 story_3;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
       
    }

    private void Start()
    {
        AudioManager.Instance.PlayBGMusic(ClipTypes.STORY);
    }
}
