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
    public bool isDragging = false; // Flag to indicate dragging status

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true; // Set dragging flag to true
        inventoryMain = GameObject.Find("InventoryMain");
        inventoryController = inventoryMain.GetComponent<InventoryController>();
        draggedItem = transform.GetChild(0);
        _slotId = draggedItem.parent.GetComponent<ButtonInfo>().slotId;
        inventoryController.draggedItemInfo = inventoryController.inventory[_slotId];
        inventoryController.inventory[_slotId] = null;
        parentAfterDrag = draggedItem.parent;
        draggedItem.SetParent(parentAfterDrag.parent.parent);
        draggedItem.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggedItem.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        // Perform a raycast to determine the slot ID under the mouse cursor
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        int hoveredSlotId = -1;
        ButtonInfo buttonInfo;

        foreach (RaycastResult result in results)
        {
            buttonInfo = result.gameObject.GetComponent<ButtonInfo>();
            if (buttonInfo != null)
            {
                hoveredSlotId = buttonInfo.slotId;
                if (hoveredSlotId == _slotId)
                    break;                
                Transform otherIcon = result.gameObject.transform.GetChild(0);
                otherIcon.SetParent(parentAfterDrag);
                otherIcon.localPosition = new Vector3(0, 0, 0);
                otherIcon.SetAsFirstSibling();
                draggedItem.SetParent(result.gameObject.transform);
                draggedItem.localPosition = new Vector3(0, 0, 0);
                draggedItem.SetAsFirstSibling();
                inventoryController.inventory[_slotId] = inventoryController.inventory[hoveredSlotId];
                inventoryController.inventory[hoveredSlotId] = inventoryController.draggedItemInfo;
                inventoryController.draggedItemInfo = null;
                break;
            }
        }

        if (hoveredSlotId < 0 || hoveredSlotId == _slotId)
        {
            draggedItem.SetParent(parentAfterDrag);
            draggedItem.localPosition = new Vector3(0, 0, 0);
            draggedItem.SetAsFirstSibling();
            inventoryController.inventory[_slotId] = inventoryController.draggedItemInfo;
            inventoryController.draggedItemInfo = null;
        }
    }
}
