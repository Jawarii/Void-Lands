using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public int stackSize;
    public int itemId;
    public string itemName;
    public string itemDescription;
    public string itemType;
    public Color textColor;

    private void Start()
    {
        itemType = gameObject.tag;
        textColor = GetComponentInChildren<TMP_Text>().color;
    }
}
