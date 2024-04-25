using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaResultBehaviour : MonoBehaviour
{
    public GameObject notifcationGO;
    public GameObject chestPrefab;
    public int monstersKilled;
    public int monstersToWin;
    void Start()
    {
        monstersKilled = 0;
        monstersToWin = 344;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (monstersKilled >= monstersToWin)
        {
            EnableVictoryNotification();
        }
    }

    void EnableVictoryNotification()
    {
        monstersKilled= 0;
        StartCoroutine(notifcationGO.GetComponent<ArenaNotificationBehavior>().Victory());
        StartCoroutine(SpawnChest());
    }
    IEnumerator SpawnChest()
    {
        yield return new WaitForSeconds(2);
        Vector2 spawnPos = new Vector2(-21, -17f);
        Instantiate(chestPrefab,spawnPos,Quaternion.identity);
    }
}
