using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ClipDataContainer", menuName = "ScriptableObjects/Create AudioClipData")]
public class AudioClipDataContainer : ScriptableObject
{
    public List<ClipData> clipDatas;

    public ClipData GetClipByType(ClipTypes clipTypes)
    {
        return clipDatas.Where(t=> t.clipTypes == clipTypes).FirstOrDefault();
    }
}

[Serializable]
public class ClipData
{
    public string name;
    public AudioClip clip;
    public ClipTypes clipTypes;
}
