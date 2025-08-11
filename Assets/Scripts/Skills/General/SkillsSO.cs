using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills System/NewSkill")]
[System.Serializable]
public class SkillsSO : ScriptableObject
{
    public string skillName;
    [TextArea(5, 8)]
    public string skillDescription;
    public string skillLevel;
    public string skillType;
    public Sprite skillIcon;
    public SkillsScript skillScript;
    public Animator _animator;
    public float coolDown;

    // Base stats that can be modified
    public float baseDamage = 1f;
    public float baseDuration = 1f;
    public float baseRange = 1f;
    public float baseArea = 1f;
    
    // Current modified stats (calculated at runtime)
    [HideInInspector]
    public float currentDamage;
    [HideInInspector]
    public float currentDuration;
    [HideInInspector]
    public float currentRange;
    [HideInInspector]
    public float currentArea;
    [HideInInspector]
    public float currentCooldown;
    
    // Special effects
    [HideInInspector]
    public bool hasStun;
    [HideInInspector]
    public float stunDuration;
    [HideInInspector]
    public bool hasSlow;
    [HideInInspector]
    public float slowPercentage;
    [HideInInspector]
    public bool hasBleed;
    [HideInInspector]
    public float bleedDamage;
    [HideInInspector]
    public float bleedDuration;
    
    // Visual modifications
    [HideInInspector]
    public Color effectColor = Color.white;
    [HideInInspector]
    public GameObject currentProjectilePrefab;
    
    public void ResetModifications()
    {
        currentDamage = baseDamage;
        currentDuration = baseDuration;
        currentRange = baseRange;
        currentArea = baseArea;
        currentCooldown = coolDown;
        
        hasStun = false;
        stunDuration = 0f;
        hasSlow = false;
        slowPercentage = 0f;
        hasBleed = false;
        bleedDamage = 0f;
        bleedDuration = 0f;
        
        effectColor = Color.white;
        currentProjectilePrefab = null;
    }
    
    public void ApplyModification(SkillModificationSO modification)
    {
        currentDamage *= modification.damageMultiplier;
        currentDuration *= modification.durationMultiplier;
        currentRange *= modification.rangeMultiplier;
        currentArea *= modification.areaMultiplier;
        currentCooldown *= modification.cooldownMultiplier;
        
        if (modification.addsStun)
        {
            hasStun = true;
            stunDuration = modification.stunDuration;
        }
        
        if (modification.addsSlow)
        {
            hasSlow = true;
            slowPercentage = modification.slowPercentage;
        }
        
        if (modification.addsBleed)
        {
            hasBleed = true;
            bleedDamage = modification.bleedDamage;
            bleedDuration = modification.bleedDuration;
        }
        
        effectColor = modification.effectColor;
        
        if (modification.changesProjectileType && modification.newProjectilePrefab != null)
        {
            currentProjectilePrefab = modification.newProjectilePrefab;
        }
    }
}