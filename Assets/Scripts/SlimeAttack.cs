using UnityEngine;
using System.Collections;

public class SlimeAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 2.0f;
    private float currentAttackCooldown;
    public GameObject attackColliderGo; // The GameObject holding the collider component
    public Collider2D attackCollider; // The actual collider component for the attack
    public float attackDuration = 1.0f;
    private EnemyMovementController slimeMovement;

    void Start()
    {
        attackCollider = attackColliderGo.GetComponent<Collider2D>();
        slimeMovement = GetComponent<EnemyMovementController>();
        currentAttackCooldown = 0f; // Allows immediate attack if in range
    }

    void Update()
    {
        if (currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
        }

        if (!slimeMovement.canMove) return; // Prevent checking for attack conditions if already attacking

        float distanceToPlayer = Vector3.Distance(transform.position, slimeMovement.playerObject.transform.position);
        if (distanceToPlayer <= slimeMovement.stoppingDistance && currentAttackCooldown <= 0)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        slimeMovement.canMove = false; // Prevent movement during attack
        animator.SetBool("canAttack", true);
        currentAttackCooldown = attackCooldown;

        // Sequence of enabling and disabling the attack collider to simulate "ticks"
        for (int i = 0; i < 3; i++) // Perform three "ticks" of the attack
        {
            yield return new WaitForSeconds(attackDuration / 6);
            attackCollider.enabled = true;
            yield return new WaitForSeconds(attackDuration / 6); // Divide by number of ticks
            attackCollider.enabled = false;
           
        }

        animator.SetBool("canAttack", false);
        slimeMovement.canMove = true; // Re-enable movement after attack
    }
}
