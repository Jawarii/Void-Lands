using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

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

        randomEggFactor = 1.0f;
        turnSpeed = 80f;
        FindInitialTarget();
    }
    private void Update()
    {
        if (target != null)
        {
            HandleHomingArrowBehaviour();
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

            if (other.gameObject == target.gameObject)
            {
                hitCount++;
                if (hitCount < maxHits)
                {
                    FindNewTarget();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        //else if (other.gameObject.CompareTag("Obstacle"))
        //{
        //    Destroy(gameObject);
        //}
    }

    private void FindInitialTarget()
    {
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
            Destroy(gameObject);
        }
    }

    private void FindNewTarget()
    {

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, homingRadius);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && hitCollider.gameObject != target.gameObject)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy < closestDistance && !hitCollider.GetComponent<EnemyStats>().isDead)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hitCollider.transform;
                }
            }
        }
        if (closestEnemy != null && !closestEnemy.GetComponent<EnemyStats>().isDead)
        {
            target = closestEnemy;

        }
        else if (target != null && target.GetComponent<EnemyStats>().isDead)
        {
            Destroy(gameObject);
        }
        RerollRandomValues();
    }

    private void RerollRandomValues()
    {
        turnSpeed = Random.Range(80f, 100f);
        randomEggFactor = Random.Range(0.6f, 1.3f);
    }
    private void HandleHomingArrowBehaviour()
    {
        Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;

        toTarget.Normalize();

        Vector2 arrowForward = -transform.up;

        float crossProduct = Vector3.Cross(arrowForward, toTarget).z;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        float adjustedTurnSpeed = 80f + turnSpeed * (1.0f / distanceToTarget);

        float rotationAmount = 10f * adjustedTurnSpeed * Time.deltaTime * crossProduct;

        transform.Rotate(0, 0, rotationAmount);

        transform.Translate(Vector2.down * speed * randomEggFactor * Time.deltaTime);

        float distanceTraveled = Vector2.Distance(transform.position, startPosition);

        if (distanceTraveled >= maxDistance && hitCount == 0)
        {
            Destroy(gameObject);
        }

        if (target != null && target.GetComponent<EnemyStats>().isDead)
        {
            FindNewTarget();
        }
    }
}