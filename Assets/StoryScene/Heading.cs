using UnityEngine;


[CreateAssetMenu(fileName ="NewHeading", menuName ="Data/New Heading")]
[System.Serializable]
public class Heading : ScriptableObject
{
    public string headingName;
    public Color textColor;
}
