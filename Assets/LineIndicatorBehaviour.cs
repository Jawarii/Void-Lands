using UnityEngine;

public class LineIndicatorBehaviour : MonoBehaviour
{
    public GameObject fill;
    public float castTime = 5f;
    private float elapsedTime = 0f;
    private Vector3 startScale;
    private Vector3 targetScale;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    void Awake()
    {
        fill = transform.GetChild(0).gameObject;
        targetScale = new Vector3(1, 1, 1);
        startScale = new Vector3(1, 0, 1);
        startPosition = new Vector3(0, 0.5f, 0);
        targetPosition = new Vector3(0, 0, 0);
    }

    void Start()
    {
        fill.transform.localScale = startScale;
        fill.transform.localPosition = startPosition;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < castTime)
        {
            float t = elapsedTime / castTime;
            fill.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            fill.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
        }
        else
        {
            fill.transform.localScale = targetScale;
            fill.transform.localPosition = targetPosition;
            Destroy(transform.parent.gameObject);
        }
    }
}
