using AStar_2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 2.0f;
    public float currentAttackCooldown;
    public float attackDuration = 0.833f;
    private EnemyMovementController golemMovement;
    public GameObject projectilePrefab;
    public EnemyStats enemyStats;
    public bool isAttacking;

    void Start()
    {
        golemMovement = transform.GetComponent<EnemyMovementController>();
        enemyStats = transform.GetComponent<EnemyStats>();
        currentAttackCooldown = 0f; // Allows immediate attack if in range
    }

    void FixedUpdate()
    {
        if (currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
        }

        if (!golemMovement.canMove) return; // Prevent checking for attack conditions if already attacking

        float distanceToPlayer = Vector2.Distance(transform.position, golemMovement.playerObject.transform.position);
        if (distanceToPlayer <= golemMovement.stoppingDistance && currentAttackCooldown <= 0)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        if (isAttacking) yield break; // Ensure that we do not stack attacks
        isAttacking = true;

        golemMovement.agent.isStopped = true;
        golemMovement.canMove = false; // Prevent movement during attack
        animator.SetBool("canAttack", true);
        animator.SetFloat("Speed", 0f);
        currentAttackCooldown = attackCooldown;

        // Wait for the major part of the attack animation to complete before spawning the projectile
        yield return new WaitForSeconds(attackDuration * 0.85f);

        // Instantiate the projectile without further range checks
        Vector3 directionToPlayer = golemMovement.playerObject.transform.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        GameObject projectile_ = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, angle)));
        projectile_.GetComponent<GolemProjectileBehavior>().enemyStats = enemyStats;

        // Wait for the rest of the attack duration
        yield return new WaitForSeconds(attackDuration * 0.15f);

        // End the attack
        animator.SetBool("canAttack", false);
        golemMovement.canMove = true;
        golemMovement.agent.isStopped = false;
        animator.SetFloat("Speed", golemMovement.agent.velocity.magnitude);
        isAttacking = false;
    }
}
