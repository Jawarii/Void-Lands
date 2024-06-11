using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInfoSO : ScriptableObject
{
    // Basic Info
    public int stackSize;
    public int itemId;
    public string itemName;
    public string itemDescription;
    public string itemType;
    public int itemLvl;
    public string itemQuality;
    public int upgradeLevel;

    // Special Visual Info
    public int currentStackSize = 1;
    public Sprite itemIcon;
    public Color textColor;

    // Weapon Special Info
    [Serializable]
    public struct WeaponMainStats
    {
        public int baseAttack;
        public int attack;
        public int minAttack;
        public int maxAttack;
    }

    public WeaponMainStats weaponMainStats;

    [Serializable]
    public struct WeaponBonusStats
    {
        public int attack;
        public float critChance;
        public float critDamage;
        public float atkSpeed;
        public float staggerDmg;
    }

    public WeaponBonusStats weaponBonusStats;

    // Gear Special Info
    [Serializable]
    public struct GearMainStats
    {
        public int baseAttack;
        public int baseHp;
        public int baseArmor;
        public int attack;
        public int hp;
        public int armor;
    }

    public GearMainStats gearMainStats;

    [Serializable]
    public struct GearBonusStats
    {
        public int attack;
        public float critChance;
        public float critDamage;
        public float atkSpeed;
        public float staggerDmg;
        public int hp;
        public int armor;
        public float cdRed;
        public float moveSpeed;
        public int recovery;
    }

    public GearBonusStats gearBonusStats;

    private System.Random rand = new System.Random();

    public void UpgradeWeapon()
    {
        if (upgradeLevel < 9)
        {
            upgradeLevel++;
            float upgradeMultiplier = 1 + (0.05f * upgradeLevel);

            weaponMainStats.attack = Mathf.RoundToInt(weaponMainStats.baseAttack * upgradeMultiplier);
            weaponMainStats.minAttack = Mathf.RoundToInt(weaponMainStats.attack * 0.9f);
            weaponMainStats.maxAttack = Mathf.RoundToInt(weaponMainStats.attack * 1.1f);

            UpdateItemName();
        }
        else
        {
            Debug.Log("Weapon has reached the maximum upgrade level.");
        }
    }

    public void UpgradeGear()
    {
        if (upgradeLevel < 9)
        {
            upgradeLevel++;
            float upgradeMultiplier = 1 + (0.05f * upgradeLevel);

            gearMainStats.attack = Mathf.RoundToInt(gearMainStats.baseAttack * upgradeMultiplier);
            gearMainStats.hp = Mathf.RoundToInt(gearMainStats.baseHp * upgradeMultiplier);
            gearMainStats.armor = Mathf.RoundToInt(gearMainStats.baseArmor * upgradeMultiplier);

            UpdateItemName();
        }
        else
        {
            Debug.Log("Gear has reached the maximum upgrade level.");
        }
    }

    private void UpdateItemName()
    {
        itemName = itemName.StartsWith("+") ? itemName.Substring(itemName.IndexOf(" ") + 1) : itemName;
        itemName = $"+{upgradeLevel} {itemName}";
    }
}
