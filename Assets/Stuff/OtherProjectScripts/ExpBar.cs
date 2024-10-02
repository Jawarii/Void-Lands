using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text lvlText;
    public GameObject stats;
    public float maxExp = 0;
    public float currentExp = 0;
    public float lvl;
    public TMP_Text expPercentText;
    // Start is called before the first frame update
    void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player");
        lvl = stats.GetComponent<PlayerStats>().lvl;
        lvlText.SetText(lvl.ToString());
        maxExp = stats.GetComponent<PlayerStats>().maxExp;
        currentExp = stats.GetComponent<PlayerStats>().currentExp;
        slider.value = currentExp / maxExp;
        float percentage = Mathf.Round(slider.value * 100f * 100f) / 100f;

        // Check if there's a decimal part
        if (percentage % 1 == 0)
        {
            // No decimal part, show as an integer
            expPercentText.text = percentage.ToString("F0") + "%";
        }
        else if (percentage * 10 % 10 == 0)
        {
            // Only one decimal place (e.g., .x0), show 1 decimal place
            expPercentText.text = percentage.ToString("F1") + "%";
        }
        else
        {
            // Show 2 decimal places
            expPercentText.text = percentage.ToString("F2") + "%";
        }
    }

    // Update is called once per frame
    void Update()
    {
        maxExp = stats.GetComponent<PlayerStats>().maxExp;
        currentExp = stats.GetComponent<PlayerStats>().currentExp;
        lvl = stats.GetComponent<PlayerStats>().lvl;
        slider.value = currentExp / maxExp;
        lvlText.SetText(lvl.ToString());
        float percentage = Mathf.Round(slider.value * 100f * 100f) / 100f;

        // Check if there's a decimal part
        if (percentage % 1 == 0)
        {
            // No decimal part, show as an integer
            expPercentText.text = percentage.ToString("F0") + "%";
        }
        else if (percentage * 10 % 10 == 0)
        {
            // Only one decimal place (e.g., .x0), show 1 decimal place
            expPercentText.text = percentage.ToString("F1") + "%";
        }
        else
        {
            // Show 2 decimal places
            expPercentText.text = percentage.ToString("F2") + "%";
        }
    }
}
