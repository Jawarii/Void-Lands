using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LiveHPEnemy : MonoBehaviour
{
    public Slider slider;
    public GameObject stats;
    public float maxHp1 = 0;
    public float hp1 = 0;
    public Camera camera;
    public Transform target;
    public Vector3 offset;
    public TMP_Text hpPercentText;
    private GameObject enemyNameGo;
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

        // Find the child GameObject named "EnemyName" under the same Canvas
        enemyNameGo = transform.parent.Find("EnemyName")?.gameObject;

        //if (enemyNameGo == null)
        //{
        //    Debug.LogWarning("EnemyName GameObject not found under the parent Canvas.");
        //}

        if (hpPercentText != null)
        {
            UpdateHPPercentageText();
        }
    }
    void Update()
    {
        if (target == null)
            return;
        SetVariables();
        transform.rotation = camera.transform.rotation;

        if (hpPercentText != null)
        {
            UpdateHPPercentageText();
        }

        AdjustScaleForDirection();
    }
    private void UpdateHPPercentageText()
    {
        float percentage = Mathf.Round(slider.value * 100f * 100f) / 100f;

        if (percentage % 1 == 0)
        {
            hpPercentText.text = percentage.ToString("F0") + "%";
        }
        else if (percentage * 10 % 10 == 0)
        {
            hpPercentText.text = percentage.ToString("F1") + "%";
        }
        else
        {
            hpPercentText.text = percentage.ToString("F2") + "%";
        }
    }
    private void AdjustScaleForDirection()
    {
        if (isHud)
            return;
        if (target.localScale.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            if (enemyNameGo != null)
            {
                enemyNameGo.transform.localScale = new Vector3(-Mathf.Abs(enemyNameGo.transform.localScale.x), enemyNameGo.transform.localScale.y, enemyNameGo.transform.localScale.z);
            }
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            if (enemyNameGo != null)
            {
                enemyNameGo.transform.localScale = new Vector3(Mathf.Abs(enemyNameGo.transform.localScale.x), enemyNameGo.transform.localScale.y, enemyNameGo.transform.localScale.z);
            }
        }
    }
    public void SetVariables()
    {
        stats = target.gameObject;
        maxHp1 = stats.GetComponent<EnemyStats>().maxHp;
        hp1 = stats.GetComponent<EnemyStats>().hp;
        slider.value = hp1 / maxHp1;
        camera = Camera.main;
    }
}
