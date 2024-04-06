using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillsWindowSlotInfo : MonoBehaviour
{
    public SkillsSO _skillSo;
    public GameObject skillsScript;
    public RuntimeAnimatorController newAnimatorController;
    void Start()
    {
        if (_skillSo != null && skillsScript != null)
        {
            transform.GetChild(0).GetComponent<Image>().enabled = true;
            transform.GetChild(0).GetComponent<Image>().sprite = _skillSo.skillIcon;
            transform.GetChild(1).GetComponent<TMP_Text>().text = "Lv." + _skillSo.skillLevel;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_skillSo == null || skillsScript == null)
        {
            transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
    }
}
