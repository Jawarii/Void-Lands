using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FirstGearGames.SmoothCameraShaker;
using System.Buffers.Text;

public class EnemyStats : MonoBehaviour
{
    public ShakeData myShake;
    public float baseHp;
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
    private bool isDead = false;

    public float hitIndicator = 0f;
    private Color baseColor;

    public AudioSource source;
    public AudioClip clip;
    public List<GameObject> goldItems = new List<GameObject>();
    public List<GameObject> gearItems = new List<GameObject>();
    public List<GameObject> upgradeItems = new List<GameObject>();

    [SerializeField] private DamagePopup damagePopup;

    public bool isBoss = false;
    private float goldAmount = 0;

    void Start()
    {
        InitializeEnemy();
    }

    void Update()
    {
        HandleHitIndicator();
        CheckHealth();
    }

    private void InitializeEnemy()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        stats = player.GetComponentInChildren<PlayerStats>();

        baseHp = 100; //Temporary

        maxHp = (int)(baseHp * Mathf.Pow(1.1f, enemyLvl - 1));
        hp = maxHp;
        prevHp = hp;

        attack = (int)(20f * Mathf.Pow(1.1f, enemyLvl - 1));
        defense = (int)(10f * Mathf.Pow(1.1f, enemyLvl - 1));

        baseColor = gameObject.GetComponent<SpriteRenderer>().color;

        _exp = enemyLvl * 10;
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
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (deathDuration > 0.5f)
        {
            DropGold();
            DropGear();
            DropUpgrades();
            player.GetComponent<ArenaResultBehaviour>().monstersKilled++;
            stats.IncreaseExp(_exp);
            Destroy(gameObject);
        }
        else
        {
            if (!isDead)
            {
                goldAmount = (int)Random.Range(5 * Mathf.Pow(1.2f, enemyLvl - 1) * 0.8f, 5 * Mathf.Pow(1.2f, enemyLvl - 1) * 1.2f);
                animator_.SetBool("isDead", true);
                isDead = true;
                gameObject.GetComponent<EnemyMovementController>().agent.isStopped = true;
                gameObject.GetComponent<EnemyMovementController>().enabled = false;
            }
            deathDuration += Time.deltaTime;
        }
    }

    private void DropGold()
    {
        if (Random.value > 0.7f)
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
        if (Random.value > 0.99f) // 10% chance to drop gear
        {
            DropItem(gearItems);
        }
    }

    private void DropUpgrades()
    {
        if (Random.value > 0.95f) // 30% chance to drop upgrades (due to lack of a normal tier, fix this later)
        {
            DropItem(upgradeItems);
        }
    }

    private void DropItem(List<GameObject> items)
    {
        // Roll for rarity
        float roll = Random.value;
        string rarity = roll > 0.95 ? "Legendary" :
                        roll > 0.85 ? "Rare" :
                        roll > 0.50 ? "Magic" : "Normal";

        foreach (GameObject item in items)
        {
            ItemInfo info = item.GetComponent<ItemInfo>();
            if (info != null && info.itemQuality == rarity)
            {
                Vector3 dropPosition = RandomDropPosition();
                Instantiate(item, dropPosition, transform.rotation);
                break; // Assuming only one item of each rarity type should be dropped
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
        if (damage < 1)
            damage = 1;
        source.Stop();
        source.pitch = Random.Range(1.35f, 1.45f);
        hp -= damage;
        damagePopup.isPlayer = false;
        damagePopup.Create(transform.position, damage, isCrit);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        hitIndicator = 0.15f;
        source.PlayOneShot(clip);
        EnemyMovementController enemyMovement = transform.GetComponent<EnemyMovementController>();
        enemyMovement.inPursuit = true;

    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        transform.position += (Vector3)direction * force * Time.deltaTime;
    }
}
