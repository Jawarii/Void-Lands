using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    public float speed = 6.0f;
    private Vector3 moveDirection;
    private float lifePeriod = 1.5f;
    private EnemyStats stats;
    public int dmgCoe = 80;
    public int critCoe = 0;
    public GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //moveDirection = transform.up;
    }

    void Update()
    {
        lifePeriod -= Time.deltaTime;
        transform.position += moveDirection * speed * Time.deltaTime;
        if (lifePeriod <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Method to set the move direction for the arrow
    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            PlayerStats playerStats = player.gameObject.GetComponent<PlayerStats>();
            EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
            float randomDmg = Random.Range(dmgCoe * .95f, dmgCoe * 1.05f) * playerStats.attack / 10.0f;
            int critCheck = Random.Range(1, 101);
            if (critCheck <= playerStats.critRate + critCoe)
            {
                enemyStats.TakeDamage((int)(randomDmg * (playerStats.critDmg / 100.0f)), true);
            }
            else
            {
                enemyStats.TakeDamage((int)randomDmg, false);
            }
            Destroy(gameObject);
        }
    }
}
