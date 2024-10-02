using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentController : MonoBehaviour
{
    public ItemInfoSO equipInfoSo;
    private ItemInfoSO prevEquipInfoSo;
    public string slotType;
    public GameObject player;
    public PlayerStats playerStats;
    public Color rarityColorInfo;
    public int slotId;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }
    private void Start()
    {
        if (equipInfoSo != null)
            rarityColorInfo = equipInfoSo.textColor;

        if (rarityColorInfo != null)
        {
            try
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().color = rarityColorInfo;
            }
            catch { }
        }
    }

    public void AddStats()
    {
        if (equipInfoSo == null)
        {
            Debug.LogError("equipInfoSo is null");
            return;
        }

        if (equipInfoSo.itemType == "Weapon")
        {
            if (equipInfoSo.weaponMainStats == null || equipInfoSo.weaponBonusStats == null)
            {
                Debug.LogError("Weapon stats are null");
                return;
            }

            // Main Stats Application
            playerStats.attack += equipInfoSo.weaponMainStats.attack;
            // Bonus Stats Application
            playerStats.attack += equipInfoSo.weaponBonusStats.attack;
            playerStats.atkSpd += equipInfoSo.weaponBonusStats.atkSpeed / 100f;
            playerStats.critRate += equipInfoSo.weaponBonusStats.critChance;
            playerStats.critDmg += equipInfoSo.weaponBonusStats.critDamage;
            playerStats.staggerDmg += equipInfoSo.weaponBonusStats.staggerDmg;
        }
        else if (equipInfoSo.itemType == "Armor" || equipInfoSo.itemType == "Helmet" ||
                 equipInfoSo.itemType == "Boots" || equipInfoSo.itemType == "Gloves" ||
                 equipInfoSo.itemType == "Belt" || equipInfoSo.itemType == "Necklace" || equipInfoSo.itemType == "Ring")
        {
            if (equipInfoSo.gearMainStats == null || equipInfoSo.gearBonusStats == null)
            {
                Debug.LogError("Gear stats are null");
                return;
            }

            // Main Stats Application
            playerStats.maxHp += equipInfoSo.gearMainStats.hp;
            playerStats.currentHp += equipInfoSo.gearMainStats.hp;
            playerStats.defense += equipInfoSo.gearMainStats.armor;
            playerStats.attack += equipInfoSo.gearMainStats.attack;
            // Bonus Stats Application
            playerStats.attack += equipInfoSo.gearBonusStats.attack;
            playerStats.atkSpd += equipInfoSo.gearBonusStats.atkSpeed / 100f;
            playerStats.critRate += equipInfoSo.gearBonusStats.critChance;
            playerStats.critDmg += equipInfoSo.gearBonusStats.critDamage;
            playerStats.staggerDmg += equipInfoSo.gearBonusStats.staggerDmg;

            playerStats.maxHp += equipInfoSo.gearBonusStats.hp;
            playerStats.currentHp += equipInfoSo.gearBonusStats.hp;
            playerStats.defense += equipInfoSo.gearBonusStats.armor;
            playerStats.cdReduction += equipInfoSo.gearBonusStats.cdRed;
            playerStats.speed += equipInfoSo.gearBonusStats.moveSpeed / 100f;
            playerStats.hpRecovery += equipInfoSo.gearBonusStats.recovery;
        }

        // Set prevEquipInfoSo
        prevEquipInfoSo = equipInfoSo;
    }


    public void RemoveStats()
    {
        if (prevEquipInfoSo == null)
            return;

        if (prevEquipInfoSo.itemType == "Weapon")
        {
            // Main Stats Removal
            playerStats.attack -= prevEquipInfoSo.weaponMainStats.attack;
            // Bonus Stats Removal
            playerStats.attack -= prevEquipInfoSo.weaponBonusStats.attack;
            playerStats.atkSpd -= prevEquipInfoSo.weaponBonusStats.atkSpeed / 100f;
            playerStats.critRate -= prevEquipInfoSo.weaponBonusStats.critChance;
            playerStats.critDmg -= prevEquipInfoSo.weaponBonusStats.critDamage;
            playerStats.staggerDmg -= prevEquipInfoSo.weaponBonusStats.staggerDmg;
        }
        else if (prevEquipInfoSo.itemType == "Armor" || prevEquipInfoSo.itemType == "Helmet" ||
                 prevEquipInfoSo.itemType == "Boots" || prevEquipInfoSo.itemType == "Gloves" ||
                 prevEquipInfoSo.itemType == "Belt" || prevEquipInfoSo.itemType == "Necklace" || prevEquipInfoSo.itemType == "Ring")
        {
            // Main Stats Removal
            playerStats.maxHp -= prevEquipInfoSo.gearMainStats.hp;
            playerStats.currentHp -= prevEquipInfoSo.gearMainStats.hp;
            playerStats.defense -= prevEquipInfoSo.gearMainStats.armor;
            playerStats.attack -= prevEquipInfoSo.gearMainStats.attack;
            // Bonus Stats Removal
            playerStats.attack -= prevEquipInfoSo.gearBonusStats.attack;
            playerStats.atkSpd -= prevEquipInfoSo.gearBonusStats.atkSpeed / 100f;
            playerStats.critRate -= prevEquipInfoSo.gearBonusStats.critChance;
            playerStats.critDmg -= prevEquipInfoSo.gearBonusStats.critDamage;
            playerStats.staggerDmg -= prevEquipInfoSo.gearBonusStats.staggerDmg;

            playerStats.maxHp -= prevEquipInfoSo.gearBonusStats.hp;
            playerStats.currentHp -= prevEquipInfoSo.gearBonusStats.hp;
            playerStats.defense -= prevEquipInfoSo.gearBonusStats.armor;
            playerStats.cdReduction -= prevEquipInfoSo.gearBonusStats.cdRed;
            playerStats.speed -= prevEquipInfoSo.gearBonusStats.moveSpeed / 100f;
            playerStats.hpRecovery -= prevEquipInfoSo.gearBonusStats.recovery;
        }
        // Empty prev equip
        prevEquipInfoSo = null;
    }

    public void LoadEquipment(ItemInfoData equipInfoData)
    {
        Debug.Log("REACHED LOADING");
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        if (equipInfoData != null)
        {
            Sprite itemIcon = ByteArrayToSprite(equipInfoData.itemIcon);
            equipInfoSo = new ItemInfoSO
            {
                stackSize = equipInfoData.stackSize,
                itemId = equipInfoData.itemId,
                itemName = equipInfoData.itemName,
                itemDescription = equipInfoData.itemDescription,
                itemType = equipInfoData.itemType,
                itemLvl = equipInfoData.itemLvl,
                itemQuality = equipInfoData.itemQuality,
                upgradeLevel = equipInfoData.upgradeLevel,
                currentStackSize = equipInfoData.currentStackSize,
                textColor = equipInfoData.textColor.ToColor(), // Assuming ColorData has ToColor method
                weaponMainStats = equipInfoData.weaponMainStats,
                weaponBonusStats = equipInfoData.weaponBonusStats,
                gearMainStats = equipInfoData.gearMainStats,
                gearBonusStats = equipInfoData.gearBonusStats,
                itemIcon = itemIcon
            };
        }
        EquipItem(equipInfoSo);
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

    public void EquipItem(ItemInfoSO itemToEquip)
    {
        if (itemToEquip == null)
            return;
        SetItemIcon(itemToEquip.itemIcon); // Set the item icon in the UI
        //RemoveStats();
        AddStats(); // Apply the item's stats
        Debug.Log("Item equipped: " + itemToEquip.itemName);
    }
    public void SetItemIcon(Sprite icon)
    {
        Image iconImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.transform.parent.gameObject.SetActive(icon != null);
        }
    }

    private void FixedUpdate()
    {

    }
}
