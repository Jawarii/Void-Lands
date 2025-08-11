using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DelayedHPEnemy : MonoBehaviour
{
    public Slider slider;
    public GameObject stats;
    public float hp1 = 0;
    public float maxHp1 = 0;
    public float delayedHp = 0;
    private Coroutine smoothUpdateCoroutine = null; // Reference to the active coroutine
    public Camera camera;
    public Transform target;
    public Vector3 offset;
    public bool isHud = false;

    void Start()
    {
        if (target == null && !isHud)
        {
            target = transform.parent.parent;
        }
        if (target == null)
            return;
        SetVariables();
        slider.value = 1f;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        SetVariables();
        // Only start coroutine if 0.3 seconds have passed
        if (stats.GetComponent<EnemyStats>().timeSince >= 0.3f)
        {
            // Start a new coroutine and cancel the previous one
            if (smoothUpdateCoroutine != null)
            {
                StopCoroutine(smoothUpdateCoroutine);
            }
            smoothUpdateCoroutine = StartCoroutine(SmoothSliderUpdate());
        }

        transform.rotation = camera.transform.rotation;

        // Adjust local scale based on target
        if (!isHud)
        {
            if (target.localScale.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private IEnumerator SmoothSliderUpdate()
    {
        float startValue = slider.value;
        float targetValue = hp1 / maxHp1;
        float elapsedTime = 0f;
        float duration = 1f / 20f; // Matches the original speed (20 * deltaTime)

        while (!Mathf.Approximately(slider.value, targetValue))
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        slider.value = targetValue;
        smoothUpdateCoroutine = null; // Reset coroutine reference when done
    }

    public void SetVariables()
    {
        stats = target.gameObject;
        maxHp1 = stats.GetComponent<EnemyStats>().maxHp;
        hp1 = stats.GetComponent<EnemyStats>().hp;
        delayedHp = stats.GetComponent<EnemyStats>().delayedHp;
        camera = Camera.main;
    }

    public void SetSlider()
    {
        slider.value = delayedHp / maxHp1;
    }
}
