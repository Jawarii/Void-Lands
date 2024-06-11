using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrowBehaviour : MonoBehaviour
{
    public float speed = 10f; // Speed of the arrow
    private int crit = 0;
    public GameObject player;
    public PlayerStats playerStats;
    public float basicAtkDmgMulti = 0.5f;
    public float knockbackForce = 10f; // Adjust the force of knockback as needed
    public float lifeTime = 1f;
    public Collider2D _collider;
    public bool isImbued = false;

    private void Start()
    {
        // Call the DestroyArrow function after 2 seconds
        StartCoroutine(DestroyArrow());
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        knockbackForce = 10f;
        //_collider = transform.GetComponent<Collider2D>();
        StartCoroutine(EnableCollider(0f));
    }

    private void Update()
    {
        // Move the arrow forward based on its current rotation
        transform.Translate(Vector2.down * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            crit = Random.Range(1, 101);

            float minDmg = basicAtkDmgMulti * (playerStats.attack - enemyStats.defense) * 0.9f;
            float maxDmg = basicAtkDmgMulti * (playerStats.attack - enemyStats.defense) * 1.1f;
            float critDmgMulti = playerStats.critDmg / 100.0f;

            if (crit <= playerStats.critRate)
            {
                enemyStats.TakeDamage((int)(Random.Range(minDmg, maxDmg) * critDmgMulti), true);
            }
            else
            {
                enemyStats.TakeDamage((int)Random.Range(minDmg, maxDmg), false);
            }
        }
    }

    IEnumerator DestroyArrow()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }

    IEnumerator EnableCollider(float _time)
    {
        yield return new WaitForSeconds(_time);

        _collider.enabled = true;
        StartCoroutine(DisableCollider());
    }
    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(0.1f);

        _collider.enabled = false;
        StartCoroutine(EnableCollider(0.1f));
    }
}
