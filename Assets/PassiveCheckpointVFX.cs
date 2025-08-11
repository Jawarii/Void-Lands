using UnityEngine;

public class PassiveCheckpointVFX : MonoBehaviour
{
    private float rotationSpeed = -25f;
    private float pulseSpeed = 2f;
    private float pulseStrength = 0.1f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Rotation
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Pulse
        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseStrength;
        transform.localScale = originalScale * (1f + scaleOffset);
    }
}
