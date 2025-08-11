using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
/// <summary>
/// Handles the dragging behavior of items in the inventory and equipment system.
/// Manages item movement between inventory slots and equipment slots.
/// </summary>
public class DraggableItemBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // References to UI elements
    private Transform parentAfterDrag;
    private Transform draggedItem;
    private GameObject inventoryMain;
    private InventoryController inventoryController;
    private int _slotId;
    public bool isDragging = false;
    private bool draggedFromEquipment = false;
    public GameObject draggedCanvas;
    public GameObject destroyCanvas;

    private bool waitingForConfirmation = false;
    private Transform storedParentBeforeDestroy;
    private int storedSlotIdBeforeDestroy;
    public DestroyConfirmationPanel destroyConfirmationPanel;
    private void Start()
    {
        // Cache references
        inventoryMain = GameObject.Find("InventoryMain");
        if (inventoryMain == null)
        {
            Debug.LogError("InventoryMain not found in scene!");
            return;
        }

        inventoryController = inventoryMain.GetComponent<InventoryController>();
        if (inventoryController == null)
        {
            Debug.LogError("InventoryController component not found on InventoryMain!");
            return;
        }

        // Cache canvas references
        if (draggedCanvas == null)
        {
            draggedCanvas = GameObject.Find("DraggedCanvas");
            if (draggedCanvas == null)
            {
                Debug.LogError("DraggedCanvas not found in scene!");
            }
        }

        if (destroyCanvas == null)
        {
            destroyCanvas = GameObject.Find("DestroyCanvas");
            if (destroyCanvas == null)
            {
                Debug.LogError("DestroyCanvas not found in scene!");
            }
        }
        if (destroyConfirmationPanel == null)
        {
            destroyConfirmationPanel = Resources.FindObjectsOfTypeAll<DestroyConfirmationPanel>()
                .FirstOrDefault(panel => panel.gameObject.name == "ConfirmationPanel");
        }
    }

    /// <summary>
    /// Called when the user starts dragging an item.
    /// Initializes the drag operation and sets up the dragged item.
    /// </summary>
    /// <param name="eventData">Data about the pointer event</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if left mouse button is pressed
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        // Ensure required components are available
        if (inventoryController == null || draggedCanvas == null || destroyCanvas == null)
        {
            Debug.LogError("Required components not initialized!");
            return;
        }

        destroyCanvas.GetComponent<Canvas>().enabled = true;
        draggedItem = transform.GetChild(0);
        if (draggedItem == null)
        {
            Debug.LogError("No child object found to drag!");
            return;
        }

        ButtonInfo checkButtonInfo = draggedItem.parent.GetComponent<ButtonInfo>();
        if (checkButtonInfo != null)
        {
            _slotId = checkButtonInfo.slotId;
            if (inventoryController.inventory[_slotId] == null)
            {
                isDragging = false;
                return;
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
            EquipmentController equipmentController = transform.GetComponent<EquipmentController>();
            if (equipmentController == null || equipmentController.equipInfoSo == null)
            {
                isDragging = false;
                return;
            }
            isDragging = true;
            draggedFromEquipment = true;
            inventoryController.draggedItemInfo = equipmentController.equipInfoSo;
            equipmentController.equipInfoSo = null;
            parentAfterDrag = draggedItem.parent;
            draggedItem.SetParent(draggedCanvas.transform);
            draggedItem.SetAsLastSibling();
        }
    }

    /// <summary>
    /// Called while the user is dragging an item.
    /// Updates the position of the dragged item to follow the mouse.
    /// </summary>
    /// <param name="eventData">Data about the pointer event</param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || draggedItem == null)
            return;

        draggedItem.position = Input.mousePosition;
    }

    /// <summary>
    /// Called when the user stops dragging an item.
    /// Handles the placement of the item in a new slot or returns it to its original position.
    /// </summary>
    /// <param name="eventData">Data about the pointer event</param>
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
            if (result.gameObject.CompareTag("DestroyCanvas") && !draggedFromEquipment)
            {
                // Pause flow, show confirmation, wait for input
                waitingForConfirmation = true;
                storedParentBeforeDestroy = parentAfterDrag;
                storedSlotIdBeforeDestroy = _slotId;

                // Show your confirmation UI here
                destroyConfirmationPanel.Show(this); // pass self to hook confirm/cancel
                return;
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
        if (isDragging)
        {
            isDragging = false;

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

            destroyCanvas.GetComponent<Canvas>().enabled = false;
        }
    }

    public void ConfirmDestroyItem()
    {
        destroyCanvas.GetComponent<Canvas>().enabled = false;
        draggedItem.SetParent(parentAfterDrag);
        draggedItem.localPosition = Vector3.zero;
        draggedItem.SetAsFirstSibling();
        inventoryController.inventory[_slotId] = inventoryController.draggedItemInfo;
        inventoryController.draggedItemInfo = null;
        parentAfterDrag.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
        parentAfterDrag.GetChild(0).gameObject.SetActive(false);
        inventoryController.inventory[_slotId] = null;
        isDragging = false;
    }
    public void CancelDestroyItem()
    {
        destroyCanvas.GetComponent<Canvas>().enabled = false;
        draggedItem.SetParent(parentAfterDrag);
        draggedItem.localPosition = Vector3.zero;
        draggedItem.SetAsFirstSibling();
        inventoryController.inventory[_slotId] = inventoryController.draggedItemInfo;
        inventoryController.draggedItemInfo = null;
        isDragging = false;
    }
    public void OnConfirmDestroy()
    {
        ConfirmDestroyItem();
    }

    public void OnCancelDestroy()
    {
        CancelDestroyItem();
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
