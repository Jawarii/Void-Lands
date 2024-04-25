using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Item", menuName = "Upgrade System/Upgrade Item")]
public class UpgradeItem : ScriptableObject
{
    public string upgradeName;
    public string description;
    public int damageBonus;
    public int critRate;
    public int cdReduction;
    public float radiusMultiplier;
    public int amount;
    public int rotatingSpeed;
    public int skillId;
    // Add other attributes as needed
}
