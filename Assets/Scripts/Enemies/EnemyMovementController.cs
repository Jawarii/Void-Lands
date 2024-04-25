using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed_;
    }

    void FixedUpdate()
    {
        FlipSpriteDirection();
        if (inPursuit && canMove)
        {
            if (!isPathBlocked || IsPathClear(currentTargetDirection, 1f))
            {
                FindPathToPlayer();
            }
            else
            {
                // Continue moving in the last set alternate direction
                MoveTowardsAlternateDirection();
            }
        }
    }

    private void FindPathToPlayer()
    {
        Vector3 directionToPlayer = (playerObject.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);

        if (distanceToPlayer > stoppingDistance)
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
        else if (distanceToPlayer <= stoppingDistance) // Close enough to stop and possibly attack
        {
            agent.isStopped = true;
            animator_.SetFloat("Speed", 0f);
        }
    }

    private bool IsPathClear(Vector2 direction, float distance)
    {
        float slimeRadius = 0.05f; // Adjust this value to match your slime's actual collider radius
        Vector2 start2D = new Vector2(transform.position.x, transform.position.y) + direction.normalized * 0.5f;

        // Use CircleCast to account for the slime's radius in the path clearance check
        RaycastHit2D hit = Physics2D.CircleCast(start2D, slimeRadius, direction, 0.5f); // Subtracting 0.5f to account for the start offset

       // Debug.DrawRay(start2D, direction * 0.5f, Color.green, 0.5f);

        if (hit.collider != null)
        {
           // Debug.Log($"CircleCast hit: {hit.collider.name}");
            return !hit.collider.CompareTag("Enemy");
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
        float angleIncrement = 45f; // Degree to increment each attempt
        Vector2 originalDirection2D = new Vector2(originalDirection.x, originalDirection.y).normalized;

        // Determine at random whether to check left or right direction first
        // but only if lastDirection was not already set
        if (lastDirection == 0)
        {
            lastDirection = Random.Range(0, 2) * 2 - 1; // Will randomly set to 1 or -1
        }

        for (float angle = angleIncrement; angle <= 180; angle += angleIncrement)
        {
            Vector2 newDirection = RotateVector2(originalDirection2D, angle * lastDirection);

            if (IsPathClear(newDirection, distance))
            {
                return newDirection * distance;
            }
        }

        // If no clear path was found in the preferred direction, try the opposite direction as a fallback
        for (float angle = angleIncrement; angle <= 180; angle += angleIncrement)
        {
            Vector2 newDirection = RotateVector2(originalDirection2D, -angle * lastDirection);

            if (IsPathClear(newDirection, distance))
            {
                // Update lastDirection since we're now choosing the opposite side
                lastDirection *= -1;
                return newDirection * distance;
            }
        }

        // Continue in the last direction if no clear path is found
        return originalDirection2D * distance;
    }
    private void FlipSpriteDirection()
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
    private void MoveTowardsAlternateDirection()
    {
        Vector2 newPosition = transform.position + currentTargetDirection;
        agent.SetDestination(newPosition);
    }
}
