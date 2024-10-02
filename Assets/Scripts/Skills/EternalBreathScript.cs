using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EternalBreathScript : SkillsScript
{
    public float duration = 2f;
    public GameObject player;
    public PlayerStats playerStats;
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Reference to the prefab you want to instantiate
    public GameObject effectPrefab;

    // Reference to the instantiated effect object
    private GameObject instantiatedEffect;

    public override void ActivateSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerStats.isImmune = true;

        // Play audio
        audioSource = GameObject.Find("BuffsSoundSource").GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);

        // Instantiate the effectPrefab at the player's location and make the player its parent
        instantiatedEffect = Instantiate(effectPrefab, player.transform.position, Quaternion.identity, player.transform);

        // Start the coroutine to disable the buff and destroy the effect after the duration
        StartCoroutine(DisableBuff());
    }

    public IEnumerator DisableBuff()
    {
        yield return new WaitForSeconds(duration);

        // Remove the player's immunity
        playerStats.isImmune = false;

        // Destroy the instantiated effect object
        if (instantiatedEffect != null)
        {
            Destroy(instantiatedEffect);
        }

        // Destroy this script's GameObject
        Destroy(gameObject);
    }
}
