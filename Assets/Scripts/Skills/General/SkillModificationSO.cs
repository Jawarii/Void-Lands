using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Modification", menuName = "Skills System/New Skill Modification")]
public class SkillModificationSO : ScriptableObject
{
    public string modificationName;
    public string description;
    public Sprite icon;
    public int requiredLevel;
    public int skillPointsCost;
    
    // Modification effects
    public float damageMultiplier = 1f;
    public float cooldownMultiplier = 1f;
    public float durationMultiplier = 1f;
    public float rangeMultiplier = 1f;
    public float areaMultiplier = 1f;
    
    // Special effects
    public bool addsStun;
    public float stunDuration;
    public bool addsSlow;
    public float slowPercentage;
    public bool addsBleed;
    public float bleedDamage;
    public float bleedDuration;
    
    // Visual modifications
    public Color effectColor = Color.white;
    public bool changesProjectileType;
    public GameObject newProjectilePrefab;
} 