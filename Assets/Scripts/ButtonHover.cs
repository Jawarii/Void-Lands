using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Canvas canvas;
    public GameObject itemTooltip;
    private RectTransform panelRectTransform;
    private RectTransform buttonRectTransform;
    private RectTransform canvasRectTransform;
    bool isOccupied = false;
    public bool isDragging = false;

    private void Start()
    {
        panelRectTransform = itemTooltip.GetComponent<RectTransform>();
        buttonRectTransform = transform.GetComponent<RectTransform>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        isDragging = transform.GetComponent<DraggableItemBehaviour>().isDragging;
    }
    private void FixedUpdate()
    {
        isDragging = transform.GetComponent<DraggableItemBehaviour>().isDragging;
        if (isDragging)
        {
            canvas.enabled = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) // Check if dragging is in progress
        {
            return; // Exit early if dragging
        }

        GameObject inventoryMain = GameObject.Find("InventoryMain");

        if (inventoryMain.GetComponent<InventoryController>().inventory[transform.GetComponent<ButtonInfo>().slotId] != null)
        {
            isOccupied = true;
        }
        else isOccupied= false;

        if (isOccupied)
        {                       
            InventoryController inventoryController = inventoryMain.GetComponent<InventoryController>();
            int slotId = transform.GetComponent<ButtonInfo>().slotId;

            GameObject itemNameObj = itemTooltip.transform.Find("Name").gameObject;
            itemNameObj.GetComponent<TMP_Text>().text = inventoryController.inventory[slotId].itemName;
            itemNameObj.GetComponent<TMP_Text>().color = inventoryController.inventory[slotId].textColor;
            GameObject itemStatsObj = itemTooltip.transform.Find("Stats").gameObject;
            itemStatsObj.GetComponent<TMP_Text>().text = "STATS PLACE HOLDER FOR SLOT ID: " + slotId.ToString();
            GameObject itemBonusStatsObj = itemTooltip.transform.Find("BonusStats").gameObject;
            itemBonusStatsObj.GetComponent<TMP_Text>().text = "BONUS STATS PLACE HOLDER FOR SLOT ID: " + slotId.ToString();

            if (inventoryController.inventory[slotId].itemType == "Weapon")
            {
                SetWeaponInfo(inventoryController, itemNameObj, itemStatsObj, itemBonusStatsObj, slotId);
            }
            SetTooltipLocation();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvas.enabled = false;
    }

    public void SetTooltipLocation()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
        Vector2 canvasPos;
        float offset = buttonRectTransform.sizeDelta.x / 2f + panelRectTransform.sizeDelta.x / 2f;
        float tooltipHeight = panelRectTransform.sizeDelta.y;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, transform.position, canvas.worldCamera, out canvasPos);
        float yUpCheck = canvasPos.y + tooltipHeight / 2f;
        float yDownCheck = canvasPos.y - tooltipHeight / 2f;
        canvas.enabled = true;

        if (yUpCheck > canvasRectTransform.sizeDelta.y / 2f)
        {
            canvasPos.y -= yUpCheck - canvasRectTransform.sizeDelta.y / 2f;
        }
        else if (yDownCheck < -canvasRectTransform.sizeDelta.y / 2f)
        {
            canvasPos.y -= yDownCheck + canvasRectTransform.sizeDelta.y / 2f;
        }

        panelRectTransform.anchoredPosition = canvasPos + new Vector2(-offset, 0f);
    }

    public void SetWeaponInfo(InventoryController inventoryController, GameObject itemNameObj, GameObject itemStatsObj, GameObject itemBonusStatsObj, int slotId)
    {
        int bonusStatAmount = 0;
        ItemInfoSO weaponInfo = inventoryController.inventory[slotId];

        //Main Stats
        itemStatsObj.GetComponent<TMP_Text>().text = "Attack " + weaponInfo.weaponMainStats.minAttack + "~" + weaponInfo.weaponMainStats.maxAttack;
        itemStatsObj.GetComponent<TMP_Text>().text += "\n" + "Crit Chance " + weaponInfo.weaponMainStats.critChance + "%";
        itemStatsObj.GetComponent<TMP_Text>().text += "\n" + "Crit Damage " + weaponInfo.weaponMainStats.critDamage + "%";
        itemStatsObj.GetComponent<TMP_Text>().text += "\n" + "Attack Speed " + weaponInfo.weaponMainStats.atkSpeed;

        itemBonusStatsObj.GetComponent<TMP_Text>().text = "";

        //Bonus Stats
        if (weaponInfo.weaponBonusStats.attack != 0)
        {
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Attack +" + weaponInfo.weaponBonusStats.attack;
            bonusStatAmount++;
        }
        if (weaponInfo.weaponBonusStats.critChance != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Crit Chance +" + weaponInfo.weaponBonusStats.critChance;
            bonusStatAmount++;
        }
        if (weaponInfo.weaponBonusStats.critDamage != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Crit Damage +" + weaponInfo.weaponBonusStats.critDamage;
            bonusStatAmount++;
        }
        if (weaponInfo.weaponBonusStats.atkSpeed != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Attack Speed +" + weaponInfo.weaponBonusStats.atkSpeed;
            bonusStatAmount++;
        }
    }
}
