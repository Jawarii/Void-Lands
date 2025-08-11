using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance; // Singleton for easy access

    [SerializeField] private float shakeDuration = 0.5f; // Default duration
    [SerializeField] private float shakeMagnitude = 0.7f; // Default intensity
    private Vector3 originalPosition; // Store the original camera position
    private float shakeElapsedTime;

    private void Awake()
    {
        // Set up the Singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        originalPosition = transform.localPosition; // Store the initial position
    }

    /// <summary>
    /// Triggers a screen shake with optional custom duration and magnitude.
    /// </summary>
    /// <param name="duration">Duration of the shake (optional)</param>
    /// <param name="magnitude">Intensity of the shake (optional)</param>
    public void TriggerShake(float? duration = null, float? magnitude = null)
    {
        shakeDuration = duration ?? shakeDuration; // Use default if not specified
        shakeMagnitude = magnitude ?? shakeMagnitude;
        shakeElapsedTime = shakeDuration;

        StopAllCoroutines(); // Stop any ongoing shake
        StartCoroutine(Shake());
    }

    private System.Collections.IEnumerator Shake()
    {
        while (shakeElapsedTime > 0)
        {
            shakeElapsedTime -= Time.deltaTime;

            // Generate a random offset
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = originalPosition + randomOffset;

            yield return null; // Wait for the next frame
        }

        // Reset to the original position after shaking
        transform.localPosition = originalPosition;
    }
}
