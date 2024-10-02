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
    private GameObject player;
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

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
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (_skillSo == null || skillsScript == null)
        {
            transform.GetChild(0).GetComponent<Image>().enabled = false;
            return; // Exit early if _skillSo or skillsScript is null
        }

        Image skillImage = transform.GetChild(0).GetComponentInChildren<Image>();
        TMP_Text text = transform.GetChild(1).GetComponentInChildren<TMP_Text>();
        Color color = skillImage.color;

        if (player.GetComponent<PlayerStats>().lvl >= int.Parse(_skillSo.skillLevel))
        {
            GetComponent<DraggableSkillBehaviour>().enabled = true;
            color.a = 1.0f; // Set alpha to fully opaque
            text.color = Color.white;
        }
        else
        {
            color.a = 0.1f; // Set alpha to semi-transparent
            GetComponent<DraggableSkillBehaviour>().enabled = false;
            text.color = Color.red;
        }

        // Reapply the updated color to the Image component
        skillImage.color = color;
    }

}
