using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics.Contracts;
using System.Linq;
using System;
using DG.Tweening;
using static Timer;


public class GamePlayHUD : MonoBehaviour
{
    public static GamePlayHUD Instance;

    public List<GoalItem> goalItems;
    public TextMeshProUGUI moveRemaining;
    public TextMeshProUGUI levelNumer;
    public TextMeshProUGUI textPlayerScore;

    public GameObject goalItemPrefab;
    public Transform goalItemParent;
    public Image levelProgress;

    public List<ProgressStar> starImages;

    private LevelInfo levelData;

    public List<Goal> goalList = new List<Goal>();
    public bool IsAllGoalCompleted = false;
    public int moveCount = 0;
    public int playerScore = 0;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Render(LevelInfo levelData)
    {
        if (levelData == null || levelData.petToCollect == null)
        {
            Debug.LogError("GamePlayHUD.Render called with invalid level data.");
            return;
        }

        goalList.Clear();
        foreach (var item in starImages)
        {
            item.Reset();
        }
        this.levelData = levelData;
        goalItems.ForEach(item => { item.gameObject.SetActive(false); });
        for (int i = 0; i < levelData.petToCollect.Count; i++)
        {
            goalItems[i].Render(levelData.petToCollect[i].jewelSprite, levelData.petToCollect[i].petCount);

            goalList.Add(new Goal(levelData.petToCollect[i].petCount, levelData.petToCollect[i].jewelType));
        }

        moveRemaining.text = levelData.moveCount.ToString();
        levelNumer.text = "Level " + PlayerInfo.MapPlayer.Level.ToString();
        GameController.action.moveCount = levelData.moveCount;
        moveCount = levelData.moveCount;
        //textPlayerScore.text = PlayerPrefs.GetInt(PlayerInfo.KEY_CLASSIC_HISCORE, 0).ToString();
        textPlayerScore.text = "0";

    }


    private void GenerateGoalItem()
    {
        GoalItem newGoalItem = Instantiate<GoalItem>(goalItemPrefab, goalItemParent);
    }

    private T Instantiate<T>(GameObject goalItemPrefab, Transform goalItemParent)
    {
        GameObject newItem = Instantiate(goalItemPrefab, goalItemParent);

        var goalItem = newItem.GetComponent<T>();

        return goalItem;
    }

    public void UpdatePetCollectedData(Jewel pet)
    {
        for (int i = 0; i < goalList.Count; i++)
        {
            if (pet.JewelType == goalList[i].type)
            {
                goalList[i].UpdateScore(1);
                goalItems[i].UpdatePetCollectedCount(1);
                UpdateLevelProgress();

                LabelsManager.instance.showLabelonProgress(); //Butterfly Animation on Target Tile Matched

            }
        }

        // Level Complete Check
        StartCoroutine(CheckForWin());

    }

    IEnumerator CheckForWin()
    {
        yield return new WaitForSeconds(1);

        IsAllGoalCompleted = goalList.All(t => t.IsComplete());

        if (IsAllGoalCompleted && GameController.action.drop.DELAY <= 0)
        {
            // Display Level Complete Popup.
            Timer.timer.Win();
        }
    }

    public bool IsPetMatched(Jewel pet)
    {
        var goal = goalList.Where(t => t.type == pet.JewelType).FirstOrDefault();
        return goal != null;

    }

    public LevelInfo GetLevelInfo()
    {
        return levelData;
    }

    public void UpdateRemaingMove()
    {
        if (moveCount > 0)
        {
            moveCount -= 1;
        }
        moveRemaining.text = moveCount.ToString();
        GameController.action.moveCount = moveCount;

    }

    public void NoMoreMove()
    {
        if (!IsAllGoalCompleted && moveCount <= 0)
        {
            Timer.timer.outOfMovesPopUp.Render();
        }
    }

    private void UpdateLevelProgress()
    {
        float totalRequired = goalList.Sum(g => g.target);
        float totalCollected = goalList.Sum(g => g.score);

        float progress = totalCollected / totalRequired;

        //Debug.Log("PROGRESS LEVEL" + progress + "--totalRequired-" + totalRequired + "--totalCollected-" + totalCollected);
        progress = Mathf.Clamp01(progress);
        levelProgress.DOFillAmount(progress, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (levelProgress.fillAmount >= 0.95f)
            {
                if (!starImages[2].IsAnimDone)
                    starImages[2].ActiveStar();
            }
            else if (levelProgress.fillAmount >= 0.6f)
            {
                if (!starImages[1].IsAnimDone)
                    starImages[1].ActiveStar();
            }
            else if (levelProgress.fillAmount >= 0.3f)
            {
                if (!starImages[0].IsAnimDone)
                    starImages[0].ActiveStar();
            }
        });


    }


    public void UpdatePlayerScore(int score, Vector3 pos)
    {
        if (GameController.action.GameState == (int)GameState.PLAYING)
        {
            PlayerInfo.Info.currentScore += score;
            playerScore += score;
            textPlayerScore.text = playerScore.ToString();

            EffectSpawner.effect.ScoreEff(score, pos);
        }
        if (playerScore < PlayerInfo.Info.currentScore)
        {
            StartCoroutine(ScoreCounter(score));
        }
    }

    WaitForSeconds scoreCounterWaitTime = new WaitForSeconds(0.1f);
    IEnumerator ScoreCounter(int score)
    {

        while (score > 0)
        {
            yield return scoreCounterWaitTime;
            playerScore += 1;
            score--;
        }
        textPlayerScore.text = playerScore.ToString();
    }

}

