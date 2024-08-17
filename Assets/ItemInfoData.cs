using UnityEngine;

[System.Serializable]
public class ItemInfoData
{
    public int stackSize;
    public int itemId;
    public string itemName;
    public string itemDescription;
    public string itemType;
    public int itemLvl;
    public string itemQuality;
    public int upgradeLevel;
    public int currentStackSize;
    public byte[] itemIcon; // Serialized as bytes
    public ColorData textColor; // Use ColorData
    public ItemInfoSO.WeaponMainStats weaponMainStats;
    public ItemInfoSO.WeaponBonusStats weaponBonusStats;
    public ItemInfoSO.GearMainStats gearMainStats;
    public ItemInfoSO.GearBonusStats gearBonusStats;
    public ItemInfoData() { }
    public ItemInfoData(ItemInfoSO item)
    {
        stackSize = item.stackSize;
        itemId = item.itemId;
        itemName = item.itemName;
        itemDescription = item.itemDescription;
        itemType = item.itemType;
        itemLvl = item.itemLvl;
        itemQuality = item.itemQuality;
        upgradeLevel = item.upgradeLevel;
        currentStackSize = item.currentStackSize;
        itemIcon = SaveData.SpriteToByteArray(item.itemIcon); // Serialize sprite to bytes
        textColor = new ColorData(item.textColor); // Convert color to ColorData
        weaponMainStats = item.weaponMainStats;
        weaponBonusStats = item.weaponBonusStats;
        gearMainStats = item.gearMainStats;
        gearBonusStats = item.gearBonusStats;
    }
}
