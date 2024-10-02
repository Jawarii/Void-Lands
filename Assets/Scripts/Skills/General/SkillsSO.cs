using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills System/NewSkill")]
[System.Serializable]
public class SkillsSO : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public string skillLevel;
    public string skillType;
    public Sprite skillIcon;
    public SkillsScript skillScript;
    public Animator _animator;
    public float coolDown;
}