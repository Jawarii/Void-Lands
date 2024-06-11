using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBossPatternBehaviour : MonoBehaviour
{
    public List<GameObject> patterns = new List<GameObject>();
    private int phase = 1;
    public float hpPercent;
    public bool isAttacking = false;

    //Phase 1 Variables
    private int _projectileAmount = 3;
    private float _projectileInterval = 0.1f;
    public float _projectileAttackCd = 5f;
    public float _projectileAttackElapsedTime = 0f;
    //Phase 2 Variables

    //Phase 3 Variables

    //Phase 4 Variables

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

    // Update is called once per frame
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

    private void PhaseOneBehaviour()
    {
        if (_projectileAttackElapsedTime <= 0)
        {
            StartCoroutine(ProjectileAttackTest());
            _projectileAttackElapsedTime = _projectileAttackCd;
        }
        else
        {
            _projectileAttackElapsedTime -= Time.deltaTime;
        }
    }

    private void PhaseThreeBehaviour()
    {
        throw new NotImplementedException();
    }

    private void PhaseTwoBehaviour()
    {
        throw new NotImplementedException();
    }

    private void PhaseFourBehaviour()
    {
        throw new NotImplementedException();
    }
    IEnumerator ProjectileAttackTest()
    {
        if (isAttacking) yield break; // Ensure that we do not stack attacks
        isAttacking = true;
        Vector3 directionToPlayer = moveController.playerObject.transform.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        for (int i = 1; i <= _projectileAmount; i++)
        {
            GameObject projectile_ = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, angle)));
            projectile_.GetComponent<GolemProjectileBehavior>().enemyStats = enemyStats;
            yield return new WaitForSeconds(_projectileInterval);
        }
        isAttacking = false;
    }
}
