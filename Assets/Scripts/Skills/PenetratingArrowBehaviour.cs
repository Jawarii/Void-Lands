using System.Collections;
using UnityEngine;

public class PenetratingArrowBehaviour : MonoBehaviour
{
    public float speed = 10f; // Speed of the arrow
    private int crit = 0;
    public GameObject player;
    public PlayerStats playerStats;
    public float basicAtkDmgMulti = 0.5f;
    public GameObject arrowPrefab; // Prefab of the arrow to spawn
    public float maxDistance = 8f; // Maximum distance the arrow can travel

    private Vector2 startPosition;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        startPosition = transform.position;
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

            if (crit <= playerStats.critRate)
            {
                enemyStats.TakeDamage((int)(Random.Range(minDmg, maxDmg) * critDmgMulti), true);
            }
            else
            {
                enemyStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);
            }

            // Spawn additional arrows to the left and right
            SpawnAdditionalArrows();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    private void SpawnAdditionalArrows()
    {
        // Calculate right and left rotations
        Quaternion rightRotation = Quaternion.Euler(0, 0, 90);
        Quaternion leftRotation = Quaternion.Euler(0, 0, -90);

        // Instantiate right arrow and set velocity
        GameObject rightArrow = Instantiate(arrowPrefab, transform.position, transform.rotation * rightRotation);
        // Instantiate left arrow and set velocity
        GameObject leftArrow = Instantiate(arrowPrefab, transform.position, transform.rotation * leftRotation);
    }
}
