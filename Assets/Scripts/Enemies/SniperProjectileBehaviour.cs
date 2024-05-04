using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperProjectileBehaviour : MonoBehaviour
{
    public float speed = 5f; // Speed of the projectile
    public float basicAtkDmgMulti = 0.5f;
    public float lifeTime = 1f;
    public EnemyStats enemyStats;

    private void Start()
    {
        // Call the DestroyArrow function after 1 seconds
        StartCoroutine(DestroyArrow());
    }

    private void Update()
    {
        // Move the arrow forward based on its current rotation
        transform.Translate(Vector2.up * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            float minDmg = basicAtkDmgMulti * (enemyStats.attack - playerStats.defense) * 0.9f;
            float maxDmg = basicAtkDmgMulti * (enemyStats.attack - playerStats.defense) * 1.1f;

            playerStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);

            // Optionally destroy the arrow upon hitting an enemy
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyArrow()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(lifeTime);

        // Destroy the arrow game object
        Destroy(gameObject);
    }
}
