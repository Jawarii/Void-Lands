using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour
{
    private float m_SpawnTime = 0.0f;
    public float spawnTime = 0.65f;
    public GameObject enemy;
    public GameObject enemy1;
    public GameObject enemy2;
    private int randomInt;
    private int randomEnemy;
    Vector2 originPos;
    Vector2 pos;

    private float cs;
    private float sn;

    public GameObject player;

    void Start()
    {
        originPos = transform.position;
        pos = transform.position;
    }
    void Update()
    {
        originPos = transform.position;
        pos = transform.position;
        spawnTime = 0.65f - (player.GetComponent<PlayerStats>().playTime / 1000f);
        if (m_SpawnTime >= spawnTime)
        {
            randomInt = UnityEngine.Random.Range(0, 360);
            float radians = randomInt * Mathf.Deg2Rad;

            cs = Mathf.Cos(radians);
            sn = Mathf.Sin(radians);

            randomEnemy = UnityEngine.Random.Range(1, 4);

            //for (int i = 0; i < 3; i++)
            //{
            //    for (int j = 0; j < 2; j++)
            //    {
            //        pos = new Vector2(originPos.x + (cs * 10.0f) + i/1.2f, originPos.y + (sn * 10.0f) + j/1.2f);
            //        Instantiate(enemy, pos, Quaternion.Euler(0, 0, 0));
            //    }              
            //}
            if (randomEnemy == 1)
            {
                if (player.GetComponent<PlayerStats>().playTime >= 300f)
                {
                    pos = new Vector2(originPos.x + (cs * 10.0f), originPos.y + (sn * 10.0f));
                    Instantiate(enemy, pos, Quaternion.Euler(0, 0, 0));
                    pos = new Vector2(originPos.x + (cs * 10.0f) + 0.8f, originPos.y + (sn * 10.0f) + 0.8f);
                    Instantiate(enemy, pos, Quaternion.Euler(0, 0, 0));
                    m_SpawnTime = 0.0f;
                }
                else
                {
                    pos = new Vector2(originPos.x + (cs * 10.0f), originPos.y + (sn * 10.0f));
                    Instantiate(enemy, pos, Quaternion.Euler(0, 0, 0));
                    m_SpawnTime = 0.0f;
                }

            }
            if (randomEnemy == 2)
            {
                if (player.GetComponent<PlayerStats>().playTime >= 300f)
                {
                    pos = new Vector2(originPos.x + (cs * 10.0f), originPos.y + (sn * 10.0f));
                    Instantiate(enemy1, pos, Quaternion.Euler(0, 0, 0));
                    pos = new Vector2(originPos.x + (cs * 10.0f) + 0.8f, originPos.y + (sn * 10.0f) + 0.8f);
                    Instantiate(enemy1, pos, Quaternion.Euler(0, 0, 0));
                    m_SpawnTime = 0.0f;
                }
                else
                {
                    pos = new Vector2(originPos.x + (cs * 10.0f), originPos.y + (sn * 10.0f));
                    Instantiate(enemy1, pos, Quaternion.Euler(0, 0, 0));
                    m_SpawnTime = 0.0f;
                }

            }
            if (randomEnemy == 3)
            {
                if (player.GetComponent<PlayerStats>().playTime >= 300f)
                {
                    pos = new Vector2(originPos.x + (cs * 10.0f), originPos.y + (sn * 10.0f));
                    Instantiate(enemy2, pos, Quaternion.Euler(0, 0, 0));
                    pos = new Vector2(originPos.x + (cs * 10.0f) + 0.8f, originPos.y + (sn * 10.0f) + 0.8f);
                    Instantiate(enemy2, pos, Quaternion.Euler(0, 0, 0));
                    m_SpawnTime = 0.0f;
                }
                else
                {
                    pos = new Vector2(originPos.x + (cs * 10.0f), originPos.y + (sn * 10.0f));
                    Instantiate(enemy2, pos, Quaternion.Euler(0, 0, 0));
                    m_SpawnTime = 0.0f;
                }
            }
        }
        else
        {
            m_SpawnTime += Time.deltaTime;
        }
    }
}
