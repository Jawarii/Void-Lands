using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSkillDamageController : MonoBehaviour
{
    public float atkDmgMulti = 2f;
    public EnemyStats enemyStats;
    public AudioSource audioSource;
    public AudioClip clip;
    public float duration = 5f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            float minDmg = atkDmgMulti * (enemyStats.attack) * 0.9f;
            float maxDmg = atkDmgMulti * (enemyStats.attack) * 1.1f;

            playerStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);
            this.GetComponent<Collider2D>().enabled = false;
        }
    }
    void Awake()
    {
        StartCoroutine(EnableCollider());
        Destroy(gameObject, duration);
    }

    public IEnumerator EnableCollider()
    {
        this.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(DisableCollider());
    }
    public IEnumerator DisableCollider()
    {
        this.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(EnableCollider());
    }
    public void PlayClip()
    {
        audioSource.PlayOneShot(clip);
    }
}
