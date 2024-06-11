using System.Collections;
using UnityEngine;

public class MiasmaExplosionController : MonoBehaviour
{
    public float basicAtkDmgMulti = 2f;
    public PlayerStats playerStats;
    public GameObject player;
    public float crit = 0;
    // public AudioSource audioSource;
    // public AudioClip clip;
    private Collider2D myCollider;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        myCollider = GetComponent<Collider2D>();
        myCollider.enabled = false; // Disable collider initially
        StartCoroutine(DisableCollider());
        //audioSource = GameObject.Find("ExplosionSoundSource").GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            crit = Random.Range(1, 101);

            float minDmg = basicAtkDmgMulti * (playerStats.attack - enemyStats.defense) * 0.9f;
            float maxDmg = basicAtkDmgMulti * (playerStats.attack - enemyStats.defense) * 1.1f;
            float critDmgMulti = playerStats.critDmg / 100.0f;

            if (crit <= playerStats.critRate)
            {
                enemyStats.TakeDamage((int)(Random.Range(minDmg, maxDmg) * critDmgMulti), true);
            }
            else
            {
                enemyStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);
            }
        }
    }

    //public void PlayClip()
    //{
    //    if (clip != null && audioSource != null)
    //    {
    //        audioSource.PlayOneShot(clip);
    //    }
    //    else
    //    {
    //        Debug.LogError("Audio clip or source not set.");
    //    }
    //}s

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(0.15f);
        this.GetComponent<Collider2D>().enabled = false;
    }
}
