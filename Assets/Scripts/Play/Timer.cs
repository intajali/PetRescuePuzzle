using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{

    public static Timer timer;

    //thay doi game time => thay doi timerbarprocess
    public float GameTime = 270;

    public UnityEngine.UI.Image Timebar;

    public Texture2D timebarTexture;

    public TimerUpdate update;

    public GameObject NoSelect;

    public GameObject PauseUI;

    public LevelCompletePopUp levelCompletePopUp;
    public OutOfMovesPopUp outOfMovesPopUp;
    public QuitPopUp quitPopUp;
   public PetRescuedPopup petRescuedPopup;
    public UnityEngine.UI.Image petToUnlocked;

    public LevelLosePopuUp LoseUI;

    public GameObject Nomove;

    [SerializeField] private AllPetReleasedPopup allPetReleasedPanel;


    private float _time;

    private const int ClassicBaseScore = 500;

    private int ClassicTargetScore;

    public int ScoreStack = 0;

    private bool startplus;

    public bool isAds;

    public bool isreq;

    public enum GameState
    {
        PLAYING = 0,
        PAUSE = 1,
        WIN = 2,
        LOST = 3,
        NOMOREMOVE = 4,
        SETTING = 5,
        MAIN_MENU = 6,
        POWER_UP_INFO = 7,
    }

    void Awake()
    {
        timer = this;
    }
    void Start()
    {
        _time = GameTime;
        Timebar.fillAmount = 0;
        updatePetinChains();
        StartCoroutine(AdsCd());

    }

    public void TimeTick(bool b)
    {
        if (b && PlayerInfo.MODE == 1)
        {
            update.enabled = true;
        }
        else
        {
            update.enabled = false;
        }

    }

    public void updatePetinChains()
    {
        int currectPetId = PlayerDataManager.Instance.GetActivePetToUnlock();
        var petData = PlayerDataManager.Instance.petRescueDataContainer.GetPetInfo(currectPetId);
        if (petData != null)
        {
            petToUnlocked.sprite = petData.sadPet;
        }
    }

    void timebarprocess(float time)
    {
        float fillamount = time / _time;
        Timebar.fillAmount = fillamount;
    }

    public void ScoreBarProcess(int score)
    {
        ScoreStack += score;
        if (!startplus)
        {
            startplus = true;
            StartCoroutine(IEScoreBarProcess());
        }


    }
    IEnumerator IEScoreBarProcess()
    {
        while (ScoreStack > 0 && GameController.action.GameState == (int)GameState.PLAYING)
        {
            ScoreStack -= 10;
            if (PlayerInfo.Info.Score + 10 < 500 * PlayerInfo.MapPlayer.Level)
            { PlayerInfo.Info.Score += 10; }
            else
            {
                PlayerInfo.Info.Score = 500 * PlayerInfo.MapPlayer.Level;
                break;
            }
            float fillamount = PlayerInfo.Info.Score / (500f * PlayerInfo.MapPlayer.Level);
            Timebar.fillAmount = fillamount;
            yield return null;
        }

        startplus = false;
    }
    public void Tick()
    {
        if (GameTime > 0 && GameController.action.GameState == (int)GameState.PLAYING)
        {
            GameTime -= Time.deltaTime;
            timebarprocess(GameTime);
        }
        else if (GameController.action.GameState == (int)GameState.PLAYING)
        {
            GameController.action.GameState = (int)GameState.LOST;
            GameTime = 0;
            Lost();
            update.enabled = false;
        }
    }

    public void Win()
    {
        GameController.action.GameState = (int)GameState.WIN;
        NoSelect.SetActive(true);
        StartCoroutine(IEWin());
        Debug.Log("WIN");
    }
    public void Lost()
    {
        
        NoSelect.SetActive(true);
        StartCoroutine(DisableAll());
        AudioManager.Instance.PlayCommonClip(ClipTypes.LEVELFAILED);
        GameTime = 0;
        update.enabled = false;
        showFullAds();
        Debug.Log("LOSE");
        if (GameController.action.GameState == (int)Timer.GameState.PLAYING)
        {
            PlayerDataManager.Instance.livesSystem.RemoveLife();
        }

        GameController.action.GameState = (int)GameState.LOST;

    }
    public void Pause()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
        if (GameController.action.GameState == (int)GameState.PLAYING)
        {
            GameController.action.GameState = (int)GameState.PAUSE;
            NoSelect.SetActive(true);
            PauseUI.SetActive(true);
            Time.timeScale = 0;
        }

    }
    public void Resume()
    {
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
        if (GameController.action.GameState == (int)GameState.PAUSE)
        {
            GameController.action.GameState = (int)GameState.PLAYING;
            Time.timeScale = 1;
            NoSelect.SetActive(false);
            PauseUI.SetActive(false);
        }
    }
    public void Restart()
    {
        if (PlayerInfo.MODE == 1)
        {
            PlayerInfo.Info.Score = 0;
            ButtonActionController.Click.ArcadeScene(PlayerInfo.MapPlayer);
        }
        else
        {
            PlayerInfo.Info.Score = 0;
            ButtonActionController.Click.ClassicScene(PlayerInfo.MapPlayer.Level);
        }
    }


    /// <summary>
    /// Load Map Scene to select Map for Arcade Mode
    /// </summary>
    public void Home()
    {
        ButtonActionController.Click.SelectMap(PlayerInfo.MODE);
        PlayerDataManager.Instance.livesSystem.RemoveLife();
        Debug.Log("Quit Button Pressed");
    }

    public void showQuitConfirmPopup()
    {
        NoSelect.SetActive(false);
        PauseUI.SetActive(false);
        //quitPopUp.Render();
        AudioManager.Instance.PlayCommonClip(ClipTypes.BUTTON_CLICK);
        ButtonActionController.Click.HomeScene();
    }
    /// <summary>
    /// Load Next Level Map for Arcade Mode
    /// </summary>
    public void Next()
    {
        ButtonActionController.Click.SelectMap(1);
        if (PlayerInfo.MapPlayer.Level < 297)
            CameraMovement.StarPointMoveIndex = PlayerInfo.MapPlayer.Level;
        else
            CameraMovement.StarPointMoveIndex = -1;
    }

   
    public void ClassicLvUp()
    {
        GameController.action.GameState = (int)GameState.WIN;
        NoSelect.SetActive(true);
        StartCoroutine(UpLevel());

    }
    IEnumerator DisableAll()
    {
        DisableJewel(false);
        yield return new WaitForSeconds(0.75f);
        LoseUI.Render();
    }
    IEnumerator IEWin()
    {
        DisableJewel(false);
        // EffectSpawner.effect.StarWinEffect(GameController.action.JewelStar.gameObject.transform.position);
        AudioManager.Instance.PlayCommonClip(ClipTypes.LEVELCOMPLETE);
        // GameController.action.JewelStar.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        if (!levelCompletePopUp.gameObject.activeInHierarchy)
        {
            Debug.Log("Display Level Complete Popup");
            levelCompletePopUp.Display(PlayerInfo.MapPlayer.Level);
        }
        showFullAds();
    }

    void showFullAds()
    {
        if (isAds)
        {
//            GoogleMobileAdsScript.advertise.ShowInterstitial();
            isAds = false;
            isreq = false;
        }
    }

    IEnumerator UpLevel()
    {
        PlayerDataManager.Instance.UpdatePlayerLevel(PlayerInfo.MapPlayer.Level + 1);
        DisableJewel(true);
        showFullAds();
        yield return new WaitForSeconds(1f);
        ButtonActionController.Click.ClassicScene(PlayerInfo.MapPlayer.Level + 1);
    }

    public void DisableJewel(bool b)
    {
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (!b)
                {
                    if (JewelSpawner.spawn.JewelGribScript[x, y] != null)
                        JewelSpawner.spawn.JewelGribScript[x, y].JewelDisable();
                }
                else
                {
                    if (JewelSpawner.spawn.JewelGribScript[x, y] != null && JewelSpawner.spawn.JewelGribScript[x, y] != GameController.action.JewelStar)
                        JewelSpawner.spawn.JewelGribScript[x, y].JewelDisable();
                }
            }
        }
    }

    IEnumerator AdsCd()
    {
        while (true)
        {
            yield return new WaitForSeconds(119f);
            isAds = true;
        }
    }
}
