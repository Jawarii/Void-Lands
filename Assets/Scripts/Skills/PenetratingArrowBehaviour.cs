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
    public bool isImbued = false;
    private Vector2 startPosition;

    public GameObject _bow;
    public PlayerAttackArcher attackArcher;
    public GameObject miasmaExplosionPrefab;

    public Transform bossTransform;

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

            float minDmg = basicAtkDmgMulti * (playerStats.attack) * 0.9f;
            float maxDmg = basicAtkDmgMulti * (playerStats.attack) * 1.1f;
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
            if (enemyStats.isBoss)
            {
                bossTransform = other.transform;
            }
            SpawnAdditionalArrows(enemyStats.isBoss);

        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    private void SpawnAdditionalArrows(bool isBoss)
    {
        if (isBoss)
        {
            // Offset the spawn points based on the arrow's local direction (relative to its rotation)
            Vector3 rightSpawnOffset = bossTransform.position + transform.right * (maxDistance / 2f); // Right side relative to arrow
            Vector3 leftSpawnOffset = bossTransform.position - transform.right * (maxDistance / 2f);  // Left side relative to arrow

            // The arrows will still rotate right and left but spawn inward
            Quaternion rightRotation = Quaternion.Euler(0, 0, 90);
            Quaternion leftRotation = Quaternion.Euler(0, 0, -90);

            // Instantiate the right arrow at the left side (relative to its local space)
            GameObject rightArrow = Instantiate(arrowPrefab, leftSpawnOffset, transform.rotation * rightRotation);
            // Instantiate the left arrow at the right side (relative to its local space)
            GameObject leftArrow = Instantiate(arrowPrefab, rightSpawnOffset, transform.rotation * leftRotation);
        }
        else
        {
            // The same behavior for non-boss arrows
            Quaternion rightRotation = Quaternion.Euler(0, 0, 90);
            Quaternion leftRotation = Quaternion.Euler(0, 0, -90);

            // Instantiate right and left arrows at the current position
            GameObject rightArrow = Instantiate(arrowPrefab, transform.position, transform.rotation * rightRotation);
            GameObject leftArrow = Instantiate(arrowPrefab, transform.position, transform.rotation * leftRotation);
        }
    }
}
