using System.Collections;
using UnityEngine;

public class RapidArrowBehaviour : MonoBehaviour
{
    public float speed = 10f; // Speed of the arrow
    private int crit = 0;
    public GameObject player;
    public PlayerStats playerStats;
    public float basicAtkDmgMulti = 0.5f;
    public float knockbackForce = 10f; // Adjust the force of knockback as needed
    public float lifeTime = 1f;

    private void Start()
    {
        // Call the DestroyArrow function after 2 seconds
        StartCoroutine(DestroyArrow());
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        knockbackForce = 10f;   
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

            float minDmg = basicAtkDmgMulti * (playerStats.attack - enemyStats.defense) * 0.95f;
            float maxDmg = basicAtkDmgMulti * (playerStats.attack - enemyStats.defense) * 1.05f;
            float critDmgMulti = playerStats.critDmg / 100.0f;

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
            enemyStats.ApplyKnockback(knockbackDirection, knockbackForce);

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
