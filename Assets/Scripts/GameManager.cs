using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveData;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerStats playerStats;
    public InventoryController inventoryController;
    public EquipmentSoInformation equipmentSo;
    public SkillBarInfo skillBarInfo;
    private float savingInterval = 0.2f;
    private float currentSavingInterval = 0.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //// Assign references when the game starts
        //AssignReferences();
        //LoadPlayerStats();
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BootstrapScene")
            return;
        AssignReferences();
        LoadPlayerStats();
        if (SaveDataWrapperLoaded())
            SaveData.ApplyCheckpointDataList(SaveData.GetCachedCheckpointData());

    }

    public void AssignReferences()
    {
        playerStats = GameObject.Find("PlayerArcher")?.GetComponent<PlayerStats>();
        inventoryController = GameObject.Find("InventoryMain")?.GetComponent<InventoryController>();
        equipmentSo = GameObject.Find("EquipmentPanel")?.GetComponent<EquipmentSoInformation>();
        skillBarInfo = GameObject.Find("SkillBar")?.GetComponent<SkillBarInfo>();

        if (playerStats == null || inventoryController == null || equipmentSo == null || skillBarInfo == null)
        {
            //Debug.LogError("One or more references could not be assigned. Check if objects exist in the scene.");
        }
    }

    private void FixedUpdate()
    {
        if (currentSavingInterval >= savingInterval)
        {
            SavePlayerStats();
            currentSavingInterval = 0;
        }
        else
        {
            currentSavingInterval += Time.deltaTime;
        }
    }

    private void SavePlayerStats()
    {
        if (playerStats == null || inventoryController == null || equipmentSo == null)
        {
            //Debug.LogError("One or more references are null: playerStats, inventoryController, or equipmentController");
            return;
        }

        SaveData.SavePlayerStats(playerStats, inventoryController, equipmentSo, skillBarInfo);
    }

    public void LoadPlayerStats()
    {
        SaveDataWrapper data = SaveData.LoadPlayerStats();
        if (data == null)
            return;

        // Apply loaded data to PlayerStats
        playerStats.lvl = data.playerData.level;
        playerStats.currentExp = data.playerData.currentExp;
        playerStats.maxExp = data.playerData.maxExp;

        // Load Inventory
        inventoryController.goldAmount = data.goldAmount;
        inventoryController.LoadInventory(data.inventoryData);
        equipmentSo.LoadAllEquipment(data.equipmentData);
        skillBarInfo.AddSkills(data.skillNames);

        Vector3 savedPosition = new Vector3(
            data.playerData.position[0],
            data.playerData.position[1],
            data.playerData.position[2]
        );

        // Apply checkpoint states
        SaveData.ApplyCheckpointDataList(data.checkpointDataPerScene);

        // Restore player position
        Vector3 respawnPosition = RespawnManager.Instance.GetClosestUnlockedCheckPoint(savedPosition);
        if (respawnPosition != Vector3.zero)
        {
            playerStats.transform.position = respawnPosition;
        }
        else
        {
            playerStats.transform.position = savedPosition;
        }

        // Apply active quest states (AFTER QuestManager is initialized)
        if (QuestManager.Instance != null && data.activeQuests != null)
        {
            SaveData.ApplyActiveQuestSaves(QuestManager.Instance, data.activeQuests);
        }
    }


}
