using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeLogic : MonoBehaviour
{
    public GameObject magicStoneSlot;
    public GameObject rareStoneSlot;
    public GameObject legendaryStoneSlot;
    public GameObject inventory;
    public InventoryController inventoryController;
    public GameObject selectedStone;
    public GameObject _slider;
    public float upgradeCastTime = 2f;

    public float magicStoneBaseChance = 1.0f;
    public float magicStoneMaxChance = 0.05f;
    public float rareStoneBaseChance = 1.0f;
    public float rareStoneMaxChance = 0.15f;
    public float legendaryStoneBaseChance = 1.0f;
    public float legendaryStoneMaxChance = 0.30f;

    private Color defaultColor = new Color(0.247f, 0.247f, 0.247f); // #3F3F3F
    private Color selectedColor = Color.cyan; // Neon color

    public ItemInfoSO itemToUpgrade;

    public TMP_Text resultText;
    public TMP_Text chanceText;

    void Start()
    {
        inventoryController = inventory.GetComponent<InventoryController>();
    }

    void Update()
    {
        if (inventoryController != null)
        {
            TraverseInventorySlots();
            UpdateChanceText();
        }
    }

    void TraverseInventorySlots()
    {
        List<ItemInfoSO> inventorySo = inventoryController.inventory;
        foreach (ItemInfoSO item in inventorySo)
        {
            if (item != null && item.itemType == "UpgradeMaterial")
            {
                TMP_Text textComponent = null;
                switch (item.itemQuality)
                {
                    case "Magic":
                        textComponent = magicStoneSlot.GetComponentInChildren<TMP_Text>();
                        break;
                    case "Rare":
                        textComponent = rareStoneSlot.GetComponentInChildren<TMP_Text>();
                        break;
                    case "Legendary":
                        textComponent = legendaryStoneSlot.GetComponentInChildren<TMP_Text>();
                        break;
                }
                if (textComponent != null)
                {
                    textComponent.text = item.currentStackSize.ToString();
                }
            }
        }
    }

    public void Upgrade()
    {
        StartCoroutine(UpgradeItemAnimation());
    }

    public void SelectMagicStone()
    {
        selectedStone = magicStoneSlot;
        UpdateButtonColors();
    }

    public void SelectRareStone()
    {
        selectedStone = rareStoneSlot;
        UpdateButtonColors();
    }

    public void SelectLegendaryStone()
    {
        selectedStone = legendaryStoneSlot;
        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
        SetButtonColor(magicStoneSlot, magicStoneSlot == selectedStone ? selectedColor : defaultColor);
        SetButtonColor(rareStoneSlot, rareStoneSlot == selectedStone ? selectedColor : defaultColor);
        SetButtonColor(legendaryStoneSlot, legendaryStoneSlot == selectedStone ? selectedColor : defaultColor);
    }

    private void SetButtonColor(GameObject buttonObject, Color color)
    {
        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = color;
        colorBlock.selectedColor = color;
        button.colors = colorBlock;
    }

    public IEnumerator UpgradeItemAnimation()
    {
        if (selectedStone)
        {
            Slider sliderComponent = _slider.GetComponent<Slider>();
            sliderComponent.value = 0;
            float elapsedTime = 0f;
            while (elapsedTime < upgradeCastTime)
            {
                elapsedTime += Time.deltaTime;
                sliderComponent.value = Mathf.Clamp01(elapsedTime / upgradeCastTime);
                yield return null;
            }
            sliderComponent.value = 1;

            List<ItemInfoSO> inventorySo = inventoryController.inventory;
            for (int i = 0; i < inventorySo.Count; i++)
            {
                ItemInfoSO item = inventorySo[i];
                if (item != null && item.itemType == "UpgradeMaterial")
                {
                    if ((selectedStone == magicStoneSlot && item.itemQuality == "Magic") ||
                        (selectedStone == rareStoneSlot && item.itemQuality == "Rare") ||
                        (selectedStone == legendaryStoneSlot && item.itemQuality == "Legendary"))
                    {
                        item.currentStackSize--;
                        GameObject.Find("Slot (" + i + ")").GetComponentInChildren<TMP_Text>().text = inventorySo[i].currentStackSize.ToString();
                        bool upgradeSuccess = UpgradeItem();

                        if (item.currentStackSize == 0)
                        {
                            GameObject.Find("Slot (" + i + ")").GetComponentInChildren<TMP_Text>().text = "";
                            (GameObject.Find("Slot (" + i + ")").transform.Find("ItemIcon")).GetComponent<Image>().sprite = null;
                            (GameObject.Find("Slot (" + i + ")").transform.Find("ItemIcon")).gameObject.SetActive(false);
                            inventoryController.inventory[i] = null;
                        }
                        selectedStone.GetComponentInChildren<TMP_Text>().text = item.currentStackSize.ToString();

                        StartCoroutine(HandleResult(upgradeSuccess));
                        break;
                    }
                }
            }
        }
    }

    private bool UpgradeItem()
    {
        float successChance = CalculateSuccessChance(itemToUpgrade.upgradeLevel, selectedStone);
        bool success = Random.value <= successChance;

        if (success)
        {
            if (itemToUpgrade.itemType == "Weapon")
            {
                itemToUpgrade.UpgradeWeapon();
            }
            else
            {
                itemToUpgrade.UpgradeGear();
            }
        }
        return success;
    }

    private float CalculateSuccessChance(int upgradeLevel, GameObject selectedStone)
    {
        if (selectedStone == magicStoneSlot)
        {
            return Mathf.Lerp(magicStoneBaseChance, magicStoneMaxChance, upgradeLevel / 8f);
        }
        else if (selectedStone == rareStoneSlot)
        {
            return Mathf.Lerp(rareStoneBaseChance, rareStoneMaxChance, upgradeLevel / 8f);
        }
        else if (selectedStone == legendaryStoneSlot)
        {
            return Mathf.Lerp(legendaryStoneBaseChance, legendaryStoneMaxChance, upgradeLevel / 8f);
        }
        return 0f;
    }

    private void UpdateChanceText()
    {
        if (itemToUpgrade != null && selectedStone != null)
        {
            float successChance = CalculateSuccessChance(itemToUpgrade.upgradeLevel, selectedStone);
            chanceText.text = $"Upgrade Chance: {(successChance * 100):0.00}%";
        }
        else
        {
            chanceText.text = "Upgrade Chance: N/A";
        }
    }

    public IEnumerator HandleResult(bool success)
    {
        Color originalColor = _slider.transform.GetChild(1).GetComponent<Image>().color;
        if (success)
        {
            _slider.transform.GetChild(1).GetComponent<Image>().color = Color.green;
            resultText.text = "Success!";
        }
        else
        {
            _slider.transform.GetChild(1).GetComponent<Image>().color = Color.red;
            resultText.text = "Failure!";
        }
        yield return new WaitForSeconds(2f);
        resultText.text = "";
        _slider.transform.GetChild(1).GetComponent<Image>().color = originalColor;
    }
}
