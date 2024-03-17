using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoSO : ScriptableObject
{
    public int stackSize;
    public int itemId;
    public string itemName;
    public int currentStackSize = 1;
    public Sprite itemIcon;
}
