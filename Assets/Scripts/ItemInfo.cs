using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public int stackSize;
    public int itemId;
    public string itemName;
    public string itemDescription;
    public string itemType;

    private void Start()
    {
        itemType = gameObject.tag;
    }
}
