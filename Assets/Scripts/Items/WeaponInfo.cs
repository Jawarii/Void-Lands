using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponInfo : ItemInfo
{
    [Serializable]
    public class WeaponMainStats
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
    private System.Random rand = new System.Random();

    void Start()
    {
        SetWeaponMainStats();
        SetWeaponBonusStats();
        SetAttackStatRange();
    }

    public void SetWeaponMainStats()
    {
        weaponMainStats.baseAttack = itemLvl * 6;
        weaponMainStats.attack = weaponMainStats.baseAttack;
    }

    public void SetWeaponBonusStats()
    {
        string qualityNormalized = itemQuality.ToUpper();

        // Initialize all stats to zero
        ResetBonusStats();

        List<Action> bonusStatActions = new List<Action>()
        {
            () => weaponBonusStats.attack = (int)((2 + itemLvl * 1.5f) * RandomRange(0.7f, 1.0f)),
            () => weaponBonusStats.critChance = (5 + itemLvl / 2f) * RandomRange(0.7f, 1.0f),
            () => weaponBonusStats.critDamage = (15 + itemLvl * 2f) * RandomRange(0.7f, 1.0f),
            () => weaponBonusStats.atkSpeed = (7 + itemLvl / 1.2f) * RandomRange(0.7f, 1.0f),
            () => weaponBonusStats.staggerDmg = (45 + itemLvl * 3f) * RandomRange(0.7f, 1.0f)
        };

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

    public void SetAttackStatRange()
    {
        weaponMainStats.minAttack = (int)(0.9f * weaponMainStats.attack);
        weaponMainStats.maxAttack = (int)(1.1f * weaponMainStats.attack);
    }

    public void ResetBonusStats()
    {
        weaponBonusStats.attack = 0;
        weaponBonusStats.critChance = 0;
        weaponBonusStats.critDamage = 0;
        weaponBonusStats.atkSpeed = 0;
        weaponBonusStats.staggerDmg = 0;
    }
}
