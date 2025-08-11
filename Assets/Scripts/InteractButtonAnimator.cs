using System.Collections;
using UnityEngine;

public class InteractButtonAnimator : MonoBehaviour
{
    // Target scales
    public float minScale = 0.8f;
    public float maxScale = 1f;

    // Duration for one full scale cycle (1 -> 0.8 -> 1)
    public float cycleDuration = 0.5f;

    public GameObject interactableGo;

    void Start()
    {
        // Start the scaling coroutine
        StartCoroutine(ScaleRoutine());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    // Coroutine to handle scaling the button endlessly
    IEnumerator ScaleRoutine()
    {
        while (true)
        {
            float timeElapsed = 0f;

            while (timeElapsed < cycleDuration)
            {
                // Calculate the new scale using PingPong for smooth back and forth scaling
                float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(timeElapsed / cycleDuration * 2, 1));

                // Apply the scale to the transform
                transform.localScale = new Vector3(scale, scale, 1f);

                // Increment the elapsed time
                timeElapsed += Time.deltaTime;

                yield return null;
            }
        }
    }
    public void Interact()
    {
        if (interactableGo == null)
            return;

        // Check for TeleportBehaviour
        TeleportBehaviour teleportBehaviour = interactableGo.GetComponent<TeleportBehaviour>();
        if (teleportBehaviour != null)
        {
            teleportBehaviour.InvokeTeleportBehaviour();
            return;
        }

        // Check for InteractableNPCBehaviour
        InteractableNPCBehaviour npcBehaviour = interactableGo.GetComponent<InteractableNPCBehaviour>();
        if (npcBehaviour != null)
        {
            npcBehaviour.InvokeNPCInteraction(); // or whatever method handles the dialogue
            return;
        }
    }
}
