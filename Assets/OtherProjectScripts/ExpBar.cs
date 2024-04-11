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
    // Start is called before the first frame update
    void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player");
        lvl = stats.GetComponent<PlayerStats>().lvl;
        lvlText.SetText(lvl.ToString());
        maxExp = stats.GetComponent<PlayerStats>().maxExp;
        currentExp = stats.GetComponent<PlayerStats>().currentExp;
        slider.value = currentExp / maxExp;
    }

    // Update is called once per frame
    void Update()
    {
        maxExp = stats.GetComponent<PlayerStats>().maxExp;
        currentExp = stats.GetComponent<PlayerStats>().currentExp;
        lvl = stats.GetComponent<PlayerStats>().lvl;
        slider.value = currentExp / maxExp;
        lvlText.SetText(lvl.ToString());
    }
}
