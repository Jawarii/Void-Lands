using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitSkil : MonoBehaviour
{
    public GameObject player;
    public float baseRadius = 5f;
    public float radiusMulti = 1f;
    public int dmgCoe = 33;
    public int critCoe = 0;
    public float cd;
    public float currentCd;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            PlayerStats playerStats = player.gameObject.GetComponent<PlayerStats>();
            EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
            float randomDmg = Random.Range(dmgCoe * .95f, dmgCoe * 1.05f) * playerStats.attack / 10.0f;
            int critCheck = Random.Range(1, 101);
            if (critCheck <= playerStats.critRate + critCoe)
            {
                enemyStats.TakeDamage((int)(randomDmg * (playerStats.critDmg / 100.0f)), true);
            }
            else
            {
                enemyStats.TakeDamage((int)randomDmg, false);
            }
        }
    }
}
