using UnityEngine;
using System.Collections;

public class ObjectivePulse : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private float pulseScale = 1.2f;
    public float pulseDuration = 0.2f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void PlayPulse()
    {
        StopAllCoroutines();
        StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float time = 0f;
        Vector3 targetScale = originalScale * pulseScale;

        // Scale up
        while (time < pulseDuration / 2f)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, time / (pulseDuration / 2f));
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        rectTransform.localScale = targetScale;

        // Scale down
        time = 0f;
        while (time < pulseDuration / 2f)
        {
            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, time / (pulseDuration / 2f));
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        rectTransform.localScale = originalScale;
    }
}
