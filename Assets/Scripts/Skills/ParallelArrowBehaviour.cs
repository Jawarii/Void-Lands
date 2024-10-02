using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelArrowBehaviour : MonoBehaviour
{
    public float speed = 20f; // Speed of the arrow
    private int crit = 0;
    public GameObject player;
    public PlayerStats playerStats;
    public float basicAtkDmgMulti = 0.5f;
    public float knockbackForce = 10f; // Adjust the force of knockback as needed
    public float maxDistance = 3f; // Maximum distance the arrow can travel
    public bool isImbued = false;
    private Vector2 startPosition;

    public GameObject _bow;
    public PlayerAttackArcher attackArcher;
    public GameObject miasmaExplosionPrefab;

    public bool canStun = false;
    private void Start()
    {
        _bow = GameObject.FindGameObjectWithTag("BowObject");
        attackArcher = _bow.GetComponent<PlayerAttackArcher>();
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        knockbackForce = 10f;
        startPosition = transform.position;
        if (attackArcher.hasImbueBuff)
            isImbued = true;
        // Call the DestroyArrow function after a certain distance has been traveled
        StartCoroutine(DestroyArrow());
    }

    private void Update()
    {
        // Move the arrow forward based on its current rotation
        transform.Translate(Vector2.down * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            crit = Random.Range(1, 101);

            float minDmg = basicAtkDmgMulti * (playerStats.attack) * 0.9f;
            float maxDmg = basicAtkDmgMulti * (playerStats.attack) * 1.1f;
            float critDmgMulti = playerStats.critDmg / 100.0f;
            if (isImbued)
            {
                GameObject explosion = Instantiate(miasmaExplosionPrefab, transform.position, Quaternion.identity);
                explosion.GetComponent<MiasmaExplosionController>().playerStats = playerStats;
            }
            if (crit <= playerStats.critRate)
            {
                enemyStats.TakeDamage((int)(Random.Range(minDmg, maxDmg) * critDmgMulti), true);
            }
            else
            {
                enemyStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);
            }

            float angle = transform.rotation.eulerAngles.z - 90f;
            Vector2 knockbackDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Apply knockback to the enemy

            // enemyStats.ApplyKnockback(knockbackDirection, knockbackForce);
            if (canStun)
                enemyStats.ApplyCrowdControl("Stun", 2f);
            // Optionally destroy the arrow upon hitting an enemy
            //Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyArrow()
    {
        // Wait until the arrow has traveled the maximum distance
        while (Vector2.Distance(transform.position, startPosition) < maxDistance)
        {
            yield return null;
        }

        // Destroy the arrow game object
        Destroy(gameObject);
    }
}
