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
    public int itemLvl;
    public string itemQuality;

    //Special Visual Info
    public int currentStackSize = 1;
    public Sprite itemIcon;
    public Color textColor;

    //Weapon Special Info
    [Serializable]
    public struct WeaponMainStats
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
        public float critChance;
        public float critDamage;
        public float atkSpeed;
        public float staggerDmg;
    }

    public WeaponBonusStats weaponBonusStats;


    [Serializable]
    public struct GearMainStats
    {
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
}
