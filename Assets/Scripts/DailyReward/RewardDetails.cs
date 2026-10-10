using UnityEngine;

[CreateAssetMenu(fileName = "DailyReward", menuName = "DailyReward/RewardDetails", order = 1)]
public class RewardDetails : ScriptableObject
{
    public int dayNumber;
    public Reward[] rewards; // List of rewards for this day

    [System.Serializable]
    public class Reward
    {
        public RewardType rewardType;
        public int rewardAmount;
        public Sprite rewardImage;
    }

    public enum RewardType
    {
        Coins,
        Diamonds,
        Gems,
        Boosts,
        Lives,
        GRENADE,
        LASER_RIFLE,
        LASER_PISTOL,
        BUBBLE_GUM
    }

    
}
