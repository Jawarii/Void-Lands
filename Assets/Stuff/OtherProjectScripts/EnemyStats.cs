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
    public float staggerHealth;
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
    public bool handledDrops = false;

    public EnemyMovementController enemyMovement;
    public BossMovementController bossMovementController;
    public float hitIndicator = 0f;
    private Color baseColor;

    public AudioSource source;
    public AudioSource enemyGettingHitSource;
    public AudioSource dropsSource;
    public AudioSource debuffSource;

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

    public int directionModifier;
    public bool isStaggered = false;
    public bool isElite = false;

    private Coroutine currentCC;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovementController>();
        InitializeEnemy();
    }

    void Update()
    {
        isCrowdControlled = isStunned || isDazed || isKnockedBack || isStaggered;

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
        //HandleHitIndicator();
        if (staggerHealth <= 0 && isBoss)
        {
            staggerHealth = 0;
            if (!isCrowdControlled)
            {
                ApplyCrowdControl("Stagger", 5f);
            }
        }
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
        maxHp = CalculateStat(baseHp, enemyLvl, factor1to10, factor11to20) - 100f; // -100f is to make earlier monsters a bit easier to kill
        maxHp = maxHp / 10f;
        maxHp = maxHp / 1.5f;
        if (isElite)
        {
            maxHp *= 6;
        }
        hp = maxHp;
        prevHp = hp;

        attack = CalculateStat(20f, enemyLvl, factor1to10, factor11to20);
        defense = CalculateStat(15f, enemyLvl, factor1to10, factor11to20);

        if (isElite)
        {
            transform.localScale *= 1.35f;
            attack *= 1.25f;
        }
        // Other properties
        baseColor = gameObject.GetComponent<SpriteRenderer>().color;
        _exp = isBoss ? enemyLvl * 360 : isElite ? enemyLvl * 60 : enemyLvl * 10;
        originalPos = transform.position;
        isBoss = gameObject.GetComponent<BossMovementController>();
        dropsSource = GameObject.Find("DropAudioSource").GetComponent<AudioSource>();
        enemyGettingHitSource = GameObject.Find("EnemyGettingHitSource").GetComponent<AudioSource>();
        debuffSource = GameObject.Find("DebuffSoundSource").GetComponent<AudioSource>();

        if (isBoss)
        {
            staggerHealth = 0.6f * maxHp;
        }
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
            isAttacking = false;
            EnemyAttack enemyAttack = transform.GetComponent<EnemyAttack>();
            if (enemyAttack != null)
            {
                enemyAttack.StopAllCoroutines();
                enemyAttack.enabled = false;
            }
            //enemyMovement.enabled = false; // Disable movement script
        }
        if (isBoss)
        {
            BossMovementController bossMovement = gameObject.GetComponent<BossMovementController>();
            bossMovement.canMove = false;
            bossMovement.agent.SetDestination(transform.position);

            ReaperBossPatternBehaviour enemyAttack = transform.GetComponent<ReaperBossPatternBehaviour>();
            if (enemyAttack != null)
            {
                enemyAttack.StopAllCoroutines();
                enemyAttack.enabled = false;
            }
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
            transform.GetComponent<EnemyMovementController>().canMove = true;  // Enable the NavMeshAgent
            transform.GetComponent<EnemyAttack>().enabled = true;       // Enable the movement script
        }
        if (isBoss)
        {
            BossMovementController bossMovement = gameObject.GetComponent<BossMovementController>();
            gameObject.GetComponent<BossMovementController>().canMove = true;
            //bossMovement.agent.SetDestination(bossMovement.originalPos);

            ReaperBossPatternBehaviour enemyAttack = transform.GetComponent<ReaperBossPatternBehaviour>();
            if (enemyAttack != null)
            {
                transform.GetComponent<ReaperBossPatternBehaviour>().enabled = true;

            }
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
        if (hp <= 0 || isDead)
            return;
        if (enemyMovement != null)
        {
            if (enemyMovement.isReturning == true)
                return;
        }
        if (isBoss)
        {
            switch (effect)
            {
                case "Stagger":
                    isStaggered = true;
                    Vector3 ccPos = new Vector3(transform.position.x, transform.position.y + (0.3f * transform.localScale.y), transform.position.z);
                    instantiatedObject = Instantiate(ccPrefab, ccPos, Quaternion.identity, transform);
                    if (currentCC != null)
                        StopCoroutine(currentCC);
                    currentCC = StartCoroutine(HandleCrowdControl(effect, duration));
                    break;
                case "Stun":
                    if (duration == 3) //Temporary solution for higher stagger rate with piercing arrow
                    {
                        int stunHealthReduction = (int)(maxHp * (4f * duration / 100f));
                        staggerHealth -= stunHealthReduction;
                    }
                    else
                    {
                        int stunHealthReduction = (int)(maxHp * (duration / 100f));
                        staggerHealth -= stunHealthReduction;
                    }
                    break;
                case "Daze":
                    int dazeHealthReduction = (int)(maxHp * (duration / 100f));
                    staggerHealth -= dazeHealthReduction;
                    break;
                case "Knockback":
                    int kbHealthReduction = (int)(maxHp * (duration / 100f));
                    staggerHealth -= kbHealthReduction;
                    break;
            }
        }
        else
        {
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
            if (currentCC != null)
                StopCoroutine(currentCC);
            currentCC = StartCoroutine(HandleCrowdControl(effect, duration));
        }
    }

    public IEnumerator HandleCrowdControl(string effect, float duration)
    {
        debuffSource.volume = 0.25f;
        debuffSource.pitch = 1.4f;
        if (effect == "Stagger")
        {
            debuffSource.volume = 1.0f;
            debuffSource.pitch = 1f;
        }
        debuffSource.PlayOneShot(ccClip);
        float targetStaggerHealth = maxHp * 0.6f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the current stagger health based on the elapsed time
            staggerHealth = Mathf.Lerp(0, targetStaggerHealth, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            // Optional: Use staggerHealth here for any effect you want to display
            yield return null;
        }

        // Ensure staggerHealth reaches the target value at the end
        staggerHealth = targetStaggerHealth;
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
            case "Stagger":
                isStaggered = false;
                staggerHealth = maxHp * 0.6f;
                ReaperBossPatternBehaviour repearBehaviour = transform.GetComponent<ReaperBossPatternBehaviour>();
                if (repearBehaviour != null)
                {
                    repearBehaviour.ResetVariablesAfterCC();
                }
                break;
        }
        if (instantiatedObject != null)
            Destroy(instantiatedObject);
    }

    private void HandleDeath()
    {
        if (animator_.GetBool("isDead") == false && !handledDrops)
        {
            animator_.SetBool("isDead", true);
            enemyGettingHitSource.PlayOneShot(deathClip);
            DropGear();
            DropUpgrades();
            goldAmount = (int)Random.Range(10 * Mathf.Pow(1.2f, enemyLvl - 1) * 0.8f, 10 * Mathf.Pow(1.2f, enemyLvl - 1) * 1.2f);
            if (isBoss && goldAmount > 0)
            {
                goldAmount *= 23;
            }
            DropGold();
            handledDrops = true;
        }

        if (deathDuration > 0.5f)
        {
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
            GoldDropBehaviour goldDropBehaviour = goldDrop.GetComponent<GoldDropBehaviour>();
            if (goldDropBehaviour != null)
            {
                goldDropBehaviour.direction = DirectionModifier(dropPosition);
            }

            goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
            goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
        }
        else if (isBoss)
        {
            int goldDropIndex = goldAmount < 10 ? 0 : goldAmount < 50 ? 1 : 2;
            Vector3 dropPosition = RandomDropPosition();
            GameObject goldDrop = Instantiate(goldItems[goldDropIndex], dropPosition, transform.rotation);
            GoldDropBehaviour goldDropBehaviour = goldDrop.GetComponent<GoldDropBehaviour>();
            if (goldDropBehaviour != null)
            {
                goldDropBehaviour.direction = DirectionModifier(dropPosition);
            }
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
        else if (Random.value > 0.995f && isArenaMob && !isBoss)
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
        if (Random.value > 0.99f && !isArenaMob && !isBoss) // 1% chance to drop upgrades
        {
            DropUpgradeItem(upgradeItems);
        }
        else if (Random.value > 0.94f && isArenaMob && !isBoss)
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
        float magicChance = 0.2f + (0.38f * (currentTierLevel - 1) / 9);      // From 20% to 40%

        // Roll for item rarity
        float roll = Random.value;
        string rarity = roll <= legendaryChance ? "Legendary" :
                        roll <= (legendaryChance + rareChance) ? "Rare" :
                        roll <= (legendaryChance + rareChance + magicChance) ? "Magic" : "Normal";

        // If the enemy is a boss, roll 10 additional times with a 15% chance each for extra drops
        if (isBoss)
        {
            rarity = "Legendary";
            for (int i = 0; i < 5; i++)
            {
                if (Random.value <= 0.20f) // 15% chance per roll
                {
                    DropItemBasedOnRarity(items, rarity);
                }
            }
        }
        // Drop the initial item based on rarity
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
            for (int i = 0; i < 3; i++)
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
        int itemLevelStarter = enemyLvl;

        if (enemyLvl >= 11 && enemyLvl <= 20)
        {
            itemLevelStarter += 3;
        }
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

            if (isBoss)
            {
                if (levelRoll <= 0.70f) // 70% chance for item level 2 times higher
                {
                    item.GetComponent<ItemInfo>().itemLvl = enemyLvl + 2;
                }
                else if (levelRoll <= 0.95f) // 25% chance for item level 1 time higher
                {
                    item.GetComponent<ItemInfo>().itemLvl = enemyLvl + 1;
                }
                else // 5% chance for same level as enemy
                {
                    item.GetComponent<ItemInfo>().itemLvl = enemyLvl;
                }
            }
            else
            {
                if (levelRoll <= 0.10f) // 10% chance for item level 2 times higher
                {
                    item.GetComponent<ItemInfo>().itemLvl = itemLevelStarter + 2;
                }
                else if (levelRoll <= 0.30f) // 20% chance for item level 1 time higher
                {
                    item.GetComponent<ItemInfo>().itemLvl = itemLevelStarter + 1;
                }
                else // 70% chance for same level as enemy
                {
                    item.GetComponent<ItemInfo>().itemLvl = itemLevelStarter;
                }
            }
        }

        // If there are items of the chosen rarity, choose one randomly to drop
        if (itemsOfRarity.Count > 0)
        {
            int randomIndex = Random.Range(0, itemsOfRarity.Count);
            GameObject itemToDrop = itemsOfRarity[randomIndex];
            Vector3 dropPosition = RandomDropPosition();
            GameObject droppedItem = Instantiate(itemToDrop, dropPosition, transform.rotation);
            ClickLootBehaviour clickLootBehaviour = droppedItem.transform.GetComponent<ClickLootBehaviour>();
            if (clickLootBehaviour != null)
            {
                clickLootBehaviour.direction = directionModifier;
            }

            // Play sound if it's a legendary item
            if (rarity == "Legendary")
            {
                dropsSource.PlayOneShot(dropClip);
            }
        }
    }

    private Vector3 RandomDropPosition()
    {
        float radius = 0.2f * transform.localScale.y; // Set the radius of item drop scatter
        Vector2 randomPoint = Random.onUnitSphere * radius;
        directionModifier = DirectionModifier(randomPoint);
        return transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
    }
    private int DirectionModifier(Vector2 randomPoint)
    {
        if (randomPoint.x > transform.position.x)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
    public void TakeDamage(int damage, bool isCrit)
    {
        //if (enemyMovement != null && enemyMovement.isReturning == true)
        //    return;
        if (isCrowdControlled)
        {
            damage = (int)((player.GetComponent<PlayerStats>().staggerDmg / 100f) * damage);
        }

        float armorEfficiency = defense / 166f + 1f;
        damage = (int)(damage / armorEfficiency);
        damage = (int)(damage / 10f);
        if (damage < 1)
            damage = 1;

        // Stop current audio and adjust pitch
        source.Stop();
        source.pitch = Random.Range(1.35f, 1.45f);
        hp -= damage;

        if (isBoss)
        {
            float staggerDamage = Random.Range(damage * 0.8f, damage * 1.2f);
            staggerHealth -= (int)staggerDamage;
        }
        // Create damage popup
        damagePopup.isPlayer = false;
        damagePopup.Create(transform.position, Mathf.Abs(transform.localScale.y * 0.5f), damage, isCrit);

        // Change the enemy's color to red
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;

        // Reset the color back to baseColor after 0.1 seconds
        Invoke(nameof(ResetColor), 0.1f);

        source.PlayOneShot(clip);

        // Handle enemy movement and agro
        enemyMovement = transform.GetComponent<EnemyMovementController>();
        if (enemyMovement != null && !enemyMovement.inPursuit && !enemyMovement.isReturning)
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
            bossMovementController = transform.GetComponent<BossMovementController>();
            if (bossMovementController)
            {
                bossMovementController.inPursuit = true;
            }
        }
    }

    // Method to reset the color after 0.1 seconds
    private void ResetColor()
    {
        gameObject.GetComponent<SpriteRenderer>().color = baseColor;
    }


    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (isBoss)
            return;
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
        handledDrops = false;

        EnableAllComponents(gameObject);
    }
}
