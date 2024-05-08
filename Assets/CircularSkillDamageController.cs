using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSkillDamageController : MonoBehaviour
{
    public float atkDmgMulti = 2f;
    public EnemyStats enemyStats;
    public AudioSource audioSource;
    public AudioClip clip;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            float minDmg = atkDmgMulti * (enemyStats.attack - playerStats.defense) * 0.9f;
            float maxDmg = atkDmgMulti * (enemyStats.attack - playerStats.defense) * 1.1f;

            playerStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);
            this.GetComponent<Collider2D>().enabled = false;
        }
    }
    void Awake()
    {
        //PlayClip();
        StartCoroutine(DisableCollider());
        audioSource = GameObject.Find("ExplosionSoundSource").GetComponent<AudioSource>();
    }
    public void PlayClip()
    {
        
        audioSource.PlayOneShot(clip);
    }
    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(0.15f);
        this.GetComponent<Collider2D>().enabled = false;
    }
}
