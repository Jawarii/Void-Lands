using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private float defaultFadeInDuration = 1.0f;
    [SerializeField] private float introFadeInDuration = 3.0f;
    [SerializeField] private float defaultFadeOutDuration = 1.0f;

    private float fadeInDuration;
    private float fadeOutDuration;
    private Image fadeImage;

    private void Start()
    {
        // Check if current scene is IntroScene and adjust fadeDuration
        if (SceneManager.GetActiveScene().name == "IntroScene" || SceneManager.GetActiveScene().name == "Level-1") // Change "IntroScene" if needed
        {
            fadeInDuration = introFadeInDuration;
        }
        else
        {
            fadeInDuration = defaultFadeInDuration;
        }

        fadeOutDuration = defaultFadeOutDuration;
        fadeImage = GetComponent<Image>();

        if (fadeImage == null)
        {
            Debug.LogError("No Image component found on this GameObject. Please attach this script to a UI Image.");
            return;
        }

        fadeImage.enabled = true;
        fadeImage.color = new Color(0, 0, 0, 1);

        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - (elapsedTime / fadeInDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.enabled = false;
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    public System.Collections.IEnumerator FadeOut()
    {
        fadeImage.enabled = true;

        float elapsedTime = 0;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / fadeOutDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
    }
}
