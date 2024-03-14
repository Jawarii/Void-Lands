using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoulderBehaviour : MonoBehaviour
{
    private PlayerStats stats;
    public GameObject player;
    public int damage = 15;
    private EnemyStats enemyStats;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyStats = gameObject.transform.parent.GetComponent<EnemyStats>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            stats = other.GetComponent<PlayerStats>();
            stats.TakeDamage((int)(damage * enemyStats.attack / 10f), false);

            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
