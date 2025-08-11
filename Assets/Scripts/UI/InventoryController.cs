using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public float goldAmount = 0;
    public TMP_Text goldAmountText;
    public List<ItemInfoSO> inventory = new List<ItemInfoSO>(70); // Inventory holds up to 70 items
    public ItemInfoSO draggedItemInfo;
    public GameObject slotsPanel;

    void Start()
    {
        goldAmountText.text = goldAmount.ToString();
    }

    void FixedUpdate()
    {
        goldAmountText.text = goldAmount.ToString();
    }

    public void AddGold(float amount)
    {
        goldAmount += amount;
    }
    public void AddItem(ItemInfo itemInfo)
    {
        if (IsInventoryFull())
        {
            //Debug.Log("Inventory is full!");
            return;
        }

        ItemInfoSO item = ScriptableObject.CreateInstance<ItemInfoSO>();
        item.itemId = itemInfo.itemId;
        item.itemLvl = itemInfo.itemLvl;
        item.itemName = itemInfo.itemName;
        item.stackSize = itemInfo.stackSize;
        item.itemIcon = itemInfo.GetComponent<SpriteRenderer>().sprite;
        item.itemType = itemInfo.itemType;
        item.textColor = itemInfo.textColor;
        item.itemQuality = itemInfo.itemQuality;
        item.upgradeLevel = itemInfo.upgradeLevel;

        if (item.stackSize > 1)
        {
            EnchantmentStoneInfo enchantStoneInfo = itemInfo as EnchantmentStoneInfo;
            int amountToAdd = enchantStoneInfo.amount;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != null && inventory[i].itemId == item.itemId)
                {
                    inventory[i].currentStackSize += amountToAdd;
                    UpdateSlotUI(i, inventory[i]);
                    break;
                }
                if (i == inventory.Count - 1)
                {
                    for (int j = 0; j < inventory.Count; j++)
                    {
                        if (inventory[j] == null)
                        {
                            inventory[j] = item;
                            UpdateSlotUI(j, item);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            for (int j = 0; j < inventory.Count; j++)
            {
                if (inventory[j] == null)
                {
                    inventory[j] = item;
                    UpdateSlotUI(j, item);
                    break;
                }
            }
        }

        if (item.itemType != null)
        {
            if (item.itemType == "Weapon")
            {
                WeaponInfo weaponInfo = itemInfo as WeaponInfo;

                // MainStats
                item.weaponMainStats.baseAttack = weaponInfo.weaponMainStats.baseAttack;
                item.weaponMainStats.attack = weaponInfo.weaponMainStats.attack;
                item.weaponMainStats.minAttack = weaponInfo.weaponMainStats.minAttack;
                item.weaponMainStats.maxAttack = weaponInfo.weaponMainStats.maxAttack;

                // BonusStats
                item.weaponBonusStats.attack = weaponInfo.weaponBonusStats.attack;
                item.weaponBonusStats.atkSpeed = weaponInfo.weaponBonusStats.atkSpeed;
                item.weaponBonusStats.critChance = weaponInfo.weaponBonusStats.critChance;
                item.weaponBonusStats.critDamage = weaponInfo.weaponBonusStats.critDamage;
                item.weaponBonusStats.staggerDmg = weaponInfo.weaponBonusStats.staggerDmg;
            }
            if (item.itemType == "Armor" || item.itemType == "Helmet" ||
                item.itemType == "Gloves" || item.itemType == "Boots" ||
                item.itemType == "Belt" || item.itemType == "Necklace" || item.itemType == "Ring")
            {
                GearInfo gearInfo = itemInfo as GearInfo;

                // MainStats
                item.gearMainStats.baseHp = gearInfo.gearMainStats.baseHp;
                item.gearMainStats.baseArmor = gearInfo.gearMainStats.baseArmor;
                item.gearMainStats.baseAttack = gearInfo.gearMainStats.baseAttack;
                item.gearMainStats.hp = gearInfo.gearMainStats.hp;
                item.gearMainStats.armor = gearInfo.gearMainStats.armor;
                item.gearMainStats.attack = gearInfo.gearMainStats.attack;

                // BonusStats
                item.gearBonusStats.attack = gearInfo.gearBonusStats.attack;
                item.gearBonusStats.atkSpeed = gearInfo.gearBonusStats.atkSpeed;
                item.gearBonusStats.critChance = gearInfo.gearBonusStats.critChance;
                item.gearBonusStats.critDamage = gearInfo.gearBonusStats.critDamage;
                item.gearBonusStats.staggerDmg = gearInfo.gearBonusStats.staggerDmg;
                item.gearBonusStats.hp = gearInfo.gearBonusStats.hp;
                item.gearBonusStats.armor = gearInfo.gearBonusStats.armor;
                item.gearBonusStats.cdRed = gearInfo.gearBonusStats.cdRed;
                item.gearBonusStats.moveSpeed = gearInfo.gearBonusStats.moveSpeed;
                item.gearBonusStats.recovery = gearInfo.gearBonusStats.recovery;
            }
        }
    }

    public bool IsInventoryFull()
    {
        // Check if all slots from 0 to 69 are taken
        for (int i = 0; i < 70; i++)
        {
            if (inventory[i] == null)
            {
                return false; // There's at least one empty slot
            }
        }
        return true; // All slots are taken
    }

    public void LoadInventory(List<ItemInfoData> itemDataList)
    {
        foreach (var itemData in itemDataList)
        {
            ItemInfoSO item = ScriptableObject.CreateInstance<ItemInfoSO>();
            item.stackSize = itemData.stackSize;
            item.itemId = itemData.itemId;
            item.itemName = itemData.itemName;
            item.itemDescription = itemData.itemDescription;
            item.itemType = itemData.itemType;
            item.itemLvl = itemData.itemLvl;
            item.itemQuality = itemData.itemQuality;
            item.upgradeLevel = itemData.upgradeLevel;
            item.currentStackSize = itemData.currentStackSize;
            item.textColor = itemData.textColor.ToColor();

            if (itemData.itemIcon != null)
            {
                item.itemIcon = SaveData.ByteArrayToSprite(itemData.itemIcon);
            }

            item.weaponMainStats = itemData.weaponMainStats;
            item.weaponBonusStats = itemData.weaponBonusStats;
            item.gearMainStats = itemData.gearMainStats;
            item.gearBonusStats = itemData.gearBonusStats;

            bool itemAdded = false;

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null)
                {
                    if (itemData.stackSize > 1 && inventory.Any(slot => slot != null && slot.itemId == item.itemId))
                    {
                        itemAdded = true;
                        break;
                    }
                    else
                    {
                        inventory[i] = item;
                        UpdateSlotUI(i, item);
                        itemAdded = true;
                        break;
                    }
                }
                else if (inventory[i].itemId == item.itemId && itemData.stackSize > 1)
                {
                    inventory[i].currentStackSize += item.currentStackSize;
                    UpdateSlotUI(i, inventory[i]);
                    itemAdded = true;
                    break;
                }
            }

            if (!itemAdded && itemData.stackSize <= 1)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] == null)
                    {
                        inventory[i] = item;
                        UpdateSlotUI(i, item);
                        break;
                    }
                }
            }
        }
    }

    private void UpdateSlotUI(int index, ItemInfoSO item)
    {
        GameObject slot = GameObject.Find("Slot (" + index + ")");
        if (slot != null)
        {
            Transform iconTransform = slot.transform.GetChild(0).Find("ItemIconX");
            if (iconTransform != null)
            {
                iconTransform.parent.gameObject.SetActive(true);
                iconTransform.GetComponent<Image>().sprite = item.itemIcon;
            }

            TMP_Text stackText = slot.GetComponentInChildren<TMP_Text>();
            if (stackText != null)
            {
                stackText.gameObject.SetActive(item.stackSize > 1);
                stackText.text = item.currentStackSize.ToString();
            }
        }
    }
}
