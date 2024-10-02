using Cinemachine;
using System.Collections;
using UnityEngine;

public class ReaperBossZoneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    public float targetOrthoSize = 5f;
    public AudioSource ostSource;
    public AudioClip bossOstClip;
    public Canvas bossCanvas;
    public float volumeTransitionDuration = 1f;
    public float duration = 1f;

    private void Start()
    {
        // Preload the boss OST by loading it into memory without playing it
        AudioClip preloadClip = bossOstClip;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ChangeCameraSize());

            if (bossCanvas != null)
                bossCanvas.enabled = true;

            if (ostSource.clip != bossOstClip)
            {
                StartCoroutine(SmoothOstTransition(bossOstClip));
            }
        }
    }

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

    IEnumerator SmoothOstTransition(AudioClip newClip)
    {
        float startVolume = ostSource.volume;
        for (float t = 0; t < volumeTransitionDuration; t += Time.deltaTime)
        {
            ostSource.volume = Mathf.Lerp(startVolume, 0, t / volumeTransitionDuration);
            yield return null;
        }

        ostSource.clip = newClip;
        ostSource.Play();

        for (float t = 0; t < volumeTransitionDuration; t += Time.deltaTime)
        {
            ostSource.volume = Mathf.Lerp(0, startVolume, t / volumeTransitionDuration);
            yield return null;
        }
    }
}
