using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // For the TextMeshProUGUI component

public class PotionController : MonoBehaviour
{
    public GameObject player;
    public PlayerStats playerStats;

    public float instantPotEffect = 0.1f;   // Instant healing amount (percentage of max HP)
    public float overtimePotEffect = 0.15f; // Healing amount over time (percentage of max HP)
    public float overtimeDuration = 3f;     // Duration for healing over time
    public float potionCooldown = 10f;      // Cooldown time in seconds
    public float scaleDuration = 0.05f;      // Duration for the scaling effect
    public float scaleFactor = 0.8f;        // Scale down factor (e.g., 0.8 for 80% size)

    private bool isOnCooldown = false;      // Tracks if the potion is on cooldown
    private float cooldownTimer;            // Tracks cooldown time
    private Image cooldownImage;            // Image to show cooldown progress
    private TMP_Text cooldownText;   // Text for displaying cooldown time
    private Transform potionImageTransform; // Transform of the potion image (child 0)

    public AudioSource audioSource;
    public AudioClip potionAudioClip;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        audioSource = GameObject.Find("BuffsSoundSource").GetComponent<AudioSource>();

        // Find the Image component for the cooldown in the children
        cooldownImage = transform.GetChild(1).GetComponentInChildren<Image>();
        potionImageTransform = transform.GetChild(0); // Get the transform of the potion image (child 0)

        // Find the TextMeshProUGUI component for cooldown text
        cooldownText = transform.Find("CooldownText").GetComponent<TMP_Text>();

        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // Initially empty (cooldown not active)
        }

        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(false); // Cooldown text is initially hidden
        }
    }

    void Update()
    {
        // Check for potion use input (key '1') and if potion is off cooldown
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isOnCooldown)
        {
            UsePotion();
        }

        // If the potion is on cooldown, update the timer and the image fill amount
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownImage != null)
            {
                // Update the fill amount with the remaining cooldown time
                cooldownImage.fillAmount = cooldownTimer / potionCooldown;
            }

            if (cooldownText != null)
            {
                // Update the cooldown text with one decimal place
                cooldownText.text = cooldownTimer.ToString("F1");
            }

            // Reset cooldown if timer is up
            if (cooldownTimer <= 0)
            {
                isOnCooldown = false;
                if (cooldownImage != null)
                {
                    cooldownImage.fillAmount = 0f; // Reset image to empty
                }

                if (cooldownText != null)
                {
                    cooldownText.gameObject.SetActive(false); // Hide cooldown text
                }
            }
        }
    }

    void UsePotion()
    {
        audioSource.PlayOneShot(potionAudioClip);
        // Instantly heal the player by the instant potion effect
        int healAmount = Mathf.FloorToInt(playerStats.maxHp * instantPotEffect);
        playerStats.currentHp = Mathf.Min(playerStats.currentHp + healAmount, playerStats.maxHp);

        // Start the over-time healing coroutine
        StartCoroutine(HealOverTime());

        // Start the cooldown timer
        StartCooldown();

        // Start the immediate scale change to indicate the click
        StartCoroutine(ScalePotionImage());
    }

    IEnumerator HealOverTime()
    {
        int totalOvertimeHeal = Mathf.FloorToInt(playerStats.maxHp * overtimePotEffect);
        int healPerTick = Mathf.FloorToInt(totalOvertimeHeal / (overtimeDuration / 0.5f)); // Healing amount every 0.5 seconds
        float tickInterval = 0.5f; // Interval for healing ticks

        float elapsedTime = 0;

        // Apply healing every 0.5 seconds for 3 seconds
        while (elapsedTime < overtimeDuration)
        {
            playerStats.currentHp = Mathf.Min(playerStats.currentHp + healPerTick, playerStats.maxHp);
            elapsedTime += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = potionCooldown;

        // Set the image fill amount to the start of the cooldown
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f; // Start full and decrease over time
        }

        // Show the cooldown text
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = potionCooldown.ToString("F1"); // Display the full cooldown initially
        }
    }

    // Coroutine to handle immediate scale down and scale back up
    IEnumerator ScalePotionImage()
    {
        // Scale down to 80% instantly
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Wait for the duration before scaling back
        yield return new WaitForSeconds(scaleDuration);

        // Scale back to original size
        transform.localScale = Vector3.one;
    }
}
