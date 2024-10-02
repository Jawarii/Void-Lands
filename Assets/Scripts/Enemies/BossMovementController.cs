using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMovementController : MonoBehaviour
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
    private int lastDirection = 0;
    public float stoppingDistance;
    public Vector3 originalPos;

    public float distanceToMove = 3f; // Distance to move towards the player
    private bool isMoving = false; // Flag to check if boss is already moving towards player

    public bool isMovingStar = false;
    public List<Vector3> pentagramPoints = new List<Vector3>();
    private int currentPointIndex = 0;

    // Add reference to player health
    public PlayerStats playerStats; // You will need to assign this reference in the Unity editor or in Start()

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed_;
        originalPos = transform.position;

        // Assuming the player has a PlayerHealth script that manages health and death
        playerStats = playerObject.GetComponent<PlayerStats>();
    }

    void FixedUpdate()
    {
        agent.speed = speed_;

        // Check if the player is dead
        if (playerStats.isDead)
        {
            ReturnToOriginalPositionAndReset(); // Stop pursuit and reset the boss
            return;
        }

        if (inPursuit && canMove)
        {
            if (!isPathBlocked || IsPathClear(currentTargetDirection, 1f))
            {
                SetAnimatorBasedOnAngle(GetAngle());
            }
            else
            {
                SetAnimatorBasedOnAngle(GetAngle());
            }
        }
    }

    void Update()
    {
        if (isMoving) // Check if boss is already moving
        {
            // Check if boss has reached the destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isMoving = false; // Reset the flag once boss reaches the destination
            }
        }
    }

    public void MoveInPentagramStar()
    {
        if (!isMovingStar)
        {
            StartCoroutine(MoveToPentagramPoints());
        }
    }

    IEnumerator MoveToPentagramPoints()
    {
        isMovingStar = true;

        // Move to original position first
        MoveToOriginalPosition();
        yield return new WaitUntil(() => !isMoving);

        // Define pentagram points
        DefinePentagramPoints();

        // Move to each pentagram point sequentially
        for (int i = 0; i < pentagramPoints.Count; i++)
        {
            MoveToPentagramPoint(pentagramPoints[i]);
            yield return new WaitUntil(() => !isMoving);
        }

        isMovingStar = false;
    }

    void MoveToPentagramPoint(Vector3 point)
    {
        agent.SetDestination(point);
        isMoving = true;
    }

    void DefinePentagramPoints()
    {
        pentagramPoints.Clear();

        // Calculate points based on the original position
        Vector3 topPoint = originalPos + Vector3.up * 5;
        Vector3 leftPoint = originalPos + Quaternion.Euler(0, 0, 72) * (topPoint - originalPos);
        Vector3 rightPoint = originalPos + Quaternion.Euler(0, 0, -72) * (topPoint - originalPos);
        Vector3 bottomLeftPoint = originalPos + Quaternion.Euler(0, 0, 144) * (topPoint - originalPos);
        Vector3 bottomRightPoint = originalPos + Quaternion.Euler(0, 0, -144) * (topPoint - originalPos);

        // Add points to the list
        pentagramPoints.Add(topPoint);
        pentagramPoints.Add(bottomLeftPoint);
        pentagramPoints.Add(rightPoint);
        pentagramPoints.Add(leftPoint);
        pentagramPoints.Add(bottomRightPoint);
        pentagramPoints.Add(topPoint);
    }

    public void SetAnimatorBasedOnAngle(float angle)
    {
        angle += 45f;
        // Here we determine the correct animation state based on the angle
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
        else // This covers both -90 to -180 degrees and 180 to 270 degrees
        {
            animator_.SetFloat("Rotation", -1f);
            animator_.SetFloat("Rotation2", 0f);
        }
    }

    public void FindPathToPlayer()
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
        Vector2 start2D = new Vector2(transform.position.x, transform.position.y) + direction.normalized * 0.5f;
        int layerMask = ~(1 << gameObject.layer);
        RaycastHit2D hit = Physics2D.Raycast(start2D, direction, distance, layerMask);

        if (hit.collider != null)
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

    public void MoveTowardsAlternateDirection()
    {
        Vector2 newPosition = transform.position + currentTargetDirection;
        agent.SetDestination(newPosition);
    }

    public float GetAngle()
    {
        return Vector2.SignedAngle(Vector2.right, (playerObject.transform.position - transform.position).normalized);
    }

    public void MoveToOriginalPosition()
    {
        agent.SetDestination(originalPos);
        isMoving = true;
    }

    public void MoveTowardsPlayer(float distance)
    {
        if (!isMoving) // Check if boss is not already moving
        {
            // Calculate the destination point towards the player
            Vector3 directionToPlayer = (playerObject.transform.position - transform.position).normalized;
            Vector3 destination = transform.position + directionToPlayer * distance;

            // Set the destination using NavMeshAgent
            agent.SetDestination(destination);
            isMoving = true; // Set the flag to true to prevent additional moves
        }
    }

    public void ReturnToOriginalPositionAndReset()
    {
        StopAllCoroutines();

        if (GetComponent<ReaperBossPatternBehaviour>())
        {
            GetComponent<ReaperBossPatternBehaviour>().StopAllCoroutines();
            GetComponent<ReaperBossPatternBehaviour>().ResetVariables();
        }
        // Stop the agent from pursuing the player
        //agent.isStopped = true;

        // Move the boss back to its original position
        MoveToOriginalPosition();

        // Reset the boss's health
        ResetHealth();

        // Stop pursuit logic
        inPursuit = false;

        transform.GetChild(0).gameObject.SetActive(true);
    }

    void ResetHealth()
    {
        // Reset health to full (assuming a maxHealth variable exists)
        GetComponent<EnemyStats>().hp = GetComponent<EnemyStats>().maxHp;
    }
}
