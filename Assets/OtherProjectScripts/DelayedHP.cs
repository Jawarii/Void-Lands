using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayedHP : MonoBehaviour
{
    public Slider slider;
    public GameObject stats;
    public float currentHp1 = 0;
    public float maxHp1 = 0;
    public float oldSlider = 0;
    private bool updt = true;

    void Start()
    {
        currentHp1 = stats.GetComponent<PlayerStats>().currentHp;
        maxHp1 = stats.GetComponent<PlayerStats>().maxHp;
        slider.value = currentHp1 / maxHp1;
    }

    void Update()
    {
        maxHp1 = stats.GetComponent<PlayerStats>().maxHp;
        currentHp1 = stats.GetComponent<PlayerStats>().currentHp;
        if (stats.GetComponent<PlayerStats>().timeSince >= 0.5)
        {   if (updt)
            {
                oldSlider = slider.value;
                updt = false;
            }
            if (slider.value != currentHp1 / maxHp1)
            {
                slider.value -= (oldSlider - (currentHp1 / maxHp1)) * Time.deltaTime * 10;
                if (slider.value < currentHp1 / maxHp1)
                {
                    slider.value = currentHp1 / maxHp1;
                    updt = true;
                }
            }
            else updt = true;
        }        
    }
}
