using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EternalBreathScript : SkillsScript
{
    public float duration = 2f;
    public GameObject player;
    public PlayerStats playerStats;
    public override void ActivateSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();

        playerStats.isImmune = true;

        StartCoroutine(DisableBuff());

    }

    public IEnumerator DisableBuff()
    {
        yield return new WaitForSeconds(duration);
        playerStats.isImmune = false;
        Destroy(gameObject);
    }
}
