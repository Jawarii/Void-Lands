using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveData
{
    public static void SavePlayerStats(PlayerStats playerStats, InventoryController inventoryController)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerData.savetest";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData playerData = new PlayerData(playerStats);
        List<ItemInfoData> itemDataList = new List<ItemInfoData>();

        foreach (var item in inventoryController.inventory)
        {
            if (item != null)
            {
                ItemInfoData itemInfoData = new ItemInfoData
                {
                    stackSize = item.stackSize,
                    itemId = item.itemId,
                    itemName = item.itemName,
                    itemDescription = item.itemDescription,
                    itemType = item.itemType,
                    itemLvl = item.itemLvl,
                    itemQuality = item.itemQuality,
                    upgradeLevel = item.upgradeLevel,
                    currentStackSize = item.currentStackSize,
                    textColor = new ColorData(item.textColor),
                    weaponMainStats = item.weaponMainStats,
                    weaponBonusStats = item.weaponBonusStats,
                    gearMainStats = item.gearMainStats,
                    gearBonusStats = item.gearBonusStats
                };

                // Convert sprite to byte array
                if (item.itemIcon != null)
                {
                    itemInfoData.itemIcon = SpriteToByteArray(item.itemIcon);
                }

                itemDataList.Add(itemInfoData);
            }
        }

        SaveDataWrapper data = new SaveDataWrapper
        {
            playerData = playerData,
            inventoryData = itemDataList
        };

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveDataWrapper LoadPlayerStats()
    {
        string path = Application.persistentDataPath + "/playerData.savetest";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveDataWrapper data = formatter.Deserialize(stream) as SaveDataWrapper;
            stream.Close();

            if (data != null)
            {
                // Assuming PlayerStats is a singleton or accessible via a global reference
                PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

                // Load the basic stats from the saved data
                playerStats.lvl = data.playerData.level;
                playerStats.currentExp = data.playerData.currentExp;
                playerStats.maxExp= data.playerData.maxExp;

                return data;
            }
            else
            {
                Debug.LogError("Failed to deserialize save data");
                return null;
            }
        }
        else
        {
            Debug.LogError("Save file was not found");
            return null;
        }
    }


    public static byte[] SpriteToByteArray(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        return texture.EncodeToPNG();
    }

    public static Sprite ByteArrayToSprite(byte[] bytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    [System.Serializable]
    public class SaveDataWrapper
    {
        public PlayerData playerData;
        public List<ItemInfoData> inventoryData;
    }
}
