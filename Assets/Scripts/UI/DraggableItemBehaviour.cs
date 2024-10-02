using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class
    DraggableItemBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform parentAfterDrag;
    Transform draggedItem;
    GameObject inventoryMain;
    InventoryController inventoryController;
    int _slotId;
    public bool isDragging = false;
    private bool draggedFromEquipment = false;
    public GameObject draggedCanvas;
    public GameObject destroyCanvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if left mouse button is pressed or the Space key is pressed as the secondary input
        Debug.Log("Begun Drag");
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            Debug.Log("Canceled Drag On Beginning");
            return; // Exit if neither left mouse button nor the Space key is pressed
        }
        Debug.Log("Begun Drag Accepted");
        destroyCanvas = GameObject.Find("DestroyCanvas");
        destroyCanvas.GetComponent<Canvas>().enabled = true;
        draggedCanvas = GameObject.Find("DraggedCanvas");

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
            isDragging = true;
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
            isDragging = true;
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

        Debug.Log("Dragging...");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag Triggered");

        if (!isDragging)
            return;

        Debug.Log("End Drag Started");

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
            Debug.Log(result.gameObject.name);
            // Check if the raycast hit a GameObject with the "destroycanvas" tag
            if (result.gameObject.CompareTag("DestroyCanvas") && !draggedFromEquipment)
            {
                draggedItem.SetParent(parentAfterDrag);
                draggedItem.localPosition = new Vector3(0, 0, 0);
                draggedItem.SetAsFirstSibling();
                inventoryController.inventory[_slotId] = inventoryController.draggedItemInfo;
                inventoryController.draggedItemInfo = null;
                parentAfterDrag.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
                parentAfterDrag.GetChild(0).gameObject.SetActive(false);
                inventoryController.inventory[_slotId] = null;
                //Destroy(draggedItem.gameObject); // Destroy the dragged item

                validDropTargetFound = true;
                break;
            }

            buttonInfo = result.gameObject.GetComponent<ButtonInfo>();
            equipmentController = result.gameObject.GetComponent<EquipmentController>();

            if (buttonInfo != null)
            {
                hoveredSlotId = buttonInfo.slotId;
                if (draggedFromEquipment)
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
                    transform.GetComponent<EquipmentController>().RemoveStats();
                    break;
                }
                else
                {
                    if (hoveredSlotId == 70 && inventoryController.draggedItemInfo.itemType == "UpgradeMaterial")
                    {
                        break;
                    }
                    if (hoveredSlotId == _slotId)
                    {
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
                equipmentController.RemoveStats();
                equipmentController.AddStats();
                break;
            }
        }
        destroyCanvas.GetComponent<Canvas>().enabled = false;
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
    void OnDisable()
    {
        Debug.Log("OnDisable Called");
        if (isDragging)
        {
            // Ensure that drag state is reset if the object is disabled
            isDragging = false;

            if (!draggedFromEquipment)
            {
                // Reset for items dragged from inventory
                draggedItem.SetParent(parentAfterDrag);
                draggedItem.localPosition = new Vector3(0, 0, 0);
                draggedItem.SetAsFirstSibling();
                inventoryController.inventory[_slotId] = inventoryController.draggedItemInfo;
                inventoryController.draggedItemInfo = null;
            }
            else
            {
                // Reset for items dragged from equipment
                draggedItem.SetParent(parentAfterDrag);
                draggedItem.localPosition = new Vector3(0, 0, 0);
                draggedItem.SetAsFirstSibling();
                transform.GetComponent<EquipmentController>().equipInfoSo = inventoryController.draggedItemInfo;
                inventoryController.draggedItemInfo = null;
            }

            // Disable the DestroyCanvas after drag reset
            destroyCanvas.GetComponent<Canvas>().enabled = false;
        }
    }
    void Update()
    {
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            Debug.Log("Forcing End Drag");
            OnEndDrag(null);
        }
    }
}
