using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInfoSO : ScriptableObject
{
    //Basic Info
    public int stackSize;
    public int itemId;
    public string itemName;
    public string itemDescription;
    public string itemType;

    //Special Visual Info
    public int currentStackSize = 1;
    public Sprite itemIcon;

    //Weapon Special Info
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
