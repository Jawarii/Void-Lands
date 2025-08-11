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
            if (playerStats == null)
            {
                //Debug.LogError("PlayerStats component not found on the player.");
                return;
            }

            EnemyStats enemyStats = GetComponentInParent<EnemyStats>();
            if (enemyStats == null)
            {
                //Debug.LogError("EnemyStats component not found on the enemy.");
                return;
            }

            float minDmg = basicAtkDmgMulti * (enemyStats.attack) * 0.9f;
            float maxDmg = basicAtkDmgMulti * (enemyStats.attack) * 1.1f;

            playerStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);
        }
    }

}
