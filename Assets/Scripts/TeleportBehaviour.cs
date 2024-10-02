using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBehaviour : MonoBehaviour
{
    // Reference to the Canvas that should be enabled
    public Canvas interactableCanvas;
    public GameObject teleportManager;

    // Name of the scene to load
    public string sceneName;

    // This method would be called to trigger some teleport behavior
    public void InvokeTeleportBehaviour()
    {
        // Implement your teleport logic here, for example, invoking the scene load
        Debug.Log("Teleport behavior invoked.");
        teleportManager.GetComponent<LoadSceneOnInput>().LoadScene(sceneName);
    }

    // This method will be called when another collider enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Enable the interactable canvas
            if (interactableCanvas != null)
            {
                interactableCanvas.enabled = true;
                interactableCanvas.GetComponentInChildren<InteractButtonAnimator>().interactableGo = transform.gameObject;
            }
            else
            {
                Debug.LogWarning("Interactable canvas is not assigned.");
            }
        }
    }

    // This method will be called when the collider exits the trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactableCanvas != null)
            {
                interactableCanvas.enabled = false;
                interactableCanvas.GetComponentInChildren<InteractButtonAnimator>().interactableGo = null;
            }
            else
            {
                Debug.LogWarning("Interactable canvas is not assigned.");
            }
        }
    }
}
