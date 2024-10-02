using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillBarInfo : MonoBehaviour
{
    public List<SkillButtonInformation> skillButtonInfoList;
    public GameObject skillWindow;

    public void Awake()
    {
        UpdateSkillBar();
    }
    public void FixedUpdate()
    {
        UpdateSkillBar();
    }

    public void UpdateSkillBar()
    {
        skillButtonInfoList.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform slotTransform = transform.GetChild(i);
            SkillButtonInformation skillButtonInfo = slotTransform.GetComponent<SkillButtonInformation>();

            if (skillButtonInfo != null)
            {
                skillButtonInfoList.Add(skillButtonInfo);
            }
        }
    }
    public void AddSkills(List<string> skillNames)
    {
        //PrintSkillNamesTest(skillButtonInfoList);
        for (int i = 0; i < skillNames.Count; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                SkillsSO skillsSo = skillWindow.transform.GetChild(j).GetComponent<SkillsWindowSlotInfo>()._skillSo;
                SkillButtonInformation skillButton = new SkillButtonInformation();
                if (skillNames[i] == skillsSo.skillName)
                {
                    string skillType = skillsSo.skillType;

                    for (int l = 0; l < 4; l++)
                    {
                        string slotType = transform.GetChild(l).GetComponent<SkillButtonInformation>().skillType;
                        if (skillType == slotType)
                        {
                            skillButton = transform.GetChild(l).GetComponent<SkillButtonInformation>();
                            break;
                        }
                    }
                    SkillAdder skillAdder = new SkillAdder();
                    skillAdder.AddSkillToBar(skillWindow.transform.GetChild(j).GetComponent<SkillsWindowSlotInfo>(), skillButton);
                }
            }
        }
    }
    public void PrintSkillNamesTest(List<SkillButtonInformation> skillButtonInfoList)
    {
        skillButtonInfoList.ForEach(skillButton => { Debug.Log(skillButton.skillSo.skillName); });
    }
}
