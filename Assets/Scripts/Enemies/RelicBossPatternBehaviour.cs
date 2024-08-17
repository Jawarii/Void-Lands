using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicBossPatternBehaviour : MonoBehaviour
{
    public List<GameObject> patterns = new List<GameObject>();
    private int phase = 1;
    public float hpPercent;
    public bool isAttacking = false;
    //Phase 1 Variables
    public float laserAttackElapsedTime = 0f;
    public float laserAttackCd = 7.5f;
    //Phase 2 Variables
    public float chasingLaserAttackElapsedTime = 0f;
    public float chasingLaserAttackCd = 15f;
    //Phase 3 Variables
    public float horizontalLaserAttackElapsedTime = 0f;
    public float horizontalLaserAttackCd = 20f;
    public float verticalLaserAttackElapsedTime = 0f;
    public float verticalLaserAttackCd = 20f;
    //Phase 4 Variables
    public float x2rotatingLaserAttackElapsedTime = 0f;
    public float x2rotatingLaserAttackCd = 30f;
    public float x4rotatingLaserAttackElapsedTime = 0f;
    public float x4rotatingLaserAttackCd = 30f;
    //Reference to the player GameObject
    private GameObject player;
    public Vector3 directionToPlayer;
    public Vector3 spawnPosition;
    public BossMovementController moveController;
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
        InvokeSpawnLaser();
        InvokeSpawnChasingLaser();
    }
    private void PhaseTwoBehaviour()
    {
        InvokeSpawnHorizontalLasers();
        InvokeSpawnChasingLaser();
        InvokeSpawnLaser(); 
    }
    private void PhaseThreeBehaviour()
    {
        InvokeSpawnX2RotatingLasers();
        InvokeSpawnHorizontalLasers();
        InvokeSpawnVerticalLasers();
        InvokeSpawnChasingLaser();
        InvokeSpawnLaser();
    }
    private void PhaseFourBehaviour()
    {
        InvokeSpawnX4RotatingLasers();
        InvokeSpawnX2RotatingLasers();
        InvokeSpawnHorizontalLasers();
        InvokeSpawnVerticalLasers();
        InvokeSpawnChasingLaser();
        InvokeSpawnLaser();
    }
    public void InvokeSpawnLaser()
    {
        if (laserAttackElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(SpawnLaser());
                laserAttackElapsedTime = laserAttackCd;
            }
        }
        else
        {
            laserAttackElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeSpawnChasingLaser()
    {
        if (chasingLaserAttackElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(SpawnChasingLaser());
                chasingLaserAttackElapsedTime = chasingLaserAttackCd;
            }
        }
        else
        {
            chasingLaserAttackElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeSpawnHorizontalLasers()
    {
        if (horizontalLaserAttackElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(SpawnHorizontalLasers());
                horizontalLaserAttackElapsedTime = horizontalLaserAttackCd;
            }
        }
        else
        {
            horizontalLaserAttackElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeSpawnVerticalLasers()
    {
        if (verticalLaserAttackElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(SpawnVerticalLasers());
                verticalLaserAttackElapsedTime = verticalLaserAttackCd;
            }
        }
        else
        {
            verticalLaserAttackElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeSpawnX2RotatingLasers()
    {
        if (x2rotatingLaserAttackElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(SpawnX2RotatingLasers());
                x2rotatingLaserAttackElapsedTime = x2rotatingLaserAttackCd;
            }
        }
        else
        {
            x2rotatingLaserAttackElapsedTime -= Time.deltaTime;
        }
    }
    public void InvokeSpawnX4RotatingLasers()
    {
        if (x4rotatingLaserAttackElapsedTime <= 0 && !isAttacking)
        {
            if (patterns.Count > 0)
            {
                StartCoroutine(SpawnX4RotatingLasers());
                x4rotatingLaserAttackElapsedTime = x4rotatingLaserAttackCd;
            }
        }
        else
        {
            x4rotatingLaserAttackElapsedTime -= Time.deltaTime;
        }
    }
    IEnumerator SpawnLaser()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        Vector3 directionToPlayer = moveController.playerObject.transform.position - transform.position;
        Vector3 spawnPosition = transform.position;
        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(1f, 1f, 1f);
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + 90f;
        moveController.SetAnimatorBasedOnAngle(angle - 90f);
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        StartCoroutine(patterns[0].GetComponent<LaserController>().InvokeSkill(spawnPosition, enemyStats, scale, rotation));
        moveController.canMove = false;
        yield return new WaitForSeconds(3.75f);
        moveController.canMove = true;
        isAttacking = false;
    }
    IEnumerator SpawnChasingLaser()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        Vector3 directionToPlayer = moveController.playerObject.transform.position - transform.position;
        Vector3 spawnPosition = transform.position;
        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(1f, 1f, 1f);
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + 90f;
        moveController.SetAnimatorBasedOnAngle(angle - 90f);
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        StartCoroutine(patterns[1].GetComponent<ChasingLaserController>().InvokeSkill(spawnPosition, enemyStats, scale, rotation));
        moveController.canMove = false;
        float castTime = patterns[1].GetComponent<ChasingLaserController>().castTime;
        yield return new WaitForSeconds(castTime);
        StartCoroutine(patterns[1].GetComponent<ChasingLaserController>().HandleRotation());
        yield return new WaitForSeconds(3);
        moveController.canMove = true;
        isAttacking = false;
    }
    IEnumerator SpawnHorizontalLasers()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        List<Vector3> spawnPosition = HorizontalLaserPositions();
        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(1.7f, 1.7f, 1f);
        float angle = -90f;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        for (int i = 0; i < spawnPosition.Count - 1; i++)
        {
            StartCoroutine(patterns[2].GetComponent<LaserController>().InvokeSkill(spawnPosition[i], enemyStats, scale, rotation));
        }
        yield return new WaitForSeconds(6f);
        isAttacking = false;
    }
    IEnumerator SpawnVerticalLasers()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        List<Vector3> spawnPosition = VerticalLaserPositions();
        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(1.7f, 1.7f, 1f);
        float angle = 0;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        for (int i = 0; i < spawnPosition.Count - 1; i++)
        {
            StartCoroutine(patterns[2].GetComponent<LaserController>().InvokeSkill(spawnPosition[i], enemyStats, scale, rotation));
        }
        yield return new WaitForSeconds(6f);
        isAttacking = false;
    }
    IEnumerator SpawnX2RotatingLasers()
    {
        if (isAttacking) yield break;
        isAttacking = true;

        Vector3 spawnPosition = transform.position;
        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(1f, 1f, 1f);

        // Choose a random angle between 0 and 360 degrees
        float angle1 = Random.Range(0f, 360f);
        float angle2 = angle1 + 180f;
        if (angle2 > 360f) angle2 -= 360f;

        Quaternion rotation1 = Quaternion.Euler(new Vector3(0f, 0f, angle1));
        Quaternion rotation2 = Quaternion.Euler(new Vector3(0f, 0f, angle2));

        // Randomly determine the rotation direction
        bool rotateClockwise = Random.value > 0.5f;

        // Instantiate and set up the first laser
        GameObject laser1 = Instantiate(patterns[3], spawnPosition, rotation1);
        var laserController1 = laser1.GetComponent<RotatingLasersController>();
        StartCoroutine(laserController1.InvokeSkill(spawnPosition, enemyStats, scale, rotation1));
        float castTime1 = laserController1.castTime;

        // Instantiate and set up the second laser
        GameObject laser2 = Instantiate(patterns[3], spawnPosition, rotation2);
        var laserController2 = laser2.GetComponent<RotatingLasersController>();
        StartCoroutine(laserController2.InvokeSkill(spawnPosition, enemyStats, scale, rotation2));
        float castTime2 = laserController2.castTime;

        yield return new WaitForSeconds(Mathf.Max(castTime1, castTime2));

        // Start rotating both lasers in the same direction
        StartCoroutine(laserController1.HandleRotation(rotateClockwise));
        StartCoroutine(laserController2.HandleRotation(rotateClockwise));

        yield return new WaitForSeconds(3);
        isAttacking = false;
        Destroy(laser1);
        Destroy(laser2);
    }
    IEnumerator SpawnX4RotatingLasers()
    {
        if (isAttacking) yield break;
        isAttacking = true;

        Vector3 spawnPosition = transform.position;
        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(1f, 1f, 1f);

        // Choose a random base angle between 0 and 360 degrees
        float baseAngle = Random.Range(0f, 360f);
        float[] angles = new float[4];

        // Calculate the perpendicular angles
        for (int i = 0; i < 4; i++)
        {
            angles[i] = (baseAngle + i * 90f) % 360f;
        }

        Quaternion[] rotations = new Quaternion[4];

        for (int i = 0; i < angles.Length; i++)
        {
            rotations[i] = Quaternion.Euler(new Vector3(0f, 0f, angles[i]));
        }

        // Randomly determine the rotation direction
        bool rotateClockwise = Random.value > 0.5f;

        // Instantiate and set up the lasers
        GameObject[] lasers = new GameObject[4];
        RotatingLasersController[] laserControllers = new RotatingLasersController[4];
        float maxCastTime = 0f;

        for (int i = 0; i < 4; i++)
        {
            lasers[i] = Instantiate(patterns[3], spawnPosition, rotations[i]);
            laserControllers[i] = lasers[i].GetComponent<RotatingLasersController>();
            StartCoroutine(laserControllers[i].InvokeSkill(spawnPosition, enemyStats, scale, rotations[i]));
            maxCastTime = Mathf.Max(maxCastTime, laserControllers[i].castTime);
        }

        yield return new WaitForSeconds(maxCastTime);

        // Start rotating all lasers in the same direction
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(laserControllers[i].HandleRotation(rotateClockwise));
        }

        yield return new WaitForSeconds(3);
        isAttacking = false;
        for (int j = 0; j < 4; j++)
        {
            Destroy(lasers[j]);
        }
    }

    public List<Vector3> HorizontalLaserPositions()
    {
        List<Vector3> positions = new List<Vector3>
    {
        new Vector3(11.35f, -3, 0),
        new Vector3(11.35f, -4, 0),
        new Vector3(11.35f, -5, 0),
        new Vector3(11.35f, -6, 0),
        new Vector3(11.35f, -7, 0),
        new Vector3(11.35f, -8, 0),
        new Vector3(11.35f, -9, 0),
        new Vector3(11.35f, -10, 0),
        new Vector3(11.35f, -11, 0),
        new Vector3(11.35f, -12, 0),
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
    public List<Vector3> VerticalLaserPositions()
    {
        List<Vector3> positions = new List<Vector3>
    {
        new Vector3(1, -2.25f, 0),
        new Vector3(2, -2.25f, 0),
        new Vector3(3, -2.25f, 0),
        new Vector3(4, -2.25f, 0),
        new Vector3(5, -2.25f, 0),
        new Vector3(6, -2.25f, 0),
        new Vector3(7, -2.25f, 0),
        new Vector3(8, -2.25f, 0),
        new Vector3(9, -2.25f, 0),
        new Vector3(10, -2.25f, 0),
        new Vector3(11, -2.25f, 0),
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
}
