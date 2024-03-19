using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClickLootBehaviour : MonoBehaviour
{
    public GameObject item;
    public GameObject inventory;
    // Start is called before the first frame update
    void Start()
    {
        item = transform.gameObject;
        inventory = GameObject.Find("InventoryMain");
    }

    // Update is called once per frame
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
       inventory.GetComponent<InventoryController>().AddItem(itemInfo);
       Destroy(item);
    }
}
