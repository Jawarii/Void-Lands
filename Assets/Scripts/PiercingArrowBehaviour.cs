using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingArrowBehaviour : MonoBehaviour
{
    public float speed = 20f; // Speed of the arrow
    private int crit = 0;
    public GameObject player;
    public PlayerStats playerStats;
    public float basicAtkDmgMulti = 0.5f;
    public float maxDistance = 8f; // Maximum distance the arrow can travel
    public bool isImbued = false;
    private Vector2 startPosition;

    public GameObject _bow;
    public PlayerAttackArcher attackArcher;
    public GameObject miasmaExplosionPrefab;

    private void Start()
    {
        _bow = GameObject.FindGameObjectWithTag("BowObject");
        attackArcher = _bow.GetComponent<PlayerAttackArcher>();
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        startPosition = transform.position;
        if (attackArcher.hasImbueBuff)
            isImbued = true;
    }

    private void Update()
    {
        // Move the arrow forward based on its current rotation
        transform.Translate(Vector2.down * speed * Time.deltaTime, Space.Self);

        // Calculate the distance traveled
        float distanceTraveled = Vector2.Distance(transform.position, startPosition);

        // If the distance traveled exceeds the maximum distance, destroy the arrow
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
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

        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
