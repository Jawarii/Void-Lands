using UnityEngine;
using System.Collections;

public class FrenzyScript : SkillsScript
{
    public float duration = 6f;
    private PlayerStats playerStats;
    private static Coroutine buffCoroutine;

    private static float atkSpeedIncrease = 0.5f;
    private static float atkIncrease;
    private static float speedIncrease = 0.5f;
    private static bool buffIsActive = false;

    public override void ActivateSkill()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        if (buffIsActive)
        {
            // Instantly remove the current buff before applying the new one
            RemoveBuff();
        }

        // Apply the new buff
        atkIncrease = playerStats.attack * 0.5f;
        playerStats.atkSpd += atkSpeedIncrease;
        playerStats.attack += atkIncrease;
        playerStats.speed += speedIncrease;

        buffIsActive = true;
        StartCoroutine(DisableBuff(duration));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void RemoveBuff()
    {
        if (playerStats == null) return;

        playerStats.atkSpd -= atkSpeedIncrease;
        playerStats.attack -= atkIncrease;
        playerStats.speed -= speedIncrease;

        buffIsActive = false;
        StartCoroutine(DestroyAfterDelay(0.1f)); // Adjust delay as needed
    }

    private IEnumerator DisableBuff(float durationX)
    {
        yield return new WaitForSeconds(durationX);

        // Remove the buff when the duration is over
        RemoveBuff();
    }
}
