using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static SkillButtonInformation;

public static class SaveData
{
    private static readonly string SavePath = Application.persistentDataPath + "/playerDataSave1.savetest";

    public static void SavePlayerStats(PlayerStats playerStats, InventoryController inventoryController, EquipmentSoInformation equipmentSo, SkillBarInfo skillBarInfo)
    {
        var formatter = new BinaryFormatter();

        using (var stream = new FileStream(SavePath, FileMode.Create))
        {
            var data = new SaveDataWrapper
            {
                playerData = new PlayerData(playerStats),
                inventoryData = CreateItemInfoDataList(inventoryController.inventory),
                equipmentData = CreateEquipmentDataList(equipmentSo.equipmentInfo),
                goldAmount = inventoryController.goldAmount,
                skillNames = CreateSkillNameList(skillBarInfo.skillButtonInfoList)
            };

            formatter.Serialize(stream, data);
        }
    }

    public static SaveDataWrapper LoadPlayerStats()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("Save file not found");
            return null;
        }

        var formatter = new BinaryFormatter();

        using (var stream = new FileStream(SavePath, FileMode.Open))
        {
            var data = formatter.Deserialize(stream) as SaveDataWrapper;

            if (data == null)
            {
                Debug.LogError("Failed to deserialize save data");
                return null;
            }

            // Assuming PlayerStats is a singleton or accessible via a global reference
            var playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            LoadBasicStats(playerStats, data.playerData);

            return data;
        }
    }

    private static List<ItemInfoData> CreateItemInfoDataList(IEnumerable<ItemInfoSO> items)
    {
        var itemDataList = new List<ItemInfoData>();

        foreach (var item in items)
        {
            if (item == null) continue;

            var itemInfoData = new ItemInfoData
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
                gearBonusStats = item.gearBonusStats,
                itemIcon = item.itemIcon != null ? SpriteToByteArray(item.itemIcon) : null
            };

            itemDataList.Add(itemInfoData);
        }

        return itemDataList;
    }

    private static List<ItemInfoData> CreateEquipmentDataList(IEnumerable<EquipmentController> equipmentInfo)
    {
        var equipDataList = new List<ItemInfoData>();

        foreach (var item in equipmentInfo)
        {
            if (item == null || item.equipInfoSo == null) continue;

            var equipInfoData = new ItemInfoData
            {
                stackSize = item.equipInfoSo.stackSize,
                itemId = item.equipInfoSo.itemId,
                itemName = item.equipInfoSo.itemName,
                itemDescription = item.equipInfoSo.itemDescription,
                itemType = item.equipInfoSo.itemType,
                itemLvl = item.equipInfoSo.itemLvl,
                itemQuality = item.equipInfoSo.itemQuality,
                upgradeLevel = item.equipInfoSo.upgradeLevel,
                currentStackSize = item.equipInfoSo.currentStackSize,
                textColor = new ColorData(item.equipInfoSo.textColor),
                weaponMainStats = item.equipInfoSo.weaponMainStats,
                weaponBonusStats = item.equipInfoSo.weaponBonusStats,
                gearMainStats = item.equipInfoSo.gearMainStats,
                gearBonusStats = item.equipInfoSo.gearBonusStats,
                itemIcon = item.equipInfoSo.itemIcon != null ? SpriteToByteArray(item.equipInfoSo.itemIcon) : null
            };

            equipDataList.Add(equipInfoData);
        }

        return equipDataList;
    }
    private static List<string> CreateSkillNameList(IEnumerable<SkillButtonInformation> skillButInfo)
    {
        var list = new List<string>();

        foreach (var skill in skillButInfo)
        {
            if (skill == null || skill.skillSo == null)
            { continue; }
            string skillName = skill.skillSo.skillName;
            list.Add(skillName);
        }
        return list;
    }
    private static void LoadBasicStats(PlayerStats playerStats, PlayerData playerData)
    {
        playerStats.lvl = playerData.level;
        playerStats.currentExp = playerData.currentExp;
        playerStats.maxExp = playerData.maxExp;
    }

    public static byte[] SpriteToByteArray(Sprite sprite)
    {
        return sprite.texture.EncodeToPNG();
    }

    public static Sprite ByteArrayToSprite(byte[] byteArray)
    {
        Texture2D texture = new Texture2D(2, 2);
        bool isLoaded = texture.LoadImage(byteArray);

        if (!isLoaded)
        {
            Debug.LogError("Failed to load texture from byte array.");
            return null;
        }

        // Set texture filter mode to Point (no filtering)
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        // Create the sprite from the texture
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }


    [System.Serializable]
    public class SaveDataWrapper
    {
        public PlayerData playerData;
        public List<ItemInfoData> inventoryData;
        public List<ItemInfoData> equipmentData;
        public float goldAmount;
        public List<string> skillNames;
    }
}
