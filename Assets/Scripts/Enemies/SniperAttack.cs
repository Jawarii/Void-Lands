using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAttack : EnemyAttack
{
    public Animator animator;
    public float attackCooldown = 2.0f;
    public float currentAttackCooldown;
    public float attackDuration = 0.833f;
    private EnemyMovementController enemyMovement;
    public GameObject projectilePrefab;
    public EnemyStats enemyStats;
    public bool isAttacking;

    void Start()
    {
        enemyMovement = transform.GetComponent<EnemyMovementController>();
        enemyStats = transform.GetComponent<EnemyStats>();
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

        if (!enemyMovement.canMove) return; // Prevent checking for attack conditions if already attacking

        float distanceToPlayer = Vector2.Distance(transform.position, enemyMovement.playerObject.transform.position);
        if (distanceToPlayer <= enemyMovement.stoppingDistance && currentAttackCooldown <= 0)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        if (isAttacking) yield break; // Ensure that we do not stack attacks
        GetComponent<EnemyStats>().isAttacking = true;
        isAttacking = GetComponent<EnemyStats>().isAttacking;

        enemyMovement.agent.isStopped = true;
        enemyMovement.canMove = false; // Prevent movement during attack
        animator.SetBool("canAttack", true);
        animator.SetFloat("Speed", 0f);
        currentAttackCooldown = attackCooldown;

        yield return new WaitForSeconds(0.667f);
        // End the attack
        if (gameObject.GetComponent<EnemyStats>().isDead)
        {
            StopCoroutine(PerformAttack());
            yield break;
        }
        animator.SetBool("canAttack", false);
        enemyMovement.canMove = true;
        enemyMovement.agent.isStopped = false;
        animator.SetFloat("Speed", enemyMovement.agent.velocity.magnitude);
        GetComponent<EnemyStats>().isAttacking = false;
        isAttacking = GetComponent<EnemyStats>().isAttacking;
    }
    public void SpawnArrows()
    {
        Vector3 directionToPlayer = enemyMovement.playerObject.transform.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        GameObject projectile_ = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, angle -90f)));
        projectile_.GetComponent<SniperProjectileBehaviour>().enemyStats = enemyStats;
    }
}
