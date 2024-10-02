using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MonoBehaviour
{
    public float speed_ = 1.0f;
    public Animator animator_;
    public bool inPursuit = false;
    public GameObject playerObject;
    public bool canMove = true;
    public GameObject colliderObject;
    public NavMeshAgent agent;
    private Vector3 currentTargetDirection;
    private bool isPathBlocked = false;
    private int lastDirection = 0; // 0 = not set, 1 = right, -1 = left
    public float stoppingDistance;

    // Roaming settings
    public float roamingRadius = 2f;
    public float minRoamInterval = 8f;
    public float maxRoamInterval = 15f;

    // Returning to original position settings
    public float maxDistanceFromOriginalPos = 10f;
    private Vector3 originalPosition;

    public bool isAttacking = false;
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed_;
        originalPosition = transform.position;
        StartCoroutine(Roam());
    }

    void FixedUpdate()
    {
        FlipSpriteDirection();
        isAttacking = GetComponent<EnemyStats>().isAttacking;
        if (inPursuit && canMove)
        {
            // Check if player is dead or if the enemy is too far from the original position
            if (IsPlayerDead() || IsTooFarFromOriginalPosition())
            {
                if (transform.GetComponent<EnemyStats>().isArenaMob)
                    return;
                ReturnToOriginalPosition();
                return;
            }

            if (!isPathBlocked || IsPathClear(currentTargetDirection, 1f))
            {
                FindPathToPlayer();
                SetAnimatorBasedOnAngle(GetAngle());
            }
            else
            {
                // Continue moving in the last set alternate direction
                MoveTowardsAlternateDirection();
                SetAnimatorBasedOnAngle(GetAngle());
            }
        }

        animator_.SetFloat("Speed", agent.velocity.magnitude);
    }

    private IEnumerator Roam()
    {
        while (true)
        {
            if (!inPursuit && canMove)
            {
                Vector3 roamTarget = GetRandomRoamingPosition();
                agent.SetDestination(roamTarget);
                animator_.SetFloat("Speed", agent.velocity.magnitude);
                SetAnimatorBasedOnAngle(GetAngle());
            }
            float roamInterval = Random.Range(minRoamInterval, maxRoamInterval);
            yield return new WaitForSeconds(roamInterval);
        }
    }

    private Vector3 GetRandomRoamingPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle * roamingRadius;
        Vector3 roamPosition = new Vector3(randomDirection.x, randomDirection.y, 0f) + originalPosition;
        return roamPosition;
    }

    private bool IsPlayerDead()
    {
        // Implement a check for whether the player is dead
        // This could be a flag in the player's script
        PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
        return playerStats != null && playerStats.isDead;
    }

    private bool IsTooFarFromOriginalPosition()
    {
        return Vector3.Distance(transform.position, originalPosition) > maxDistanceFromOriginalPos;
    }

    private void ReturnToOriginalPosition()
    {
        inPursuit = false;
        agent.isStopped = false;
        agent.SetDestination(originalPosition);
        animator_.SetFloat("Speed", agent.velocity.magnitude);
        gameObject.transform.Find("AgroRadius").gameObject.SetActive(true);
        transform.GetComponent<EnemyStats>().hp = transform.GetComponent<EnemyStats>().maxHp;
    }

    public void SetAnimatorBasedOnAngle(float angle)
    {
        angle += 45f;
        if (angle > 0.0f && angle <= 90f)
        {
            animator_.SetFloat("Rotation", 1f);
            animator_.SetFloat("Rotation2", 0f);
        }
        else if (angle > 90f && angle <= 180f)
        {
            animator_.SetFloat("Rotation", 0f);
            animator_.SetFloat("Rotation2", 1f);
        }
        else if (angle > -90f && angle <= 0f)
        {
            animator_.SetFloat("Rotation", 0f);
            animator_.SetFloat("Rotation2", -1f);
        }
        else
        {
            animator_.SetFloat("Rotation", -1f);
            animator_.SetFloat("Rotation2", 0f);
        }
    }

    private void FindPathToPlayer()
    {
        Vector3 directionToPlayer = (playerObject.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);

        if (distanceToPlayer > stoppingDistance && !isAttacking)
        {
            agent.isStopped = false;
            animator_.SetBool("canAttack", false);
            animator_.SetFloat("Speed", agent.velocity.magnitude);

            if (!IsPathClear(directionToPlayer, 1f))
            {
                isPathBlocked = true;
                Vector3 alternateDirection = FindAlternatePath(directionToPlayer, distanceToPlayer);
                currentTargetDirection = alternateDirection;
                MoveTowardsAlternateDirection();
            }
            else
            {
                isPathBlocked = false;
                currentTargetDirection = playerObject.transform.position - transform.position;
                agent.SetDestination(playerObject.transform.position);
            }
        }
        else if (distanceToPlayer <= stoppingDistance)
        {
            agent.isStopped = true;
            animator_.SetFloat("Speed", 0f);
        }
    }

    private bool IsPathClear(Vector2 direction, float distance)
    {
        float slimeRadius = 0.05f;
        Vector2 start2D = new Vector2(transform.position.x, transform.position.y) + direction.normalized * 0.5f;

        // Perform a CircleCast, excluding the enemy's own collider
        RaycastHit2D hit = Physics2D.CircleCast(start2D, slimeRadius, direction, distance);

        if (hit.collider != null && hit.collider.gameObject != gameObject) // Exclude self
        {
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Obstacle"))
                return false;
        }

        return true;
    }


    private Vector2 RotateVector2(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);

        return v;
    }

    private Vector2 FindAlternatePath(Vector2 originalDirection, float distance)
    {
        float angleIncrement = 45f;
        Vector2 originalDirection2D = new Vector2(originalDirection.x, originalDirection.y).normalized;

        if (lastDirection == 0)
        {
            lastDirection = Random.Range(0, 2) * 2 - 1;
        }

        for (float angle = angleIncrement; angle <= 180; angle += angleIncrement)
        {
            Vector2 newDirection = RotateVector2(originalDirection2D, angle * lastDirection);

            if (IsPathClear(newDirection, distance))
            {
                return newDirection * distance;
            }
        }

        for (float angle = angleIncrement; angle <= 180; angle += angleIncrement)
        {
            Vector2 newDirection = RotateVector2(originalDirection2D, -angle * lastDirection);

            if (IsPathClear(newDirection, distance))
            {
                lastDirection *= -1;
                return newDirection * distance;
            }
        }

        return originalDirection2D * distance;
    }

    private void FlipSpriteDirection()
    {
        if (inPursuit && agent.velocity.magnitude < 0.01f)
        {
            if ((playerObject.transform.position - transform.position).normalized.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if ((playerObject.transform.position - transform.position).normalized.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            if ((agent.destination - transform.position).normalized.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if ((agent.destination - transform.position).normalized.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void MoveTowardsAlternateDirection()
    {
        Vector2 newPosition = transform.position + currentTargetDirection;
        agent.SetDestination(newPosition);
    }
    public float GetAngle()
    {
        if (inPursuit && agent.velocity.magnitude < 0.01)
        {
            return Vector2.SignedAngle(Vector2.right, (playerObject.transform.position - transform.position).normalized);
        }
        else
        {
            return Vector2.SignedAngle(Vector2.right, (agent.destination - transform.position).normalized);
        }
    }
}
