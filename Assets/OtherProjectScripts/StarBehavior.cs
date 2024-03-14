using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBehavior : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector3 moveDirection;
    public Vector3 rotation_ = new Vector3(0, 0, 90);
    public float spinPeriod = 1.5f;
    private EnemyStats stats;
    public int dmgCoe = 40;
    public int critCoe = 0;
    public GameObject player;
    public float range_ = 5f;
    private Vector3 originPos;
    private float distance_;
    private float spinTime_ = 0f;
    private bool isReturning = false;
    public float hitCd = 0.3f;
    private float hitTime_ = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        moveDirection = transform.up;
        originPos = new Vector3(transform.position.x, transform.position.y,transform.position.z);
    }

    void Update()
    {
        hitTime_ -= Time.deltaTime;
        transform.position += moveDirection * speed * Time.deltaTime;
        distance_ = Vector3.Distance(transform.position, originPos);
        if (distance_ >= range_ && !isReturning)
        {
            speed = 0.15f;
            spinTime_ += Time.deltaTime;
            if (spinTime_ >= spinPeriod)
            {
                speed = -12f;
                originPos = transform.position;
                isReturning = true;
            }
        }
        if (isReturning)
        {
            distance_ = Vector3.Distance(transform.position, player.transform.position);
            moveDirection = transform.position - player.transform.position;
            moveDirection.Normalize();
            if (distance_ <= 0.5f)
            {
                Destroy(gameObject);
            }
        }
        Vector3 rotation = rotation_ * Time.deltaTime;
        transform.Rotate(rotation);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && hitTime_ <= 0f)
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
            hitTime_ = hitCd;
        }
    }
}
