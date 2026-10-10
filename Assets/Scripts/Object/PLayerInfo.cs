using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{

    public static PlayerInfo Info;      // infomations of player

    public static Player MapPlayer;     // player object

    public static byte MODE;            // mode : Arcade or Classic 

    public static int BACKGROUND;       // background of mode

    public int Score;

    public const string KEY_CLASSIC_HISCORE = "classichightscore";

    public int currentScore;

    public TextMesh textlv;

  
    void Awake()
    {
        Debug.Assert(Info == null , "PlayerInfo object is NULL");
        if(Info == null)
        {
           Info = this;
        }
        BACKGROUND = MapPlayer.Background;

    }

    void Start()
    {
       // Score = 0;
        EffectSpawner.effect.SetLevel(MapPlayer.Level);
        EffectSpawner.effect.SetBest(MapPlayer.HightScore);
       // EffectSpawner.effect.SetScore(Score);
        textlv.text = MapPlayer.Level.ToString();
    }

    public void UpdatePlayerHighScore()
    {
        int score = GetHighScore();

        score += currentScore;

        PlayerPrefs.SetInt(KEY_CLASSIC_HISCORE,score);
        PlayerDataManager.Instance.SetHighScore(score);
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(KEY_CLASSIC_HISCORE, 0);
    }
}
