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
        AssignReferences();
        LoadPlayerStats();
    }

    public void AssignReferences()
    {
        playerStats = GameObject.Find("PlayerArcher")?.GetComponent<PlayerStats>();
        inventoryController = GameObject.Find("InventoryMain")?.GetComponent<InventoryController>();
        equipmentSo = GameObject.Find("EquipmentPanel")?.GetComponent<EquipmentSoInformation>();
        skillBarInfo = GameObject.Find("SkillBar")?.GetComponent<SkillBarInfo>();

        if (playerStats == null || inventoryController == null || equipmentSo == null || skillBarInfo == null)
        {
            Debug.LogError("One or more references could not be assigned. Check if objects exist in the scene.");
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
            Debug.LogError("One or more references are null: playerStats, inventoryController, or equipmentController");
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

        Vector3 position = new Vector3(data.playerData.position[0], data.playerData.position[1], data.playerData.position[2]);
        Vector3 position2 = GameObject.Find("RespawnLocations").transform.GetChild(0).transform.position;
        if (position2 != null)
        {
            playerStats.transform.position = position2;
        }
        else
        {
            playerStats.transform.position = position;
        }

        // Load Inventory
        inventoryController.goldAmount = data.goldAmount;
        inventoryController.LoadInventory(data.inventoryData);
        equipmentSo.LoadAllEquipment(data.equipmentData);
        skillBarInfo.AddSkills(data.skillNames);
    }
}
