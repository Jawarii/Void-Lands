using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class LightningFlashController : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float maxAlpha = 0.6f;

    public void TriggerFlash() // <- This method will appear!
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        float t = 0f;
        Color color = flashImage.color;
        color.a = maxAlpha;
        flashImage.color = color;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(maxAlpha, 0f, t / flashDuration);
            flashImage.color = color;
            yield return null;
        }

        color.a = 0f;
        flashImage.color = color;
    }
}
