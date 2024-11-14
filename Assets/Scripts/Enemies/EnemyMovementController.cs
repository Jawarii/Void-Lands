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

    public bool isReturning = false; // Flag to track return to position

    // Roaming settings
    public float roamingRadius = 2f;
    public float minRoamInterval = 8f;
    public float maxRoamInterval = 15f;

    // Returning to original position settings
    private float maxDistanceFromOriginalPos = 12.5f;
    private Vector3 originalPosition;

    public bool isAttacking = false;

    private float originalSpeed;
    private float roamSpeed;

    private float flipThreshold = 0.1f; // Define a threshold for flip adjustments
    private float lastFlipTime = 0f;
    private Vector3 lastScale;

    // Timer for pursuit check
    private float pursuitCheckTimer = 0f;
    private float pursuitCheckInterval = 0.5f; // Interval in seconds

    // Cooldown for MoveTowardsAlternateDirection
    private float lastAlternateMoveTime = 0f;
    private float alternateMoveCooldown = 0.5f;

    void Start()
    {
        originalSpeed = speed_;
        roamSpeed = speed_ * 0.5f;
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
        animator_.SetFloat("Speed", agent.velocity.magnitude);

        if (isReturning) // If returning, prevent pursuit
        {
            SetAnimatorBasedOnAngle(GetAngle());
            return;
        }

        if (inPursuit && canMove)
        {
            // Check if player is dead or if the enemy is too far from the original position
            if (IsPlayerDead() || IsTooFarFromOriginalPosition())
            {
                if (transform.GetComponent<EnemyStats>().isArenaMob)
                    return;

                StartCoroutine(ReturnToOriginalPosition());
                return;
            }

            if (!isPathBlocked || IsPathClear(currentTargetDirection, 1f))
            {
                speed_ = originalSpeed;
                agent.speed = speed_;
                FindPathToPlayer();
                SetAnimatorBasedOnAngle(GetAngle());
            }
            else if (Time.time - lastAlternateMoveTime >= alternateMoveCooldown)
            {
                speed_ = originalSpeed;
                agent.speed = speed_;
                MoveTowardsAlternateDirection();
                lastAlternateMoveTime = Time.time;
                SetAnimatorBasedOnAngle(GetAngle());
            }
        }
    }

    private IEnumerator Roam()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!inPursuit && canMove && !isReturning)
            {
                Vector3 roamTarget = GetRandomRoamingPosition();

                // Sample a point on the NavMesh closest to roamTarget
                NavMeshHit hit;
                if (NavMesh.SamplePosition(roamTarget, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    speed_ = roamSpeed;
                    agent.speed = speed_;
                    animator_.SetFloat("Speed", agent.velocity.magnitude);
                    SetAnimatorBasedOnAngle(GetAngle());
                }
            }
            float roamInterval = Random.Range(minRoamInterval, maxRoamInterval);
            yield return new WaitForSeconds(roamInterval);
        }
    }

    private bool IsTooFarFromOriginalPosition()
    {
        return Vector3.Distance(transform.position, originalPosition) > maxDistanceFromOriginalPos;
    }

    IEnumerator ReturnToOriginalPosition()
    {
        inPursuit = false;
        isReturning = true; // Set returning flag
        agent.isStopped = true;
        animator_.SetFloat("Speed", agent.velocity.magnitude);
        transform.GetComponent<EnemyStats>().hp = transform.GetComponent<EnemyStats>().maxHp;
        yield return new WaitForSeconds(0.5f);
        speed_ = originalSpeed * 1.5f;
        agent.isStopped = false;
        agent.speed = speed_;

        // Ensure original position is on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(originalPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        animator_.SetBool("canAttack", false);
        animator_.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void UpdateReturnStatus()
    {
        if (isReturning && Vector3.Distance(transform.position, originalPosition) <= 0.1f)
        {
            isReturning = false; // Reset returning flag once back at original position
            transform.GetComponent<EnemyStats>().hp = transform.GetComponent<EnemyStats>().maxHp;
            speed_ = originalSpeed;
            agent.speed = speed_;
            gameObject.transform.Find("AgroRadius").gameObject.SetActive(true);
        }
    }

    void Update()
    {
        UpdateReturnStatus();
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
        PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
        return playerStats != null && playerStats.isDead;
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
        float slimeRadius = 0.5f;
        Vector2 start2D = new Vector2(transform.position.x, transform.position.y) + direction.normalized * 0.5f;

        // Perform a CircleCast, excluding the enemy's own collider
        RaycastHit2D hit = Physics2D.CircleCast(start2D, slimeRadius, direction, distance);

        if (hit.collider != null && hit.collider.gameObject != gameObject) // Exclude self
        {
            if (hit.collider.CompareTag("Enemy"))
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

    void StopAllCoroutinesExceptCurrent()
    {
        // Get all components on this GameObject
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            // Skip the current script
            if (component == this)
                continue;

            // Stop all coroutines in other scripts
            component.StopAllCoroutines();
        }
    }
}
