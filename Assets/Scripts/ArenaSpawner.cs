using System.Collections;
using UnityEngine;

public class ArenaSpawner : MonoBehaviour
{
    public GameObject golemPrefab;
    public GameObject slimePrefab;
    public int interval = 15;
    public int waves = 11;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        //yield return new WaitForSeconds(20);
        for (int i = 1; i <= waves; i++)
        {
            StartCoroutine(SpawnMonsters(i));
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator SpawnMonsters(int wave)
    {
        if (waves == 11)
        {
            if (wave <= 5)
            {
                for (int j = 0; j < 4; j++) // Spawn 4 monsters per wave
                {
                    GameObject prefabToSpawn = Random.Range(0, 2) == 0 ? golemPrefab : slimePrefab;
                    Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                    Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

                    yield return new WaitForSeconds(0.05f); // Optional: delay between spawns
                }
            }
            else
            {
                for (int j = 0; j < 6; j++) // Spawn 5 monsters per wave
                {
                    GameObject prefabToSpawn = Random.Range(0, 2) == 0 ? golemPrefab : slimePrefab;
                    Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                    Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

                    yield return new WaitForSeconds(0.05f); // Optional: delay between spawns
                }
            }
        }
        if (waves == 6)
        {
            if (wave <= 3)
            {
                for (int j = 0; j < 4; j++) // Spawn 4 monsters per wave
                {
                    GameObject prefabToSpawn = Random.Range(0, 2) == 0 ? golemPrefab : slimePrefab;
                    Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                    Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

                    yield return new WaitForSeconds(0.05f); // Optional: delay between spawns
                }
            }
            else
            {
                for (int j = 0; j < 6; j++) // Spawn 6 monsters per wave
                {
                    GameObject prefabToSpawn = Random.Range(0, 2) == 0 ? golemPrefab : slimePrefab;
                    Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                    Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

                    yield return new WaitForSeconds(0.05f); // Optional: delay between spawns
                }
            }
        }
    }
}
