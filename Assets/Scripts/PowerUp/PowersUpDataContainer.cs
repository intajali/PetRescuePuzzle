using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowersUpData", menuName = "ScriptableObjects/PowersUpData", order = 3)]

public class PowersUpDataContainer : ScriptableObject
{
    public List<PowerUpDatModel> PowerUpDatModels;

    public void SaveData(PowerTypes powerType , PowerUpDatModel data)
    {
        PowerUpDatModel temp = PowerUpDatModels.Find(t => t.powerType == powerType);
        if (temp != null)
        {
            int itemIndex = PowerUpDatModels.IndexOf(temp);
            PowerUpDatModels[itemIndex] = new PowerUpDatModel()
            {
                powerType = powerType,
                cost = temp.cost,
                PowerAvailable = data.PowerAvailable,
                reward = temp.reward,
            };
        }
    }

    public PowerUpDatModel GetPowerUpData(PowerTypes powerType)
    {
        return PowerUpDatModels.Find(t => t.powerType == powerType);
    }
}

[Serializable]
public class PowerUpDatModel
{
    public PowerTypes powerType;
    public int cost;
    public int reward;
    public int PowerAvailable;
}
