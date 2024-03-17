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
    public GameObject slotsPanel;
    void Start()
    {
        goldAmountText.text = goldAmount.ToString();
    }

    // Update is called once per frame
    void Update()
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
        if (item.stackSize > 1)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != null && inventory[i].itemId == item.itemId)
                {
                    inventory[i].currentStackSize++;
                    GameObject.Find("Slot (" + i + ")").GetComponentInChildren<TMP_Text>().text = inventory[i].currentStackSize.ToString();
                    (GameObject.Find("Slot (" + i + ")").transform.Find("ItemIcon")).gameObject.SetActive(true);
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
                            GameObject.Find("Slot (" + j + ")").GetComponentInChildren<TMP_Text>().text = inventory[j].currentStackSize.ToString();
                            (GameObject.Find("Slot (" + j + ")").transform.Find("ItemIcon")).gameObject.SetActive(true);
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
    }
}
