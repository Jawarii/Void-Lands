using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayedHP : MonoBehaviour
{
    public Slider slider;
    public GameObject stats;
    public float hp1 = 0;
    public float maxHp1 = 0;
    public float oldSlider = 0;
    private bool updt = true;

    void Start()
    {
        hp1 = stats.GetComponent<PlayerStats>().hp;
        maxHp1 = stats.GetComponent<PlayerStats>().maxHp;
        slider.value = hp1 / maxHp1;
    }

    void Update()
    {
        maxHp1 = stats.GetComponent<PlayerStats>().maxHp;
        hp1 = stats.GetComponent<PlayerStats>().hp;
        if (stats.GetComponent<PlayerStats>().timeSince >= 0.5)
        {   if (updt)
            {
                oldSlider = slider.value;
                updt = false;
            }
            if (slider.value != hp1 / maxHp1)
            {
                slider.value -= (oldSlider - (hp1 / maxHp1)) * Time.deltaTime * 10;
                if (slider.value < hp1 / maxHp1)
                {
                    slider.value = hp1 / maxHp1;
                    updt = true;
                }
            }
            else updt = true;
        }        
    }
}
