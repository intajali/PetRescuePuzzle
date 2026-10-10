using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class CheckForInternet 
{
    private int max_time = 5;

    public class InternetChecker : MonoBehaviour
    {
        public IEnumerator CheckInternetConnection(System.Action<bool> callback)
        {
            UnityWebRequest request = new UnityWebRequest("https://www.google.com");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                callback(false);
            }
            else
            {
                callback(true);
            }
        }
    }

}
