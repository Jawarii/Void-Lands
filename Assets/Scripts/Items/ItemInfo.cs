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
    public int itemLvl;
    public string itemQuality;

    public Color textColor;

    private void Awake()
    {
        itemType = gameObject.tag;
        SetTextColorBasedOnQuality();
    }

    private void SetTextColorBasedOnQuality()
    {
        string qualityNormalized = itemQuality.ToUpper(); // Convert to upper case for case-insensitive comparison

        switch (qualityNormalized)
        {
            case "MAGIC":
                ColorUtility.TryParseHtmlString("#7B7CF5", out textColor);
                break;
            case "RARE":
                ColorUtility.TryParseHtmlString("#EAEC0A", out textColor);
                break;
            case "LEGENDARY":
                ColorUtility.TryParseHtmlString("#F78005", out textColor);
                break;
            default: // Covers any other cases, including "NORMAL" or unexpected qualities
                ColorUtility.TryParseHtmlString("#FFFFFF", out textColor);
                break;
        }
        GetComponentInChildren<TMP_Text>().text = itemName;
        GetComponentInChildren<TMP_Text>().color = textColor;
    }
}
