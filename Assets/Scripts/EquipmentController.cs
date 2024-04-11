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
        // Empty prev equip
        prevEquipInfoSo = null;
    }
}
