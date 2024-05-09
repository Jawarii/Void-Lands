using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float randomExplosionsCd = 3f;
    public float randomExplosionsElapsedTime;

    //Phase 2 Variables
    public float dashExplosionCd = 5;
    public float dashExplosionElapsedTime;

    //Phase 3 Variables
    public float pentagramExplosionCd = 15;
    public float pentagramExplosionElapsedTime;

    //Phase 4 Variables
    public float largeExplosionCd = 15;
    public float largeExplosionElapsedTime;
    public float cloneExplosionCd = 30f;
    public float cloneExplosionElapsedTime;
    public GameObject clonePrefab;

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
        if (!moveController.inPursuit)
            return;
        hpPercent = transform.GetComponent<EnemyStats>().hp / transform.GetComponent<EnemyStats>().maxHp;
        UpdatePhases();
    }

    public void PhaseOneBehaviour()
    {
        InvokeFrontalExplosions();
        InvokeRandomExplosions();
    }
    public void PhaseTwoBehaviour()
    {
        InvokeDashExplosions();
        InvokeRandomExplosions();
    }
    public void PhaseThreeBehaviour()
    {
        InvokePentagramExplosions();
        InvokeDashExplosions();
        InvokeRandomExplosions();
    }
    public void PhaseFourBehaviour()
    {
        InvokeCloneExplosions();
        InvokeLargeExplosion();
        InvokePentagramExplosions();
        InvokeRandomExplosions();
        InvokeDashExplosions();
    }
    public void InvokeFrontalExplosions()
    {
        if (frontalExplosionElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(FrontalPurpleExplosions(15, 0.05f));
                frontalExplosionCd = Random.Range(3, 6);
                frontalExplosionElapsedTime = frontalExplosionCd;
            }
        }
        else
        {
            frontalExplosionElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeRandomExplosions()
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
    }
    public void InvokeDashExplosions()
    {
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
    }
    public void InvokePentagramExplosions()
    {
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
    public void InvokeCloneExplosions()
    {
        if (cloneExplosionElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(CloneExplosion());
                cloneExplosionCd = Random.Range(25, 30);
                cloneExplosionElapsedTime = cloneExplosionCd;
            }
        }
        else
        {
            cloneExplosionElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeLargeExplosion()
    {
        if (largeExplosionElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                moveController.speed_ = 30f;
                moveController.MoveToOriginalPosition();
                StartCoroutine(LargeExplosion(1.25f));
                largeExplosionCd = Random.Range(10, 15);
                largeExplosionElapsedTime = largeExplosionCd;
            }
        }
        else
        {
            largeExplosionElapsedTime -= Time.deltaTime;
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
        Vector3 scale = new Vector3(2.25f, 2.25f, 1);
        if (phase == 4)
            amount = 3;
        for (int i = 0; i < amount * 2; i++)
        {
            Vector3 randomOffset = Random.insideUnitCircle * radius;
            Vector3 spawnPosition = player.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            StartCoroutine(patterns[0].GetComponent<PurpleExplosionController>().InvokeSkill(spawnPosition, enemyStats, scale));
        }
    }
    IEnumerator LargeExplosion(float waitTime)
    {
        isAttacking = true;
        yield return new WaitForSeconds(waitTime);

        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(15f, 15f, 1);

        Vector3 spawnPosition = transform.position;

        StartCoroutine(patterns[1].GetComponent<PurpleExplosionController>().InvokeSkill(spawnPosition, enemyStats, scale));
        yield return new WaitForSeconds(3f);
        isAttacking = false;
    }
    IEnumerator CloneExplosion()
    {
        isAttacking = true;
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        collider.enabled = false;

        // Fade out the sprite
        float duration = 1.0f;
        float currentTime = 0f;
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1.0f, 0.0f, currentTime / duration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = endColor; // Ensure it's fully transparent
        spriteRenderer.enabled = false;

        List<Vector3> positions = ClonePositions();
        moveController.speed_ = 10f;
        moveController.agent.SetDestination(positions[3]);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, positions[3]) < 0.1f);
        yield return new WaitForSeconds(2);

        spriteRenderer.color = startColor;
        spriteRenderer.enabled = true;
        collider.enabled = true;
        StartCoroutine(LargeExplosion(0));
        for (int i = 0; i < 3; i++)
        {
            Instantiate(clonePrefab, positions[i], Quaternion.identity);
        }
    }
    public List<Vector3> ClonePositions()
    {
        List<Vector3> positions = new List<Vector3>
    {
        new Vector3(5, -6, 0),
        new Vector3(5, 1, 0),
        new Vector3(-6.5f, 1, 0),
        new Vector3(-6.5f, -6, 0)
    };

        // Shuffle the positions to randomize their order
        int n = positions.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Vector3 value = positions[k];
            positions[k] = positions[n];
            positions[n] = value;
        }

        return positions;
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
        else if (hpPercent > 0.25f)
        {
            phase = 3;
        }
        else
        {
            phase = 4;
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
            case 4:
                PhaseFourBehaviour();
                break;
        }
    }
}
