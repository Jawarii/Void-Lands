using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 2.0f;
    private float currentAttackCooldown;
    public float attackDuration = 1.0f;
    private EnemyMovementController golemMovement;
    public GameObject projectilePrefab;
    public EnemyStats enemyStats;

    void Start()
    {
        golemMovement = GetComponent<EnemyMovementController>();
        enemyStats = transform.GetComponent<EnemyStats>();
        currentAttackCooldown = 0f; // Allows immediate attack if in range
    }

    void Update()
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
        golemMovement.canMove = false; // Prevent movement during attack
        animator.SetBool("canAttack", true);
        currentAttackCooldown = attackCooldown;

        yield return new WaitForSeconds(attackDuration * 0.85f);

        Vector3 directionToPlayer = golemMovement.playerObject.transform.position - transform.position;

        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Instantiate the projectile facing the player
        GameObject projectile_ = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, angle)));
        projectile_.GetComponent<GolemProjectileBehavior>().enemyStats = enemyStats;
        StartCoroutine(EndAttack());

    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(attackDuration * 0.15f);
        currentAttackCooldown = attackCooldown;
        animator.SetBool("canAttack", false);
        golemMovement.canMove = true; // Re-enable movement after attack
    }

}
