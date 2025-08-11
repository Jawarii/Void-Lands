using UnityEngine;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour
{
    public int slotId;
    public Color rarityColorInfo;
    private InventoryController inventoryController;

    private void Awake()
    {

        string slotIdName = gameObject.name;  // Get the name of the GameObject
        string slotIdNumber = "";

        // Iterate through each character in the GameObject's name
        foreach (var chr in slotIdName)
        {
            // Check if the character is a digit
            if (char.IsDigit(chr))
            {
                // Append the digit to the slotIdNumber string
                slotIdNumber += chr;
            }
        }

        // Convert the extracted number string to an integer
        if (slotIdNumber.Length > 0)
        {
            slotId = int.Parse(slotIdNumber);
        }
        else
        {
            //Debug.LogWarning("No numbers found in the GameObject name.");
            slotId = 70;  // Set a default value if no numbers were found
        }

        inventoryController = GameObject.Find("InventoryMain").GetComponent<InventoryController>();
        ItemInfoSO itemInfoSo;
        itemInfoSo = inventoryController.inventory[slotId];
        if (itemInfoSo != null)
            rarityColorInfo = itemInfoSo.textColor;

        if (rarityColorInfo != null)
        {
            gameObject.transform.GetChild(0).GetComponent<Image>().color = rarityColorInfo;
        }
    }

    private void FixedUpdate()
    {
        ItemInfoSO itemInfoSo;
        itemInfoSo = inventoryController.inventory[slotId];

        if (itemInfoSo != null)
            rarityColorInfo = itemInfoSo.textColor;

        if (rarityColorInfo != null)
        {
            try
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().color = rarityColorInfo;
            }
            catch { }
        }
    }
}
