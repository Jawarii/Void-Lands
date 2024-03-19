using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
public class WeaponInfo : ItemInfo
{
    [Serializable]
    public struct WeaponMainStats
    {
        public int minAttack;
        public int maxAttack;
        public int critChance;
        public int critDamage;
        public int atkSpeed;
    }
    public WeaponMainStats weaponMainStats;

    [Serializable]
    public struct WeaponBonusStats
    {
        public int attack;
        public int critChance;
        public int critDamage;
        public int atkSpeed;    
    }
    public WeaponBonusStats weaponBonusStats;
}
