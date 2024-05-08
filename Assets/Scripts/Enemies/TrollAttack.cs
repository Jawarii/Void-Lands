using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 2.0f;
    private float currentAttackCooldown;
    public GameObject attackColliderGo; // The GameObject holding the collider component
    public Collider2D attackCollider; // The actual collider component for the attack
    public float attackDuration = 1.0f;
    private EnemyMovementController enemyMovement;
    public bool isAttacking;
    //public Vector3 originalPos;
    //public Vector3 newPos;
    //public float setDistanceFromPlayer = 0.5f; // Desired distance from the player

    void Start()
    {
        attackCollider = attackColliderGo.GetComponent<Collider2D>();
        enemyMovement = GetComponent<EnemyMovementController>();
        currentAttackCooldown = 0f; // Allows immediate attack if in range
    }

    void FixedUpdate()
    {
        if (currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
        }

        if (!enemyMovement.canMove) return; // Prevent checking for attack conditions if already attacking

        float distanceToPlayer = Vector3.Distance(transform.position, enemyMovement.playerObject.transform.position);
        if (distanceToPlayer <= enemyMovement.stoppingDistance && currentAttackCooldown <= 0)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        if (isAttacking) yield break; // Ensure that we do not stack attacks
        //originalPos = transform.position;
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        //newPos = playerPosition - directionToPlayer * setDistanceFromPlayer;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        attackColliderGo.transform.rotation = Quaternion.Euler(0, 0, angle + 90); // Adjust rotation to match direction
        enemyMovement.SetAnimatorBasedOnAngle(angle);
        isAttacking = true;
        enemyMovement.agent.isStopped = true;
        enemyMovement.canMove = false; // Prevent movement during attack
        animator.SetBool("canAttack", true);
        animator.SetFloat("Speed", 0f);
        currentAttackCooldown = attackCooldown;

        yield return new WaitForSeconds(1f);

        animator.SetBool("canAttack", false);
        enemyMovement.canMove = true;
        enemyMovement.agent.isStopped = false;
        animator.SetFloat("Speed", enemyMovement.agent.velocity.magnitude);
        isAttacking = false;
    }

    public void HandleColliderFunc()
    {
        StartCoroutine(HandleCollider());
    }

    IEnumerator HandleCollider()
    {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        attackCollider.enabled = false;
    }
}
