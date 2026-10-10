using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RobotType
{
    TYPE_1 = 111, TYPE_2 = 222, TYPE_3 = 333, TYPE_4 = 444
}

public class RobotManager : MonoBehaviour
{
    private Dictionary<int,RobotType> robotTypes = new Dictionary<int,RobotType>();
    
    public RobotType currentRobotType;

    public void Start()
    {
       robotTypes.Add(111, RobotType.TYPE_1);
       robotTypes.Add(222, RobotType.TYPE_2);
       robotTypes.Add(333, RobotType.TYPE_3);
       robotTypes.Add(444, RobotType.TYPE_4);
    }
    public RobotType GetRobotType(int key)
    {
        return robotTypes[key];
    }

    public void UpdateRobotType(RobotType type)
    {
        currentRobotType = type;        
    }
}



