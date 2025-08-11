using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillAdder : MonoBehaviour
{
    public void AddSkillToBar(SkillsWindowSlotInfo skillSlotInfo, SkillButtonInformation targetSkillButton)
    {
        if (targetSkillButton.skillType == skillSlotInfo._skillSo.skillType)
        {
            // Assign the skill data to the target skill button
            targetSkillButton.skillSo = skillSlotInfo._skillSo;
            targetSkillButton.skillsScript = skillSlotInfo.skillsScript;
            targetSkillButton.newAnimatorController = skillSlotInfo.newAnimatorController;

            // Check if the skill icon is not null and then update the skill button UI with the new skill's icon
            RawImage skillImage = targetSkillButton.transform.GetChild(0).GetComponent<RawImage>();
            Sprite skillSprite = skillSlotInfo.transform.GetChild(0).GetComponent<Image>().sprite;

            if (skillSprite != null)
            {
                skillImage.texture = skillSprite.texture;
                skillImage.enabled = true;
            }
            else
            {
                //Debug.LogWarning("Skill icon is missing!");
            }

            //Debug.Log("Skill added directly to the skill bar.");
        }
        else
        {
            //Debug.LogWarning("Skill type mismatch! Cannot add the skill to the target slot.");
        }
    }

}
