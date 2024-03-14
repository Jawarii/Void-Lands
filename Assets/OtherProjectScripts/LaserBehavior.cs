using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserBehavior : MonoBehaviour
{
    public Color startColor;
    public Color endColor;
    public float lerpDuration = 2.0f; // Duration of the color interpolation in seconds
    public float lerpTime = 0.0f;

    private Renderer renderer; // Reference to the object's renderer (you might need to adjust this)

    private bool isIncreasing = true;

    private void Start()
    {
        renderer = GetComponent<Renderer>(); // Assuming the script is attached to an object with a renderer
    }

    private void Update()
    {
        // Increment the lerpTime based on time
        if (isIncreasing)
        {
            lerpTime += Time.deltaTime;
        }
        if (!isIncreasing)
        {
            lerpTime -= Time.deltaTime;
        }

        // Calculate the interpolation factor (t) between 0 and 1
        float t = Mathf.Clamp01(lerpTime / lerpDuration);

        // Use Color.Lerp to interpolate between startColor and endColor
        Color lerpedColor = Color.Lerp(startColor, endColor, t);

        // Apply the lerped color to the object's material or color property
        renderer.material.color = lerpedColor;

        // Reset lerpTime if it exceeds lerpDuration (optional)
        if (lerpTime >= lerpDuration && isIncreasing)
        {
            //lerpTime = 0.0f;
            isIncreasing = false;
        }
        if (lerpTime <= 0 && !isIncreasing)
        {
            //lerpTime = lerpDuration;
            isIncreasing = true;
        }
    }
}
