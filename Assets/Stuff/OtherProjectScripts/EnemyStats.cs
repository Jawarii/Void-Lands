using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FirstGearGames.SmoothCameraShaker;
using System.Buffers.Text;
using static Unity.VisualScripting.Member;

public class EnemyStats : MonoBehaviour
{
    public ShakeData myShake;
    public float baseHp = 100;
    public float maxHp;
    public float hp;
    public float attack;
    public float defense;
    public float timeSince = 0;
    public GameObject player;
    public Animator animator_;
    private PlayerStats stats;
    private float prevHp;
    public int enemyLvl = 1;
    public int _exp = 10;
    private float deathDuration = 0;
    public bool isDead = false;
    public bool isAttacking = false;
    public bool isStunned = false;
    public bool isCrowdControlled = false;
    public bool isDazed = false;
    public bool isKnockedBack = false;

    private EnemyMovementController enemyMovement;

    public float hitIndicator = 0f;
    private Color baseColor;

    public AudioSource source;
    public AudioSource enemyGettingHitSource;
    public AudioSource dropsSource;

    public AudioClip clip;
    public AudioClip deathClip;
    public AudioClip dropClip;
    public AudioClip ccClip;

    public List<GameObject> goldItems = new List<GameObject>();
    public List<GameObject> gearItems = new List<GameObject>();
    public List<GameObject> upgradeItems = new List<GameObject>();

    [SerializeField] private DamagePopup damagePopup;

    public bool isBoss = false;
    private float goldAmount = 0;
    private float agroRadius = 1.0f;
    private float respawnTimer = 60.0f;

    public GameObject ccPrefab;
    public GameObject instantiatedObject;
    public Vector3 originalPos;

    public bool isArenaMob = false;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovementController>();
        InitializeEnemy();
    }

    void Update()
    {
        isCrowdControlled = isStunned || isDazed || isKnockedBack;

        // Disable movement and actions when crowd-controlled
        if (player.GetComponent<PlayerStats>().isDead && isArenaMob)
        {
            Destroy(gameObject); return;
        }
        if (isCrowdControlled)
        {
            DisableMovementAndActions();
        }
        else
        {
            EnableMovementAndActions();
        }
        HandleHitIndicator();
        CheckHealth();
    }

    private void InitializeEnemy()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        stats = player.GetComponentInChildren<PlayerStats>();

        // Base factor for levels 1-10 and levels 11+
        float factor1to10 = 1.25f;
        float factor11to20 = 1.12f;
        float factor21to30 = 1.1f;
        // Calculate HP, Attack, and Defense
        maxHp = CalculateStat(baseHp, enemyLvl, factor1to10, factor11to20);
        hp = maxHp;
        prevHp = hp;

        attack = CalculateStat(20f, enemyLvl, factor1to10, factor11to20);
        defense = CalculateStat(15f, enemyLvl, factor1to10, factor11to20);

        // Other properties
        baseColor = gameObject.GetComponent<SpriteRenderer>().color;
        _exp = enemyLvl * 10;
        originalPos = transform.position;
        isBoss = gameObject.GetComponent<BossMovementController>();
        dropsSource = GameObject.Find("DropAudioSource").GetComponent<AudioSource>();
        enemyGettingHitSource = GameObject.Find("EnemyGettingHitSource").GetComponent<AudioSource>();
    }

    private int CalculateStat(float baseStat, int level, float factor1to10, float factor11to20)
    {
        if (level <= 10)
        {
            // Apply only the 1.25 factor for levels 1 to 10
            return (int)(baseStat * Mathf.Pow(factor1to10, level - 1));
        }
        else
        {
            // Apply the 1.25 factor for levels 1 to 10, then apply the 1.20 factor for levels 11 to the current level
            int remainingLevels = level - 10;
            return (int)(baseStat * Mathf.Pow(factor1to10, 10 - 1) * Mathf.Pow(factor11to20, remainingLevels));
        }
    }

    private void DisableMovementAndActions()
    {
        if (hp <= 0 || isDead)
        {
            return;
        }
        // Check if enemyMovement and its agent are valid
        if (enemyMovement != null && enemyMovement.agent != null)
        {
            enemyMovement.canMove = false;
            enemyMovement.agent.SetDestination(transform.position);
            transform.GetComponent<EnemyAttack>().StopAllCoroutines();
            transform.GetComponent<EnemyAttack>().enabled = false;
            //enemyMovement.enabled = false; // Disable movement script
        }
    }

    private void EnableMovementAndActions()
    {
        if (hp <= 0 || isDead)
        {
            return;
        }
        // Check if enemyMovement and its agent are valid
        if (enemyMovement != null && enemyMovement.agent != null)
        {
            enemyMovement.canMove = true;  // Enable the NavMeshAgent
            transform.GetComponent<EnemyAttack>().enabled = true;       // Enable the movement script
        }
    }

    private void HandleHitIndicator()
    {
        if (hitIndicator > 0)
        {
            hitIndicator -= Time.deltaTime;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = baseColor;
        }
    }

    private void CheckHealth()
    {
        timeSince += Time.deltaTime;
        if (prevHp != hp)
        {
            timeSince = 0;
            prevHp = hp;
        }
        if (hp <= 0)
        {
            Destroy(instantiatedObject);
            HandleDeath();
        }
    }
    public void ApplyCrowdControl(string effect, float duration)
    {
        if (hp <= 0 || isDead || isBoss)
            return;
        ClearCrowdControl(name);
        switch (effect)
        {
            case "Stun":
                isStunned = true;
                break;
            case "Daze":
                isDazed = true;
                break;
            case "Knockback":
                isKnockedBack = true;
                break;
        }
        Vector3 ccPos = new Vector3(transform.position.x, transform.position.y + (0.3f * transform.localScale.y), transform.position.z);
        instantiatedObject = Instantiate(ccPrefab, ccPos, Quaternion.identity, transform);
        StartCoroutine(HandleCrowdControl(effect, duration));
    }

    public IEnumerator HandleCrowdControl(string effect, float duration)
    {
        source.PlayOneShot(ccClip);
        yield return new WaitForSeconds(duration);
        ClearCrowdControl(effect);
    }

    public void ClearCrowdControl(string effect)
    {
        switch (effect)
        {
            case "Stun":
                isStunned = false;
                break;
            case "Daze":
                isDazed = false;
                break;
            case "Knockback":
                isKnockedBack = false;
                break;
        }
        if (instantiatedObject != null)
            Destroy(instantiatedObject);
    }

    private void HandleDeath()
    {
        if (animator_.GetBool("isDead") == false)
        {
            animator_.SetBool("isDead", true);
            enemyGettingHitSource.PlayOneShot(deathClip);
        }

        if (deathDuration > 0.417f)
        {
            DropGold();
            DropGear();
            DropUpgrades();
            //player.GetComponent<ArenaResultBehaviour>().monstersKilled++;
            int levelDiff = (int)stats.lvl - enemyLvl;
            int adjustedExp;
            if (levelDiff == 1)
            {
                adjustedExp = (int)(_exp * 0.75f);
            }
            else if (levelDiff == 2)
            {
                adjustedExp = (int)(_exp * 0.1f);
            }
            else if (levelDiff == 3)
            {
                adjustedExp = (int)(_exp * 0.03f);
            }
            else if (levelDiff > 3)
            {
                adjustedExp = (int)(_exp * 0.001f);
            }
            else
            {
                adjustedExp = _exp;
            }

            stats.IncreaseExp(adjustedExp);

            if (isBoss || isArenaMob)
            {
                if (isArenaMob)
                {
                    GameObject arenaController = GameObject.Find("ArenaController");
                    if (arenaController != null)
                    {
                        arenaController.GetComponent<ArenaResultBehaviour>().monstersKilled++;
                    }
                }
                Destroy(gameObject);
            }
            else
            {
                DisableAllComponents(gameObject);
            }
        }
        else
        {
            if (!isDead)
            {

                goldAmount = (int)Random.Range(10 * Mathf.Pow(1.2f, enemyLvl - 1) * 0.8f, 10 * Mathf.Pow(1.2f, enemyLvl - 1) * 1.2f);
                if (isBoss && goldAmount > 0)
                {
                    goldAmount *= 53;
                }
                isDead = true;
                if (GetComponent<EnemyMovementController>() != null)
                {
                    if (GetComponent<EnemyMovementController>().agent != null)
                        GetComponent<EnemyMovementController>().agent.isStopped = true;

                    GetComponent<EnemyMovementController>().enabled = false;
                    GetComponentInChildren<Canvas>().enabled = false;
                    GetComponent<EnemyMovementController>().inPursuit = false;
                    GetComponent<EnemyMovementController>().canMove = false;
                    transform.GetChild(1).gameObject.SetActive(true);
                    GetComponent<Collider2D>().enabled = false;
                }
            }
            deathDuration += Time.deltaTime;
        }
    }

    private void DropGold()
    {
        if (Random.value > 0.85f && !isBoss)
        {
            int goldDropIndex = goldAmount < 30 ? 0 : goldAmount < 90 ? 1 : 2;
            Vector3 dropPosition = RandomDropPosition();
            GameObject goldDrop = Instantiate(goldItems[goldDropIndex], dropPosition, transform.rotation);
            goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
            goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
        }
        else if (isBoss)
        {
            int goldDropIndex = goldAmount < 10 ? 0 : goldAmount < 50 ? 1 : 2;
            Vector3 dropPosition = RandomDropPosition();
            GameObject goldDrop = Instantiate(goldItems[goldDropIndex], dropPosition, transform.rotation);
            goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
            goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
        }
    }

    private void DropGear()
    {
        if (Random.value > 0.90f && !isArenaMob && !isBoss) // 10% chance to drop gear
        {
            DropGearItem(gearItems);
        }
        else if (Random.value > 0.99f && isArenaMob && !isBoss)
        {
            DropGearItem(gearItems);
        }
        else if (isBoss)
        {
            DropGearItem(gearItems);
        }
    }
    private void DropUpgrades()
    {
        if (Random.value > 0.98f && !isArenaMob && !isBoss) // 2% chance to drop upgrades
        {
            DropUpgradeItem(upgradeItems);
        }
        else if (Random.value > 0.90f && isArenaMob && !isBoss)
        {
            DropUpgradeItem(upgradeItems);
        }
        else if (isBoss)
        {
            DropUpgradeItem(upgradeItems);
        }
    }

    // Handle the drop logic for gear/weapons
    private void DropGearItem(List<GameObject> items)
    {
        // Calculate the chances based on the enemy's level
        int currentTierLevel = (enemyLvl - 1) % 10 + 1; // Levels 1-10 will cycle

        // Determine the chances for each rarity
        float legendaryChance = 0.01f + (0.19f * (currentTierLevel - 1) / 9); // From 1% to 20%
        float rareChance = 0.04f + (0.36f * (currentTierLevel - 1) / 9);      // From 4% to 40%
        float magicChance = 0.2f + (0.38f * (currentTierLevel - 1) / 9); // From 20% to 40%

        if (isBoss)
        {
            legendaryChance = 1f;
        }

        // Roll for rarity
        float roll = Random.value;

        string rarity = roll <= legendaryChance ? "Legendary" :
                        roll <= (legendaryChance + rareChance) ? "Rare" :
                        roll <= (legendaryChance + rareChance + magicChance) ? "Magic" : "Normal";

        if (isBoss)
        {
            DropItemBasedOnRarity(items, rarity);
        }
        DropItemBasedOnRarity(items, rarity);
    }

    // Handle the drop logic for upgrade items
    private void DropUpgradeItem(List<GameObject> items)
    {
        int currentTierLevel = (enemyLvl - 1) % 10 + 1; // Levels 1-10 will cycle
        if (enemyLvl > 10)
            currentTierLevel = 10;
        // Determine the chances for each rarity
        float legendaryChance = 0.01f + (0.11f * (currentTierLevel - 1) / 9); // From 1% to 12%
        float rareChance = 0.03f + (0.27f * (currentTierLevel - 1) / 9);      // From 3% to 30%
        float magicChance = 0.96f - (0.38f * (currentTierLevel - 1) / 9); // From 96% to 58%

        // Roll for rarity

        if (isBoss)
        {
            for (int i = 0; i < 5; i++)
            {
                float roll = Random.value;

                string rarity = roll <= legendaryChance ? "Legendary" :
                                roll <= (legendaryChance + rareChance) ? "Rare" :
                                roll <= (legendaryChance + rareChance + magicChance) ? "Magic" : "Normal";
                DropItemBasedOnRarity(items, rarity);
            }
        }
        else
        {
            float roll = Random.value;

            string rarity = roll <= legendaryChance ? "Legendary" :
                            roll <= (legendaryChance + rareChance) ? "Rare" :
                            roll <= (legendaryChance + rareChance + magicChance) ? "Magic" : "Normal";
            DropItemBasedOnRarity(items, rarity);
        }

    }

    // Helper method to handle item drop based on rarity
    private void DropItemBasedOnRarity(List<GameObject> items, string rarity)
    {
        // Create a list to store items of the chosen rarity
        List<GameObject> itemsOfRarity = new List<GameObject>();

        // Iterate through all items and add those of the chosen rarity to the list
        foreach (GameObject item in items)
        {
            ItemInfo info = item.GetComponent<ItemInfo>();
            if (info != null && info.itemQuality == rarity)
            {
                itemsOfRarity.Add(item);
            }
        }
        foreach (GameObject item in itemsOfRarity)
        {
            float levelRoll = Random.value; // Random value between 0 and 1

            if (levelRoll <= 0.10f) // 10% chance for item level 2 times higher
            {
                item.GetComponent<ItemInfo>().itemLvl = enemyLvl + 2;
            }
            else if (levelRoll <= 0.30f) // 20% chance for item level 1 time higher
            {
                item.GetComponent<ItemInfo>().itemLvl = enemyLvl + 1;
            }
            else // 70% chance for same level as enemy
            {
                item.GetComponent<ItemInfo>().itemLvl = enemyLvl;
            }
        }

        // If there are items of the chosen rarity, choose one randomly to drop
        if (itemsOfRarity.Count > 0)
        {
            int randomIndex = Random.Range(0, itemsOfRarity.Count);
            GameObject itemToDrop = itemsOfRarity[randomIndex];
            Vector3 dropPosition = RandomDropPosition();
            Instantiate(itemToDrop, dropPosition, transform.rotation);

            // Play sound if it's a legendary item
            if (rarity == "Legendary")
            {
                dropsSource.PlayOneShot(dropClip);
            }
        }
    }

    private Vector3 RandomDropPosition()
    {
        float radius = 0.35f; // Set the radius of item drop scatter
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        return transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
    }

    public void TakeDamage(int damage, bool isCrit)
    {
        if (isCrowdControlled)
        {
            damage = (int)((player.GetComponent<PlayerStats>().staggerDmg / 100f) * damage);
        }
        float armorEffenciency = defense / 166f + 1f;
        damage = (int)(damage / armorEffenciency);
        if (damage < 1)
            damage = 1;
        source.Stop();
        source.pitch = Random.Range(1.35f, 1.45f);
        hp -= damage;
        damagePopup.isPlayer = false;
        damagePopup.Create(transform.position, transform.GetComponent<CircleCollider2D>().radius, damage, isCrit);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        hitIndicator = 0.15f;
        source.PlayOneShot(clip);
        EnemyMovementController enemyMovement = transform.GetComponent<EnemyMovementController>();
        if (enemyMovement != null && enemyMovement.inPursuit == false)
        {
            enemyMovement.inPursuit = true;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, agroRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.gameObject != transform.gameObject)
                {
                    EnemyMovementController otherEnemyMovement = collider.GetComponent<EnemyMovementController>();
                    if (otherEnemyMovement != null)
                    {
                        otherEnemyMovement.inPursuit = true;
                    }
                }
            }
        }
        else
        {
            BossMovementController bossMovementController = transform.GetComponent<BossMovementController>();
            if (bossMovementController)
            {
                bossMovementController.inPursuit = true;
            }
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        transform.position += (Vector3)direction * force * Time.deltaTime;
    }

    public void DisableAllComponents(GameObject target)
    {
        Component[] components = target.GetComponents<Component>();

        foreach (Component component in components)
        {
            if (component is Behaviour behaviourComponent)
            {
                behaviourComponent.enabled = false;
            }
            else if (component is Renderer rendererComponent)
            {
                rendererComponent.enabled = false;
            }
        }

        foreach (Transform child in target.transform)
        {
            child.gameObject.SetActive(false);
        }

        HandleRespawn();
    }

    public void EnableAllComponents(GameObject target)
    {
        Component[] components = target.GetComponents<Component>();

        foreach (Component component in components)
        {
            if (component is Behaviour behaviourComponent)
            {
                behaviourComponent.enabled = true;
            }
            else if (component is Renderer rendererComponent)
            {
                rendererComponent.enabled = true;
            }
        }

        foreach (Transform child in target.transform)
        {
            child.gameObject.SetActive(true);
        }

        EnemyMovementController enemyMovementController = gameObject.GetComponent<EnemyMovementController>();
        if (enemyMovementController != null)
        {
            animator_.SetBool("canAttack", false);
            enemyMovementController.canMove = true;
            enemyMovementController.agent.isStopped = false;
            animator_.SetFloat("Speed", enemyMovementController.agent.velocity.magnitude);
            enemyMovementController.inPursuit = false;
        }
        gameObject.GetComponentInChildren<Canvas>().enabled = true;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public void HandleRespawn()
    {
        StartCoroutine(RespawnAfterDelay(respawnTimer));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.position = originalPos;

        hp = maxHp;
        prevHp = hp;
        deathDuration = 0;
        isDead = false;
        isAttacking = false;
        animator_.SetBool("isDead", false);

        EnableAllComponents(gameObject);
    }
}
