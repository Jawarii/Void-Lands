using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternsBehaviour : MonoBehaviour
{
    public List<GameObject> patterns = new List<GameObject>();
    private int phase = 1;
    public float hpPercent;
    public bool isAttacking = false;

    //Phase 1 Variables
    public float frontalExplosionCd = 5;
    public float frontalExplosionElapsedTime;
    public float randomExplosionsCd = 0.5f;
    public float randomExplosionsElapsedTime;

    //Phase 2 Variables
    public float dashExplosionCd = 5;
    public float dashExplosionElapsedTime;
    public float pentagramExplosionCd = 15;
    public float pentagramExplosionElapsedTime;

    //Reference to the player GameObject
    private GameObject player;
    public Vector3 directionToPlayer;
    public Vector3 spawnPosition;
    public BossMovementController moveController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hpPercent = transform.GetComponent<EnemyStats>().hp / transform.GetComponent<EnemyStats>().maxHp;
        moveController = transform.GetComponent<BossMovementController>();
    }

    void Update()
    {
        hpPercent = transform.GetComponent<EnemyStats>().hp / transform.GetComponent<EnemyStats>().maxHp;
        UpdatePhases();
    }

    public void PhaseOneBehaviour()
    {

        if (frontalExplosionElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(FrontalPurpleExplosions(10, 0.05f));
                frontalExplosionCd = Random.Range(3, 6);
                frontalExplosionElapsedTime = frontalExplosionCd;
            }
        }
        else
        {
            frontalExplosionElapsedTime -= Time.deltaTime;
        }

        if (randomExplosionsElapsedTime <= 0)
        {
            if (patterns.Count > 0)
            {
                SpawnRandomExplosions(phase);
                randomExplosionsCd = Random.Range(2, 5);
                randomExplosionsElapsedTime = randomExplosionsCd;

            }
        }
        else
        {
            randomExplosionsElapsedTime -= Time.deltaTime;
        }
    }
    public void PhaseTwoBehaviour()
    {
        if (dashExplosionElapsedTime <= 0 && !isAttacking )
        {
            if (patterns.Count > 0)
            {
                moveController.speed_ = 10;
                moveController.MoveTowardsPlayer(12);
                StartCoroutine(DashPurpleExplosions(15, 0.05f));
                dashExplosionCd = Random.Range(3, 6);
                dashExplosionElapsedTime = dashExplosionCd;
            }
        }
        else
        {
            dashExplosionElapsedTime -= Time.deltaTime;
        }
        if (randomExplosionsElapsedTime <= 0)
        {
            if (patterns.Count > 0)
            {
                SpawnRandomExplosions(phase);
                randomExplosionsCd = Random.Range(2, 5);
                randomExplosionsElapsedTime = randomExplosionsCd;
            }
        }
        else
        {
            randomExplosionsElapsedTime -= Time.deltaTime;
        }
    }
    public void PhaseThreeBehaviour()
    {
        if (randomExplosionsElapsedTime <= 0)
        {
            if (patterns.Count > 0)
            {
                SpawnRandomExplosions(phase);
                randomExplosionsCd = Random.Range(2, 5);
                randomExplosionsElapsedTime = randomExplosionsCd;
            }
        }
        else
        {
            randomExplosionsElapsedTime -= Time.deltaTime;
        }
        if (dashExplosionElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                moveController.speed_ = 10;
                moveController.MoveTowardsPlayer(12);
                StartCoroutine(DashPurpleExplosions(15, 0.05f));
                dashExplosionCd = Random.Range(3, 6);
                dashExplosionElapsedTime = dashExplosionCd;
            }
        }
        else
        {
            dashExplosionElapsedTime -= Time.deltaTime;
        }
        if (pentagramExplosionElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                moveController.speed_ = 50;
                pentagramExplosionCd = Random.Range(10, 15);
                pentagramExplosionElapsedTime = pentagramExplosionCd;
                moveController.MoveInPentagramStar();
                StartCoroutine(DashPurpleExplosions(40, 0.03f));
            }
        }
        else
        {
            pentagramExplosionElapsedTime -= Time.deltaTime;
        }
    }
    IEnumerator FrontalPurpleExplosions(float amount, float interval)
    {
        isAttacking = true;
        for (int i = 1; i <= amount; i++)
        {
            if (i == 1)
            {
                directionToPlayer = (player.transform.position - transform.position).normalized;
            }
            spawnPosition = transform.position + directionToPlayer * (i / 1.5f);

            EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
            Vector3 scale = new Vector3(1.5f, 1.5f, 1);
            StartCoroutine(patterns[0].GetComponent<PurpleExplosionController>().InvokeSkill(spawnPosition, enemyStats, scale));
            yield return new WaitForSeconds(interval);
        }
        isAttacking = false;
    }
    IEnumerator DashPurpleExplosions(float amount, float interval)
    {
        isAttacking = true;
        for (int i = 1; i <= amount; i++)
        {
            spawnPosition = transform.position;

            EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
            Vector3 scale = new Vector3(2f, 2f, 1);
            StartCoroutine(patterns[0].GetComponent<PurpleExplosionController>().InvokeSkill(spawnPosition, enemyStats, scale));
            yield return new WaitForSeconds(interval);
        }
        yield return new WaitForSeconds(1.5f);
        moveController.MoveToOriginalPosition();
        isAttacking = false;
    }
    public void SpawnRandomExplosions(int amount)
    {
        float radius = 3f;
        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(2.5f, 2.5f, 1);

        for (int i = 0; i < amount * 2; i++)
        {
            Vector3 randomOffset = Random.insideUnitCircle * radius;
            Vector3 spawnPosition = player.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            StartCoroutine(patterns[0].GetComponent<PurpleExplosionController>().InvokeSkill(spawnPosition, enemyStats, scale));
        }
    }
    public void UpdatePhases()
    {
        if (hpPercent > 0.75f)
        {
            phase = 1;
        }
        else if (hpPercent > 0.50f)
        {
            phase = 2;
        }
        else
        {
            phase = 3;
        }
        switch (phase)
        {
            case 1:
                PhaseOneBehaviour();
                break;
            case 2:
                PhaseTwoBehaviour();
                break;
            case 3:
                PhaseThreeBehaviour();
                break;
        }
    }
}
