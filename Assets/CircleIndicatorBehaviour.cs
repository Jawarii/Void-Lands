using UnityEngine;

public class CircleIndicatorBehaviour : MonoBehaviour
{
    public GameObject fillCircle;
    public float castTime = 5f;
    private float elapsedTime = 0f;
    private Vector3 startScale;
    private Vector3 targetScale;

    void Awake()
    {
        fillCircle = transform.GetChild(0).gameObject;
        startScale = Vector3.zero;
        targetScale = transform.localScale;
    }

    void Start()
    {
        fillCircle.transform.localScale = startScale;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < castTime)
        {
            float t = elapsedTime / castTime;
            fillCircle.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        }
        else
        {
            fillCircle.transform.localScale = targetScale;
            Destroy(gameObject);
        }
    }
}
