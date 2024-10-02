using Cinemachine;
using System.Collections;
using UnityEngine;

public class ReaperBossSafeTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;

    // Target orthographic size
    public float targetOrthoSize = 3.5f;
    public AudioSource ostSource;
    public AudioClip safeOstClip;
    public Canvas bossCanvas;
    public float volumeTransitionDuration = 1f; // Duration of volume transition

    // Duration of the size change
    public float duration = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ChangeCameraSize());
            if (bossCanvas != null)
                bossCanvas.GetComponent<Canvas>().enabled = false;

            // Start the OST transition if a new clip is needed
            if (ostSource.clip != safeOstClip)
            {
                StartCoroutine(SmoothOstTransition(safeOstClip));
            }
        }
    }

    // Coroutine to smoothly change the camera size
    IEnumerator ChangeCameraSize()
    {
        float startOrthoSize = mainCamera.m_Lens.OrthographicSize;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            mainCamera.m_Lens.OrthographicSize = Mathf.Lerp(startOrthoSize, targetOrthoSize, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.m_Lens.OrthographicSize = targetOrthoSize;
    }

    // Coroutine to smoothly transition between OSTs
    IEnumerator SmoothOstTransition(AudioClip newClip)
    {
        // Fade out the current OST
        float startVolume = ostSource.volume;
        for (float t = 0; t < volumeTransitionDuration; t += Time.deltaTime)
        {
            ostSource.volume = Mathf.Lerp(startVolume, 0, t / volumeTransitionDuration);
            yield return null;
        }

        // Change the audio clip and play it
        ostSource.clip = newClip;
        ostSource.Play();

        // Fade in the new OST
        for (float t = 0; t < volumeTransitionDuration; t += Time.deltaTime)
        {
            ostSource.volume = Mathf.Lerp(0, startVolume, t / volumeTransitionDuration);
            yield return null;
        }
    }
}
