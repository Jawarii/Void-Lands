using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSkill : MonoBehaviour
{
    public GameObject prefab;
    public float skillCd = 0;
    public float cd = 0;
    public float scanRadius = 10f;
    public int amount_ = 3;
    private GameObject closestEnemy;
    private Transform playerTransform;
    public int dmgCoe = 60;
    public int critCoe = 5;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        cd -= Time.deltaTime;
        if (cd <= 0)
        {
            cd = skillCd;

            closestEnemy = FindClosestEnemy();

            if (closestEnemy != null)
            {
                for (int i = 0; i < amount_; i++)
                {
                    Vector3 direction = closestEnemy.transform.position - transform.position;
                    direction.z = 0;
                    float angleOffset = 10f;
                    float totalAngle = angleOffset * (amount_ - 1); // Total angle to distribute

                    float middleOffset = (amount_ % 2 == 0) ? angleOffset / 2 : 0f; // Offset for even amount
                    float angle = (i - (amount_ / 2)) * angleOffset + middleOffset;

                    Quaternion rotation = Quaternion.Euler(0, 0, angle) * Quaternion.FromToRotation(Vector3.up, direction.normalized);
                    GameObject arrow = Instantiate(prefab, transform.position, rotation);
                    ArrowBehaviour arrowBehavior = arrow.GetComponent<ArrowBehaviour>();
                    arrowBehavior.SetMoveDirection(rotation * Vector3.up);
                    arrowBehavior.dmgCoe = dmgCoe;
                    arrowBehavior.critCoe = critCoe;
                }
            }
        }
    }



    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            Vector3 direction = enemy.transform.position - transform.position;
            direction.z = 0;
            float distance = direction.magnitude;

            if (distance < closestDistance && distance <= scanRadius)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }
        return closestEnemy;
    }
}
