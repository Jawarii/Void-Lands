using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Combat Stats
    [Header("Combat Stats")]
    public float baseHp = 0;
    public float maxHp = 0;
    public float currentHp = 0;
    public float baseAttack = 0;
    public float attack = 0;
    public float atkSpd = 1.0f;
    public float baseDefense = 0;
    public float defense = 0;
    public float critRate = 5.0f;
    public float critDmg = 150.0f;
    public float staggerDmg = 110f;
    public float speed = 1.0f;
    public float hpRecovery = 1f;
    public float cdReduction = 0f;

    // Bonus Stats
    [Header("Bonus Stats")]
    public float atkMulti = 0f;
    public float speedMulti = 0f;
    public float atkSpeedMulti = 0f;
    public float defenseSpeedMulti = 0f;
    public float critRateMulti = 0f;
    public float hpMulti = 0f;
    public float critDmgMulti = 0f;
    public float hpRecoveryMulti = 0f;

    // Level Stats
    [Header("Level Stats")]
    public float lvl = 1;
    public float currentExp = 0;
    public float maxExp;

    // Variables
    [Header("Variables")]
    private bool levelUp = false;
    private float prevHp;
    public float timeSince = 0;
    public Skills playerSkills;
    public float hpRecCd = 3f;
    public float hpRecCdCur = 0;
    public float hitIndicator = 0f;
    private Color baseColor;

    // Upgrade Variables
    private System.Random random = new System.Random();

    // PlayTime
    [Header("PlayTime")]
    public float playTime = 0.0f;

    // Sound
    [Header("Sound")]
    public AudioSource source;
    public AudioClip clip;

    // Popup
    [SerializeField] private DamagePopup damagePopup;

    // Spawner & Boss Objects
    public GameObject bossObj;

    public float timeScaleMod = 1.0f;

    public bool isImmune = false;

    void Start()
    {
        maxExp = Mathf.Pow(lvl, 1.35f) * 100f;

        maxHp -= baseHp;
        baseHp = (int)(100f * Mathf.Pow(1.1f, lvl - 1));
        maxHp += baseHp;
        currentHp = maxHp;
        prevHp = currentHp;

        attack -= baseAttack;
        baseAttack = (int)(20f * Mathf.Pow(1.1f, lvl - 1));
        attack += baseAttack; // Initial attack value

        defense -= baseDefense;
        baseDefense = (int)(10f * Mathf.Pow(1.1f, lvl - 1));
        defense += baseDefense;

        baseColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        playTime += Time.deltaTime;
        HandleHitIndicator();
    }

    void FixedUpdate()
    {
        timeSince += Time.deltaTime;
        hpRecCdCur += Time.deltaTime;
        //------------------------
        // Reminder: this is used for delayedHp UI logic. dont delete.
        if (prevHp != currentHp)
        {
            timeSince = 0;
            prevHp = currentHp;
        }
        //------------------------

        if (currentExp >= maxExp)
        {
            LevelUp();
        }
        if (hpRecCdCur >= hpRecCd && currentHp < maxHp)
        {
            currentHp += hpRecovery;
            hpRecCdCur = 0;
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

    public void IncreaseExp(float exp)
    {
        currentExp += exp;
    }

    public void LevelUp()
    {
        lvl++;
        currentExp = currentExp - maxExp;
        maxExp = Mathf.Pow(lvl, 1.35f) * 100f;
        IncreaseStats();
    }

    public void IncreaseStats()
    {
        maxHp -= baseHp;
        baseHp = (int)(100f * Mathf.Pow(1.1f, lvl - 1));
        maxHp += baseHp;
        currentHp = maxHp;

        attack -= baseAttack;
        baseAttack = (int)(20f * Mathf.Pow(1.1f, lvl - 1));
        attack += baseAttack;

        //attack = CalculateAttack(); // Use dynamic calculation
        defense -= baseDefense;
        baseDefense = (int)(10f * Mathf.Pow(1.1f, lvl - 1));
        defense += baseDefense;
    }

    private float CalculateAttack()
    {
        // Calculate the final attack value considering buffs or other modifications
        return baseAttack + atkMulti * baseAttack;
    }

    public void TakeDamage(int damage, bool isCrit)
    {
        if (isImmune)
            return;
        if (damage < 1)
            damage = 1;
        source.Stop();
        currentHp -= damage;
        damagePopup.isPlayer = true;
        damagePopup.Create(transform.position, 1f, damage, isCrit);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        hitIndicator = 0.15f;
        source.PlayOneShot(clip);
    }
}
