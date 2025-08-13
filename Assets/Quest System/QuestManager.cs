// [NO CHANGES TO USINGS OR CLASS DECLARATION]

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public List<QuestInstance> activeQuests = new();

    public event Action<string> OnQuestAcquired;
    public event Action<string> OnQuestProgressUpdated;
    public event Action<string> OnObjectiveCompleted;
    public event Action<string, int, int, Dictionary<ItemInfo, int>> OnQuestRewarded;

    public InventoryController inventoryController;
    public PlayerStats playerStats;

    public List<QuestSO> questDatabase = new(); // assign all quest assets here

    public QuestSO GetQuestAssetById(string questId)
    {
        for (int i = 0; i < questDatabase.Count; i++)
        {
            var so = questDatabase[i];
            if (so != null && so.questName == questId) return so;
        }

        // Optional fallback if you keep quests under Resources/Quests
        // var fromResources = Resources.Load<QuestSO>("Quests/" + questId);
        // return fromResources;

        return null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        inventoryController = FindObjectOfType<InventoryController>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    public void AddQuest(QuestSO newQuest)
    {
        if (HasQuest(newQuest.questName))
        {
            Debug.Log($"Quest '{newQuest.questName}' already active.");
            return;
        }

        QuestInstance instance = new QuestInstance { questData = newQuest };
        activeQuests.Add(instance);
        Debug.Log($"Quest added: {newQuest.questName}");

        OnQuestAcquired?.Invoke(newQuest.questName);
    }

    public bool HasQuest(string questName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questData.questName == questName)
                return true;
        }
        return false;
    }

    public bool IsObjectiveCompleted(string questName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questData.questName == questName)
                return quest.isComplete;
        }
        return false;
    }

    public bool IsRewarded(string questName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questData.questName == questName)
                return quest.isRewarded;
        }
        return false;
    }

    public QuestInstance GetQuestByName(string questName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questData.questName == questName)
                return quest;
        }
        return null;
    }

    public void NotifyKill(string tag)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.isComplete || quest.questData.questType != QuestType.Kill) continue;

            if (quest.questData.targetTag == tag)
            {
                quest.currentProgress++;

                if (quest.currentProgress >= quest.questData.targetAmount)
                {
                    quest.isComplete = true;
                    OnObjectiveCompleted?.Invoke(quest.questData.questName);
                    Debug.Log($"Kill quest objectives completed: {quest.questData.questName}");
                }
                else
                {
                    Debug.Log($"Progress: {quest.currentProgress}/{quest.questData.targetAmount}");
                    OnQuestProgressUpdated?.Invoke(quest.questData.questName);
                }
            }
        }
    }

    public void MarkObjectiveCompleteForTalkQuest(string questName)
    {
        var quest = GetQuestByName(questName);
        if (quest != null && quest.questData.questType == QuestType.Talk && !quest.isComplete)
        {
            quest.isComplete = true;
            OnObjectiveCompleted?.Invoke(questName);
            Debug.Log($"Talk quest completed: {questName}");
        }
    }

    public void CompleteQuest(string questName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questData.questName == questName && !quest.isRewarded)
            {
                quest.isComplete = true;
                quest.isRewarded = true;
                Debug.Log($"Quest fully rewarded: {questName}");

                int gold = quest.questData.goldReward;
                int exp = quest.questData.expReward;

                inventoryController.AddGold(gold);
                playerStats.IncreaseExp(exp);

                Dictionary<ItemInfo, int> itemRewards = new();

                foreach (var itemReward in quest.questData.itemRewards)
                {
                    for (int i = 0; i < itemReward.quantity; i++)
                    {
                        ItemInfo itemInfo;

                        if (itemReward.itemPrefab.CompareTag("UpgradeMaterial"))
                        {
                            itemInfo = itemReward.itemPrefab.GetComponent<ItemInfo>();
                        }
                        else
                        {
                            GameObject instancedObj = Instantiate(itemReward.itemPrefab);
                            itemInfo = instancedObj.GetComponent<ItemInfo>();
                            itemInfo.itemTier = itemReward.gearTier;
                            itemInfo.HandleStatsRoll();
                            Destroy(instancedObj);
                        }

                        inventoryController.AddItem(itemInfo);

                        if (!itemRewards.ContainsKey(itemInfo))
                            itemRewards[itemInfo] = 1;
                        else
                            itemRewards[itemInfo]++;
                    }
                }

                OnQuestRewarded?.Invoke(questName, gold, exp, itemRewards);
                return;
            }
        }
    }
}
