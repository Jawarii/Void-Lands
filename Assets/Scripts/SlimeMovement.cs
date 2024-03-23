using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeMovement : MonoBehaviour
{
    public float speed_ = 1.0f;
    public Animator animator_;
    public bool inPursuit = false;
    public GameObject playerObject;
    public float atkCd = 2.0f;
    private float currentAtkCd = 0.0f;
    public float atkTime = 1.0f;
    private float currentAtkTime = 0.0f;
    private bool canMove = true;
    public GameObject colliderObject;
    public NavMeshAgent agent;
    private Vector3 destination;
    private Vector3 currentTargetDirection;
    private bool isPathBlocked = false;
    private int lastDirection = 0; // 0 = not set, 1 = right, -1 = left

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed_;
        currentAtkTime = atkTime;
    }

    private void Update()
    {
        currentAtkCd -= Time.deltaTime;
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
        else if (!canMove)
        {
            PerformAttack();
        }
    }

    private void FindPathToPlayer()
    {
        Vector3 directionToPlayer = (playerObject.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);

        if (distanceToPlayer > 0.6f && currentAtkCd <= 0)
        {
            agent.isStopped = false;
            animator_.SetBool("canAttack", false);
            animator_.SetFloat("Speed", speed_);

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
        else if (distanceToPlayer <= 0.6f && currentAtkCd <= 0) // Close enough to stop and possibly attack
        {
            //PrepareForAttack();
            canMove = false;
            agent.isStopped = true;
            animator_.SetBool("canAttack", true);
            animator_.SetFloat("Speed", 0);
            currentAtkCd = atkCd;
            currentAtkTime = atkTime;
        }
    }

    private bool IsPathClear(Vector2 direction, float distance)
    {
        float slimeRadius = 0.05f; // Adjust this value to match your slime's actual collider radius
        Vector2 start2D = new Vector2(transform.position.x, transform.position.y) + direction.normalized * 0.3f;

        // Use CircleCast to account for the slime's radius in the path clearance check
        RaycastHit2D hit = Physics2D.CircleCast(start2D, slimeRadius, direction, 0.5f); // Subtracting 0.5f to account for the start offset

        Debug.DrawRay(start2D, direction * 0.5f, Color.green, 0.5f);

        if (hit.collider != null)
        {
            Debug.Log($"CircleCast hit: {hit.collider.name}");
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

        Debug.Log("No clear alternate path found, sticking to last direction");
        // Continue in the last direction if no clear path is found
        return originalDirection2D * distance;
    }



    private void PrepareForAttack()
    {
        canMove = false;
        agent.isStopped = true;
        animator_.SetBool("canAttack", true);
        animator_.SetFloat("Speed", 0);
        currentAtkCd = atkCd;
        currentAtkTime = atkTime;
    }
    private void PerformAttack()
    {
        currentAtkTime -= Time.deltaTime;
        if (currentAtkTime < 0.9f && currentAtkTime > 0.75f)
        {
            colliderObject.SetActive(true);
        }
        else if (currentAtkTime < 0.6f && currentAtkTime > 0.45f)
        {
            colliderObject.SetActive(true);
        }
        else if (currentAtkTime < 0.3f && currentAtkTime > 0.15f)
        {
            colliderObject.SetActive(true);
        }
        else
        {
            colliderObject.SetActive(false);
        }
        if (currentAtkTime < 0)
        {
            animator_.SetBool("canAttack", false);
            canMove = true;
            colliderObject.SetActive(false); // Reset attack cooldown
             // Reset attack time
        }
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
