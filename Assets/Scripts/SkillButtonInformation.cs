using UnityEngine;
using UnityEngine.UI;

public class SkillButtonInformation : MonoBehaviour
{
    public SkillsSO _skillSo;
    public GameObject skillsScript;
    public RuntimeAnimatorController newAnimatorController;
    void Start()
    {
        if (_skillSo != null && skillsScript != null)
        {
            transform.GetComponentInChildren<RawImage>().enabled = true;
            transform.GetComponentInChildren<RawImage>().texture = _skillSo.skillIcon.texture;
            
        }
        else
        {
            transform.GetComponentInChildren<RawImage>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_skillSo == null || skillsScript == null)
        {
            transform.GetComponentInChildren<RawImage>().enabled = false;
        }
    }
}
