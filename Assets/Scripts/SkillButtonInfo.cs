using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillButtonInfo
{

    public SkillsSO skillSo;
    public GameObject skillsScript;
    public RuntimeAnimatorController newAnimatorController;
    public string skillType;

    public SkillButtonInfo() { }
    public SkillButtonInfo(SkillButtonInformation skillButtonInformation)
    {
        skillSo = skillButtonInformation.skillSo;
        skillType = skillButtonInformation.skillType;
        skillsScript = skillButtonInformation.skillsScript;
        newAnimatorController = skillButtonInformation.newAnimatorController;
    }
}
