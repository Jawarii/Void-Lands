using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    public GameObject player;
    public float baseRadius = 5f;
    public float radiusMulti = 1f;
    public int dmgCoe = 33;
    public int critCoe = 0;
    public float cd;
    public float currentCd;
    public float duration;
    public float currentDuration;
    public Collider2D collider_;
    public SpriteRenderer spriteRenderer;
    public bool inUse;

    void Start()
    {
        transform.localScale *= baseRadius;
    }
    void Update()
    {
        if (currentCd <= 0)
        {
            UseSkill();
        }
        else
        {
            currentCd -= Time.deltaTime;
        }
        if (currentDuration >= duration)
        {
            StopSkill();
        }
        else
        {
            if (inUse)
            {
                currentDuration += Time.deltaTime;
            }
        }
        transform.localScale = new Vector3(radiusMulti * baseRadius, radiusMulti * baseRadius, 1f);
    }

    public void UseSkill()
    {
        collider_.enabled = true;
        spriteRenderer.enabled = true;
        inUse = true;
        currentCd = cd;
    }
    public void StopSkill()
    {
        collider_.enabled = false;
        spriteRenderer.enabled = false;
        currentDuration = 0;
        inUse = false;
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
        }
    }
}
