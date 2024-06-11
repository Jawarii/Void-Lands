using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrenzyScript : SkillsScript
{
    public float duration = 6f;
    public GameObject player;
    public PlayerStats playerStats;
    public float atkSpeedIncrease;
    public float atkIncrease;
    public float speedIncrease;
    public override void ActivateSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();

        atkSpeedIncrease = 0.5f;
        playerStats.atkSpd += atkSpeedIncrease;

        atkIncrease = playerStats.attack * 0.5f;
        playerStats.attack += atkIncrease;

        speedIncrease = 0.5f;
        playerStats.speed += speedIncrease;

        StartCoroutine(DisableBuff());

    }

    public IEnumerator DisableBuff()
    {
        yield return new WaitForSeconds(duration);

        playerStats.atkSpd -= atkSpeedIncrease;
        playerStats.attack -= atkIncrease;
        playerStats.speed -= speedIncrease;

        Destroy(gameObject);
    }
}
