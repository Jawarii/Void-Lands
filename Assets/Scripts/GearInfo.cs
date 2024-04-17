using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
public class GearInfo : ItemInfo
{
    [Serializable]
    public class GearMainStats
    {
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
        SetItemMainStats();
        SetItemBonusStats();
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
            () => gearBonusStats.hp = (int)((2 + itemLvl * 3f) * RandomRange(0.7f, 1.0f)),
            () => gearBonusStats.moveSpeed = (10 + itemLvl) * RandomRange(0.7f, 1.0f),
            () => gearBonusStats.armor = (int) ((1 + itemLvl/3f) * RandomRange(0.7f, 1.0f)),
            () => gearBonusStats.cdRed = (2 + itemLvl / 6f) * RandomRange(0.7f, 1.0f),
            () => gearBonusStats.recovery = (int)((1 + itemLvl / 5f) * RandomRange(0.7f, 1.0f))
        };
        }
        else if (itemType == "Gloves" || itemType == "Necklace" || itemType == "Ring")
        {
            bonusStatActions = new List<Action>()
        {
            () => gearBonusStats.attack = (int)((2 + itemLvl * 1.5f) * RandomRange(0.7f, 1.0f)),
            () => gearBonusStats.critChance = (5 + itemLvl / 2f) * RandomRange(0.7f, 1.0f),
            () => gearBonusStats.critDamage = (15 + itemLvl * 2f) * RandomRange(0.7f, 1.0f),
            () => gearBonusStats.atkSpeed = (7 + itemLvl / 1.2f) * RandomRange(0.7f, 1.0f),
            () => gearBonusStats.staggerDmg = (45 + itemLvl * 3f) * RandomRange(0.7f, 1.0f)
        };
        }
        else
        {
            bonusStatActions = new List<Action>()
        {
            () => gearBonusStats.hp = (int)((2 + itemLvl * 3f) * RandomRange(0.7f, 1.0f)),
            () => gearBonusStats.armor = (int) ((1 + itemLvl/3f) * RandomRange(0.7f, 1.0f)),
            () => gearBonusStats.cdRed = (2 + itemLvl / 6f) * RandomRange(0.7f, 1.0f),
            () => gearBonusStats.recovery = (int)((1 + itemLvl / 5f) * RandomRange(0.7f, 1.0f))
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
                gearMainStats.attack = itemLvl * 2;
                gearMainStats.hp = 0;
                gearMainStats.armor = 0;
                break;
            case "Ring":
                gearMainStats.attack = itemLvl * 2;
                gearMainStats.hp = 0;
                gearMainStats.armor = 0;
                break;
            default:
                gearMainStats.attack = 0;
                gearMainStats.hp = itemLvl * 10;
                gearMainStats.armor = itemLvl;
                break;
        }
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
}