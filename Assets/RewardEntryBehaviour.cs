using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardEntryBehaviour : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeInDuration = 0.25f;
    public float fadeOutDuration = 1f;
    public float lifetime = 2f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Start fully transparent for fade-in
    }

    public void BeginFadeAndDestroy(float delay)
    {
        StartCoroutine(FadeInOutSequence(delay));
    }

    private IEnumerator FadeInOutSequence(float delay)
    {
        // Fade In
        float elapsedIn = 0f;
        while (elapsedIn < fadeInDuration)
        {
            elapsedIn += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedIn / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Wait at full opacity
        yield return new WaitForSeconds(delay);

        // Fade Out
        float elapsedOut = 0f;
        while (elapsedOut < fadeOutDuration)
        {
            elapsedOut += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedOut / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        Destroy(gameObject);
    }
}
