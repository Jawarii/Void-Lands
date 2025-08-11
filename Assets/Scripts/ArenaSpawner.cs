using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawner : MonoBehaviour
{
    public List<GameObject> monsterPrefabs; // List of monster prefabs
    public int interval = 15;
    public int waves = 11;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(0.75f);
        for (int i = 1; i <= waves; i++)
        {
            StartCoroutine(SpawnMonsters(i));
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator SpawnMonsters(int wave)
    {
        int monsterCount = wave <= (waves / 2) ? 2 : 3; // Spawn 2 or 3 monsters based on wave number

        for (int j = 0; j < monsterCount; j++)
        {
            GameObject prefabToSpawn = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(0.2f); // Optional: delay between spawns
        }
    }
    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().isDead)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }
}
