using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCollectionView : MonoBehaviour
{
    private List<CollectedPet> petsCollected = new List<CollectedPet>();

    public GameObject collectedPetItemObject;
    public Transform parentTransform;

    public void RenderView()
    {
        var allPetsData = PlayerDataManager.Instance.petRescueDataContainer.PetInfoList;
        for (int i = 0; i < allPetsData.Count; i++)
        {
            if (allPetsData[i].isUnlocked)
            {
                CollectedPet data = GenerateItem(allPetsData[i].petIndexID);
                data.RenderView(allPetsData[i].happyPet);
            }
        }

    }

    public CollectedPet GenerateItem(int index)
    {
        CollectedPet data = null;
        if (index <= petsCollected.Count)
        {
            data = petsCollected[index-1];
        }
        else
        {
            GameObject temp = Instantiate(collectedPetItemObject, parentTransform);
            data = temp.GetComponent<CollectedPet>();
            data.gameObject.SetActive(true);
            petsCollected.Add(data);
        }

        return data;
    }
}
