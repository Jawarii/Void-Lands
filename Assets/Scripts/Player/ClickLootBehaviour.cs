using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickLootBehaviour : MonoBehaviour
{
    public GameObject item;
    public GameObject inventory;
    public Button myButton;

    void Start()
    {
        item = transform.gameObject;
        inventory = GameObject.Find("InventoryMain");
        myButton = item.GetComponentInChildren<Button>();

        if (myButton != null)
        {
            myButton.onClick.AddListener(OnClickButton);
        }
    }

    void Update()
    {
        if (inventory == null)
        {
            inventory = GameObject.Find("InventoryMain");
        }
    }

    public void LootItem()
    {
        ItemInfo itemInfo = item.GetComponent<ItemInfo>();
        InventoryController inventoryController = inventory.GetComponent<InventoryController>();

        if (!inventoryController.IsInventoryFull()) // Check if inventory is full
        {
            inventoryController.AddItem(itemInfo); // Add the item if inventory is not full
            Destroy(item); // Destroy the looted item
        }
        else
        {   
            Debug.Log("Inventory is full!"); // Optionally, notify the player that the inventory is full
        }
    }

    void OnClickButton()
    {
        LootItem();
    }

    void OnDestroy()
    {
        if (myButton != null)
        {
            myButton.onClick.RemoveListener(OnClickButton); // Clean up the listener
        }
    }
}
