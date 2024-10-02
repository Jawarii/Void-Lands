//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class EquipmentData
//{
//    public List<ItemInfoData> items;
//    public List<string> slotTypes; // List to hold slot types for each item
//    public GameObject player;
//    public PlayerStats playerStats;

//    public EquipmentData(EquipmentSoInformation equipmentSoInfo)
//    {
//        items = new List<ItemInfoData>();
//        slotTypes = new List<string>(); // Initialize the list for slot types

//        foreach (var item in equipmentSoInfo.equipmentInfo)
//        {
//            if (item != null)
//            {
//                items.Add(item.equipInfoSo.ToData());
//                slotTypes.Add(item.slotType); // Add the slot type to the list
//                player = item.player;
//                playerStats = item.playerStats;
//            }
//        }
//    }

//    public void ApplyTo(EquipmentSoInformation equipmentSoInfo)
//    {
//        // Clear the existing equipment info to reset it before applying new data
//        equipmentSoInfo.equipmentInfo.Clear();

//        for (int i = 0; i < items.Count; i++)
//        {
//            ItemInfoSO newItem = ScriptableObject.CreateInstance<ItemInfoSO>();
//            newItem.FromData(items[i]);

//            // Create a new EquipmentController to hold this item information
//            EquipmentController newEquipment = new EquipmentController();
//            newEquipment.equipInfoSo = newItem;
//            newEquipment.slotType = slotTypes[i]; // Use the corresponding slot type from the list
//            newEquipment.player = player;
//            newEquipment.playerStats = playerStats;

//            // Add this equipment to the equipment info list
//            equipmentSoInfo.equipmentInfo.Add(newEquipment);
//        }
//    }
//}
