using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSkillBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool IsDragging; // Flag to indicate if a skill is being dragged

    private Transform draggedSkill;

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true; // Set flag to true when dragging starts
        draggedSkill = Instantiate(transform, transform.parent.parent, true);
        draggedSkill.localScale *= 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggedSkill.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        SkillButtonInformation skillButtonInformation;

        foreach (RaycastResult result in results)
        {
            skillButtonInformation = result.gameObject.GetComponent<SkillButtonInformation>();
            if (skillButtonInformation != null)
            {
                if (skillButtonInformation.skillType == draggedSkill.GetComponent<SkillsWindowSlotInfo>()._skillSo.skillType)
                {
                    skillButtonInformation.skillSo = draggedSkill.GetComponent<SkillsWindowSlotInfo>()._skillSo;
                    skillButtonInformation.skillsScript = draggedSkill.GetComponent<SkillsWindowSlotInfo>().skillsScript;
                    skillButtonInformation.newAnimatorController = draggedSkill.GetComponent<SkillsWindowSlotInfo>().newAnimatorController;
                    result.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = draggedSkill.GetChild(0).GetComponent<Image>().sprite.texture;
                    result.gameObject.transform.GetChild(0).GetComponent<RawImage>().enabled = true;
                    Debug.Log("Done");
                    break;
                }
            }
        }
        IsDragging = false; // Reset flag when dragging ends
        Destroy(draggedSkill.gameObject);
    }
}
