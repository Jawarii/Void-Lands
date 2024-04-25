using UnityEngine;
using UnityEngine.Rendering.Universal;
public class ControlAllLights : MonoBehaviour
{
    private Light2D[] allLights;

    void Start()
    {
        // This will gather all Light components from the children of this GameObject
        allLights = GetComponentsInChildren<Light2D>();
        ToggleLights(false);
    }

    public void ToggleLights(bool state)
    {
        // Enable or disable all lights
        foreach (Light2D light in allLights)
        {
            light.enabled = state;
        }
    }
}
