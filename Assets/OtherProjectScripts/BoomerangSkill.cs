using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangSkill : MonoBehaviour
{
    public GameObject prefab;
    public float skillCd = 4f;
    public float cd = 0;
    private Transform playerTransform;
    public int amount_ = 1;
    public int dmgCoe = 60;
    public int critCoe = 5;
    public float hitCd = 0.3f;

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
            for (int i = 0; i < amount_; i++)
            {
                Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                GameObject star = Instantiate(prefab, transform.position, randomRotation);
                star.GetComponent<StarBehavior>().dmgCoe = dmgCoe;
                star.GetComponent<StarBehavior>().critCoe = critCoe;
                star.GetComponent<StarBehavior>().hitCd = hitCd;
            }
        }
    }
}
