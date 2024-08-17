using UnityEngine;
using static SaveData;

public class GameManager : MonoBehaviour
{
    public PlayerStats playerStats;
    public InventoryController inventoryController;

    void Awake()
    {
        LoadPlayerStats();
    }

    private void FixedUpdate()
    {
        SavePlayerStats();
    }

    private void SavePlayerStats()
    {
        SaveData.SavePlayerStats(playerStats, inventoryController);
    }

    private void LoadPlayerStats()
    {
        SaveDataWrapper data = SaveData.LoadPlayerStats();
        if (data == null)
            return;

        // Apply loaded data to PlayerStats
        playerStats.lvl = data.playerData.level;
        playerStats.currentExp= data.playerData.currentExp;
        playerStats.maxExp= data.playerData.maxExp;

        Vector3 position = new Vector3(data.playerData.position[0], data.playerData.position[1], data.playerData.position[2]);
        playerStats.transform.position = position;

        // Load Inventory
        inventoryController.LoadInventory(data.inventoryData);
    }
}
