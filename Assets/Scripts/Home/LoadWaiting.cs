using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LoadWaiting : MonoBehaviour
{
    public UnityEngine.UI.Image loadbar;    // Image loading fake

    public static LoadWaiting instance;


    public bool IsInternetConnected = false;

    [SerializeField] private float fillDuration = 3f;

    /// <summary>
    /// fill image by second and go to Home scene
    /// </summary>
    /// <returns></returns>
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (instance == null)
            instance = this;
        StartCoroutine(LoadingScneAsync());
    }

    IEnumerator LoadingScneAsync()
    {
        yield return null;

        loadbar.fillAmount = 0f;
        float elapsed = 0f;
        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            loadbar.fillAmount = Mathf.Clamp01(elapsed / fillDuration);
            yield return null;
        }
        loadbar.fillAmount = 1f;

        float timeout = 10f;
        float timer = 0f;
        while ((PlayerDataManager.Instance == null || !PlayerDataManager.Instance.IsDataLoaded)
               && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (PlayerDataManager.Instance != null)
        {
            int storyIndex = PlayerDataManager.Instance.GetStoryDone();
            if (storyIndex == 0)
            {
                SceneManager.LoadScene("StoryScene");
            }
            else
            {
                SceneManager.LoadScene("HomeScene");
            }
        }
        else
        {
            SceneManager.LoadScene("HomeScene");
        }
    }
}
