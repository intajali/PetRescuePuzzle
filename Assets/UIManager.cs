using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] public SettingPenal setiingPenal;
    [SerializeField] public LeaderBoard leaerboardPanel;
    [SerializeField] public PetsReleased petReleasedPanel;
    [SerializeField] public StorePenal storePanel;
    [SerializeField] public AboutUs aboutUsPanel;
    [SerializeField] public DailyReward dailyRewards;
    [SerializeField] public MainMenu mainMenu;
    [SerializeField] public ProfilePanel playerProfile;
    [SerializeField] public RegisterPanel registerPanel;
    //[SerializeField] public Button ShowleaerboarBtn;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
       // AddListeners();
    }

   /* private void AddListeners()
    {
        ShowleaerboarBtn.onClick.AddListener(leaderboardPanelShow);
    }




    public void leaderboardPanelShow()
    {
        Debug.Log("leaderboardPanelShow");
        leaerboardPanel.RenderView();
    }
    /* [SerializeField] Button backBtn;
     [SerializeField] Button dailyRewardBtn;
     [SerializeField] GameObject dailyRewardPanel;

     // Start is called before the first frame update
     void OnEnable()
     {
         dailyRewardBtn.onClick.AddListener(OnDailyRewardButtonClick);
         backBtn.onClick.AddListener(OnBackButtonClick);
     }

     // Update is called once per frame
     void Update()
     {

     }

     void OnDailyRewardButtonClick()
     {
         // Log a message for debugging
         Debug.Log("Daily reward button clicked!");

         // Toggle the visibility of the daily reward panel
         if (dailyRewardPanel != null)
         {
             dailyRewardPanel.SetActive(!dailyRewardPanel.activeSelf);
         }
     }

     void OnBackButtonClick()
     {
         dailyRewardPanel.SetActive(false);
     }

     private void OnDisable()
     {
         // Properly remove the listener when the UIManager is destroyed
         dailyRewardBtn.onClick.RemoveListener(OnDailyRewardButtonClick);
         dailyRewardBtn.onClick.RemoveListener(OnBackButtonClick);
     }
    */

}
