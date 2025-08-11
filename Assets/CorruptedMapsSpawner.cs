using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedMapsSpawner : MonoBehaviour
{
    private List<System.Action<EnemyStats, EnemyMovementController>> selectedBadModifiers;
    private List<System.Action<EnemyStats, EnemyMovementController>> selectedGoodModifiers;
    private float selectedLootAmountMod;

    [Header("Enemy Prefabs")]
    [SerializeField] private List<GameObject> normalEnemyPrefabs;
    [SerializeField] private List<GameObject> eliteEnemyPrefabs;

    [Header("Enemy Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int enemiesPerPoint = 9;
    [SerializeField] private int corruptionLvl = 1;
    [SerializeField] private int softLvl = 30;

    [Header("Corruption Modifiers")]
    [SerializeField] private float healthMultiplier = 1.5f;
    [SerializeField] private float damageMultiplier = 1.3f;
    [SerializeField] private float speedMultiplier = 1.3f;
    [SerializeField] private float defenseMultiplier = 1.3f;

    [Header("Good Modifiers")]
    [SerializeField] private float lootAmountMod = 1.5f; // How many items it drops when it drops
    [SerializeField] private float lootRarityMod = 1.9f; // How much better rarity the items can be
    [SerializeField] private float lootQuantityMod = 1.9f; // How many of an item per stack if it's a material drop
    [SerializeField] private float lootChanceMod = 1.9f; // If enemy has a drop rate lower than 100% it will increase
    private void Start()
    {
        RandomizeCorruptionModifiers(); // Prepare once
        SpawnCorruptedEnemies();        // Spawn using stored modifiers
    }
    private void RandomizeCorruptionModifiers()
    {
        selectedBadModifiers = new List<System.Action<EnemyStats, EnemyMovementController>>()
    {
        (stats, move) => { stats.maxHp *= healthMultiplier; },
        (stats, move) => { stats.attack *= damageMultiplier; },
        (stats, move) => { stats.defense *= defenseMultiplier; },
        (stats, move) => { if (move != null) move.speed_ *= speedMultiplier; }
    };

        selectedGoodModifiers = new List<System.Action<EnemyStats, EnemyMovementController>>()
    {
        (stats, move) => { stats.lootRarityMod = lootRarityMod; },
        (stats, move) => { stats.lootQuantityMod = lootQuantityMod; },
        (stats, move) => { stats.lootChanceMod = lootChanceMod; }
    };

        Shuffle(selectedBadModifiers);
        Shuffle(selectedGoodModifiers);

        // Keep only 2 of each
        selectedBadModifiers = selectedBadModifiers.GetRange(0, 2);
        selectedGoodModifiers = selectedGoodModifiers.GetRange(0, 2);

        // Calculate lootAmountMod based on number of bad mods (2)
        float bonusPerBad = Random.Range(1.6f, 2.1f);
        selectedLootAmountMod = bonusPerBad * selectedBadModifiers.Count;

        Debug.Log($"[CorruptedMap] Applied 2 bad & 2 good modifiers. LootAmountMod = {selectedLootAmountMod:F2}");
    }

    private List<T> Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }
        return list;
    }
    private void SpawnCorruptedEnemies()
    {
        foreach (Transform point in spawnPoints)
        {
            int elitesSpawned = 0;
            List<bool> spawnAsElite = new List<bool>();

            // Roll for each enemy whether it's elite
            for (int i = 0; i < enemiesPerPoint; i++)
            {
                bool isElite = Random.value <= 0.12f;
                spawnAsElite.Add(isElite);
                if (isElite) elitesSpawned++;
            }

            // If no elite rolled, force last one to be elite
            if (elitesSpawned == 0)
            {
                spawnAsElite[enemiesPerPoint - 1] = true;
            }

            // Now spawn the enemies
            for (int i = 0; i < enemiesPerPoint; i++)
            {
                bool isElite = spawnAsElite[i];
                GameObject prefab = isElite ? GetRandomPrefab(eliteEnemyPrefabs) : GetRandomPrefab(normalEnemyPrefabs);
                GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);
                ApplyCorruptionModifiers(enemy);
            }
        }
    }
    private GameObject GetRandomPrefab(List<GameObject> prefabList)
    {
        if (prefabList == null || prefabList.Count == 0)
        {
            Debug.LogWarning("Missing prefab list!");
            return null;
        }

        int index = Random.Range(0, prefabList.Count);
        return prefabList[index];
    }


    private void ApplyCorruptionModifiers(GameObject enemy)
    {
        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        EnemyMovementController moveCtrl = enemy.GetComponent<EnemyMovementController>();

        if (stats == null) return;

        stats.enemyLvl = softLvl + corruptionLvl;

        // Apply selected corruption modifiers
        foreach (var modifier in selectedBadModifiers)
            modifier.Invoke(stats, moveCtrl);

        foreach (var modifier in selectedGoodModifiers)
            modifier.Invoke(stats, moveCtrl);

        // Set loot amount mod based on corruption severity
        stats.lootAmountMod = selectedLootAmountMod;

        // Finalize HP
        stats.hp = stats.maxHp;
        stats.respawnTimer = 999999f;
    }
}
