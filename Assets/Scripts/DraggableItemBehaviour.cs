using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform parentAfterDrag;
    Transform draggedItem;
    GameObject inventoryMain;
    InventoryController inventoryController;
    int _slotId;
    public bool isDragging = false;
    private bool draggedFromEquipment = false;
    public GameObject draggedCanvas;

    public void OnBeginDrag(PointerEventData eventData)
    {

        draggedCanvas = GameObject.Find("DraggedCanvas");
        isDragging = true;
        inventoryMain = GameObject.Find("InventoryMain");
        inventoryController = inventoryMain.GetComponent<InventoryController>();
        draggedItem = transform.GetChild(0);

        ButtonInfo checkButtonInfo = draggedItem.parent.GetComponent<ButtonInfo>();

        if (checkButtonInfo != null)
        {
            _slotId = checkButtonInfo.slotId;
            if (inventoryController.inventory[_slotId] == null)
            {
                isDragging = false;
                return; // Exit the function if there's no item in the slot
            }
            inventoryController.draggedItemInfo = inventoryController.inventory[_slotId];
            inventoryController.inventory[_slotId] = null;
            parentAfterDrag = draggedItem.parent;
            draggedItem.SetParent(draggedCanvas.transform);
            draggedItem.SetAsLastSibling();

        }
        else
        {
            if (transform.GetComponent<EquipmentController>().equipInfoSo == null)
            {
                isDragging = false;
                return; // Exit the function if there's no item equipped
            }
            draggedFromEquipment = true;
            inventoryController.draggedItemInfo = transform.GetComponent<EquipmentController>().equipInfoSo;
            transform.GetComponent<EquipmentController>().equipInfoSo = null;
            parentAfterDrag = draggedItem.parent;
            draggedItem.SetParent(draggedCanvas.transform);
            draggedItem.SetAsLastSibling();

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        draggedItem.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        int hoveredSlotId = -1;
        ButtonInfo buttonInfo;
        EquipmentController equipmentController;
        bool validDropTargetFound = false;

        foreach (RaycastResult result in results)
        {
            buttonInfo = result.gameObject.GetComponent<ButtonInfo>();
            equipmentController = result.gameObject.GetComponent<EquipmentController>(); 
            if (buttonInfo != null)
            {
                hoveredSlotId = buttonInfo.slotId;
                if (draggedFromEquipment == true)
                {
                    if (inventoryController.inventory[hoveredSlotId] != null &&
                        inventoryController.inventory[hoveredSlotId].itemType != inventoryController.draggedItemInfo.itemType)
                        break;
                    transform.GetComponent<EquipmentController>().equipInfoSo = inventoryController.inventory[hoveredSlotId];
                    inventoryController.inventory[hoveredSlotId] = inventoryController.draggedItemInfo;
                    inventoryController.draggedItemInfo = null;
                    Transform otherIcon = result.gameObject.transform.GetChild(0);
                    otherIcon.SetParent(parentAfterDrag);
                    otherIcon.localPosition = new Vector3(0, 0, 0);
                    otherIcon.SetAsFirstSibling();
                    draggedItem.SetParent(result.gameObject.transform);
                    draggedItem.localPosition = new Vector3(0, 0, 0);
                    draggedItem.SetAsFirstSibling();
                    validDropTargetFound = true;
                    break;
                }
                else
                {               
                    if (hoveredSlotId == _slotId)
                    {
                        draggedItem.SetParent(parentAfterDrag);
                        draggedItem.localPosition = new Vector3(0, 0, 0);
                        draggedItem.SetAsFirstSibling();
                        inventoryController.inventory[_slotId] = inventoryController.draggedItemInfo;
                        inventoryController.draggedItemInfo = null;
                        validDropTargetFound = true;
                        break;
                    }
                    else
                    {
                        inventoryController.inventory[_slotId] = inventoryController.inventory[hoveredSlotId];
                        inventoryController.inventory[hoveredSlotId] = inventoryController.draggedItemInfo;
                        inventoryController.draggedItemInfo = null;
                        Transform otherIcon = result.gameObject.transform.GetChild(0);
                        otherIcon.SetParent(parentAfterDrag);
                        otherIcon.localPosition = new Vector3(0, 0, 0);
                        otherIcon.SetAsFirstSibling();
                        draggedItem.SetParent(result.gameObject.transform);
                        draggedItem.localPosition = new Vector3(0, 0, 0);
                        draggedItem.SetAsFirstSibling();
                        validDropTargetFound = true;
                        break;
                    }
                }
            }
            else if (equipmentController != null)
            {
                string slotType = equipmentController.slotType;
                string itemType = inventoryController.draggedItemInfo.itemType;
                if (slotType != itemType)
                    break;
                else if (parentAfterDrag == result.gameObject.transform)
                    break;
                Transform otherIcon = result.gameObject.transform.GetChild(0);
                otherIcon.SetParent(parentAfterDrag);
                otherIcon.localPosition = new Vector3(0, 0, 0);
                otherIcon.SetAsFirstSibling();
                draggedItem.SetParent(result.gameObject.transform);
                draggedItem.localPosition = new Vector3(0, 0, 0);
                draggedItem.SetAsFirstSibling();
                inventoryController.inventory[_slotId] = equipmentController.equipInfoSo;
                equipmentController.equipInfoSo = inventoryController.draggedItemInfo;
                inventoryController.draggedItemInfo = null;
                validDropTargetFound = true;
                break;
            }
        }
        if (!validDropTargetFound)
        {
            if (!draggedFromEquipment)
            {
                draggedItem.SetParent(parentAfterDrag);
                draggedItem.localPosition = new Vector3(0, 0, 0);
                draggedItem.SetAsFirstSibling();
                inventoryController.inventory[_slotId] = inventoryController.draggedItemInfo;
                inventoryController.draggedItemInfo = null;
            }
            else
            {
                draggedItem.SetParent(parentAfterDrag);
                draggedItem.localPosition = new Vector3(0, 0, 0);
                draggedItem.SetAsFirstSibling();
                transform.GetComponent<EquipmentController>().equipInfoSo = inventoryController.draggedItemInfo;
                inventoryController.draggedItemInfo = null;
            }
        }
    }
}
