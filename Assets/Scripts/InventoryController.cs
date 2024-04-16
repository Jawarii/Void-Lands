//using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public float goldAmount = 0;
    public TMP_Text goldAmountText;
    public List<ItemInfoSO> inventory = new List<ItemInfoSO>();
    public ItemInfoSO draggedItemInfo;
    public GameObject slotsPanel;
    void Start()
    {
        goldAmountText.text = goldAmount.ToString();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        goldAmountText.text = goldAmount.ToString();
    }

    public void AddItem(ItemInfo itemInfo)
    {
        ItemInfoSO item = ScriptableObject.CreateInstance<ItemInfoSO>();
        item.itemId = itemInfo.itemId;
        item.itemName = itemInfo.itemName;
        item.stackSize = itemInfo.stackSize;
        item.itemIcon = itemInfo.GetComponent<SpriteRenderer>().sprite;
        item.itemType= itemInfo.itemType;
        item.textColor= itemInfo.textColor;

        if (item.stackSize > 1)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != null && inventory[i].itemId == item.itemId)
                {
                    inventory[i].currentStackSize++;
                    (GameObject.Find("Slot (" + i + ")").transform.Find("ItemIcon")).gameObject.SetActive(true);
                    GameObject.Find("Slot (" + i + ")").GetComponentInChildren<TMP_Text>().text = inventory[i].currentStackSize.ToString();
                    (GameObject.Find("Slot (" + i + ")").transform.Find("ItemIcon")).GetComponent<Image>().sprite = item.itemIcon;
                    break;
                }
                if (i == inventory.Count - 1)
                {
                    for (int j = 0; j < inventory.Count; j++)
                    {
                        if (inventory[j] != null)
                        {
                            continue;
                        }
                        else
                        {
                            inventory[j] = item;
                            (GameObject.Find("Slot (" + j + ")").transform.Find("ItemIcon")).gameObject.SetActive(true);
                            GameObject.Find("Slot (" + j + ")").GetComponentInChildren<TMP_Text>().text = inventory[j].currentStackSize.ToString();
                            (GameObject.Find("Slot (" + j + ")").transform.Find("ItemIcon")).GetComponent<Image>().sprite = item.itemIcon;
                          
                            break;
                        }
                    }
                }    
            }
        }
        else
        {
            for (int j = 0; j < inventory.Count; j++)
            {
                if (inventory[j] != null)
                {
                    continue;
                }
                else
                {
                    inventory[j] = item;
                    //GameObject.Find("Slot (" + j + ")").GetComponentInChildren<TMP_Text>().text = inventory[j].currentStackSize.ToString();
                    (GameObject.Find("Slot (" + j + ")").transform.Find("ItemIcon")).gameObject.SetActive(true);
                    (GameObject.Find("Slot (" + j + ")").transform.Find("ItemIcon")).GetComponent<Image>().sprite = item.itemIcon;
                    break;
                }
            }
        }
        if (item.itemType!= null)
        {
            if (item.itemType == "Weapon")
            {
                WeaponInfo weaponInfo = itemInfo as WeaponInfo;

                //MainStats
                item.weaponMainStats.attack = weaponInfo.weaponMainStats.attack;
                item.weaponMainStats.minAttack = weaponInfo.weaponMainStats.minAttack;
                item.weaponMainStats.maxAttack = weaponInfo.weaponMainStats.maxAttack;

                //BonusStats
                item.weaponBonusStats.attack = weaponInfo.weaponBonusStats.attack;
                item.weaponBonusStats.atkSpeed = weaponInfo.weaponBonusStats.atkSpeed;
                item.weaponBonusStats.critChance = weaponInfo.weaponBonusStats.critChance;
                item.weaponBonusStats.critDamage = weaponInfo.weaponBonusStats.critDamage;
                item.weaponBonusStats.staggerDmg = weaponInfo.weaponBonusStats.staggerDmg;
            }
            if (item.itemType == "Armor" || item.itemType == "Helmet" || item.itemType == "Gloves" || item.itemType == "Boots" || item.itemType == "Belt")
            {
                GearInfo gearInfo = itemInfo as GearInfo;

                //MainStats
                item.gearMainStats.hp = gearInfo.gearMainStats.hp;
                item.gearMainStats.armor = gearInfo.gearMainStats.armor;

                //BonusStats
                item.gearBonusStats.attack = gearInfo.gearBonusStats.attack;
                item.gearBonusStats.atkSpeed = gearInfo.gearBonusStats.atkSpeed;
                item.gearBonusStats.critChance = gearInfo.gearBonusStats.critChance;
                item.gearBonusStats.critDamage = gearInfo.gearBonusStats.critDamage;
                item.gearBonusStats.staggerDmg = gearInfo.gearBonusStats.staggerDmg;
                item.gearBonusStats.hp = gearInfo.gearBonusStats.hp;
                item.gearBonusStats.armor = gearInfo.gearBonusStats.armor;
                item.gearBonusStats.cdRed = gearInfo.gearBonusStats.cdRed;
                item.gearBonusStats.moveSpeed = gearInfo.gearBonusStats.moveSpeed;
                item.gearBonusStats.recovery = gearInfo.gearBonusStats.recovery;
            }

        }
    }
}
