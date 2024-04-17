using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyStats : MonoBehaviour
{
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
    public GameObject _camera;

    void Start()
    {
        _camera = GameObject.Find("CM vcam1");
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

        maxHp = 100f + (enemyLvl - 1) * 10f * 10f;
        hp = maxHp;
        prevHp = hp;

        attack = 10f + (enemyLvl - 1) * 2f * 10f;
        defense = (enemyLvl - 1) * 1f * 10f;

        baseColor = gameObject.GetComponent<SpriteRenderer>().color;
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
            Destroy(gameObject);
        }
        else
        {
            if (!isDead)
            {
                goldAmount = Random.Range(2, 6);
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
        int goldDropIndex = goldAmount < 10 ? 0 : goldAmount < 50 ? 1 : 2;
        Vector3 dropPosition = RandomDropPosition();
        GameObject goldDrop = Instantiate(goldItems[goldDropIndex], dropPosition, transform.rotation);
        goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
        goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
    }

    private void DropGear()
    {
        if (Random.value > 0.9f) // 10% chance to drop gear
        {
            DropItem(gearItems);
        }
    }

    private void DropUpgrades()
    {
        if (Random.value > 0.4f) // 30% chance to drop upgrades (due to lack of a normal tier, fix this later)
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
        hp -= damage;
        damagePopup.isPlayer = false;
        damagePopup.Create(transform.position, damage, isCrit);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        hitIndicator = 0.12f;
        source.PlayOneShot(clip);

        EnemyMovementController enemyMovement = transform.GetComponent<EnemyMovementController>();
        enemyMovement.inPursuit = true;
        _camera.GetComponent<CameraShake>().ShakeCamera();
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        transform.position += (Vector3)direction * force * Time.deltaTime;
    }
}
