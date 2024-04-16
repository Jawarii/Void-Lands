using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClickLootBehaviour : MonoBehaviour
{
    public GameObject item;
    public GameObject inventory;
    public Button myButton;
    // Start is called before the first frame update
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
