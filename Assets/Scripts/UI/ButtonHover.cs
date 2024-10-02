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
            GameObject itemLevelGo = itemTooltip.transform.Find("ItemLvlText").gameObject;
            itemLevelGo.GetComponent<TMP_Text>().text = "";
            GameObject itemNameObj = itemTooltip.transform.Find("Name").gameObject;
            itemNameObj.GetComponent<TMP_Text>().text = inventoryController.inventory[slotId].itemName;
            itemNameObj.GetComponent<TMP_Text>().color = inventoryController.inventory[slotId].textColor;
            GameObject itemStatsObj = itemTooltip.transform.Find("Stats").gameObject;
            itemStatsObj.GetComponent<TMP_Text>().text = "Upgrade Stone";
            GameObject itemBonusStatsObj = itemTooltip.transform.Find("BonusStats").gameObject;
            itemBonusStatsObj.GetComponent<TMP_Text>().text = "This Stone is Used to Upgrade Your Gear or Weapons up to +9 by Using the Upgrade Window Acessible by Clicking [U] on your Keyboard";
            
            if (inventoryController.inventory[slotId].itemType == "Weapon")
            {
                itemLevelGo = itemTooltip.transform.Find("ItemLvlText").gameObject;
                itemLevelGo.GetComponent<TMP_Text>().text = inventoryController.inventory[slotId].itemLvl.ToString() + " Item Power";
                SetWeaponInfo(inventoryController, itemNameObj, itemStatsObj, itemBonusStatsObj, slotId);
            }
            else if (inventoryController.inventory[slotId].itemType == "Armor" || inventoryController.inventory[slotId].itemType == "Helmet" ||
                inventoryController.inventory[slotId].itemType == "Boots" || inventoryController.inventory[slotId].itemType == "Gloves" ||
                inventoryController.inventory[slotId].itemType == "Belt" || inventoryController.inventory[slotId].itemType == "Necklace"
                || inventoryController.inventory[slotId].itemType == "Ring")
            {
                itemLevelGo = itemTooltip.transform.Find("ItemLvlText").gameObject;
                itemLevelGo.GetComponent<TMP_Text>().text = inventoryController.inventory[slotId].itemLvl.ToString() + " Item Power";
                SetGearInfo(inventoryController, itemNameObj, itemStatsObj, itemBonusStatsObj, slotId);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
            SetTooltipLocation();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvas.enabled = false;
    }

    public void SetTooltipLocation()
    {
        Vector2 canvasPos;
        float offset = buttonRectTransform.sizeDelta.x / 2f + panelRectTransform.sizeDelta.x / 2f;
        float tooltipHeight = panelRectTransform.sizeDelta.y;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, transform.position, canvas.worldCamera, out canvasPos);
        float yUpCheck = canvasPos.y + tooltipHeight / 2f;
        float yDownCheck = canvasPos.y - tooltipHeight / 2f;
        

        if (yUpCheck > canvasRectTransform.sizeDelta.y / 2f)
        {
            canvasPos.y -= yUpCheck - canvasRectTransform.sizeDelta.y / 2f;
        }
        else if (yDownCheck < -canvasRectTransform.sizeDelta.y / 2f)
        {
            canvasPos.y -= yDownCheck + canvasRectTransform.sizeDelta.y / 2f;
        }

        panelRectTransform.anchoredPosition = canvasPos + new Vector2(-offset, 0f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
        canvas.enabled = true;
    }

    public void SetWeaponInfo(InventoryController inventoryController, GameObject itemNameObj, GameObject itemStatsObj, GameObject itemBonusStatsObj, int slotId)
    {
        int bonusStatAmount = 0;
        ItemInfoSO weaponInfo = inventoryController.inventory[slotId];

        //Main Stats
        itemStatsObj.GetComponent<TMP_Text>().text = "Attack " + weaponInfo.weaponMainStats.minAttack + "~" + weaponInfo.weaponMainStats.maxAttack;

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
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Crit Chance +" + weaponInfo.weaponBonusStats.critChance.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (weaponInfo.weaponBonusStats.critDamage != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Crit Damage +" + weaponInfo.weaponBonusStats.critDamage.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (weaponInfo.weaponBonusStats.atkSpeed != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Attack Speed +" + weaponInfo.weaponBonusStats.atkSpeed.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (weaponInfo.weaponBonusStats.staggerDmg != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Stagger Damage +" + weaponInfo.weaponBonusStats.staggerDmg.ToString("0.0") + "%";
            bonusStatAmount++;
        }
    }

    public void SetGearInfo(InventoryController inventoryController, GameObject itemNameObj, GameObject itemStatsObj, GameObject itemBonusStatsObj, int slotId)
    {
        int bonusStatAmount = 0;
        ItemInfoSO gearInfo = inventoryController.inventory[slotId];

        //Main Stats
        if (gearInfo.itemType == "Necklace" || gearInfo.itemType == "Ring")
        {
            itemStatsObj.GetComponent<TMP_Text>().text = "Attack " + gearInfo.gearMainStats.attack;
        }
        else
        {
            itemStatsObj.GetComponent<TMP_Text>().text = "Health " + gearInfo.gearMainStats.hp;
            itemStatsObj.GetComponent<TMP_Text>().text += "\nArmor " + gearInfo.gearMainStats.armor;
        }
        itemBonusStatsObj.GetComponent<TMP_Text>().text = "";

        //Bonus Stats
        if (gearInfo.gearBonusStats.attack != 0)
        {
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Attack +" + gearInfo.gearBonusStats.attack;
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.critChance != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Crit Chance +" + gearInfo.gearBonusStats.critChance.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.critDamage != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Crit Damage +" + gearInfo.gearBonusStats.critDamage.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.atkSpeed != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Attack Speed +" + gearInfo.gearBonusStats.atkSpeed.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.staggerDmg != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Stagger Damage +" + gearInfo.gearBonusStats.staggerDmg.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.hp != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Health +" + gearInfo.gearBonusStats.hp;
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.armor != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Armor +" + gearInfo.gearBonusStats.armor;
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.cdRed != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Cooldown Reduction +" + gearInfo.gearBonusStats.cdRed.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.moveSpeed != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Movement Speed +" + gearInfo.gearBonusStats.moveSpeed.ToString("0.0") + "%";
            bonusStatAmount++;
        }
        if (gearInfo.gearBonusStats.recovery != 0)
        {
            if (bonusStatAmount > 0)
            {
                itemBonusStatsObj.GetComponent<TMP_Text>().text += "\n";
            }
            itemBonusStatsObj.GetComponent<TMP_Text>().text += "Recovery +" + gearInfo.gearBonusStats.recovery;
            bonusStatAmount++;
        }
    }
}
