//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class InventoryData
//{
//    public float goldAmount;
//    public List<ItemInfoData> items;

//    public InventoryData(InventoryController inventoryController)
//    {
//        goldAmount = inventoryController.goldAmount;
//        items = new List<ItemInfoData>();

//        foreach (var item in inventoryController.inventory)
//        {
//            if (item != null)
//            {
//                items.Add(item.ToData());
//            }
//        }
//    }

//    public void ApplyTo(InventoryController inventoryController)
//    {   
//        inventoryController.goldAmount = goldAmount;
//       // inventoryController.inventory.Clear();
//        foreach (var itemData in items)
//        {
//            ItemInfoSO newItem = ScriptableObject.CreateInstance<ItemInfoSO>();
//            newItem.FromData(itemData);
//            inventoryController.inventory.Add(newItem);
//        }
//    }
//}
