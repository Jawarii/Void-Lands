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

    void Start()
    {
        objShaker = GetComponent<ObjectShaker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    IEnumerator DropLoot()
    {
        StartCoroutine(DropGold());
        StartCoroutine(DropGear(gearItems, 2)); // Specify number of items to drop
        StartCoroutine(DropStones(upgradeItems, 15)); // Drop one stone
        yield return null;
    }

    private Vector3 RandomDropPosition()
    {
        float minRadius = 0.65f;  // Minimum distance from the chest
        float maxRadius = 1.3f;  // Maximum distance for dropping items
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad; // Convert degrees to radians

        float randomRadius = Random.Range(minRadius, maxRadius);
        Vector2 randomPoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randomRadius;

        return transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
    }

    IEnumerator DropGold()
    {
        for (int i = 0; i < 25; i++)
        {
            int goldAmount = (int)(Random.value * 1500f);
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
            Vector3 dropPosition = RandomDropPosition();
            Instantiate(item, dropPosition, transform.rotation);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator DropStones(List<GameObject> items, int numItems)
    {
        for (int i = 0; i < numItems; i++)
        {
            GameObject item = items[Random.Range(0, items.Count)];
            Vector3 dropPosition = RandomDropPosition();
            Instantiate(item, dropPosition, transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (isOpen)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown("f"))
            {
                StartCoroutine(OpenChest());
            }
        }
    }
}
