using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class ArenaNotificationBehavior : MonoBehaviour
{
    public bool startCountDown;
    public bool isCountingDown;
    public Canvas canvas;  // Keep the canvas reference
    public CanvasGroup canvasGroup;  // Use CanvasGroup for better control over fading
    public TMP_Text countdownText;
    public float lerpDuration = 0.15f;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public GameObject spawner;

    public AudioClip ostClip;
    public AudioClip combatOstClip;
    public AudioSource ostAudioSource;

    //public ControlAllLights controlAllLights;
    //public Light2D globalLight;

    private void Start()
    {
        canvas = GetComponent<Canvas>();  // Ensure you add a Canvas component in the Unity Editor if not already added
        canvasGroup = GetComponent<CanvasGroup>(); // Ensure you add a CanvasGroup component in the Unity Editor
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>(); // Add CanvasGroup if it doesn't exist
        }
        isCountingDown = false;
        startCountDown = false;
        countdownText = GetComponentInChildren<TMP_Text>();
    }

    void FixedUpdate()
    {
        if (!isCountingDown && startCountDown)
        {
            StartCoroutine(Countdown());
            isCountingDown = true;
            startCountDown = false;
        }
    }

    IEnumerator Countdown()
    {
        countdownText.color = Color.red;
        int startSize = 200;  // Start font size
        int endSize = 40;     // End font size
        int finalMessageStartSize = 120;  // Start size for the final message
        int finalMessageEndSize = 18;    // End size for the final message
        float time;

        canvas.enabled = true;  // Make sure the canvas itself is enabled
        canvasGroup.alpha = 1;  // Ensure the canvas group is fully visible
        StartCoroutine(CrossfadeAudio(ostAudioSource, combatOstClip, 0.4f, 3f));
        for (int i = 3; i > 0; i--)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(audioClip);
            countdownText.text = i.ToString();
            time = 0f;
            while (time < lerpDuration)
            {
                time += Time.deltaTime;
                countdownText.fontSize = (int)Mathf.Lerp(startSize, endSize, time / lerpDuration);
                yield return null;
            }

            countdownText.fontSize = endSize;
            yield return new WaitForSeconds(1 - lerpDuration);
        }
        audioSource.PlayOneShot(audioClip);
        countdownText.text = "Survive the Onslaught!";
        time = 0f;  // Reset time for the new lerp
        while (time < lerpDuration)
        {
            time += Time.deltaTime;
            countdownText.fontSize = (int)Mathf.Lerp(finalMessageStartSize, finalMessageEndSize, time / lerpDuration);
            yield return null;
        }
        countdownText.fontSize = finalMessageEndSize;
        yield return new WaitForSeconds(1); // Display final message for 1 second

        // Start fading out the text
        time = 0f;
        float fadeDuration = 1f;  // Duration for the fade
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = 1 - (time / fadeDuration);
            yield return null;
        }
        isCountingDown = false;
        canvas.enabled = false;  // Optionally disable the canvas if no longer needed
        canvasGroup.alpha = 1;  // Reset alpha to ensure it's visible next time
        spawner.SetActive(true);
    }

    public IEnumerator Victory()
    {
        countdownText.color = Color.yellow;
        int finalMessageStartSize = 120;  // Start size for the final message
        int finalMessageEndSize = 24;    // End size for the final message
        float time;

        canvas.enabled = true;  // Make sure the canvas itself is enabled
        canvasGroup.alpha = 1;  // Ensure the canvas group is fully visible

        //audioSource.pitch = 2f;
        StartCoroutine(CrossfadeAudio(ostAudioSource, ostClip, 0.4f, 1f));
        countdownText.text = "Success!";
        time = 0f;  // Reset time for the new lerp
        while (time < lerpDuration)
        {
            time += Time.deltaTime;
            countdownText.fontSize = (int)Mathf.Lerp(finalMessageStartSize, finalMessageEndSize, time / lerpDuration);
            yield return null;
        }

        countdownText.fontSize = finalMessageEndSize;
        yield return new WaitForSeconds(1); // Display final message for 1 second

        // Start fading out the text
        time = 0f;
        float fadeDuration = 1f;  // Duration for the fade
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = 1 - (time / fadeDuration);
            yield return null;
        }
        canvas.enabled = false;  // Optionally disable the canvas if no longer needed
        canvasGroup.alpha = 1;  // Reset alpha to ensure it's visible next time
    }
    public void StartArena()
    {
        startCountDown = true;
    }

    IEnumerator CrossfadeAudio(AudioSource audioSource, AudioClip newClip, float newVolume, float crossfadeTime)
    {
        float originalVolume = audioSource.volume;

        // Fade out the current track
        for (float t = 0; t < crossfadeTime; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(originalVolume, 0, t / crossfadeTime);
            yield return null;
        }

        // Change the clip and start playing the new track
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in the new track
        for (float t = 0; t < crossfadeTime; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, newVolume, t / crossfadeTime);
            yield return null;
        }

        audioSource.volume = newVolume; // Ensure it sets to the exact volume after lerping
    }
}
