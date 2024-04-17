using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public ItemInfoSO equipInfoSo;
    private ItemInfoSO prevEquipInfoSo;
    public string slotType;
    public GameObject player;
    public PlayerStats playerStats;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }

    public void AddStats()
    {
        if (equipInfoSo == null)
            return;
        if (equipInfoSo.itemType == "Weapon")
        {
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
            // Main Stats Application
            playerStats.maxHp += equipInfoSo.gearMainStats.hp;
            playerStats.defense += equipInfoSo.gearMainStats.armor;
            playerStats.attack += equipInfoSo.gearMainStats.attack;
            // Bonus Stats Application
            playerStats.attack += equipInfoSo.gearBonusStats.attack;
            playerStats.atkSpd += equipInfoSo.gearBonusStats.atkSpeed / 100f;
            playerStats.critRate += equipInfoSo.gearBonusStats.critChance;
            playerStats.critDmg += equipInfoSo.gearBonusStats.critDamage;
            playerStats.staggerDmg += equipInfoSo.gearBonusStats.staggerDmg;

            playerStats.maxHp += equipInfoSo.gearBonusStats.hp;
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
            // Main Stats Application
            playerStats.maxHp -= prevEquipInfoSo.gearMainStats.hp;
            playerStats.defense -= prevEquipInfoSo.gearMainStats.armor;
            playerStats.attack -= prevEquipInfoSo.gearMainStats.attack;
            // Bonus Stats Application
            playerStats.attack -= prevEquipInfoSo.gearBonusStats.attack;
            playerStats.atkSpd -= prevEquipInfoSo.gearBonusStats.atkSpeed / 100f;
            playerStats.critRate -= prevEquipInfoSo.gearBonusStats.critChance;
            playerStats.critDmg -= prevEquipInfoSo.gearBonusStats.critDamage;
            playerStats.staggerDmg -= prevEquipInfoSo.gearBonusStats.staggerDmg;

            playerStats.maxHp -= prevEquipInfoSo.gearBonusStats.hp;
            playerStats.defense -= prevEquipInfoSo.gearBonusStats.armor;
            playerStats.cdReduction -= prevEquipInfoSo.gearBonusStats.cdRed;
            playerStats.speed -= prevEquipInfoSo.gearBonusStats.moveSpeed / 100f;
            playerStats.hpRecovery -= prevEquipInfoSo.gearBonusStats.recovery;
        }
        // Empty prev equip
        prevEquipInfoSo = null;
    }
}
