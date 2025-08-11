using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static SkillButtonInformation;

public static class SaveData
{
    private static readonly string SavePath = Application.persistentDataPath + "/playerDataSave1.savetest";
    private static SaveDataWrapper cachedData;

    [System.Serializable]
    public class CheckpointData
    {
        public int indexInParent;
        public bool isUnlockedByDefault;
        public bool isInvisible;
        public bool isUnlocked;
    }

    public static void SavePlayerStats(PlayerStats playerStats, InventoryController inventoryController, EquipmentSoInformation equipmentSo, SkillBarInfo skillBarInfo)
    {
        var formatter = new BinaryFormatter();

        using (var stream = new FileStream(SavePath, FileMode.Create))
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            List<CheckpointData> currentSceneCheckpoints = CreateCheckpointDataList();

            if (cachedData != null)
            {
                cachedData.playerData = new PlayerData(playerStats);
                cachedData.inventoryData = CreateItemInfoDataList(inventoryController.inventory);
                cachedData.equipmentData = CreateEquipmentDataList(equipmentSo.equipmentInfo);
                cachedData.goldAmount = inventoryController.goldAmount;
                cachedData.skillNames = CreateSkillNameList(skillBarInfo.skillButtonInfoList);
                cachedData.sceneName = currentScene;

                if (cachedData.checkpointDataPerScene == null)
                    cachedData.checkpointDataPerScene = new Dictionary<string, List<CheckpointData>>();

                cachedData.checkpointDataPerScene[currentScene] = currentSceneCheckpoints;

                formatter.Serialize(stream, cachedData);
            }
            else
            {
                var newData = new SaveDataWrapper
                {
                    playerData = new PlayerData(playerStats),
                    inventoryData = CreateItemInfoDataList(inventoryController.inventory),
                    equipmentData = CreateEquipmentDataList(equipmentSo.equipmentInfo),
                    goldAmount = inventoryController.goldAmount,
                    skillNames = CreateSkillNameList(skillBarInfo.skillButtonInfoList),
                    sceneName = currentScene,
                    checkpointDataPerScene = new Dictionary<string, List<CheckpointData>>
                    {
                        { currentScene, currentSceneCheckpoints }
                    }
                };

                cachedData = newData;
                formatter.Serialize(stream, newData);
            }
        }
    }

    public static SaveDataWrapper LoadPlayerStats()
    {
        if (!File.Exists(SavePath))
            return null;

        var formatter = new BinaryFormatter();

        using (var stream = new FileStream(SavePath, FileMode.Open))
        {
            var data = formatter.Deserialize(stream) as SaveDataWrapper;

            if (data == null)
                return null;

            cachedData = data;

            var playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            LoadBasicStats(playerStats, data.playerData);

            return data;
        }
    }

    private static List<CheckpointData> CreateCheckpointDataList()
    {
        var list = new List<CheckpointData>();
        GameObject parent = GameObject.Find("RespawnLocations");
        if (parent == null) return list;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var cp = parent.transform.GetChild(i).GetComponent<CheckPoint>();
            if (cp == null) continue;

            list.Add(new CheckpointData
            {
                indexInParent = i,
                isUnlockedByDefault = cp.isUnlockedByDefault,
                isInvisible = cp.isInvisible,
                isUnlocked = cp.IsUnlocked
            });
        }

        return list;
    }

    public static void ApplyCheckpointDataList(Dictionary<string, List<CheckpointData>> dataPerScene)
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!dataPerScene.ContainsKey(sceneName)) return;

        var dataList = dataPerScene[sceneName];
        GameObject parent = GameObject.Find("RespawnLocations");
        if (parent == null) return;

        foreach (var data in dataList)
        {
            if (data.indexInParent >= parent.transform.childCount) continue;

            var cp = parent.transform.GetChild(data.indexInParent).GetComponent<CheckPoint>();
            if (cp == null) continue;

            cp.isUnlockedByDefault = data.isUnlockedByDefault;
            cp.isInvisible = data.isInvisible;
            if (data.isUnlocked)
                cp.Unlock();
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
            if (skill == null || skill.skillSo == null) continue;
            list.Add(skill.skillSo.skillName);
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
        if (!texture.LoadImage(byteArray)) return null;

        texture.filterMode = FilterMode.Point;
        texture.Apply();

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
        public Dictionary<string, List<CheckpointData>> checkpointDataPerScene;
        public string sceneName;
    }

    public static bool SaveDataWrapperLoaded()
    {
        return cachedData != null && cachedData.checkpointDataPerScene != null;
    }

    public static Dictionary<string, List<CheckpointData>> GetCachedCheckpointData()
    {
        return cachedData?.checkpointDataPerScene;
    }

}
