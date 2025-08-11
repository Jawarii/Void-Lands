using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class
    GearInfo : ItemInfo
{
    [Serializable]
    public class GearMainStats
    {
        public int baseHp;
        public int baseArmor;
        public int baseAttack;
        public int hp;
        public int armor;
        public int attack;
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

    private void Start()
    {
        HandleStatsRoll();
    }

    void SetItemBonusStats()
    {
        List<Action> bonusStatActions = new List<Action>();
        string qualityNormalized = itemQuality.ToUpper();
        ResetBonusStats();

        if (itemType == "Boots")
        {
            bonusStatActions = new List<Action>()
            {
                () => gearBonusStats.hp = (int)((2 + itemLvl * 4.5f) * RandomRange(0.7f, 1.0f)),
                () => gearBonusStats.moveSpeed = (10 + itemLvl) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.armor = (int) ((2 + itemLvl / 1.5f) * RandomRange(0.7f, 1.0f)),
                () => gearBonusStats.cdRed = (3.2f + itemLvl * 0.264f) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.recovery = (int)((1.5f + itemLvl * 0.75f) * RandomRange(0.7f, 1.0f))
            };
        }
        else if (itemType == "Gloves" || itemType == "Ring")
        {
            bonusStatActions = new List<Action>()
            {
                () => gearBonusStats.attack = (int)((2 + itemLvl * 1.5f) * RandomRange(0.7f, 1.0f)),
                () => gearBonusStats.critChance = (3.5f + itemLvl * 0.35f) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.critDamage = (7.5f + itemLvl) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.atkSpeed = (5 + itemLvl * 0.55f) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.staggerDmg = (6 + itemLvl) * RandomRange(0.7f, 1.0f)
            };
        }
        else if (itemType == "Necklace")
        {
            bonusStatActions = new List<Action>()
            {
                () => gearBonusStats.attack = (int)((2 + itemLvl * 1.5f) * RandomRange(0.7f, 1.0f)),
                () => gearBonusStats.critChance = (3.5f + itemLvl * 0.35f) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.critDamage = (7.5f + itemLvl) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.atkSpeed = (5 + itemLvl * 0.55f) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.staggerDmg = (6 + itemLvl) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.moveSpeed = (10 + itemLvl) * RandomRange(0.7f, 1.0f)
            };
        }
        else
        {
            bonusStatActions = new List<Action>()
            {
                () => gearBonusStats.hp = (int)((2 + itemLvl * 4.5f) * RandomRange(0.7f, 1.0f)),
                () => gearBonusStats.armor = (int) ((2 + itemLvl / 1.5f) * RandomRange(0.7f, 1.0f)),
                () => gearBonusStats.cdRed = (3.2f + itemLvl * 0.264f) * RandomRange(0.7f, 1.0f),
                () => gearBonusStats.recovery = (int)((1.5f + itemLvl * 0.75f) * RandomRange(0.7f, 1.0f))
            };
        }
        int numberOfStats = qualityNormalized switch
        {
            "MAGIC" => 1,
            "RARE" => 2,
            "LEGENDARY" => 3,
            _ => 0
        };

        Shuffle(bonusStatActions); // Shuffle the actions to randomize which stats are applied

        for (int i = 0; i < numberOfStats; i++)
        {
            bonusStatActions[i](); // Apply the stat modification
        }
    }

    private void Shuffle(List<Action> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            Action value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private float RandomRange(float min, float max)
    {
        return (float)rand.NextDouble() * (max - min) + min;
    }

    void SetItemMainStats()
    {
        switch (itemType)
        {
            case "Necklace":
            case "Ring":
                gearMainStats.baseAttack = itemLvl * 2;
                gearMainStats.baseHp = 0;
                gearMainStats.baseArmor = 0;
                break;
            default:
                gearMainStats.baseAttack = 0;
                gearMainStats.baseHp = itemLvl * 7;
                gearMainStats.baseArmor = (int)(itemLvl * 1.5f);
                break;
        }

        // Initialize current stats with base stats
        gearMainStats.attack = gearMainStats.baseAttack;
        gearMainStats.hp = gearMainStats.baseHp;
        gearMainStats.armor = gearMainStats.baseArmor;
    }

    public void ResetBonusStats()
    {
        gearBonusStats.attack = 0;
        gearBonusStats.critChance = 0;
        gearBonusStats.critDamage = 0;
        gearBonusStats.atkSpeed = 0;
        gearBonusStats.staggerDmg = 0;
        gearBonusStats.hp = 0;
        gearBonusStats.armor = 0;
        gearBonusStats.cdRed = 0;
        gearBonusStats.moveSpeed = 0;
        gearBonusStats.recovery = 0;
    }

    public override void HandleStatsRoll()
    {
        itemLvl = itemTier * 5;
        SetItemMainStats();
        SetItemBonusStats();
    }
}
