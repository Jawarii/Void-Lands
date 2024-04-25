using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBehaviour : MonoBehaviour
{
    public float basicAtkDmgMulti = 1.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            EnemyStats enemyStats = GetComponentInParent<EnemyStats>(); // Assuming the EnemyStats component is on the parent GameObject.

            // Calculate damage based on enemy's attack and player's defense
            float minDmg = basicAtkDmgMulti * (enemyStats.attack - playerStats.defense) * 0.9f;
            float maxDmg = basicAtkDmgMulti * (enemyStats.attack - playerStats.defense) * 1.1f;

            // Deal damage to the player
            playerStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);
        }
    }
}
