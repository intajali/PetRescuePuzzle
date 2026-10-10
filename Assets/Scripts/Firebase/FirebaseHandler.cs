using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseHandler : MonoBehaviour
{
    public static FirebaseHandler Instance;
    private DatabaseReference dbRef;
    private FirebaseAuth auth;

    public FirebaseUser LoggedUser { get; private set; } = null;
    public DatabaseReference DbRef => dbRef;
    private bool firebaseInitialized = false;

    public bool IsTimeToSaveDataToFirebase = true;
    private bool guestLoginStarted = false;
    private bool restoreAuthStarted = false;
    private void Awake()
    {
        Debug.Log("Firebase Handler :: Awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Awake :: Else");
            Destroy(gameObject);
            return;
        }

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            try
            {
                Debug.Log("Dependency status: " + task.Result);
                var dependencyStatus = task.Result;
                
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    Exception appEx = null;
                    FirebaseApp firebaseApp = null;
                    try { firebaseApp = Firebase.FirebaseApp.DefaultInstance; }
                    catch (Exception ex) { appEx = ex; }
                   
                    Exception dbEx = null;
                    try { dbRef = FirebaseDatabase.DefaultInstance.RootReference; }
                    catch (Exception ex) { dbEx = ex; }
                 
                    Exception authEx = null;
                    try { auth = FirebaseAuth.DefaultInstance; }
                    catch (Exception ex) { authEx = ex; }
                    
                    if (auth == null)
                    {
                        firebaseInitialized = false;
                        Debug.LogError("FirebaseAuth.DefaultInstance is null. The Auth native library failed to load.");
                        return;
                    }

                    firebaseInitialized = true;
                    Debug.Log("Firebase initialized successfully.");


                    // MOVED THIS LINE INSIDE THE BLOCK
                    auth.StateChanged += Auth_StateChanged; // Line 43 (formerly Line 54)
                                                            Auth_StateChanged(this, EventArgs.Empty); // Manually check initial state
                                                            // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    Debug.LogError(String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            }
            catch (Exception ex)
            {
                Debug.Log("<color=red> Firebase Exception :: </color>" + ex.GetType().Name + " " + ex.Message + "\n" + ex.StackTrace);
            }
        });

        //auth.StateChanged += Auth_StateChanged;
    }

    private void Auth_StateChanged(object sender, EventArgs e)
    {
         Debug.Log("Sender :: " + sender.ToString());
         Debug.Log("Event Args :: " + e.ToString());

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.LoadLocalData();
        }

      
        if (!firebaseInitialized)
        {
            Debug.Log("<color=red> Firebase Not Initialized. :: </color>");
            Debug.Log("<color=blue> LOAD LOCAL DATA :: Internet no connected. :: </color>");
            return;
        }

        if (auth.CurrentUser != null)
        {
            // User is signed In 
            LoggedUser = auth.CurrentUser;
            Debug.LogFormat("User signed in: {0} (UID: {1})", LoggedUser.DisplayName, LoggedUser.UserId);
            // Trigger loading player data from Firebase
            PlayerDataManager.Instance.LoadPlayerData(LoggedUser.UserId);
        }
        else
        {
            Debug.Log("No user signed in. Waiting to restore persisted auth before guest login.");
            LoggedUser = null;
            if (!restoreAuthStarted)
            {
                restoreAuthStarted = true;
                StartCoroutine(RestoreAuthThenGuestLogin());
            }
        }

    }

    private IEnumerator RestoreAuthThenGuestLogin()
    {
        // Firebase Auth can report CurrentUser as null for a short time on mobile after launch.
        // Signing in anonymously immediately would create a NEW uid and look like a fresh level 1 player.
        for (int i = 0; i < 25; i++)
        {
            if (auth != null && auth.CurrentUser != null)
            {
                LoggedUser = auth.CurrentUser;
                Debug.LogFormat("Restored persisted user: {0}", LoggedUser.UserId);
                PlayerDataManager.Instance.LoadPlayerData(LoggedUser.UserId);
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("<color=blue>InternetChecker.Instance :: </color>" + InternetChecker.Instance);
        Debug.Log("<color=blue>InternetChecker.Instance.IsInternetConnected :: </color>" + (InternetChecker.Instance != null && InternetChecker.Instance.IsInternetConnected));
        if (InternetChecker.Instance != null && InternetChecker.Instance.IsInternetConnected)
        {
            Debug.Log("Attempting guest login :: LoginAsGuest()");
            LoginAsGuest();
        }
        else
        {
            Debug.Log("<color=blue>No Internet Found :: LOAD LOCAL DATA :: FireBaseHandler :: </color>");
            PlayerDataManager.Instance.LoadLocalData();
        }
    }

    /// <summary>
    /// Create anonymous user in firebase database
    /// </summary>
    public async void LoginAsGuest()
    {
        if (!firebaseInitialized)
        {
            Debug.LogWarning("Firebase not initialized yet. Cannot log in as guest.");
            return;
        }
        if (LoggedUser != null || (auth != null && auth.CurrentUser != null))
        {
            Debug.Log("Already logged in as a user, skipping guest login.");
            LoggedUser = LoggedUser ?? auth.CurrentUser;
            PlayerDataManager.Instance.LoadPlayerData(LoggedUser.UserId);
            return;
        }

        if (guestLoginStarted)
        {
            Debug.Log("Guest login already in progress, skipping duplicate request.");
            return;
        }

        guestLoginStarted = true;
        Debug.Log("Attempting to sign in anonymously...");

        await auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                guestLoginStarted = false;
                FirebaseException firebaseException = task.Exception.GetBaseException() as FirebaseException;
                Debug.LogError("LoginAsGuest :: ErrorCode :: " + (AuthError)firebaseException.ErrorCode);
                string expMessage = task.Exception.InnerExceptions[0].Message;
                Debug.LogError("LoginAsGuest encountered an error: " + expMessage);
                if (PlayerDataManager.Instance != null)
                {
                    PlayerDataManager.Instance.LoadLocalData();
                }
            }
            else if (task.IsCanceled)
            {
                guestLoginStarted = false;
                Debug.LogError("LoginAsGuest request is cancled: ");
            }
            else if (task.IsCompleted)
            {
                AuthResult result = task.Result;
                LoggedUser = result.User;

                Debug.LogFormat("Firebase Guest user created/signed in successfully: {0} (UID: {1})",
                                           LoggedUser.DisplayName ?? "Anonymous", LoggedUser.UserId);

                PlayerDataManager.Instance.LoadPlayerData(LoggedUser.UserId);
            }

        });
    }
    public int timeTick = 0;
    public IEnumerator ClockTimer()
    {
        while (true)
        {

            yield return new WaitForSeconds(1f);
            timeTick += 1;
            if (timeTick >= 60)
            {
                IsTimeToSaveDataToFirebase = true;
                timeTick = 0;
            }
        }
    }

    public IEnumerator CheckForDataSaveToFireBase(PlayerFirebaseData playerData)
    {
        if (IsTimeToSaveDataToFirebase)
        {
            yield return new WaitForSeconds(0.5f);
            SaveUserGameplayDataToFirebase(playerData);
            IsTimeToSaveDataToFirebase = false;
            timeTick = 0;

        }
    }

    /// <summary>
    /// Save User Data as a instance
    /// </summary>
    /// <param name="playerData"></param>
    public async void SaveUserGameplayDataToFirebase(PlayerFirebaseData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData is NULL");
            return;
        }

        if (LoggedUser == null)
        {
         
            Debug.LogError("LoggedUser is NULL");
            return;
        }

        string json = JsonConvert.SerializeObject(playerData);

        await DbRef.Child("users").Child(LoggedUser.UserId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"SavePlayerDataToFirebase: Failed to save data: {task.Exception}");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log($"Player data saved successfully for UID: {LoggedUser.UserId}");
                }
            });
    }

/// <summary>
/// Save Updated Username to Firebase
/// </summary>
/// <param name="username"></param>
    public async void SaveUpdatedUsernameToFirebase(string username)
    {
        if (LoggedUser == null)
        {
            Debug.LogError("LoggedUser is NULL");
            return;
        }
        
        string json = username;

        await dbRef.Child("users").Child(LoggedUser.UserId).Child("username").SetValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"SaveUsernameToFirebase: Failed to save data: {task.Exception}");
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"Username saved successfully for UID: {LoggedUser.UserId}");
            }
        });
    }

    /// <summary>
    /// Save Pets list data 
    /// </summary>
    /// <param name="userInfo"></param>
    public async void SavePetUnlockedData(Dictionary<string, PetFirebaseData> _pets)
    {
        if (_pets == null || _pets.Count == 0)
            return;

        if (LoggedUser == null)
        {
            Debug.LogError("LoggedUser is NULL");
            return;
        }

        string json = JsonConvert.SerializeObject(_pets);

        await dbRef.Child("users").Child(LoggedUser.UserId).Child("pets").SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"SavePlayerDataToFirebase: Failed to save data: {task.Exception}");
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"Player data saved successfully for UID: {LoggedUser.UserId}");
            }
        });
    }

    /// <summary>
    /// Save User Pet Information Data to firebase
    /// </summary>
    /// <param name="userPetInfo"></param>
    public async void SaveUserPetInfoToFirebase(UserPetInfo userPetInfo)
    {
        string json = JsonConvert.SerializeObject(userPetInfo);

        await dbRef.Child("users").Child(LoggedUser.UserId).Child("userInfo").Child("userPetInfo").SetRawJsonValueAsync(json).ContinueWithOnMainThread(tag =>
        {

        });
    }

    /// <summary>
    /// Update User Setting Data to firebase
    /// </summary>
    /// <param name="settingsData"></param>
    public async void SaveUserSettingsToFirebase(UserSettingsData settingsData)
    {
        string json = JsonConvert.SerializeObject(settingsData);

        await dbRef.Child("users").Child(LoggedUser.UserId).Child("settingsData").SetRawJsonValueAsync(json).ContinueWithOnMainThread(tag =>
        {

        });
    }

    /// <summary>
    /// Update User PowerUps data to firebase
    /// </summary>
    /// <param name="powerUps"></param>
    public async void SaveUserPowerUpsToFirebase(UserPowerUps powerUps)
    {
        string json = JsonConvert.SerializeObject(powerUps);

        await dbRef.Child("users").Child(LoggedUser.UserId).Child("powerUps").SetRawJsonValueAsync(json).ContinueWithOnMainThread(tag =>
        {

        });
    }

    /// <summary>
    /// Save User High Score to Firebase
    /// </summary>
    /// <param name="highScore"></param>
    public async void SaveUserHighScoreToFirebase(int highScore)
    {
        if (LoggedUser == null)
        {
            Debug.LogError("SaveUserHighScoreToFirebase() :: LoggedUser is NULL");
            return;
        }

        string json = highScore.ToString();

        await dbRef.Child("users").Child(LoggedUser.UserId).Child("highScore").SetValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Failed to push new score to leaderboard: {task.Exception}");
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"New score {highScore} to user == {LoggedUser.UserId}.");
            }
        });
    }

    /// <summary>
    /// Update Leaderboad data to firebase
    /// </summary>
    /// <param name="username"></param>
    /// <param name="score"></param>
    /// <param name="avatarID"></param>
    public void UpdateLeaderboardToFirebase(string username, int score, int avatarID)
    {
        if (dbRef == null)
        {
            Debug.LogError("Firebase Database reference is null. Cannot update leaderboard.");
            return;
        }

        if(LoggedUser == null)
        {
            Debug.LogError("LoggedUser is NULL");
            return;
        }

        var newScoreEntry = new Dictionary<string, object>
        {
            { "uid", LoggedUser.UserId },
            { "username", username },
            { "highScore", score },
            { "timestamp", DateTime.UtcNow.ToString("o") },
            { "selectedAvatarID", avatarID }// Current time for sorting
        };

        dbRef.Child("leaderboards").Child(LoggedUser.UserId).SetRawJsonValueAsync(JsonConvert.SerializeObject(newScoreEntry))
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Failed to push new score to leaderboard: {task.Exception}");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log($"New score {score} for {username} pushed to leaderboard.");
                }
            });
    }

    public void Logout()
    {
        if (auth != null)
        {
            auth.SignOut();
            LoggedUser = null;
            Debug.Log("User logged out.");
        }
    }

    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= Auth_StateChanged;
            auth = null;
        }
    }

}
