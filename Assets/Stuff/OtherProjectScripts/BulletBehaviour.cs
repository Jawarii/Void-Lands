using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float speed = 6.0f;
    private Vector3 moveDirection;
    private float lifePeriod = 1.5f;
    private PlayerStats stats;
    public GameObject player;
    public GameObject boss;
    public int damage = 10;
    public EnemyStats enemyStats;


    void Start()
    {
        moveDirection = transform.up;
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.Find("BossTest");
        enemyStats = boss.GetComponent<EnemyStats>();
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            stats = other.GetComponent<PlayerStats>();
            stats.TakeDamage((int)(damage * enemyStats.attack / 10f), false);
            Destroy(gameObject);
        }
    }
}
