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
    public float magicStoneMaxChance = 0.01f;
    public float rareStoneBaseChance = 1.0f;
    public float rareStoneMaxChance = 0.06f;
    public float legendaryStoneBaseChance = 1.0f;
    public float legendaryStoneMaxChance = 0.20f;

    private Color defaultColor = new Color(0.247f, 0.247f, 0.247f); // #3F3F3F
    private Color selectedColor = Color.cyan; // Neon color

    public ItemInfoSO itemToUpgrade;

    public TMP_Text resultText;
    public TMP_Text chanceText;

    private bool isUpgradeInProgress = false;  // Flag to track if an upgrade is in progress

    public Color originalColor;

    public AudioSource audioSource;
    public AudioClip failClip;
    public AudioClip successClip;
    public AudioClip clickClip;
    public AudioClip castClip;
    public AudioClip[] hammerSounds;
    void Start()
    {
        inventoryController = inventory.GetComponent<InventoryController>();
        itemToUpgrade = inventoryController.inventory[70];
        originalColor = _slider.transform.GetChild(1).GetComponent<Image>().color;
    }

    void Update()
    {
        if (inventoryController != null)
        {
            itemToUpgrade = inventoryController.inventory[70];
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
        audioSource.PlayOneShot(clickClip);
        // Check if an upgrade is already in progress
        if (isUpgradeInProgress)
        {
            resultText.text = "Upgrade already in progress!";
            return;
        }

        // Check if an item and stone are selected for the upgrade
        if (selectedStone == null || itemToUpgrade == null)
        {
            resultText.text = "No item to upgrade or no stone selected!";
            return;
        }
        StopAllCoroutines();
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
        // Set the flag to indicate the upgrade is in progress
        isUpgradeInProgress = true;

        // Reset the color, text, and slider value
        _slider.transform.GetChild(1).GetComponent<Image>().color = originalColor; // Reset the color to its original state
        Slider sliderComponent = _slider.GetComponent<Slider>();
        sliderComponent.value = 0;
        resultText.text = "";

        // Check if the selected stone's currentStackSize is less than 1
        List<ItemInfoSO> inventorySo = inventoryController.inventory;
        bool validStone = false;

        foreach (ItemInfoSO item in inventorySo)
        {
            if (item != null && item.itemType == "UpgradeMaterial")
            {
                if ((selectedStone == magicStoneSlot && item.itemQuality == "Magic") ||
                    (selectedStone == rareStoneSlot && item.itemQuality == "Rare") ||
                    (selectedStone == legendaryStoneSlot && item.itemQuality == "Legendary"))
                {
                    if (item.currentStackSize >= 1)
                    {
                        validStone = true;
                    }
                    break;
                }
            }
        }

        // If no valid stone is found, do not start the upgrade animation
        if (!validStone)
        {
            resultText.text = "Not enough stones!";
            isUpgradeInProgress = false;  // Reset the flag
            yield break;
        }

        StartCoroutine(PlayHammerClips());
        // Proceed with the upgrade animation
        float elapsedTime = 0f;
        while (elapsedTime < upgradeCastTime)
        {
            elapsedTime += Time.deltaTime;
            sliderComponent.value = Mathf.Clamp01(elapsedTime / upgradeCastTime);
            yield return null;
        }
        sliderComponent.value = 1;

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

                    TMP_Text slotText = GameObject.Find("Slot (" + i + ")").transform.GetChild(0).GetComponentInChildren<TMP_Text>();
                    if (slotText != null)
                    {
                        slotText.text = inventorySo[i].currentStackSize.ToString();
                    }

                    bool upgradeSuccess = UpgradeItem();
                    selectedStone.GetComponentInChildren<TMP_Text>().text = item.currentStackSize.ToString();

                    if (item.currentStackSize < 1)
                    {
                        GameObject.Find("Slot (" + i + ")").transform.GetChild(0).GetComponentInChildren<TMP_Text>().text = "";
                        (GameObject.Find("Slot (" + i + ")").transform.Find("ItemIcon").transform.Find("ItemIconX")).GetComponent<Image>().sprite = null;
                        (GameObject.Find("Slot (" + i + ")").transform.Find("ItemIcon")).gameObject.SetActive(false);
                        inventoryController.inventory[i] = null;
                    }

                    StartCoroutine(HandleResult(upgradeSuccess));
                    break;
                }
            }
        }

        // Reset the flag after the upgrade animation is complete
        isUpgradeInProgress = false;
    }

    public IEnumerator PlayHammerClips()
    {
        int i = 0;
        audioSource.PlayOneShot(hammerSounds[i]);
        yield return new WaitForSeconds(hammerSounds[i].length);
        i++;
        audioSource.PlayOneShot(hammerSounds[i]);
        yield return new WaitForSeconds(hammerSounds[i].length);
        i++;
        audioSource.PlayOneShot(hammerSounds[i]);
        yield return new WaitForSeconds(hammerSounds[i].length);
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
        float successChance = 0f;

        // Quadratic falloff formulas for smoother drop
        if (selectedStone == magicStoneSlot)
        {
            // Starts at 100%, drops to 75%, 50%, then 1%
            if (upgradeLevel < 3)
            {
                successChance = Mathf.Lerp(1f, 0.5f, upgradeLevel / 2f);
            }
            else if (upgradeLevel < 6)
            {
                successChance = Mathf.Lerp(0.3f, 0.15f, (upgradeLevel - 3) / 2f);
            }
            else
            {
                successChance = Mathf.Lerp(0.05f, magicStoneMaxChance, (upgradeLevel - 6) / 2f);
            }
        }
        else if (selectedStone == rareStoneSlot)
        {
            // Starts at 100%, drops to 90%, 50%, then 8%
            if (upgradeLevel < 3)
            {
                successChance = Mathf.Lerp(1f, 1.0f, upgradeLevel / 2f);
            }
            else if (upgradeLevel < 6)
            {
                successChance = Mathf.Lerp(0.9f, 0.6f, (upgradeLevel - 3) / 2f);
            }
            else
            {
                successChance = Mathf.Lerp(0.12f, rareStoneMaxChance, (upgradeLevel - 6) / 2f);
            }
        }
        else if (selectedStone == legendaryStoneSlot)
        {
            // Starts at 100%, drops to 90%, 60%, then 20%
            if (upgradeLevel < 3)
            {
                successChance = Mathf.Lerp(1f, 1.0f, upgradeLevel / 2f);
            }
            else if (upgradeLevel < 6)
            {
                successChance = Mathf.Lerp(1f, 0.8f, (upgradeLevel - 3) / 2f);
            }
            else
            {
                successChance = Mathf.Lerp(0.6f, legendaryStoneMaxChance, (upgradeLevel - 6) / 2f);
            }
        }

        return successChance;
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
        if (success)
        {
            _slider.transform.GetChild(1).GetComponent<Image>().color = Color.green;
            resultText.text = "Success!";
            audioSource.PlayOneShot(successClip);
        }
        else
        {
            _slider.transform.GetChild(1).GetComponent<Image>().color = Color.red;
            resultText.text = "Failure!";
            audioSource.PlayOneShot(failClip);
        }
        yield return new WaitForSeconds(2f);
        resultText.text = "";
        _slider.transform.GetChild(1).GetComponent<Image>().color = originalColor;
        Slider sliderComponent = _slider.GetComponent<Slider>();
        sliderComponent.value = 0;
    }
}
