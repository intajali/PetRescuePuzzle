using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class Loader : MonoBehaviour
{
    public static Loader Instance;

    public GameObject loadingIcon;
    public float rotationSpeed = 5;
    private float rotationOffet = 20;
    public bool AllowLoading = false;

    public GameObject loaderPrefab;

    // Start is called before the first frame update
    void Start()
    {
        loaderPrefab = this.gameObject;
        if (Instance != null)
        {
            Destroy(gameObject);
            
        }  
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        loaderPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (AllowLoading)
        {
            loadingIcon.transform.eulerAngles += new Vector3(0, 0, rotationOffet * rotationSpeed * Time.deltaTime);
        }
    }

    public void ActiveLoader()
    {
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
            AllowLoading = true ;
        }
        gameObject.SetActive(true);
    }

    public void HideLoader()
    {
        AllowLoading = false ;
        gameObject.SetActive(false);
    }

    public async void LoadingScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("Scene Progress is " + scene.progress.ToString());
        scene.allowSceneActivation = false;

        loaderPrefab.SetActive(true);

       //gameObject.SetActive(true);

       // ActiveLoader();

        await Task.Delay(100);

        scene.allowSceneActivation = true;
    }
}
