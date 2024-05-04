using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgroBehaviour : MonoBehaviour
{
    public float agroRadius = 1f; // Radius to detect other enemies.
    private GameObject enemy;

    void Awake()
    {
        enemy = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Agro the current enemy.
            EnemyMovementController enemyMovement = enemy.GetComponent<EnemyMovementController>();
            enemyMovement.inPursuit = true;

            // Notify other enemies within the agro radius.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, agroRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.gameObject != enemy)
                {
                    EnemyMovementController otherEnemyMovement = collider.GetComponent<EnemyMovementController>();
                    if (otherEnemyMovement != null)
                    {
                        otherEnemyMovement.inPursuit = true;
                    }
                }
            }

            // Disable this trigger once activated.
            gameObject.SetActive(false);
        }
    }
}
