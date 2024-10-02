using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSoInformation : MonoBehaviour
{
    public List<EquipmentController> equipmentInfo;

    public void Awake()
    {
        UpdateEquipment();
    }
    public void FixedUpdate()
    {
        UpdateEquipment();
    }
    public void UpdateEquipment()
    {
        // Clear the existing list to avoid duplicate entries
        equipmentInfo.Clear();

        // Iterate through each child Transform of the current GameObject
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform slotTransform = transform.GetChild(i);

            // Get the EquipmentController component attached to the child Transform
            EquipmentController equipmentController = slotTransform.GetComponent<EquipmentController>();

            // Add the EquipmentController to the list if it's not null
            if (equipmentController != null)
            {
                equipmentInfo.Add(equipmentController);
            }
        }
    }
    public void LoadAllEquipment(List<ItemInfoData> equipmentList)
    {
        Debug.Log("REACHED LOAD ALL");
        foreach (var item in equipmentList)
        {
            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }
            if (equipmentList == null)
            {
                Debug.Log("list is null");
                continue;
            }
            string itemType = item.itemType;
            for (int i = 0; i < equipmentInfo.Count; i++)
            {
                EquipmentController equipmentController = transform.GetChild(i).GetComponent<EquipmentController>();
                if (equipmentController.slotType == itemType)
                {
                    equipmentController.LoadEquipment(item);
                }
            }
        }
    }
}
