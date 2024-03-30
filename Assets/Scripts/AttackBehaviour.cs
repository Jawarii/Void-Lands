using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    private int crit = 0;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            crit = Random.Range(1, 101);
            if (crit <= 33)
            {
                other.GetComponent<EnemyStats>().TakeDamage((int)((float)Random.Range(16, 19) * 1.5f), true);
            }
            else
            {
                other.GetComponent<EnemyStats>().TakeDamage(Random.Range(16, 19), false);
            }
        }
    }
}
