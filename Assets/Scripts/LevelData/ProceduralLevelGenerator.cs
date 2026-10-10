using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates level goals for play beyond hand-authored ScriptableObject levels.
/// Uses late-game level entries as pet templates and scales difficulty by level number.
/// </summary>
public static class ProceduralLevelGenerator
{
    public const int ProceduralMapStart = 101;
    public const int ProceduralMapEnd = 297;

    public static LevelInfo Generate(int level, LevelDataContainer container)
    {
        int customLevelCount = container.LevelInfoList.Count;
        int difficulty = level - customLevelCount;
        var rng = new System.Random(level);

        LevelInfo template = container.LevelInfoList[customLevelCount - 1];
        int templatePetCount = template.petToCollect.Count > 0
            ? template.petToCollect[0].petCount
            : 35;

        int goalCount = difficulty > 20 ? 2 : rng.Next(0, 2) == 0 ? 1 : 2;
        goalCount = Mathf.Min(goalCount, 2);

        var petPool = BuildPetPool(container);
        var selectedPets = PickPets(petPool, goalCount, rng);

        int moveCount = template.moveCount + difficulty / 5;
        moveCount = Mathf.Clamp(moveCount, template.moveCount, 55);

        int petCount = templatePetCount + difficulty / 3;
        petCount = Mathf.Clamp(petCount, templatePetCount, 50);

        int coinRewards = template.coinRewards + difficulty;

        var levelInfo = new LevelInfo
        {
            moveCount = moveCount,
            coinRewards = coinRewards,
            starsObtained = 0,
            petToCollect = new List<PetLevelInfo>()
        };

        foreach (var pet in selectedPets)
        {
            levelInfo.petToCollect.Add(new PetLevelInfo
            {
                jewelSprite = pet.jewelSprite,
                jewelType = pet.jewelType,
                petCount = Mathf.Clamp(petCount + rng.Next(-3, 4), 20, 50)
            });
        }

        return levelInfo;
    }

    public static string GetMapName(int level, int customLevelCount)
    {
        if (level <= customLevelCount)
            return "classic";

        int mapPoolSize = ProceduralMapEnd - ProceduralMapStart + 1;
        int mapIndex = ProceduralMapStart + ((level - customLevelCount - 1) % mapPoolSize);
        return mapIndex.ToString();
    }

    static List<PetLevelInfo> BuildPetPool(LevelDataContainer container)
    {
        var pool = new List<PetLevelInfo>();
        var seenTypes = new HashSet<int>();

        int startIndex = Mathf.Max(0, container.LevelInfoList.Count / 2);
        for (int i = startIndex; i < container.LevelInfoList.Count; i++)
        {
            foreach (var pet in container.LevelInfoList[i].petToCollect)
            {
                if (pet.jewelSprite == null || seenTypes.Contains(pet.jewelType))
                    continue;

                seenTypes.Add(pet.jewelType);
                pool.Add(pet);
            }
        }

        return pool;
    }

    static List<PetLevelInfo> PickPets(List<PetLevelInfo> pool, int count, System.Random rng)
    {
        var shuffled = pool.OrderBy(_ => rng.Next()).ToList();
        var result = new List<PetLevelInfo>();
        var usedTypes = new HashSet<int>();

        foreach (var pet in shuffled)
        {
            if (!usedTypes.Add(pet.jewelType))
                continue;

            result.Add(pet);
            if (result.Count >= count)
                break;
        }

        if (result.Count == 0 && pool.Count > 0)
            result.Add(pool[rng.Next(pool.Count)]);

        return result;
    }
}
