using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelDataContainer : ScriptableObject
{
    public List<LevelInfo> LevelInfoList;

    public int CustomLevelCount => LevelInfoList != null ? LevelInfoList.Count : 0;

    public bool IsProceduralLevel(int levelIndex) => levelIndex > CustomLevelCount;

    public LevelInfo GetLevelInfo(int levelIndex)
    {
        if (levelIndex <= 0 || LevelInfoList == null || LevelInfoList.Count == 0)
            return null;

        if (levelIndex <= LevelInfoList.Count)
            return LevelInfoList[levelIndex - 1];

        return ProceduralLevelGenerator.Generate(levelIndex, this);
    }
}

[Serializable]
public class LevelInfo
{
    public List<PetLevelInfo> petToCollect;
    public int moveCount;
    public int coinRewards;
    public int starsObtained;
}

[Serializable]
public class PetLevelInfo
{
    public Sprite jewelSprite;
    public int jewelType;
    public int petCount;
}
