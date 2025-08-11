using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [System.Serializable]
    public class SkillModificationNode
    {
        public SkillModificationSO modification;
        public List<int> prerequisites; // Indices of required modifications
        public Vector2 position; // UI position in the tree
    }

    [System.Serializable]
    public class SkillTree
    {
        public string skillName;
        public List<SkillModificationNode> modifications = new List<SkillModificationNode>();
    }

    public List<SkillTree> skillTrees = new List<SkillTree>();
    private Dictionary<string, List<SkillModificationSO>> activeModifications = new Dictionary<string, List<SkillModificationSO>>();

    public void ApplyModification(string skillName, SkillModificationSO modification)
    {
        if (!activeModifications.ContainsKey(skillName))
        {
            activeModifications[skillName] = new List<SkillModificationSO>();
        }
        
        activeModifications[skillName].Add(modification);
    }

    public void RemoveModification(string skillName, SkillModificationSO modification)
    {
        if (activeModifications.ContainsKey(skillName))
        {
            activeModifications[skillName].Remove(modification);
        }
    }

    public List<SkillModificationSO> GetActiveModifications(string skillName)
    {
        if (activeModifications.ContainsKey(skillName))
        {
            return activeModifications[skillName];
        }
        return new List<SkillModificationSO>();
    }

    public bool CanApplyModification(string skillName, SkillModificationSO modification, int playerLevel, int availableSkillPoints)
    {
        if (playerLevel < modification.requiredLevel || availableSkillPoints < modification.skillPointsCost)
        {
            return false;
        }

        // Check prerequisites
        var tree = skillTrees.Find(t => t.skillName == skillName);
        if (tree != null)
        {
            var node = tree.modifications.Find(n => n.modification == modification);
            if (node != null)
            {
                foreach (var prereqIndex in node.prerequisites)
                {
                    if (prereqIndex >= 0 && prereqIndex < tree.modifications.Count)
                    {
                        var prereqMod = tree.modifications[prereqIndex].modification;
                        if (!activeModifications.ContainsKey(skillName) || 
                            !activeModifications[skillName].Contains(prereqMod))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }
} 