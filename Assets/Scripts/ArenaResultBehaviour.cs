using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaResultBehaviour : MonoBehaviour
{
    public GameObject notifcationGO;
    public GameObject chestPrefab;
    public int monstersKilled;
    public int monstersToWin;
    public GameObject player;
    public bool isSuccessful = false;
    public bool isFailed = false;
    void Start()
    {
        monstersKilled = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (monstersKilled >= monstersToWin && !isSuccessful)
        {
            EnableVictoryNotification();
            isSuccessful = true;
        }
        if (player.GetComponent<PlayerStats>().isDead && !isFailed)
        {
            EnableFailureNotification();
            isFailed = true;
        }
    }

    void EnableVictoryNotification()
    {
        monstersKilled = 0;
        StartCoroutine(notifcationGO.GetComponent<ArenaNotificationBehavior>().Victory());
        StartCoroutine(SpawnChest());
    }
    void EnableFailureNotification()
    {
        monstersKilled = 0;
        StartCoroutine(notifcationGO.GetComponent<ArenaNotificationBehavior>().Failure());
    }
    IEnumerator SpawnChest()
    {
        yield return new WaitForSeconds(2);
        Vector2 spawnPos = new Vector2(-21, -17f);
        Instantiate(chestPrefab, spawnPos, Quaternion.identity);
    }
}
