using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PetRescueData", menuName = "ScriptableObjects/PetRescueData", order = 2)]
public class PetRescueDataContainer : ScriptableObject
{
    public List<PetInfo> PetInfoList;

    public PetInfo GetPetInfo(int petIndex)
    {
        if (petIndex > 0 && petIndex < PetInfoList.Count)
        {
            return PetInfoList[petIndex - 1];
        }

        return null;
    }

    public void SavePetRescuedData(int petIndex)
    {
        if (petIndex > 0 && petIndex < PetInfoList.Count)
        {
            PetInfoList[petIndex - 1].isUnlocked = true;
        }
    }
}

[Serializable]
public class PetInfo
{
    public int petIndexID;
    public bool isUnlocked;
    public int pointsToUnlock;
    public int collectedPoints;
    public string petName;
    public Sprite sadPet;
    public Sprite NormalPet;
    public Sprite happyPet;


}

