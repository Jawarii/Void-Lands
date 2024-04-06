using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LiveHP : MonoBehaviour
{
    public Slider slider;
    public GameObject stats;
    public float maxHp1 = 0;
    public float hp1 = 0;
    public TMP_Text hpValueText;
    
    void Start()
    {
        maxHp1 = stats.GetComponent<PlayerStats>().maxHp;
        hp1 = stats.GetComponent<PlayerStats>().currentHp;
        slider.value = hp1 / maxHp1;
        hpValueText.SetText(hp1.ToString() + "/" + maxHp1.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        maxHp1 = stats.GetComponent<PlayerStats>().maxHp;
        hp1 = stats.GetComponent<PlayerStats>().currentHp;
        slider.value = hp1 / maxHp1;
        hpValueText.SetText(hp1.ToString() + "/" + maxHp1.ToString());
    }
}
