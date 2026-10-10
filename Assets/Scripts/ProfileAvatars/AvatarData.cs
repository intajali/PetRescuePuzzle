using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AvatarData", menuName = "ScriptableObjects/AvatarData", order = 3)]
public class AvatarData : ScriptableObject
{
    public List<AvatarInfo> avatarInfoList;  // The list holding AvatarInfo objects

    public AvatarInfo GetAvatarInfo(int avatarIndex)
    {
        if (avatarIndex < avatarInfoList.Count)
        {
            return avatarInfoList[avatarIndex];
        }

        return null;  // Return null if the index is out of range
    }
}

    [Serializable]
public class AvatarInfo
{
    public int avatarIndexID;
    public bool isSelected;
    public Sprite avatarIcon;


}

