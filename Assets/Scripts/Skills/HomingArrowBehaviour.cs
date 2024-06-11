using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingArrowBehaviour : MonoBehaviour
{
    public float speed = 10f; // Speed of the arrow
    private int crit = 0;
    public GameObject player;
    public PlayerStats playerStats;
    public float basicAtkDmgMulti = 0.5f;
    public float knockbackForce = 10f; // Adjust the force of knockback as needed
    public float maxDistance = 8f; // Maximum distance the arrow can travel
    public float homingRadius = 5f; // Radius within which the arrow will seek new targets
    public int maxHits = 5; // Maximum number of targets the arrow can hit
    public float turnSpeed; // Speed at which the arrow turns towards the target
    public float arcChangeAngle; // Angle change after hitting a target

    private Vector2 startPosition;
    private Transform target;
    private int hitCount = 0;
    private float randomEggFactor;

    public bool isImbued = false;

    public GameObject _bow;
    public PlayerAttackArcher attackArcher;
    public GameObject miasmaExplosionPrefab;

    private void Start()
    {
        _bow = GameObject.FindGameObjectWithTag("BowObject");
        attackArcher = _bow.GetComponent<PlayerAttackArcher>();
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        startPosition = transform.position;
        if (attackArcher.hasImbueBuff)
            isImbued = true;
        RerollRandomValues();
        FindInitialTarget();
    }

    private void Update()
    {
        if (target != null)
        {
            Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle + 90)), turnSpeed * Time.deltaTime);

            Debug.Log($"Direction: {direction}, Rotate Amount: {rotateAmount}");
        }

        // Move the arrow forward based on its current rotation, with a random egg shape factor
        transform.Translate(Vector2.down * speed * randomEggFactor * Time.deltaTime);

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

            hitCount++;

            if (hitCount < maxHits)
            {
                RerollRandomValues();
                FindNewTarget();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    private void FindInitialTarget()
    {
        // Find the initial target within the homing radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, homingRadius);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hitCollider.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy;
        }
    }

    private void FindNewTarget()
    {
        // Find the next target within the homing radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, homingRadius);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hitCollider.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy;
        }
        else
        {
            // No more targets, destroy the arrow
            Destroy(gameObject);
        }
    }

    private void RerollRandomValues()
    {
        // Randomize turn speed between 300 and 400
        turnSpeed = Random.Range(350f, 450f);

        // Randomize the egg shape factor between 0.8 and 1.2
        if (hitCount > 0)
            randomEggFactor = Random.Range(0.5f, 1.5f);
        else
            randomEggFactor = 1f;

        // Randomize the angle change between 45 ~ 135 and -45 ~ -135
        arcChangeAngle = Random.Range(45f, 135f) * (Random.value > 0.5f ? 1 : -1);
    }
}
