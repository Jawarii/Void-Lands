using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTooltipHandler : MonoBehaviour
{
    public GameObject tooltipPrefab; // Assign the tooltip prefab in the Inspector
    private GameObject currentTooltip;

    private void Update()
    {
        if (DraggableSkillBehaviour.IsDragging)
        {
            // Destroy the tooltip if dragging starts
            if (currentTooltip != null)
            {
                Destroy(currentTooltip);
                //Debug.Log("Tooltip destroyed because dragging started.");
            }
            return; // Skip further processing while dragging
        }

        // Check if hovering over a UI element with SkillsWindowSlotInfo
        if (IsHoveringOverSkill(out SkillsWindowSlotInfo slotInfo))
        {
            if (currentTooltip == null)
            {
                //Debug.Log("Hovering over a skill. Creating tooltip.");

                // Instantiate the tooltip
                currentTooltip = Instantiate(tooltipPrefab, transform);

                // Populate the tooltip
                PopulateTooltip(slotInfo._skillSo);

                // Position the tooltip
                AdjustTooltipPosition(slotInfo);
            }
        }
        else
        {
            // Destroy the tooltip if hover ends
            if (currentTooltip != null)
            {
                //Debug.Log("No longer hovering. Destroying tooltip.");
                Destroy(currentTooltip);
            }
        }
    }

    private bool IsHoveringOverSkill(out SkillsWindowSlotInfo slotInfo)
    {
        slotInfo = null;

        // Check for UI element under the mouse
        var pointerEventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        var raycastResults = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (var result in raycastResults)
        {
            var skillSlotInfo = result.gameObject.GetComponent<SkillsWindowSlotInfo>();
            if (skillSlotInfo != null)
            {
                slotInfo = skillSlotInfo;
                //Debug.Log($"Hovering over skill: {slotInfo.name}");
                return true;
            }
        }

        return false;
    }

    private void PopulateTooltip(SkillsSO skill)
    {
        if (skill == null || currentTooltip == null) return;

        TMP_Text[] textComponents = currentTooltip.GetComponentsInChildren<TMP_Text>();
        Image[] imageComponents = currentTooltip.GetComponentsInChildren<Image>();

        foreach (var text in textComponents)
        {
            if (text.name == "SkillName")
            {
                text.text = skill.skillName;
                //Debug.Log($"Set Skill Name: {skill.skillName}");
            }
            else if (text.name == "SkillDescription")
            {
                if (skill.coolDown > 0)
                {
                    text.text = skill.skillDescription + "\n\n<color=#FFD700>Cooldown: " + skill.coolDown + "s</color>";
                }
                else
                {
                    text.text = skill.skillDescription;
                }
                //Debug.Log($"Set Skill Description: {skill.skillDescription}");
            }
            else if (text.name == "SkillRequirements")
            {
                text.text = "Requires Lv. " + skill.skillLevel;
                //Debug.Log($"Set Skill Requirements: Requires Lv. {skill.skillLevel}");
            }
        }

        foreach (var image in imageComponents)
        {
            if (image.name == "SkillIcon")
            {
                image.sprite = skill.skillIcon;
                //Debug.Log($"Set Skill Icon.");
            }
        }
    }

    private void AdjustTooltipPosition(SkillsWindowSlotInfo slotInfo)
    {
        // Get RectTransforms
        RectTransform skillRectTransform = slotInfo.GetComponent<RectTransform>();
        RectTransform tooltipRectTransform = currentTooltip.transform.Find("SkillTooltipPanel").GetComponent<RectTransform>();

        if (skillRectTransform == null || tooltipRectTransform == null)
        {
            //Debug.LogWarning("RectTransforms are null. Cannot adjust tooltip position.");
            return;
        }

        // Get the canvas the tooltip belongs to
        Canvas canvas = tooltipRectTransform.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            //Debug.LogWarning("Canvas is null. Cannot adjust tooltip position.");
            return;
        }

        // Force layout rebuild to ensure tooltip size is accurate
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRectTransform);

        // Re-log tooltip size after rebuild
        float tooltipWidth = tooltipRectTransform.rect.width;
        float tooltipHeight = tooltipRectTransform.rect.height;
        //Debug.Log($"Tooltip recalculated size: {tooltipWidth} x {tooltipHeight}");

        // Convert skill position to canvas space
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        Vector2 skillLocalPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                skillRectTransform.position,
                canvas.worldCamera,
                out skillLocalPos))
        {
            //Debug.LogWarning("Failed to convert skill position to local canvas space.");
            return;
        }

        //Debug.Log($"Skill local position in canvas: {skillLocalPos}");

        // Calculate the tooltip's position
        float offsetX = skillRectTransform.rect.width / 2f + tooltipWidth / 2f; // Offset to align to the right of the skill
        Vector2 tooltipPosition = skillLocalPos + new Vector2(offsetX, 0);

        //Debug.Log($"Tooltip initial position: {tooltipPosition}");

        // Clamp the position to canvas bounds
        Vector2 canvasSize = canvasRectTransform.sizeDelta;

        // Clamp vertically
        if (tooltipPosition.y + tooltipHeight / 2f > canvasSize.y / 2f)
        {
            tooltipPosition.y = canvasSize.y / 2f - tooltipHeight / 2f;
            //Debug.Log($"Clamped tooltip vertically (top): {tooltipPosition}");
        }
        else if (tooltipPosition.y - tooltipHeight / 2f < -canvasSize.y / 2f)
        {
            tooltipPosition.y = -canvasSize.y / 2f + tooltipHeight / 2f;
            //Debug.Log($"Clamped tooltip vertically (bottom): {tooltipPosition}");
        }

        // Clamp horizontally
        if (tooltipPosition.x + tooltipWidth / 2f > canvasSize.x / 2f)
        {
            tooltipPosition.x = canvasSize.x / 2f - tooltipWidth / 2f;
            //Debug.Log($"Clamped tooltip horizontally (right): {tooltipPosition}");
        }
        else if (tooltipPosition.x - tooltipWidth / 2f < -canvasSize.x / 2f)
        {
            tooltipPosition.x = -canvasSize.x / 2f + tooltipWidth / 2f;
            //Debug.Log($"Clamped tooltip horizontally (left): {tooltipPosition}");
        }

        // Apply the final anchored position
        tooltipRectTransform.anchoredPosition = tooltipPosition;

        //Debug.Log($"Final tooltip position: {tooltipPosition}");
    }
}
