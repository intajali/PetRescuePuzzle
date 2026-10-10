using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InternetChecker : MonoBehaviour
{
    public static InternetChecker Instance;
    public int internetCheckTime = 5;
    public int count = 0;

    public bool IsInternetConnected = false;
    public float t = 0;

    Coroutine routine = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            StartCoroutine(CheckingForInternet());
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public IEnumerator CheckingForInternet()
    {
        while (true)
        {
            StartCoroutine(CheckInternetConnection());
            yield return new WaitForSeconds(internetCheckTime);
        }

    }

    public IEnumerator CheckInternetConnection()
    {
        UnityWebRequest request = new UnityWebRequest("https://www.google.com");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            //callback(false);
            IsInternetConnected = false;
        }
        else
        {
            IsInternetConnected = true;
            //callback(true);
        }
    }

}
