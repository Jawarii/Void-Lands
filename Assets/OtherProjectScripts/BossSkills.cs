using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkills : MonoBehaviour
{
    public GameObject player;
    public int dmgCoe = 50;
    public float cd;
    public float currentCd;
    public float duration;
    public float currentDuration;
    public Collider2D collider_;
    public SpriteRenderer spriteRenderer;
    public bool inUse;

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
            EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(dmgCoe, false);
        }
    }
}
