using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public GameObject player;
    public float speed = 4.0f;
    float colTimer = 0.8f;
    float dist = 0;
    bool isColliding = false;
    private PlayerStats stats;
    public EnemyStats bossStats; // Rotation speed in degrees per second
    private float currentRotation = .0f;
    public BulletSpawner bulletSpawner;
    public bool canMove = true;
    public bool canRotate = true;
    private float rotateCd = 0f;
    private Vector3 direction;
    public float rotationSpeed = 0; // Degrees per second for a 180-degree rotation in 1 second

    public float maxRotationSpeed = 360.0f; // Maximum rotation speed in degrees per second
    public float minRotationSpeed = 1.0f;  // Minimum rotation speed in degrees per second
    private Quaternion targetRotation;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        stats = player.GetComponent<PlayerStats>();
        bossStats = gameObject.GetComponent<EnemyStats>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == player)
        {
            isColliding = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject == player)
        {
            isColliding = false;
        }
    }

    private void Update()
    {
        colTimer += Time.deltaTime;
        direction = player.transform.position - transform.position;
        direction.Normalize();
        dist = Vector3.Distance(player.transform.position, transform.position);

        if (canRotate)
        {
            RotateTowardsPlayer();
        }

        if (canMove)
        {
            MoveBasedOnRotation();
        }

        if (isColliding && colTimer >= 0.8f)
        {
            colTimer = 0;
            stats.TakeDamage((int)(30f * bossStats.attack / 10f), false);
        }
        if (!canRotate)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, step);
        }
    }

    private void RotateTowardsPlayer()
    {
        // Calculate the target rotation based on the direction to the player
        float newRotationZ = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, -newRotationZ);

        // Calculate the rotation speed based on the angle to the player
        float angleToPlayer = Quaternion.Angle(transform.rotation, targetRotation);
        float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, angleToPlayer / 180.0f);

        // Rotate towards the targetRotation using Quaternion.RotateTowards
        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }

    private void MoveBasedOnRotation()
    {
        // Calculate the movement direction based on the boss's current rotation
        Vector3 movementDirection = transform.up; // Assuming forward movement along the boss's up direction

        if (dist >= 0.5f)
        {
            // Move the boss in the calculated movement direction
            transform.position += movementDirection * speed * Time.deltaTime;
        }
    }
}
