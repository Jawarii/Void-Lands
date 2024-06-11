using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiasmaScript : SkillsScript
{
    public float coolDown = 5f;
    public float cdElapsedTime = 0f;
    public float duration = 5f;
    public GameObject _bow;

    public override void ActivateSkill()
    {
        playerAttack.hasImbueBuff = true;
        StartCoroutine(DisableBuff());
    }

    public IEnumerator DisableBuff()
    {
        yield return new WaitForSeconds(duration);
        playerAttack.hasImbueBuff = false;
        Destroy(gameObject);
    }
}
