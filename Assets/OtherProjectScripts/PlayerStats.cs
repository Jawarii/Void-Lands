using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    void Start()
    {
        maxExp = lvl * 6;

        maxHp = 100f + (lvl - 1) * 10f;
        currentHp = maxHp;
        prevHp = currentHp;

        attack = 10f + (lvl - 1) * 2f;
        defense = (lvl - 1) * 1f;
    }

    void Update()
    {
        playTime += Time.deltaTime;
    }
    void FixedUpdate()
    {
        timeSince += Time.deltaTime;
        hpRecCdCur += Time.deltaTime;
        if (prevHp != currentHp)
        {
            timeSince = 0;
            prevHp = currentHp;
        }
        if (currentExp >= maxExp)
        {
            LevelUp();
        }
        if (hpRecCdCur >= hpRecCd && currentHp < maxHp)
        {
            currentHp += hpRecovery;
            hpRecCdCur= 0;
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
        maxExp = lvl * 6f;
        IncreaseStats(lvl);
    }
    public void IncreaseStats(float lvl_)
    {
        maxHp += 10;
        attack += 2;
        defense += 1;
    }
    public void TakeDamage(int damage, bool isCrit)
    {
        if (damage < 1)
            damage = 1;
        currentHp -= damage;
        damagePopup.isPlayer = true;
        damagePopup.Create(transform.position, damage, isCrit);
    }
}
