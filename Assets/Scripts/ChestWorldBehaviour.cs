using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestWorldBehaviour : MonoBehaviour
{
    public ObjectShaker objShaker;
    public Sprite openChestSprite;
    public Sprite closeChestSprite;
    public SpriteRenderer spriteRenderer;

    public List<GameObject> goldItems = new List<GameObject>();
    public List<GameObject> gearItems = new List<GameObject>();
    public List<GameObject> upgradeItems = new List<GameObject>();

    public bool isOpen = false;
    public bool inRange = false;

    public int chestLvl = 11;

    public AudioSource dropSource;
    public AudioClip dropClip;

    public int chestPrice = 1000;
    void Start()
    {

        objShaker = GetComponentInParent<ObjectShaker>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        dropSource = GameObject.Find("DropAudioSource").GetComponent<AudioSource>();
        closeChestSprite = spriteRenderer.sprite;
    }

    private void Update()
    {
        if (!isOpen && inRange)
        {
            InventoryController inventoryController = GameObject.Find("InventoryMain").GetComponent<InventoryController>();
            float playerGold = inventoryController.goldAmount;
            if (Input.GetKeyDown("f") && playerGold >= chestPrice)
            {
                inventoryController.goldAmount -= chestPrice;
                StartCoroutine(OpenChest());
            }
        }
    }
    IEnumerator OpenChest()
    {
        isOpen = true;
        objShaker.enabled = true;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.sprite = openChestSprite;
        StartCoroutine(DropLoot());
        yield return new WaitForSeconds(0.1f);
        objShaker.enabled = false;
        yield return new WaitForSeconds(0.5f);
        isOpen = false;
        spriteRenderer.sprite = closeChestSprite;
    }

    IEnumerator DropLoot()
    {
        //StartCoroutine(DropGold());
        StartCoroutine(DropGear(gearItems, 2)); // Specify number of items to drop
        //StartCoroutine(DropStones(upgradeItems, 12)); // Specify number of items to drop
        yield return null;
    }

    private Vector3 RandomDropPosition()
    {
        float minRadius = 0.5f;  // Minimum distance from the chest
        float maxRadius = 0.65f;  // Maximum distance for dropping items
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad; // Convert degrees to radians

        float randomRadius = Random.Range(minRadius, maxRadius);
        Vector2 randomPoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randomRadius;

        return transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
    }

    IEnumerator DropGold()
    {
        for (int i = 0; i < 15; i++)
        {
            int goldAmount = (int)(Random.value * 51f);
            int goldDropIndex = goldAmount < 10 ? 0 : goldAmount < 50 ? 1 : 2;
            Vector3 dropPosition = RandomDropPosition();
            GameObject goldDrop = Instantiate(goldItems[goldDropIndex], dropPosition, transform.rotation);
            goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
            goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator DropGear(List<GameObject> items, int numItems)
    {
        List<GameObject> selectedItems = new List<GameObject>();

        // Define rarity chances (adjust the values as needed)
        float legendaryChance = 0.12f; // 12% chance for Legendary
        float rareChance = 0.30f;      // 30% chance for Rare
        float magicChance = 0.58f;     // 58% chance for Magic

        while (selectedItems.Count < numItems)
        {
            // Roll for rarity
            float roll = Random.value;

            string selectedRarity;
            if (roll <= legendaryChance)
            {
                selectedRarity = "Legendary";
            }
            else if (roll <= legendaryChance + rareChance)
            {
                selectedRarity = "Rare";
            }
            else if (roll <= legendaryChance + rareChance + magicChance)
            {
                selectedRarity = "Magic";
            }
            else
            {
                selectedRarity = "Magic";
            }

            // Filter items by the rolled rarity
            List<GameObject> itemsOfRarity = new List<GameObject>();
            foreach (GameObject item in items)
            {
                ItemInfo info = item.GetComponent<ItemInfo>();
                if (info != null && info.itemQuality == selectedRarity)
                {
                    itemsOfRarity.Add(item);
                }
            }

            // Select a random item from the filtered list
            if (itemsOfRarity.Count > 0)
            {
                GameObject selectedItem = itemsOfRarity[Random.Range(0, itemsOfRarity.Count)];

                // Set the item's level (either the chest level or chest level + 1)
                int gearLevel = chestLvl + 1;
                selectedItem.GetComponent<ItemInfo>().itemLvl = gearLevel;

                // Add the selected item to the list
                selectedItems.Add(selectedItem);
            }
        }

        // Drop the selected items
        foreach (GameObject item in selectedItems)
        {
            Vector3 dropPosition = RandomDropPosition();
            Instantiate(item, dropPosition, transform.rotation);

            // Play drop sound if the item is Legendary
            if (item.GetComponent<ItemInfo>().itemQuality == "Legendary")
            {
                dropSource.PlayOneShot(dropClip);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator DropStones(List<GameObject> items, int numItems)
    {
        int currentTierLevel = (chestLvl - 1) % 10 + 1; // Levels 1-10 will cycle
        if (chestLvl > 10)
            currentTierLevel = 10;

        // Determine the chances for each rarity
        float legendaryChance = 0.01f + (0.11f * (currentTierLevel - 1) / 9); // From 1% to 12%
        float rareChance = 0.03f + (0.27f * (currentTierLevel - 1) / 9);      // From 3% to 30%
        float magicChance = 0.96f - (0.38f * (currentTierLevel - 1) / 9);     // From 96% to 58%

        // Loop through the number of items to drop
        for (int i = 0; i < numItems; i++)
        {
            float roll = Random.value;

            // Determine the rarity based on the rolled value
            string rarity = roll <= legendaryChance ? "Legendary" :
                            roll <= (legendaryChance + rareChance) ? "Rare" :
                            roll <= (legendaryChance + rareChance + magicChance) ? "Magic" : "Normal";

            // Call a method to drop the item based on its rarity
            DropItemBasedOnRarity(items, rarity);

            yield return new WaitForSeconds(0.1f); // Adjust delay between drops if needed
        }
    }

    private void DropItemBasedOnRarity(List<GameObject> items, string rarity)
    {
        // Create a list to store items of the chosen rarity
        List<GameObject> itemsOfRarity = new List<GameObject>();

        // Iterate through all items and add those of the chosen rarity to the list
        foreach (GameObject item in items)
        {
            ItemInfo info = item.GetComponent<ItemInfo>();
            if (info != null && info.itemQuality == rarity)
            {
                itemsOfRarity.Add(item);
            }
        }
        foreach (GameObject item in itemsOfRarity)
        {
            float levelRoll = Random.value; // Random value between 0 and 1

            if (levelRoll <= 0.30f) // 30% chance for item level 1 time higher
            {
                item.GetComponent<ItemInfo>().itemLvl = chestLvl + 1;
            }
            else // 70% chance for same level as enemy
            {
                item.GetComponent<ItemInfo>().itemLvl = chestLvl;
            }
        }

        // If there are items of the chosen rarity, choose one randomly to drop
        if (itemsOfRarity.Count > 0)
        {
            int randomIndex = Random.Range(0, itemsOfRarity.Count);
            GameObject itemToDrop = itemsOfRarity[randomIndex];
            Vector3 dropPosition = RandomDropPosition();
            Instantiate(itemToDrop, dropPosition, transform.rotation);

            // Play sound if it's a legendary item
            if (rarity == "Legendary")
            {
                dropSource.PlayOneShot(dropClip);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            inRange = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            inRange = false;
    }
}
