using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnInput : MonoBehaviour
{
    // Reference to the player's stats
    public PlayerStats playerStats;
    private Vector3 initialPosition;
    private float initialHealth;

    // Reference to the player's attack script
    public PlayerAttackArcher playerAttack;

    // Reference to the CastingCanvas and its slider
    private GameObject castingCanvas;
    private Slider castingSlider;

    // Name of the default scene to teleport to when pressing "T"
    public string defaultSceneName;

    // Reference to the currently running coroutine
    private Coroutine currentTeleportCoroutine;

    public AudioSource audioSource;
    public AudioClip castingClip;

    // Reference to the teleport effect GameObject and its animator
    public GameObject teleportEffectPrefab;
    private GameObject teleportEffectInstance;
    private Animator teleportEffectAnimator;

    // Update is called once per frame
    void Update()
    {
        // Check if the player presses the "T" key for default teleportation
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (playerStats.transform.GetComponent<PlayerMovement>().isMoving == false)
            {
                playerAttack = playerStats.transform.GetComponentInChildren <PlayerAttackArcher>();
                initialPosition = playerStats.transform.position;
                initialHealth = playerStats.currentHp;
                audioSource = playerStats.gameObject.GetComponent<AudioSource>();
                audioSource.Stop();
                audioSource.PlayOneShot(castingClip);
                StartTeleportation(defaultSceneName, 3f);
            }
        }
    }

    // Public method to load a scene with a specified name (used by portals)
    public void LoadScene(string sceneName)
    {
        // Store the player's initial position and health
        initialPosition = playerStats.transform.position;
        initialHealth = playerStats.currentHp;

        // Start the teleport to the specified scene
        StartTeleportation(sceneName, 1f);
    }

    // Method to start teleportation with cancellation handling
    private void StartTeleportation(string sceneName, float delay)
    {
        // If there is an ongoing teleport, cancel it
        if (currentTeleportCoroutine != null)
        {
            StopCoroutine(currentTeleportCoroutine);
            DisableCastingCanvas();
            DisableTeleportEffect();
        }

        // Start a new teleportation coroutine
        currentTeleportCoroutine = StartCoroutine(LoadSceneAfterDelay(sceneName, delay));
    }

    // Coroutine to handle the delay and scene loading
    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        float timeElapsed = 0f;

        // Find the CastingCanvas GameObject in the scene
        castingCanvas = GameObject.Find("CastingCanvas");

        if (castingCanvas != null)
        {
            // Enable the CastingCanvas
            castingCanvas.gameObject.GetComponent<Canvas>().enabled = true;
            // Get the slider component from the first child
            castingSlider = castingCanvas.transform.GetChild(0).GetComponent<Slider>();
            castingSlider.value = 0f;  // Reset the slider value to 0
        }

        // Instantiate and enable the teleport effect at the player's position
        EnableTeleportEffect();

        while (timeElapsed < delay)
        {
            // Update the slider value based on timeElapsed / delay
            if (castingSlider != null)
            {
                castingSlider.value = timeElapsed / delay;
            }

            // Check if teleportation should be cancelled (HP loss, movement, or attack)
            if (playerStats.currentHp < initialHealth * 0.95f)
            {
                Debug.Log("Teleportation cancelled: Player lost more than 5% HP.");
                DisableCastingCanvas();
                DisableTeleportEffect();
                audioSource.Stop();
                yield break;  // Cancel teleportation
            }

            if (playerStats.transform.GetComponent<PlayerMovement>().isMoving == true)
            {
                Debug.Log("Teleportation cancelled: Player moved.");
                DisableCastingCanvas();
                DisableTeleportEffect();
                audioSource.Stop();
                yield break;  // Cancel teleportation
            }

            if (playerAttack.isAttacking)
            {
                Debug.Log("Teleportation cancelled: Player attacked.");
                DisableCastingCanvas();
                DisableTeleportEffect();
                audioSource.Stop();
                yield break;  // Cancel teleportation
            }

            // Wait for the next frame and increment timeElapsed
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Load the specified scene after the delay if no conditions were met
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            DisableCastingCanvas();
            DisableTeleportEffect();
            GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.Find("RespawnLocations").transform.GetChild(0).transform.position;
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }

        DisableTeleportEffect();  // Disable effect after teleportation is complete
    }

    // Method to instantiate and enable the teleport effect
    private void EnableTeleportEffect()
    {
        Vector3 animationPos = new Vector3(playerStats.transform.position.x, playerStats.transform.position.y - 0.2f, playerStats.transform.position.z);

        if (teleportEffectInstance == null)
        {
            // Instantiate the teleport effect at the player's position
            teleportEffectInstance = Instantiate(teleportEffectPrefab, animationPos, Quaternion.identity);
            teleportEffectAnimator = teleportEffectInstance.GetComponent<Animator>();
        }
        else
        {
            // Re-enable the effect if it was disabled

            teleportEffectInstance.transform.position = animationPos;
            teleportEffectInstance.SetActive(true);
        }

        // Play the teleportation animation
        if (teleportEffectAnimator != null)
        {
            teleportEffectAnimator.SetTrigger("TeleportStart");
        }
    }

    // Method to disable the teleport effect
    private void DisableTeleportEffect()
    {
        if (teleportEffectInstance != null)
        {
            teleportEffectInstance.SetActive(false);
        }
    }

    // Method to disable the casting canvas
    private void DisableCastingCanvas()
    {
        if (castingCanvas != null)
        {
            castingCanvas.gameObject.GetComponent<Canvas>().enabled = false;
        }
    }
}
