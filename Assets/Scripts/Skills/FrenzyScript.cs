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

    public AudioSource audioSource;
    public AudioClip audioClip;

    // Reference to the prefab you want to instantiate
    public GameObject effectPrefab;

    // Reference to the instantiated effect object
    private GameObject instantiatedEffect;

    public override void ActivateSkill()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        if (buffIsActive)
        {
            // Instantly remove the current buff before applying the new one
            RemoveBuff();
        }

        // Apply the new buff

        atkSpeedIncrease = playerStats.atkSpd * 0.5f;
        atkIncrease = playerStats.attack * 0.5f;
        speedIncrease = playerStats.speed * 0.5f;
        playerStats.atkSpd += atkSpeedIncrease;
        playerStats.attack += atkIncrease;
        playerStats.speed += speedIncrease;

        audioSource = GameObject.Find("BuffsSoundSource").GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);
        Transform playerTransform = playerStats.gameObject.transform;
        Vector3 playerPos = playerTransform.position;
        Vector3 animatationPos = new Vector3(playerPos.x, playerPos.y + 0.35f, playerPos.z);
        // Instantiate the effectPrefab at the player's location and make the player its parent
        instantiatedEffect = Instantiate(effectPrefab, animatationPos, Quaternion.identity, playerTransform);
        buffIsActive = true;
        StartCoroutine(DisableBuff(duration));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        if (instantiatedEffect != null)
        {
            Destroy(instantiatedEffect);
        }
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
