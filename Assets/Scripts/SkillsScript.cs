using UnityEngine;

public class SkillsScript : MonoBehaviour
{
    public PlayerAttackArcher playerAttack;
    public virtual void InitiateSkill()
    {
        playerAttack = GameObject.FindGameObjectWithTag("BowObject").GetComponent<PlayerAttackArcher>();
        ActivateSkill();
    }
    public virtual void ActivateSkill()
    {
        // Starts Skills, This Gets Overriden.
    }
}
