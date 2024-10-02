using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

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
    public AudioSource levelUpSource;
    public AudioClip levelUpClip;
    public AudioClip deathClip;
    public AudioClip notifyClip;

    // Popup
    [SerializeField] private DamagePopup damagePopup;

    // Spawner & Boss Objects
    public GameObject bossObj;

    public float timeScaleMod = 1.0f;

    public bool isImmune = false;

    public Canvas levelUpCanvas;

    public GameObject respawnLocations;

    public bool isDead = false;
    public PlayerMovement playerMovement;

    public float temSpeed = 0;

    public PlayerInformation playerInformation;
    void Start()
    {
        playerInformation = transform.GetComponent<PlayerInformation>();
        //playerMovement = transform.GetComponent<PlayerMovement>();
        respawnLocations = GameObject.Find("RespawnLocations");
        source = GameObject.Find("PlayerGettingHitSource").GetComponent<AudioSource>();
        maxExp = Mathf.Pow(lvl, 2f) * 100f;

        maxHp -= baseHp;
        baseHp = (int)(100f * Mathf.Pow(1.1f, lvl - 1));
        maxHp += baseHp;
        currentHp = maxHp;
        prevHp = currentHp;

        attack -= baseAttack;
        baseAttack = 20 + (int)(20f * Mathf.Pow(1.1f, lvl - 1));
        attack += baseAttack; // Initial attack value

        defense -= baseDefense;
        baseDefense = 15 + (int)(15f * Mathf.Pow(1.1f, lvl - 1));
        defense += baseDefense;

        baseColor = gameObject.GetComponent<SpriteRenderer>().color;

        levelUpSource = GameObject.Find("LevelUpAudioSource").GetComponent<AudioSource>();
        levelUpCanvas = GameObject.Find("LevelUpCanvas").GetComponent<Canvas>();
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
        if (hpRecCdCur >= hpRecCd && currentHp < maxHp && !isDead)
        {
            currentHp += hpRecovery;
            if (currentHp > maxHp)
                currentHp = maxHp;
            hpRecCdCur = 0;
        }
        if (lvl >= 10)
        {
            playerInformation.lvl1IsComplete = true;
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
        if (lvl >= 10)
        {
            playerInformation.lvl1IsComplete = true;
        }
        if (lvl >= 20)
        {
            playerInformation.lvl2IsComplete = true;
        }
        if (lvl >= 30)
        {
            playerInformation.lvl3IsComplete = true;
        }
        currentExp = currentExp - maxExp;
        maxExp = Mathf.Pow(lvl, 2f) * 100f;
        IncreaseStats();
        levelUpSource.PlayOneShot(levelUpClip);
        StartCoroutine(HandleLevelUpPanel());
    }

    public void IncreaseStats()
    {
        maxHp -= baseHp;
        baseHp = (int)(100f * Mathf.Pow(1.1f, lvl - 1));
        maxHp += baseHp;
        if (!isDead)
            currentHp = maxHp;

        attack -= baseAttack;
        baseAttack = 20 + (int)(20f * Mathf.Pow(1.1f, lvl - 1));
        attack += baseAttack;

        //attack = CalculateAttack(); // Use dynamic calculation
        defense -= baseDefense;
        baseDefense = 15 + (int)(15f * Mathf.Pow(1.1f, lvl - 1));
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
        float armorEfficiency = defense / 166f + 1f;
        damage = (int)(damage / armorEfficiency);
        if (damage < 1)
            damage = 1;
        source.Stop();
        currentHp -= damage;
        damagePopup.isPlayer = true;
        damagePopup.Create(transform.position, 1f, damage, isCrit);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        hitIndicator = 0.15f;
        source.PlayOneShot(clip);
        if (currentHp <= 0)
        {
            currentHp = 0;
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        GetComponent<Animator>().SetBool("isDead", true);
        source.PlayOneShot(deathClip);
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Disable all components except for PlayerStats and SpriteRenderer

        MonoBehaviour[] components = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            if (component != this && component != playerMovement)
            {
                component.enabled = false;
            }
        }
        temSpeed = playerMovement.speed;
        playerMovement.speed = 0;
        playerMovement.canMove = false;
        playerMovement.canDash = false;

        // Disable other components like Collider but skip disabling the SpriteRenderer
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        // Disable the first two children of the player
        if (transform.childCount >= 2)
        {
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetChild(1).gameObject.SetActive(false);
        }

        // Wait for 3 seconds before respawning
        yield return new WaitForSeconds(3f);

        // Find the closest respawn point
        Transform closestRespawn = FindClosestRespawnPoint();

        // Re-enable all components
        foreach (MonoBehaviour component in components)
        {
            if (component != this)
            {
                component.enabled = true;
            }
        }

        // Re-enable other components like Collider
        if (collider != null) collider.enabled = true;

        // Re-enable the first two children of the player
        if (transform.childCount >= 2)
        {
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(1).gameObject.SetActive(true);
        }

        // Reset HP and reposition the player at the closest respawn point
        currentHp = maxHp;
        if (closestRespawn != null)
        {
            transform.position = closestRespawn.position;
        }
        GetComponent<Animator>().SetBool("isDead", false);
        isDead = false;
        playerMovement.speed = temSpeed;
        playerMovement.canMove = true;
        playerMovement.canDash = true;
    }

    private Transform FindClosestRespawnPoint()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // Iterate through all child objects of respawnLocations
        foreach (Transform respawnPoint in respawnLocations.transform)
        {
            float distance = Vector3.Distance(currentPosition, respawnPoint.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = respawnPoint;
            }
        }

        return closest;
    }


    public IEnumerator HandleLevelUpPanel()
    {
        levelUpCanvas.GetComponent<Canvas>().enabled = true;
        levelUpCanvas.GetComponentInChildren<Image>().color.a.Equals(1);
        levelUpCanvas.GetComponentInChildren<TMP_Text>().text = "You Leveled Up!";
        yield return new WaitForSeconds(3f);
        // Lerp Opacity Here
        yield return new WaitForSeconds(1f);
        levelUpCanvas.GetComponent<Canvas>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        if (lvl == 3 || lvl == 6 || lvl == 9)
        {
            levelUpCanvas.GetComponent<Canvas>().enabled = true;
            source.PlayOneShot(notifyClip);
            levelUpCanvas.GetComponentInChildren<TMP_Text>().text = "You Unlocked New Skills, Press [K]";
        }
        else if (lvl == 10 || lvl == 20 || lvl == 30)
        {
            levelUpCanvas.GetComponent<Canvas>().enabled = true;
            source.PlayOneShot(notifyClip);
            levelUpCanvas.GetComponentInChildren<TMP_Text>().text = "You Unlocked The Boss of This Area";
            yield return new WaitForSeconds(3f);
            // Lerp Opacity Here
            yield return new WaitForSeconds(1f);
            levelUpCanvas.GetComponent<Canvas>().enabled = false;
            yield return new WaitForSeconds(0.5f);
            levelUpCanvas.GetComponent<Canvas>().enabled = true;
            source.PlayOneShot(notifyClip);
            levelUpCanvas.GetComponentInChildren<TMP_Text>().text = "You Unlocked The Arena of This Area";
        }
        else
        {
            yield break;
        }
        yield return new WaitForSeconds(3f);
        // Lerp Opacity Here
        yield return new WaitForSeconds(1f);
        levelUpCanvas.GetComponent<Canvas>().enabled = false;
    }
}
