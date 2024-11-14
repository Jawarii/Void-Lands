using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenChestBehaviour : MonoBehaviour
{
    public ObjectShaker objShaker;
    public Sprite openChestSprite;
    public SpriteRenderer spriteRenderer;

    public List<GameObject> goldItems = new List<GameObject>();
    public List<GameObject> gearItems = new List<GameObject>();
    public List<GameObject> upgradeItems = new List<GameObject>();

    public bool isOpen = false;
    public int directionModifier;
    public int chestLvl = 11;

    public AudioSource dropSource;
    public AudioClip dropClip;
    private bool inRange;

    void Start()
    {
        objShaker = GetComponentInParent<ObjectShaker>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        dropSource = GameObject.Find("DropAudioSource").GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!isOpen && inRange)
        {
            if (Input.GetKeyDown("f"))
            {
                StartCoroutine(OpenChest());
            }
        }
    }
    IEnumerator OpenChest()
    {
        isOpen = true;
        objShaker.enabled = true;
        yield return new WaitForSeconds(1f);
        spriteRenderer.sprite = openChestSprite;
        StartCoroutine(DropLoot());
        yield return new WaitForSeconds(0.1f);
        objShaker.enabled = false;

    }
    private int DirectionModifier(Vector2 randomPoint)
    {
        if (randomPoint.x > transform.position.x)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
    IEnumerator DropLoot()
    {
        StartCoroutine(DropGold());
        StartCoroutine(DropGear(gearItems, 1)); // Specify number of items to drop
        StartCoroutine(DropStones(upgradeItems, 3)); // Specify number of items to drop
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
        for (int i = 0; i < 4; i++)
        {
            int goldAmount = (int)(Random.value * 299f + 15f);
            int goldDropIndex = goldAmount < 50 ? 0 : goldAmount < 250 ? 1 : 2;
            Vector3 dropPosition = RandomDropPosition();
            GameObject goldDrop = Instantiate(goldItems[goldDropIndex], dropPosition, transform.rotation);
            GoldDropBehaviour goldDropBehaviour = goldDrop.GetComponent<GoldDropBehaviour>();
            if (goldDropBehaviour != null)
            {
                goldDropBehaviour.direction = DirectionModifier(dropPosition);
            }
            goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
            goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator DropGear(List<GameObject> items, int numItems)
    {
        List<GameObject> selectedItems = new List<GameObject>();

        while (selectedItems.Count < numItems)
        {
            GameObject item = items[Random.Range(0, items.Count)];

            if (!selectedItems.Contains(item))
            {
                selectedItems.Add(item);
            }
        }

        foreach (GameObject item in selectedItems)
        {
            // Randomly determine if the dropped gear is level 11 or 12
            int gearLevel = Random.value < 0.7f ? chestLvl : chestLvl + 1; // 70% chance for level 11, 30% chance for level 12
            // Assuming the item prefab has a script that can set its level, adjust it here
            item.GetComponent<ItemInfo>().itemLvl = gearLevel;
            Vector3 dropPosition = RandomDropPosition();
            GameObject instantiatedObj = Instantiate(item, dropPosition, transform.rotation);
            ClickLootBehaviour clickLootBehaviour = instantiatedObj.transform.GetComponent<ClickLootBehaviour>();
            if (clickLootBehaviour != null)
            {
                clickLootBehaviour.direction = DirectionModifier(dropPosition);
            }
            if (item.GetComponent<ItemInfo>().itemQuality == "Legendary")
            {
                dropSource.PlayOneShot(dropClip);
            }
            yield return new WaitForSeconds(0.5f);
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

            yield return new WaitForSeconds(0.15f); // Adjust delay between drops if needed
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
            GameObject instantiatedObj = Instantiate(itemToDrop, dropPosition, transform.rotation);
            if (instantiatedObj.GetComponent<EnchantmentStoneInfo>())
            {
                instantiatedObj.GetComponent<EnchantmentStoneInfo>().amount = Random.Range(1, 4);
            }
            ClickLootBehaviour clickLootBehaviour = instantiatedObj.transform.GetComponent<ClickLootBehaviour>();
            if (clickLootBehaviour != null)
            {
                clickLootBehaviour.direction = DirectionModifier(dropPosition);
            }

            // Play sound if it's a legendary item
            if (rarity == "Legendary")
            {
                dropSource.PlayOneShot(dropClip);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            inRange = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            inRange = false;
    }
}
