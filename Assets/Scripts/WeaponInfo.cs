using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
public class WeaponInfo : ItemInfo
{
    [Serializable]
    public class WeaponMainStats
    {
        public int attack;
        public int minAttack;
        public int maxAttack;
    }

    public WeaponMainStats weaponMainStats;

    [Serializable]
    public struct WeaponBonusStats
    {
        public int attack;
        public int critChance;
        public int critDamage;
        public int atkSpeed;
        public int staggerDmg;
    }

    public WeaponBonusStats weaponBonusStats;

    void Awake()
    {
        SetAttackStatRange();
    }
    public void SetAttackStatRange()
    {
        // Calculate minAttack and maxAttack based on attack
        weaponMainStats.minAttack = (int)(0.9f * weaponMainStats.attack);
        weaponMainStats.maxAttack = (int)(1.1f * weaponMainStats.attack);
    }
}