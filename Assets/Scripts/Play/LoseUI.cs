using UnityEngine;
using System.Collections;

public class LoseUI : MonoBehaviour
{
    public UnityEngine.UI.Text Score;   // current score

    public UnityEngine.UI.Text Best;    // best score

    private int playerScore;            // score tmp

    private int star;                   // star number

    void Start()
    {
        if (PlayerInfo.MODE != 1)
            playerScore = PlayerInfo.Info.Score + (PlayerInfo.MapPlayer.Level - 1) * 500;
        else
            playerScore = PlayerInfo.Info.Score;
        // display score text
        Score.text = playerScore.ToString();
        // display best score text
        Best.text = getBestScore(playerScore).ToString();

    }

    /// <summary>
    /// compare score with best score
    /// </summary>
    /// <param name="score">score by player</param>
    /// <returns>best score</returns>
    int getBestScore(int score)
    {
        if (PlayerInfo.MODE != 1)
        {
            if (score > PlayerInfo.MapPlayer.HightScore)
            {
                PlayerInfo.MapPlayer.HightScore = score;
                PlayerPrefs.SetInt(PlayerInfo.KEY_CLASSIC_HISCORE, PlayerInfo.MapPlayer.HightScore);
            }
        }
        return PlayerInfo.MapPlayer.HightScore;
    }

}
