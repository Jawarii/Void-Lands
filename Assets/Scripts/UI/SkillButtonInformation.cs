using UnityEngine;
using UnityEngine.UI;

public class SkillButtonInformation : MonoBehaviour
{
    public SkillsSO skillSo;
    public GameObject skillsScript;
    public RuntimeAnimatorController newAnimatorController;
    public string skillType;
    void Start()
    {
        if (skillSo != null && skillsScript != null)
        {
            transform.GetComponentInChildren<RawImage>().enabled = true;
            transform.GetComponentInChildren<RawImage>().texture = skillSo.skillIcon.texture;

        }
        else
        {
            transform.GetComponentInChildren<RawImage>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (skillSo == null || skillsScript == null)
        {
            transform.GetComponentInChildren<RawImage>().enabled = false;
        }
    }
}
