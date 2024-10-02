using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperBossPatternBehaviour : MonoBehaviour
{
    public List<GameObject> patterns = new List<GameObject>();
    private int phase = 1;
    public float hpPercent;
    public bool isAttacking = false;
    public bool isTargetExplosion = false;
    //Phase 1 Variables
    private int _projectileAmount = 3;
    private float _projectileInterval = 0.2f;
    public float _projectileAttackCd = 8f;
    public float _projectileAttackElapsedTime = 0f;
    public float targettedExplosionsCd = 12f;
    public float targettedExplosionsElapsedTime = 0f;
    //Phase 2 Variables
    public float teleportProjectileCd = 24f;
    public float teleportProjectileElapsedTime = 0f;
    //Phase 3 Variables
    public float ringAttackCd = 16f;
    public float ringAttackElapsedTime = 0f;
    //Phase 4 Variables
    public float enhancedRingAttackCd = 24f;
    public float enhancedRingAttackElapsedTime = 0f;
    //Reference to the player GameObject
    private GameObject player;
    public Vector3 directionToPlayer;
    public Vector3 spawnPosition;
    public BossMovementController moveController;
    public GameObject projectilePrefab;
    public EnemyStats enemyStats;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hpPercent = transform.GetComponent<EnemyStats>().hp / transform.GetComponent<EnemyStats>().maxHp;
        moveController = transform.GetComponent<BossMovementController>();
        enemyStats = transform.GetComponent<EnemyStats>();
    }

    void Update()
    {
        if (!moveController.inPursuit)
            return;
        hpPercent = transform.GetComponent<EnemyStats>().hp / transform.GetComponent<EnemyStats>().maxHp;
        UpdatePhases();
    }
    public void UpdatePhases()
    {
        if (hpPercent > 0.75f)
        {
            if (phase <= 1) // Reason for this check is that when the boss heals, it doesn't go back to a previous phase.
                phase = 1;
        }
        else if (hpPercent > 0.50f)
        {
            if (phase <= 2) // Reason for this check is that when the boss heals, it doesn't go back to a previous phase.
                phase = 2;
        }
        else if (hpPercent > 0.25f)
        {
            if (phase <= 3) // Reason for this check is that when the boss heals, it doesn't go back to a previous phase.
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
    private void PhaseOneBehaviour()
    {
        InvokeProjectileFanAttack();
        InvokeTargettedExplosions();
    }
    private void PhaseTwoBehaviour()
    {
        InvokeTeleportProjectileAttack();
        InvokeProjectileFanAttack();
        InvokeTargettedExplosions();
    }
    private void PhaseThreeBehaviour()
    {
        InvokeTeleportProjectileAttack();
        InvokeRingAttack();
        InvokeProjectileFanAttack();
        InvokeTargettedExplosions();
    }
    private void PhaseFourBehaviour()
    {
        InvokeEnhancedRingAttack();
        InvokeTeleportProjectileAttack();
        InvokeRingAttack();
        InvokeProjectileFanAttack();
        InvokeTargettedExplosions();
    }
    public void InvokeTargettedExplosions()
    {
        if (targettedExplosionsElapsedTime <= 0)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(TargettedExplosions(phase, 1f));
            }
        }
        else
        {
            targettedExplosionsElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeProjectileFanAttack()
    {
        if (_projectileAttackElapsedTime <= 0 && !isAttacking)
        {
            StartCoroutine(ProjectileFanAttack());
            _projectileAttackElapsedTime = _projectileAttackCd;
        }
        else
        {
            _projectileAttackElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeTeleportProjectileAttack()
    {
        moveController.speed_ = 30f;
        if (teleportProjectileElapsedTime <= 0 && !isAttacking)
        {
            StartCoroutine(TeleportProjectileAttack());
            teleportProjectileElapsedTime = teleportProjectileCd;
        }
        else
        {
            teleportProjectileElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeRingAttack()
    {
        if (ringAttackElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(RingAttack());
                ringAttackElapsedTime = ringAttackCd;
            }
        }
        else
        {
            ringAttackElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeEnhancedRingAttack()
    {
        if (enhancedRingAttackElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(EnhancedRingAttack());
                enhancedRingAttackElapsedTime = enhancedRingAttackCd;
            }
        }
        else
        {
            enhancedRingAttackElapsedTime -= Time.deltaTime;
        }
    }
    IEnumerator TargettedExplosions(float amount, float interval)
    {
        if (isTargetExplosion) yield break;
        isTargetExplosion = true;
        for (int i = 1; i <= amount; i++)
        {
            spawnPosition = player.transform.position;

            EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
            Vector3 scale = new Vector3(1.35f, 1.35f, 1);
            StartCoroutine(patterns[0].GetComponent<ExplosionController>().InvokeSkill(spawnPosition, enemyStats, scale));
            yield return new WaitForSeconds(interval);
        }
        float targettedExplosionsCdRnd = Random.Range(targettedExplosionsCd * 0.75f, targettedExplosionsCd + 1);
        targettedExplosionsElapsedTime = targettedExplosionsCdRnd;
        isTargetExplosion = false;
    }
    IEnumerator ProjectileFanAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        Vector3 directionToPlayer = moveController.playerObject.transform.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        float spreadAngle = 45f;
        int projectilesPerFan = 5;

        for (int line = 0; line < _projectileAmount; line++)
        {
            float startAngle = angle - spreadAngle / 2;

            for (int i = 0; i < projectilesPerFan; i++)
            {
                float projectileAngle = startAngle + (spreadAngle / (projectilesPerFan - 1)) * i;
                GameObject projectile_ = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, projectileAngle)));
                projectile_.GetComponent<GolemProjectileBehavior>().enemyStats = enemyStats;
            }

            yield return new WaitForSeconds(_projectileInterval);
        }
        isAttacking = false;
    }
    IEnumerator TeleportProjectileAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        List<Vector3> positions = TeleportPositions();
        for (int _index = 0; _index < 4; _index++)
        {
            moveController.speed_ = 30f;
            moveController.agent.SetDestination(positions[_index]);
            yield return new WaitForSeconds(1f);
            Vector3 directionToPlayer = moveController.playerObject.transform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            float spreadAngle = 45f;
            int projectilesPerFan = 5;

            for (int line = 0; line < _projectileAmount; line++)
            {
                float startAngle = angle - spreadAngle / 2;

                for (int i = 0; i < projectilesPerFan; i++)
                {
                    float projectileAngle = startAngle + (spreadAngle / (projectilesPerFan - 1)) * i;
                    GameObject projectile_ = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, projectileAngle)));
                    projectile_.GetComponent<GolemProjectileBehavior>().enemyStats = enemyStats;
                }

                yield return new WaitForSeconds(_projectileInterval);
            }
        }
        moveController.MoveToOriginalPosition();
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
    IEnumerator RingAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        float angleIncrement = 20f;

        for (int i = 1; i <= 3; i++)
        {
            for (int j = 0; j < (120f / angleIncrement) * i; j++)
            {
                float angle = j * angleIncrement * 3 / i;
                float radians = angle * Mathf.Deg2Rad;
                Vector3 spawnPosition = transform.position + new Vector3(Mathf.Cos(radians) * i * 1.5f, Mathf.Sin(radians) * i * 1.5f, 0);
                EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
                Vector3 scale = new Vector3(2f, 2f, 1);
                StartCoroutine(patterns[0].GetComponent<ExplosionController>().InvokeSkill(spawnPosition, enemyStats, scale));
            }
            yield return new WaitForSeconds(1.5f);
        }
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
    IEnumerator EnhancedRingAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        float angleIncrement = 20f;

        List<(int, int)> radiusPairs = new List<(int, int)>
    {
        (1, 3),
        (2, 3),
        (1, 2)
    };

        radiusPairs = ShufflePairs(radiusPairs);

        foreach (var pair in radiusPairs)
        {
            StartCoroutine(SpawnRing(pair.Item1, angleIncrement));
            StartCoroutine(SpawnRing(pair.Item2, angleIncrement));
            yield return new WaitForSeconds(1.5f);
        }
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    IEnumerator SpawnRing(int radius, float angleIncrement)
    {
        int numExplosions = (int)(120f / angleIncrement) * radius;
        for (int j = 0; j < numExplosions; j++)
        {
            float angle = j * angleIncrement * 3 / radius;
            float radians = angle * Mathf.Deg2Rad;
            Vector3 spawnPosition = transform.position + new Vector3(Mathf.Cos(radians) * radius * 1.5f, Mathf.Sin(radians) * radius * 1.5f, 0);
            EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
            Vector3 scale = new Vector3(2f, 2f, 1);
            StartCoroutine(patterns[0].GetComponent<ExplosionController>().InvokeSkill(spawnPosition, enemyStats, scale));
        }
        yield return null;
    }

    List<(int, int)> ShufflePairs(List<(int, int)> inputList)
    {
        System.Random rng = new System.Random();
        int n = inputList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value = inputList[k];
            inputList[k] = inputList[n];
            inputList[n] = value;
        }
        return inputList;
    }

    public List<Vector3> TeleportPositions()
    {
        List<Vector3> positions = new List<Vector3>
    {
        new Vector3(0.55f, -4.4f, 0),
        new Vector3(0.55f, -10.55f, 0),
        new Vector3(11.55f, -4.4f, 0),
        new Vector3(11.55f, -10.55f, 0)
    };

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
    public void ResetVariables()
    {
        // Reset phase and HP percentage
        phase = 1;
        hpPercent = transform.GetComponent<EnemyStats>().hp / transform.GetComponent<EnemyStats>().maxHp;

        // Reset attack and behavior flags
        isAttacking = false;
        isTargetExplosion = false;

        // Reset timers for all attacks
        _projectileAttackElapsedTime = 0f;
        targettedExplosionsElapsedTime = 0f;
        teleportProjectileElapsedTime = 0f;
        ringAttackElapsedTime = 0f;
        enhancedRingAttackElapsedTime = 0f;

        // Optionally reset cooldowns (if needed to change them during the battle)
        _projectileAttackCd = 8f;
        targettedExplosionsCd = 12f;
        teleportProjectileCd = 24f;
        ringAttackCd = 16f;
        enhancedRingAttackCd = 24f;

        Debug.Log("Boss variables reset to default values.");
    }

}
