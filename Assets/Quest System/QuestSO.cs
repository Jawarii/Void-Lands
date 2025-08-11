using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class QuestSO : ScriptableObject
{
    public string questName;
    public QuestType questType;

    [TextArea(2, 5)]
    public string questDescription;

    public string targetTag;
    public int targetAmount;

    [Header("Rewards")]
    public int goldReward;
    public int expReward;

    [Header("Item Rewards")]
    public List<ItemPrefabReward> itemRewards;
}

[System.Serializable]
public class ItemPrefabReward
{
    public GameObject itemPrefab; // Your existing loot prefab (with ItemInfo assigned)
    public int quantity = 1;
    public int gearTier = 1;
}
