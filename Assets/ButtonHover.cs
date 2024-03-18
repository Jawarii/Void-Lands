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
    bool isOccupied = false;

    private void Start()
    {
        panelRectTransform = itemTooltip.GetComponent<RectTransform>();
        buttonRectTransform = transform.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject inventoryMain = GameObject.Find("InventoryMain");

        if (inventoryMain.GetComponent<InventoryController>().inventory[transform.GetComponent<ButtonInfo>().slotId] != null)
        {
            isOccupied = true;
        }

        if (isOccupied)
        {

            canvas.enabled = true;

            float offset = buttonRectTransform.sizeDelta.x / 2f + panelRectTransform.sizeDelta.x / 2f;

            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, transform.position, canvas.worldCamera, out canvasPos);

            panelRectTransform.anchoredPosition = canvasPos - new Vector2(offset, 0f);

            GameObject itemNameObj = itemTooltip.transform.Find("Name").gameObject;
            itemNameObj.GetComponent<TMP_Text>().text = inventoryMain.GetComponent<InventoryController>().inventory[transform.GetComponent<ButtonInfo>().slotId].itemName;
            GameObject itemStatsObj = itemTooltip.transform.Find("Stats").gameObject;
            itemStatsObj.GetComponent<TMP_Text>().text = "STATS PLACE HOLDER FOR SLOT ID: " + transform.GetComponent<ButtonInfo>().slotId.ToString();
            GameObject itemBonusStatsObj = itemTooltip.transform.Find("BonusStats").gameObject;
            itemBonusStatsObj.GetComponent<TMP_Text>().text = "BONUS STATS PLACE HOLDER FOR SLOT ID: " + transform.GetComponent<ButtonInfo>().slotId.ToString();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvas.enabled = false;
    }
}
