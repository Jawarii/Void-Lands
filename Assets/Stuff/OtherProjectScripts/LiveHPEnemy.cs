using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

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

    void Start()
    {
        if (target == null)
        {
            target = transform.parent.parent;
        }
        stats = target.gameObject;
        hp1 = stats.GetComponent<EnemyStats>().hp;
        maxHp1 = stats.GetComponent<EnemyStats>().maxHp;
        slider.value = hp1 / maxHp1;
        camera = Camera.main;
        if (hpPercentText != null)
        {
            float percentage = Mathf.Round(slider.value * 100f * 100f) / 100f;

            // Check if there's a decimal part
            if (percentage % 1 == 0)
            {
                // No decimal part, show as an integer
                hpPercentText.text = percentage.ToString("F0") + "%";
            }
            else if (percentage * 10 % 10 == 0)
            {
                // Only one decimal place (e.g., .x0), show 1 decimal place
                hpPercentText.text = percentage.ToString("F1") + "%";
            }
            else
            {
                // Show 2 decimal places
                hpPercentText.text = percentage.ToString("F2") + "%";
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        maxHp1 = stats.GetComponent<EnemyStats>().maxHp;
        hp1 = stats.GetComponent<EnemyStats>().hp;
        slider.value = hp1 / maxHp1;
        transform.rotation = camera.transform.rotation;
        if (hpPercentText != null)
        {
            float percentage = Mathf.Round(slider.value * 100f * 100f) / 100f;

            // Check if there's a decimal part
            if (percentage % 1 == 0)
            {
                // No decimal part, show as an integer
                hpPercentText.text = percentage.ToString("F0") + "%";
            }
            else if (percentage * 10 % 10 == 0)
            {
                // Only one decimal place (e.g., .x0), show 1 decimal place
                hpPercentText.text = percentage.ToString("F1") + "%";
            }
            else
            {
                // Show 2 decimal places
                hpPercentText.text = percentage.ToString("F2") + "%";
            }

        }
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
