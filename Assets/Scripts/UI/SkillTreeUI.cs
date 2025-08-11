using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkillTreeUI : MonoBehaviour
{
    public GameObject modificationNodePrefab;
    public GameObject connectionLinePrefab;
    public Transform nodesContainer;
    public Transform linesContainer;
    public TextMeshProUGUI skillPointsText;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescriptionText;
    
    private SkillTreeManager skillTreeManager;
    private SkillsSO currentSkill;
    private List<GameObject> nodeObjects = new List<GameObject>();
    private List<GameObject> lineObjects = new List<GameObject>();
    
    private void Start()
    {
        skillTreeManager = FindObjectOfType<SkillTreeManager>();
        if (skillTreeManager == null)
        {
            Debug.LogError("SkillTreeManager not found in scene!");
        }
    }
    
    public void ShowSkillTree(SkillsSO skill)
    {
        currentSkill = skill;
        ClearUI();
        
        if (skill == null || skillTreeManager == null) return;
        
        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.skillDescription;
        
        var tree = skillTreeManager.skillTrees.Find(t => t.skillName == skill.skillName);
        if (tree == null) return;
        
        // Create nodes
        foreach (var node in tree.modifications)
        {
            GameObject nodeObj = Instantiate(modificationNodePrefab, nodesContainer);
            nodeObj.transform.localPosition = node.position;
            
            // Set up node UI
            var icon = nodeObj.transform.Find("Icon").GetComponent<Image>();
            var nameText = nodeObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var levelText = nodeObj.transform.Find("Level").GetComponent<TextMeshProUGUI>();
            
            icon.sprite = node.modification.icon;
            nameText.text = node.modification.modificationName;
            levelText.text = "Lv. " + node.modification.requiredLevel;
            
            // Set up button
            var button = nodeObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnModificationNodeClicked(node.modification));
            
            // Check if modification is active
            var activeMods = skillTreeManager.GetActiveModifications(skill.skillName);
            if (activeMods.Contains(node.modification))
            {
                nodeObj.GetComponent<Image>().color = Color.green;
            }
            
            nodeObjects.Add(nodeObj);
        }
        
        // Create connection lines
        for (int i = 0; i < tree.modifications.Count; i++)
        {
            var node = tree.modifications[i];
            foreach (var prereqIndex in node.prerequisites)
            {
                if (prereqIndex >= 0 && prereqIndex < tree.modifications.Count)
                {
                    GameObject line = Instantiate(connectionLinePrefab, linesContainer);
                    var lineRenderer = line.GetComponent<LineRenderer>();
                    
                    Vector3 startPos = tree.modifications[prereqIndex].position;
                    Vector3 endPos = node.position;
                    
                    lineRenderer.SetPosition(0, startPos);
                    lineRenderer.SetPosition(1, endPos);
                    
                    lineObjects.Add(line);
                }
            }
        }
    }
    
    private void OnModificationNodeClicked(SkillModificationSO modification)
    {
        if (currentSkill == null || skillTreeManager == null) return;
        
        var playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats == null) return;
        
        bool canApply = skillTreeManager.CanApplyModification(
            currentSkill.skillName,
            modification,
            (int)playerStats.lvl,
            playerStats.skillPoints
        );
        
        if (canApply)
        {
            skillTreeManager.ApplyModification(currentSkill.skillName, modification);
            currentSkill.ApplyModification(modification);
            playerStats.skillPoints -= modification.skillPointsCost;
            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        if (currentSkill == null) return;
        
        var playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            skillPointsText.text = "Skill Points: " + playerStats.skillPoints;
        }
        
        ShowSkillTree(currentSkill);
    }
    
    private void ClearUI()
    {
        foreach (var obj in nodeObjects)
        {
            Destroy(obj);
        }
        nodeObjects.Clear();
        
        foreach (var obj in lineObjects)
        {
            Destroy(obj);
        }
        lineObjects.Clear();
    }
} 