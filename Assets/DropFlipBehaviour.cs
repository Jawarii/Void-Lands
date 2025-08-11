using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFlipBehaviour : MonoBehaviour
{
    public AudioSource dropsSource;
    public AudioClip dropClip;

    private List<GameObject> trackedItems = new List<GameObject>();

    private void Start()
    {
        dropsSource = GameObject.Find("DropAudioSource").GetComponent<AudioSource>();
    }
    void Update()
    {
        // Detect new items with ClickLootBehaviour in the scene
        ClickLootBehaviour[] allItems = FindObjectsOfType<ClickLootBehaviour>();

        foreach (ClickLootBehaviour item in allItems)
        {
            if (!trackedItems.Contains(item.gameObject))
            {
                // Add newly instantiated item to the tracked list
                trackedItems.Add(item.gameObject);

                // Start the flip animation
                StartCoroutine(FlipItem(item.gameObject, item.direction));
            }
        }

        // Detect new items with GoldDropBehaviour in the scene
        GoldDropBehaviour[] goldItems = FindObjectsOfType<GoldDropBehaviour>();

        foreach (GoldDropBehaviour item in goldItems)
        {
            if (!trackedItems.Contains(item.gameObject))
            {
                // Add newly instantiated item to the tracked list
                trackedItems.Add(item.gameObject);

                // Start the flip animation
                StartCoroutine(FlipItem(item.gameObject, 1));
            }
        }
    }

    private IEnumerator FlipItem(GameObject item, float directionModifier)
    {
        // Floating and spinning parameters for 2D
        float floatHeight = Random.Range(1.05f, 1.15f);  // How high the item floats
        float floatUpTime = 0.30f;  // Duration of the upward movement
        float floatDownTime = 0.20f;  // Duration of the downward movement (falling faster)
        float maxSpinSpeed = 1200f;  // Maximum spin speed (degrees per second)
        float minSpinSpeed = 300f;   // Minimum spin speed at the peak (slower rotation)

        Vector3 initialPosition = item.transform.position;

        // Use the direction modifier to determine the X offset
        float randomXOffset = directionModifier * Random.Range(0.35f, 0.55f);  // Adjusted random X offset based on direction modifier
        float randomYOffset = Random.Range(0.1f, -0.3f); // Y offset to ensure item lands slightly lower

        Vector3 destinationPosition = new Vector3(initialPosition.x + randomXOffset, initialPosition.y + randomYOffset, initialPosition.z);
        Vector3 peakPosition = new Vector3(initialPosition.x + (randomXOffset / 2), initialPosition.y + floatHeight, initialPosition.z);  // Float upwards while moving in X
        Quaternion initialRotation = item.transform.rotation;  // Store the initial rotation

        // Determine spin direction based on randomXOffset
        float spinDirection = randomXOffset < 0 ? 1f : -1f;  // If X offset is negative, spin clockwise, otherwise counterclockwise

        // Disable canvas during flip
        Canvas canvas = item.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.enabled = false;
        }

        float elapsedTime = 0f;

        // Upward motion with deceleration (slowing down as it reaches the peak) + steady X movement
        while (elapsedTime < floatUpTime && item != null)
        {
            elapsedTime += Time.deltaTime;

            // Ease out the upward motion to simulate deceleration
            float t = Mathf.Sin((elapsedTime / floatUpTime) * (Mathf.PI / 2));  // Easing function for smooth deceleration
            item.transform.position = Vector3.Lerp(initialPosition, peakPosition, t);  // Move steadily in both X and Y

            // Spin slower as the item reaches the peak
            float currentSpinSpeed = Mathf.Lerp(maxSpinSpeed, minSpinSpeed, t);  // Interpolate spin speed from max to min
            item.transform.Rotate(Vector3.forward * currentSpinSpeed * spinDirection * Time.deltaTime);  // Rotate on Z-axis

            yield return null;
        }

        // At the peak, we can spin the item briefly without moving
        yield return new WaitForSeconds(0.0f);  // Spin for a brief moment

        // Reset elapsed time for downward acceleration
        elapsedTime = 0f;

        // Downward motion with acceleration (speeding up as it falls) + steady X movement
        while (elapsedTime < floatDownTime && item != null)
        {
            elapsedTime += Time.deltaTime;

            // Ease in the downward motion to simulate acceleration (falling faster)
            float t = Mathf.Sin((elapsedTime / floatDownTime) * (Mathf.PI / 2));  // Easing function for smooth acceleration
            item.transform.position = Vector3.Lerp(peakPosition, destinationPosition, t);  // Move steadily in both X and Y

            // Spin faster as the item falls
            float currentSpinSpeed = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, t);  // Interpolate spin speed from min to max
            item.transform.Rotate(Vector3.forward * currentSpinSpeed * spinDirection * Time.deltaTime);  // Rotate on Z-axis

            yield return null;
        }

        // Ensure it reaches exactly the destination position and reset rotation
        if (item != null)
        {
            item.transform.position = destinationPosition;
            item.transform.rotation = initialRotation;
        }

        // Re-enable canvas after the flip
        if (canvas != null)
        {
            canvas.enabled = true;
        }

        // Play sound if it's a Legendary item
        ItemInfo info = item?.GetComponent<ItemInfo>();
        if (info != null && info.itemQuality == "Legendary")
        {
            dropsSource?.PlayOneShot(dropClip);
        }
    }
}
