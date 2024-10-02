using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBatAttack : EnemyAttack
{
    public Animator animator;
    public float attackCooldown = 2.0f;
    private float currentAttackCooldown;
    public GameObject attackColliderGo; // The GameObject holding the collider component
    public Collider2D attackCollider; // The actual collider component for the attack
    public float attackDuration = 1.0f;
    private EnemyMovementController batMovement;
    public bool isAttacking;
    public Vector3 originalPos;
    public Vector3 newPos;
    public float setDistanceFromPlayer = 0.5f; // Desired distance from the player

    void Start()
    {
        attackCollider = attackColliderGo.GetComponent<Collider2D>();
        batMovement = GetComponent<EnemyMovementController>();
        currentAttackCooldown = 0f; // Allows immediate attack if in range
        isAttacking = GetComponent<EnemyStats>().isAttacking;
    }

    void FixedUpdate()
    {
        isAttacking = GetComponent<EnemyStats>().isAttacking;
        if (currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
        }

        if (!batMovement.canMove) return; // Prevent checking for attack conditions if already attacking

        float distanceToPlayer = Vector3.Distance(transform.position, batMovement.playerObject.transform.position);
        if (distanceToPlayer <= batMovement.stoppingDistance && currentAttackCooldown <= 0)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        if (isAttacking) yield break; // Ensure that we do not stack attacks
        originalPos = transform.position;
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        newPos = playerPosition - directionToPlayer * setDistanceFromPlayer;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        attackColliderGo.transform.rotation = Quaternion.Euler(0, 0, angle + 90); // Adjust rotation to match direction
        batMovement.SetAnimatorBasedOnAngle(angle);
        GetComponent<EnemyStats>().isAttacking = true;
        isAttacking = GetComponent<EnemyStats>().isAttacking;
        batMovement.agent.isStopped = true;
        batMovement.canMove = false; // Prevent movement during attack
        animator.SetBool("canAttack", true);
        animator.SetFloat("Speed", 0f);
        currentAttackCooldown = attackCooldown;

        yield return new WaitForSeconds(0.667f);
        if (gameObject.GetComponent<EnemyStats>().isDead)
        {
            StopCoroutine(PerformAttack());
            yield break;
        }
        animator.SetBool("canAttack", false);
        batMovement.canMove = true;
        batMovement.agent.isStopped = false; 
        animator.SetFloat("Speed", batMovement.agent.velocity.magnitude);
        GetComponent<EnemyStats>().isAttacking = false;
        isAttacking = false;
        //transform.position = originalPos;
    }

    public void HandleColliderFunc()
    {
        StartCoroutine(HandleCollider());
    }

    IEnumerator HandleCollider()
    {
        attackCollider.enabled = true;
        transform.position = newPos;
        yield return new WaitForSeconds(0.1f);
        attackCollider.enabled = false;
    }

}
